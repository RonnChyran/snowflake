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
    [Plugin("GroupScraper")]
    public class GroupScraper : Scraper
    {
        public GroupScraper()
            : base(typeof(GroupScraper), AttachTarget.Root, SeedContent.RootSeedType)
        {
        }

        public override async Task<IEnumerable<SeedTreeAwaitable>> ScrapeAsync(ISeed parent,
            ILookup<string, SeedContent> rootSeeds, ILookup<string, SeedContent> childSeeds, ILookup<string, SeedContent> siblingSeeds)
        {
            return _("MyGroup", "Hello World",
                    __(
                        ("Test", "Hello World"),
                        ("TestTwo", "Foo Bar"))
                    );
        }
    }
}
