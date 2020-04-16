﻿using HotChocolate.Types;
using Snowflake.Framework.Remoting.GraphQL.Schema;
using Snowflake.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snowflake.Support.GraphQLFrameworkQueries.Queries.Stone
{
    public class StoneQueries
       : ObjectTypeExtension
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Name("Query");
            descriptor.Field("stone")
                .Description("Provides access to Stone platform and controller definitions.")
                .Type<StoneProviderType>()
                .Resolver(ctx => ctx.Service<IStoneProvider>());
        }
    }
}
