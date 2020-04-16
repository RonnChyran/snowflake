﻿using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snowflake.Framework.Remoting.GraphQL.Model.PlatformInfo
{
    internal class StoneMetadataType
        : ObjectType<KeyValuePair<string, string>>
    {
        protected override void Configure(IObjectTypeDescriptor<KeyValuePair<string, string>> descriptor)
        {
            descriptor.Name("StoneMetadata")
                .Description("Metadata for a Stone-registered object.");
            descriptor.Field(p => p.Key).Name("key").Description("The metadata key.");
            descriptor.Field(p => p.Value).Name("value").Description("The metadata value.");
        }
    }
}
