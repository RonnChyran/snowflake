﻿using HotChocolate.Types;
using Snowflake.Configuration;
using Snowflake.Model.Game;
using Snowflake.Model.Game.LibraryExtensions;
using Snowflake.Remoting.GraphQL;
using Snowflake.Remoting.GraphQL.FrameworkQueries.Mutations.Relay;
using Snowflake.Services;
using Snowflake.Support.GraphQL.FrameworkQueries.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snowflake.Support.GraphQL.FrameworkQueries.Mutations.Configuration
{
    public sealed class ConfigurationMutations
        : ObjectTypeExtension
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Name("Mutation");
            descriptor.Field("updateGameConfigurationValues")
                .UseAutoSubscription()
                .UseClientMutationId()
                .Description("Updates the provided configuration values.")
                .Argument("input", a => a.Type<UpdateGameConfigurationValueInputType>())
                .Resolver(async ctx =>
                {
                    var configStore = ctx.SnowflakeService<IGameLibrary>().GetExtension<IGameConfigurationExtensionProvider>();
                    var input = ctx.Argument<UpdateGameConfigurationValueInput>("input");
                    var newValues = new List<IConfigurationValue>();
                    var newValueGuid = new List<(Guid valueCollection, Guid value)>();
                    foreach (var value in input.Values)
                    {
                        var owningGuid = await configStore.GetOwningValueCollectionAsync(value.ValueID);
                        if (owningGuid == default) continue; // value is an orphan or not found.

                        await configStore.UpdateValueAsync(value.ValueID, value.Value);
                        newValues.Add(new ConfigurationValue(value.Value, value.ValueID));
                        newValueGuid.Add((owningGuid, value.ValueID));
                    }
                    return new UpdateGameConfigurationValuePayload()
                    {
                        Values = newValues,
                        Collections = newValueGuid.GroupBy(k => k.valueCollection, v => v.value),
                    };
                }).Type<NonNullType<UpdateGameConfigurationValuePayloadType>>();
            descriptor.Field("updatePluginConfigurationValues")
                .UseAutoSubscription()
                .UseClientMutationId()
                .Description("Updates .")
                .Argument("input", a => a.Type<UpdatePluginConfigurationValueInputType>())
                .Resolver(async ctx =>
                {
                    var input = ctx.Argument<UpdatePluginConfigurationValueInput>("input");
                    var plugin = ctx.SnowflakeService<IPluginManager>().Get(input.Plugin);
                    if (plugin == null) throw new ArgumentException("Specified plugin was not found.");
                    var configStore = plugin?.Provision?.ConfigurationStore;
                    if (configStore == null) throw new ArgumentException("Specified plugin is not a provisioned plugin.");
                    var newValues = new List<Guid>();
                    foreach (var value in input.Values)
                    {
                        await configStore.SetAsync(new ConfigurationValue(value.Value, value.ValueID));
                        newValues.Add(value.ValueID);
                    }

                    var configuration = plugin.GetPluginConfiguration();
                    return new UpdatePluginConfigurationValuePayload()
                    {
                        Values = newValues.Select(g => configuration.ValueCollection[g].value).Where(v => v != null).ToList(),
                        Plugin = plugin,
                    };
                }).Type<NonNullType<UpdatePluginConfigurationValuePayloadType>>();
        }
    }
}
