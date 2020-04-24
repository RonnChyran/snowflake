﻿using HotChocolate.Types;
using Snowflake.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snowflake.Remoting.GraphQL.Model.Configuration
{
    public sealed class OptionDescriptorType
        : ObjectType<IConfigurationOptionDescriptor>
    {
        protected override void Configure(IObjectTypeDescriptor<IConfigurationOptionDescriptor> descriptor)
        {
            descriptor.Name("OptionDescriptor")
                .Description("Describes a single configuration option.");

            descriptor.Field(o => o.Description)
                .Description("Describes the option in detail.")
                .Type<NonNullType<StringType>>();
            descriptor.Field(o => o.DisplayName)
                .Description("The human readable name of this option.")
                .Type<NonNullType<StringType>>();
            descriptor.Field(o => o.OptionName)
                .Description("The option name as it is serialized into the configuration. " +
                "This is different from the option key, and has none of the same guarantees, namely, uniqueness.")
                .Type<NonNullType<StringType>>();
            descriptor.Field(o => o.OptionKey)
                .Description("The string that uniquely identifies this option with regard to the section.")
                .Type<NonNullType<StringType>>();
            descriptor.Field(o => o.Simple)
                .Description("Whether or not this option should be considered a 'simple' option to show to the user.")
                .Type<NonNullType<BooleanType>>();
            descriptor.Field(o => o.Private)
                .Description("Whether or not tihs option should be showed to the user by default.")
                .Type<NonNullType<BooleanType>>();
            descriptor.Field(o => o.Flag).Description("Whether or not this option is a flag, " +
                "meaning it is not serialized to the configuration file, and instead triggers some side effect.")
                .Type<NonNullType<BooleanType>>();
            descriptor.Field(o => o.IsPath)
                .Description("Whether or not this option is a file path option.")
                .Deprecated("Prefer using `optionType`.")
                .Type<NonNullType<BooleanType>>();
            descriptor.Field(o => o.IsSelection)
                .Description("Whether or not this option is a selection option.")
                .Deprecated("Prefer using `optionType`.")
                .Type<NonNullType<BooleanType>>();
            descriptor.Field(o => o.Min)
                .Description("The minimum value allowed if this option is a numeric option.");
            descriptor.Field(o => o.Max)
                .Description("The maximum value allowed if this option is a numeric option.");
            descriptor.Field(o => o.Increment)
                .Description("The increment allowed if this option is a numeric option.");
            descriptor.Field(o => o.OptionType)
                .Description("The type of value this option will take.")
                .Type<NonNullType<ConfigurationOptionTypeEnum>>();
            descriptor.Field(o => o.PathType)
                .Description("If this option is a Path, the type of path value this option takes.")
                .Type<NonNullType<PathTypeEnum>>();
            descriptor.Field(o => o.Default)
                .Name("defaultValue")
                .Description("The default value of this option.")
                .Type<AnyType>();
            descriptor.Field(o => o.SelectionOptions)
                .Name("selectionDescriptors")
                .Type<NonNullType<ListType<NonNullType<SelectionOptionDescriptorType>>>>();
            descriptor.Field(o => o.CustomMetadata)
                .Description("Custom, plugin-defined metadata for this option, if any.")
                .Type<NonNullType<ListType<NonNullType<OptionMetadataType>>>>();
        }
    }
}
