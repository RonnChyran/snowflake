﻿using Snowflake.Records.File;
using Snowflake.Records.Game;
using Snowflake.Scraping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snowflake.Support.ScrapeProvider
{
    public class ScrapeJob
    {
        public IEnumerable<IScraper> Scrapers { get; }
        public ISeedRootContext Context { get; }
        public ScrapeJob(IEnumerable<SeedContent> initialSeeds)
        {
            this.Context = new SeedRootContext();
            foreach (var seed in initialSeeds)
            {
                this.Context.AddSeed(seed, this.Context.Root);
            }
        }

        public IEnumerable<ISeed> Progress(IEnumerable<SeedContent> seedsToAdd)
        {
            foreach (var seed in seedsToAdd)
            {
                this.Context.AddSeed(seed, this.Context.Root);
            }

            foreach (var scraper in this.Scrapers)
            {
                foreach (var matchingSeed in this.Context.GetAllOfType(scraper.ParentType))
                {

                    var requiredChildren = scraper.RequiredChildSeeds.SelectMany(seed => this.Context.GetChildren(matchingSeed)
                                .Where(child => child.Content.Type == seed)).ToLookup(x => x.Content.Type, x => x.Content); ;
                    var requiredRoots = scraper.RequiredRootSeeds.SelectMany(seed => this.Context.GetChildren(this.Context.Root)
                              .Where(child => child.Content.Type == seed)).ToLookup(x => x.Content.Type, x => x.Content);
                    if (!requiredChildren.Any() || !requiredRoots.Any())
                    {
                        continue;
                    }

                    var resultsToAppend = new List<ISeed>();
                    var results = scraper.Scrape(matchingSeed, requiredRoots, requiredChildren);
                    if (scraper.IsGroup)
                    {
                        this.Context.AddSeed((scraper.GroupType, results.First(r => r.Type == scraper.GroupValueType).Value), matchingSeed);
                        resultsToAppend.AddRange(results.Select(r => new Seed(r, scraper.GetType().Name, groupSeed)));
                    }
                    else
                    {
                        resultsToAppend.AddRange(results.Select(r => new Seed(r, scraper.GetType().Name, matchingSeed)));
                    }

                    // mark that matchingSeed was visited.
                    this.Context.Add
                }
            }

            /*
             * Add seeds to add.
             * Check if we fulfill the requirements, and if so, do not continue.
             * 
             * foreach scraper in scrapers:
             *   if seed exists with the given key
             *     if seed fulfills the requirements of the scraper
             *       get the results of scraper(seed)
             *       make list of seed values to attach
             *       is the scraper group? if so group into one seed value and add to the list.
             *       where does the seed attach?
             *       if it attaches to root, attach all to root.
             *       if it attaches to parent, attach all to parent.
             *       if it doesn't attach, drop it.
             *   keep track of visited.!
             *  return the currently seeds, and true or false if we want to progresss.
             */
            yield break;
        }

        public IEnumerable<ISeed> Cull(IDictionary<string, string> cullers)
        {
            /**
             * group seeds by type.
             * run culler for each type (culler mapping? configuration? that makes this require a plugin provision)
             * return culled seeds.
             */
            yield break;
        }

        public IEnumerable<IFileRecord> TraverseFiles()
        {
            /**
             * makes every 'file' type into a FileRecord
             */
            yield break;
        }

        public IEnumerable<IGameRecord> TraverseGames()
        {
            /**
             * makes every 'result' type into a GameRecord with traversed files.
             * theoretically this should only return one if there is one result,
             * but the user can override this because seeds with source __user are untouched when culled.
             */
            yield break;
        }

    }
}