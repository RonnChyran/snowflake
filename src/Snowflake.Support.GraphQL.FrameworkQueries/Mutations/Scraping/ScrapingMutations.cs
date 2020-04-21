﻿using HotChocolate.Types;
using Snowflake.Extensibility.Queueing;
using Snowflake.Model.Game;
using Snowflake.Remoting.GraphQL;
using Snowflake.Remoting.GraphQL.FrameworkQueries.Mutations.Relay;
using Snowflake.Scraping;
using Snowflake.Scraping.Extensibility;
using Snowflake.Services;
using Snowflake.Support.GraphQL.FrameworkQueries.Subscriptions;
using Snowflake.Support.GraphQL.FrameworkQueries.Subscriptions.Scraping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snowflake.Support.GraphQL.FrameworkQueries.Mutations.Scraping
{
    public sealed class ScrapingMutations
        : ObjectTypeExtension
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Name("Mutation");
            descriptor.Field("createScrapeContext")
                .UseClientMutationId()
                .UseAutoSubscription()
                .Argument("input", arg => arg.Type<CreateScrapeContextInputType>())
                .Resolver(async ctx =>
                {
                    var input = ctx.Argument<CreateScrapeContextInput>("input");
                    var plugins = ctx.SnowflakeService<IPluginManager>();
                    var cullers = plugins.GetCollection<ICuller>()
                        .Where(c => input.Cullers.Contains(c.Name, StringComparer.InvariantCultureIgnoreCase));
                    var scrapers = plugins.GetCollection<IScraper>()
                        .Where(c => input.Scrapers.Contains(c.Name, StringComparer.InvariantCultureIgnoreCase));
                    var game = await ctx.SnowflakeService<IGameLibrary>().GetGameAsync(input.GameID);
                    if (game == null) throw new ArgumentException("The specified game does not exist.");
                    var jobQueueFactory = ctx.SnowflakeService<IAsyncJobQueueFactory>();
                    var jobQueue = jobQueueFactory.GetJobQueue<IScrapeContext, IEnumerable<ISeed>>(false);
                    var guid = await jobQueue.QueueJob(new GameScrapeContext(game, scrapers, cullers));
                    IScrapeContext context = jobQueue.GetSource(guid);
                    return new CreateScrapeContextPayload()
                    {
                        ClientMutationID = input.ClientMutationID,
                        ScrapeContext = context,
                        JobID = guid,
                    };
                })
                .Type<NonNullType<CreateScrapeContextPayloadType>>();

            descriptor.Field("deleteScrapeContext")
                .Description("Deletes a scrape context once it has completed. If it has not completed, the deletion will fail.")
                .UseClientMutationId()
                .UseAutoSubscription()
                .Argument("input", arg => arg.Type<DeleteScrapeContextInputType>())
                .Resolver(ctx =>
                {
                    var input = ctx.Argument<DeleteScrapeContextInput>("input");
                   
                    var jobQueueFactory = ctx.SnowflakeService<IAsyncJobQueueFactory>();
                    var jobQueue = jobQueueFactory.GetJobQueue<IScrapeContext, IEnumerable<ISeed>>(false);

                    var jobId = input.JobID;
                    bool result = jobQueue.TryRemoveSource(jobId, out IScrapeContext scrapeContext);

                    return new DeleteScrapeContextPayload()
                    {
                        ScrapeContext = scrapeContext,
                        JobID = jobId,
                        Success = result,
                    };
                })
                .Type<NonNullType<DeleteScrapeContextPayloadType>>();

            descriptor.Field("nextScrapeContextStep")
                .Description("Proceeds to the next step of the specified scrape context. " +
                "Returns the output of the next step in the scrape context iterator, until " +
                "it is exhausted. If the iterator is exhausted, `current` will be null, and `hasNext` will be false . " +
                "If the specified scrape context does not exist, `context` and `current` will be null, and `hasNext` will be false. ")
                .UseClientMutationId()
                .Argument("input", arg => arg.Type<NextScrapeContextStepInputType>())
                .Resolver(async ctx =>
                {
                    var input = ctx.Argument<NextScrapeContextStepInput>("input");
                    var jobQueue = ctx.SnowflakeService<IAsyncJobQueueFactory>()
                        .GetJobQueue<IScrapeContext, IEnumerable<ISeed>>(false);
                    var scrapeContext = jobQueue.GetSource(input.JobID);
                    if (scrapeContext == null) throw new ArgumentException("The specified game does not exist.");

                    var addSeeds = input.Seeds;
                    if (addSeeds != null) 
                    {
                        foreach (var graft in addSeeds)
                        {
                            var seed = scrapeContext.Context[graft.SeedID];
                            if (seed == null) continue;
                            var seedTree = graft.Tree.ToSeedTree();
                            var seedGraft = seedTree.Collapse(seed, ISeed.ClientSource);
                            scrapeContext.Context.AddRange(seedGraft);
                        }
                    }

                    var (current, movedNext) = await jobQueue.GetNext(input.JobID);
                   
                    if (movedNext)
                    {
                        var payload = new ScrapeContextStepPayload
                        {
                            ScrapeContext = scrapeContext,
                            Current = current,
                            JobID = input.JobID
                        };
                        await ctx.SendEventMessage(new OnScrapeContextStepMessage(input.JobID, payload));
                        return payload;
                    }
                    var completePayload = new ScrapeContextCompletePayload
                    {
                        ScrapeContext = scrapeContext,
                        JobID = input.JobID
                    };

                    await ctx.SendEventMessage(new OnScrapeContextCompleteMessage(input.JobID, completePayload));
                    return completePayload;

                }).Type<NonNullType<ScrapeContextPayloadInterface>>();

            descriptor.Field("exhaustScrapeContextSteps")
                .Description("Exhausts the specified scrape context until completion. " +
                "Returns the output of the last step of the scrape context, when there are no more remaining left to continue with.")
                .UseClientMutationId()
                .Argument("input", arg => arg.Type<NextScrapeContextStepInputType>())
                .Resolver(async ctx =>
                {
                    var input = ctx.Argument<NextScrapeContextStepInput>("input");
                    var jobQueue = ctx.SnowflakeService<IAsyncJobQueueFactory>()
                        .GetJobQueue<IScrapeContext, IEnumerable<ISeed>>(false);
                    var scrapeContext = jobQueue.GetSource(input.JobID);
                    if (scrapeContext == null) throw new ArgumentException("The specified game does not exist.");

                    var addSeeds = input.Seeds;
                    if (addSeeds != null)
                    {
                        foreach (var graft in addSeeds)
                        {
                            var seed = scrapeContext.Context[graft.SeedID];
                            if (seed == null) continue;
                            var seedTree = graft.Tree.ToSeedTree();
                            var seedGraft = seedTree.Collapse(seed, ISeed.ClientSource);
                            scrapeContext.Context.AddRange(seedGraft);
                        }
                    }

                    await foreach (IEnumerable<ISeed> val in jobQueue.AsEnumerable(input.JobID)) { 
                        ScrapeContextStepPayload payload = new ScrapeContextStepPayload
                        {
                            ScrapeContext = scrapeContext,
                            Current = val,
                            JobID = input.JobID
                        };
                        await ctx.SendEventMessage(new OnScrapeContextStepMessage(input.JobID, payload));
                    }

                    var completePayload = new ScrapeContextCompletePayload
                    {
                        ScrapeContext = scrapeContext,
                        JobID = input.JobID
                    };

                    await ctx.SendEventMessage(new OnScrapeContextCompleteMessage(input.JobID, completePayload));
                    return completePayload;

                }).Type<NonNullType<ScrapeContextCompletePayloadType>>();

            descriptor.Field("applyScrapeResults")
                .Description("Applies the specified scrape results to the specified game as-is.")
                .UseClientMutationId()
                .UseAutoSubscription()
                .Argument("input", arg => arg.Type<ApplyScrapeResultsInputType>())
                .Resolver(async ctx =>
                {
                    var input = ctx.Argument<ApplyScrapeResultsInput>("input");
                    var jobQueue = ctx.SnowflakeService<IAsyncJobQueueFactory>()
                        .GetJobQueue<IScrapeContext, IEnumerable<ISeed>>(false);
                    var gameLibrary = ctx.SnowflakeService<IGameLibrary>();
                    var pluginManager = ctx.SnowflakeService<IPluginManager>();
                    var fileTraversers = pluginManager.GetCollection<IFileInstallationTraverser>()
                        .Where(c => input.FileTraversers.Contains(c.Name, StringComparer.InvariantCultureIgnoreCase));
                    
                    var metaTraversers = pluginManager.GetCollection<IGameMetadataTraverser>()
                        .Where(c => input.MetadataTraversers.Contains(c.Name, StringComparer.InvariantCultureIgnoreCase));

                    var scrapeContext = jobQueue.GetSource(input.JobID);
                    if (scrapeContext == null) throw new ArgumentException("The specified scrape context does not exist.");
                    IGame? game = null;

                    if (input.GameID == default)
                    {
                        var recordSeed = scrapeContext.Context.GetAllOfType("scrapecontext_record").FirstOrDefault();
                        if (recordSeed == null) throw new ArgumentException("No valid game found to apply this scrape context.");
                        game = await gameLibrary.GetGameAsync(Guid.Parse(recordSeed.Content.Value));
                    }
                    else
                    {
                        game = await gameLibrary.GetGameAsync(input.GameID);
                    }

                    if (game == null) throw new ArgumentException("The specified game does not exist.");


                    foreach (var traverser in metaTraversers)
                    {
                        await traverser.TraverseAll(game, scrapeContext.Context.Root, scrapeContext.Context);
                    }

                    foreach (var traverser in fileTraversers)
                    {
                        await traverser.TraverseAll(game, scrapeContext.Context.Root, scrapeContext.Context);
                    }

                    jobQueue.TryRemoveSource(input.JobID, out var _);

                    return new ApplyScrapeResultsPayload
                    {
                        Game = game,
                        ScrapeContext = scrapeContext,
                    };

                }).Type<NonNullType<ApplyScrapeResultsPayloadType>>();

        }
    }
}
