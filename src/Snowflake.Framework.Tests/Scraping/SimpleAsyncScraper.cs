﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Snowflake.Extensibility;
using System.Threading.Tasks;
using static Snowflake.Scraping.Extensibility.SeedBuilder;
using Snowflake.Scraping.Extensibility;

namespace Snowflake.Scraping.Tests
{
    [Plugin("SimpleAsyncScraper")]
    public class SimpleAsyncScraper : Scraper
    {
        public SimpleAsyncScraper()
            : base(typeof(AsyncScraper), AttachTarget.Root, SeedContent.RootSeedType)
        {
        }

        public override async Task<IEnumerable<SeedTreeAwaitable>> ScrapeAsync(ISeed parent,
            ILookup<string, SeedContent> rootSeeds, ILookup<string, SeedContent> childSeeds, ILookup<string, SeedContent> siblingSeeds)
        {
            return _(Task.FromResult<SeedTree>(("TestAsync", "Hello World")));
        }
    }
}
