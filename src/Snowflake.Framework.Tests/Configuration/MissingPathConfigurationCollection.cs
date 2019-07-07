﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snowflake.Configuration;
using Snowflake.Configuration.Attributes;
using Snowflake.Configuration.Tests;

namespace Snowflake.Configuration.Tests
{
    [ConfigurationTarget("target")]
    public interface
        MissingPathConfigurationCollection : IConfigurationCollection<MissingPathConfigurationCollection>
    {
        [ConfigurationTargetMember("target")] MissingPathConfigurationSection PathConfiguration { get; set; }
    }

    [ConfigurationSection("NoPath", "NoPath")]
    public interface MissingPathConfigurationSection : IConfigurationSection<MissingPathConfigurationSection>
    {
        [ConfigurationOption("BadPath", "/my/bad/path", PathType.Directory)]
        string BadPath { get; set; }
    }

}
