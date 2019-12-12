﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumsNET;
using Newtonsoft.Json.Linq;
using Snowflake.Configuration.Input;
using Snowflake.Input.Controller;
using Snowflake.Input.Device;

namespace Snowflake.JsonConverters
{
    internal class InputMappingConverter : JsonCreationConverter<IDeviceInputMapping>
    {
        /// <inheritdoc/>
        protected override IDeviceInputMapping Create(Type objectType, JObject jObject)
        {
            IDictionary<DeviceCapability, string> controllerElements = (from prop in
                    jObject.Value<JObject>("Controller").Properties()
                        .Concat(jObject.Value<JObject>("Keyboard").Properties())
                select new
                {
                    element = Enums.Parse<DeviceCapability>(prop.Name),
                    value = prop.Value.Value<string>(),
                }).ToDictionary(o => o.element, o => o.value);

            IEnumerable<string> deviceLayouts = jObject.Value<JArray>("DeviceLayouts").Values<string>();
            //InputApi inputApi = Enums.Parse<InputApi>(jObject.Value<string>("InputApi"));
            //return new InputMapping(controllerElements, inputApi, deviceLayouts)

                //InputApi inputApi = Enums.Parse<InputApi>(jObject.Value<string>("InputApi"));

            //todo
            return new InputMapping(controllerElements, InputDriverType.DirectInput);
        }
    }
}
