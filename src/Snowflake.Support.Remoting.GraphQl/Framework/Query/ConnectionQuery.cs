﻿using GraphQL.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Snowflake.Support.Remoting.GraphQl.Framework.Query
{
    class ConnectionQuery
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public Type GraphType { get; set; }
        public QueryArguments Arguments { get; set; }
        public Type ItemsType { get; set; }
        public Type ReturnType { get; set; }
        public Func<ResolveFieldContext<object>, object> Resolver { get; set; }


    }
}
