﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Snowflake.Extensibility;
using System.Threading.Tasks;
using Snowflake.Scraping.Extensibility;
using static Snowflake.Scraping.Extensibility.SeedBuilder;

namespace Snowflake.Scraping.Tests
{
    [Directive(AttachTarget.Root, Directive.Requires, "Test")]
    [Plugin("DependentScraper")]
    public class DependentScraper : Scraper
    {
        public DependentScraper()
            : base(typeof(DependentScraper), AttachTarget.Root, SeedContent.RootSeedType)
        {
        }

        public override async IAsyncEnumerable<SeedTree> ScrapeAsync(ISeed parent,
            ILookup<string, SeedContent> rootSeeds, ILookup<string, SeedContent> childSeeds,
            ILookup<string, SeedContent> siblingSeeds)
        {
            yield return ("TestDependent", $"{rootSeeds["Test"].First()} from Dependent Scraper");
        }
    }
}
