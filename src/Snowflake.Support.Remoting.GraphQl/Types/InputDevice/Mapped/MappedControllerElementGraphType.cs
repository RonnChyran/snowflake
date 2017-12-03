﻿using GraphQL.Types;
using Snowflake.Input.Controller.Mapped;
using Snowflake.Support.Remoting.GraphQl.Types.ControllerLayout;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snowflake.Support.Remoting.GraphQl.Types.InputDevice.Mapped
{
    public class MappedControllerElementGraphType : ObjectGraphType<IMappedControllerElement>
    {
        public MappedControllerElementGraphType()
        {
            Name = "MappedControllerElement";
            Field<ControllerElementEnum>("deviceElement",
                description: "The element on the real input device.",
                resolve: context => context.Source.DeviceElement);
            Field<ControllerElementEnum>("layoutElement",
                description: "The element on the emulated input device the real device element maps to.",
                resolve: context => context.Source.LayoutElement);
        }
    }
}