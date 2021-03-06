﻿using HotChocolate.Language;
using HotChocolate.Subscriptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snowflake.Support.GraphQL.FrameworkQueries.Subscriptions
{
    internal abstract class EventMessage<TTopic>
    {
        public object? Payload { get; set; }
        public TTopic Topic { get; set; }
        public string ArgumentName { get; }
        public EventMessage(TTopic identifier, object payload, string argumentName)
        {
            this.Topic = identifier;
            this.Payload = payload;
            this.ArgumentName = argumentName;
        }
    }
}
