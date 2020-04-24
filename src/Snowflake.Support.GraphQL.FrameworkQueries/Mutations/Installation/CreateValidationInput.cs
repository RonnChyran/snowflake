﻿using HotChocolate.Types;
using Snowflake.Remoting.GraphQL.FrameworkQueries.Mutations.Relay;
using Snowflake.Remoting.GraphQL.Model.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Snowflake.Support.GraphQL.FrameworkQueries.Mutations.Installation
{
    internal sealed class CreateValidationInput
        : RelayMutationBase
    {
        public Guid GameID { get; set; }
        public string Orchestrator { get; set; }
    }

    internal sealed class CreateValidationInputType
        : InputObjectType<CreateValidationInput>
    {
        protected override void Configure(IInputObjectTypeDescriptor<CreateValidationInput> descriptor)
        {
            descriptor.Name(nameof(CreateValidationInput))
                .WithClientMutationId();

            descriptor.Field(i => i.GameID)
                .Name("gameId")
                .Type<NonNullType<UuidType>>();
            descriptor.Field(i => i.Orchestrator)
                .Type<NonNullType<ListType<NonNullType<StringType>>>>();
        }
    }

}
