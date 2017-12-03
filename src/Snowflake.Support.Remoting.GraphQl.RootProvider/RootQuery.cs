﻿using GraphQL.Relay.Types;
using GraphQL.Types;
using GraphQL.Types.Relay;
using Snowflake.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snowflake.Support.Remoting.GraphQl.RootProvider
{
    internal class RootQuery : ObjectGraphType<object>
    {
        public RootQuery()
        {
            this.Name = "Query";
            this.Description = "The query root of Snowflake's GraphQL interface";
        }
    }
}
