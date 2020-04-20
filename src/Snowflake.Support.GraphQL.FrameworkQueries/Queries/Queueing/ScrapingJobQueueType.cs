﻿using HotChocolate.Types;
using Snowflake.Extensibility.Queueing;
using Snowflake.Scraping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snowflake.Support.GraphQL.FrameworkQueries.Queries.Queueing
{
    internal sealed class ScrapingJobQueueType
        : ObjectType<IAsyncJobQueue<IScrapeContext, IEnumerable<ISeed>>>
    {
        protected override void Configure(IObjectTypeDescriptor<IAsyncJobQueue<IScrapeContext, IEnumerable<ISeed>>> descriptor)
        {
            descriptor.Name("ScrapingJobQueue")
                .Description("Provides access to values in the scraping job queue");

            descriptor.Field("job")
                .Argument("jobId", arg => arg.Type<NonNullType<UuidType>>())
                .Resolver(ctx =>
                {
                    var context = ctx.Parent<IAsyncJobQueue<IScrapeContext, IEnumerable<ISeed>>>();
                    return (context, ctx.Argument<Guid>("jobId"));
                })
                .Type<ScrapingJobType>();
            descriptor.Field(s => s.GetActiveJobs())
                .Name("activeJobIds")
                .Description("The jobs currently active in the scraping queue.")
                .Type<NonNullType<ListType<NonNullType<UuidType>>>>();
        }
    }
}