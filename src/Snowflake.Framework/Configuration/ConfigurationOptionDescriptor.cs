﻿using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;

namespace Snowflake.Configuration
{
    /// <summary>
    /// Default constructor for <see cref="IConfigurationOptionDescriptor"/>
    /// </summary>
    public class ConfigurationOptionDescriptor : IConfigurationOptionDescriptor
    {
        /// <inheritdoc/>
        public string DisplayName { get; }

        /// <inheritdoc/>
        public string Description { get; }

        /// <inheritdoc/>
        public bool Simple { get; }

        /// <inheritdoc/>
        public bool Private { get; }

        /// <inheritdoc/>
        public bool IsFlag { get; }

        /// <inheritdoc/>
        public double Max { get; }

        /// <inheritdoc/>
        public double Min { get; }

        /// <inheritdoc/>
        public double Increment { get; }

        /// <inheritdoc/>
        public bool IsPath => this.PathType != PathType.NotPath;

        /// <inheritdoc/>
        public PathType PathType { get; }

        /// <inheritdoc/>
        public string OptionName { get; }

        /// <inheritdoc/>
        public string OptionKey { get; }

        /// <inheritdoc/>
        public object Default { get; }

        /// <inheritdoc/>
        public bool IsSelection { get; }

        /// <inheritdoc/>
        public Type Type { get; }

        /// <inheritdoc/>
        public string Unset { get; }

        /// <inheritdoc/>
        public IDictionary<string, object> CustomMetadata { get; }

        /// <inheritdoc/>
        public ConfigurationOptionType OptionType { get; }

        /// <inheritdoc/>
        public IEnumerable<ISelectionOptionDescriptor> SelectionOptions { get; }

        /// <inheritdoc/>
        public string RootPath { get; }

        /// <inheritdoc/>
        public string Filter { get; }

        /// <inheritdoc/>
        public string ResourceType { get; }

        internal ConfigurationOptionDescriptor(ConfigurationOptionAttribute configOption,
            IEnumerable<CustomMetadataAttribute> customMetadata, string keyName)
        {
            this.Default = configOption.Default;
            this.Type = configOption.Default.GetType();
            this.Unset = configOption.Unset;
            this.DisplayName = configOption.DisplayName;
            this.Description = configOption.Description;
            this.PathType = configOption.PathType;
            this.Simple = configOption.Simple;
            this.Private = configOption.Private;
            this.IsFlag = configOption.Flag;
            this.Max = configOption.Max;
            this.Min = configOption.Min;
            this.CustomMetadata = customMetadata.ToDictionary(m => m.Key, m => m.Value);
            this.Increment = configOption.Increment;
            this.OptionName = configOption.OptionName;
            this.OptionKey = keyName;
            this.OptionType = ConfigurationOptionDescriptor.GetOptionType(this.Type, this.IsPath);
            this.IsSelection = this.OptionType == ConfigurationOptionType.Selection;
            this.SelectionOptions = this.IsSelection
                ? Enums.GetMembers(this.Type)
                    .Select(m => new SelectionOptionDescriptor(m))
                    .ToList()
                : Enumerable.Empty<ISelectionOptionDescriptor>();
            this.RootPath = configOption.RootPath;
            this.Filter = configOption.Filter;
            this.ResourceType = configOption.ResourceType;
        }
 
        private static ConfigurationOptionType GetOptionType(Type t, bool isPath)
        {
            if (t.IsEnum)
            {
                return ConfigurationOptionType.Selection;
            }

            if (t == typeof(Guid))
            {
                return ConfigurationOptionType.Resource;
            }

            if (t == typeof(int) || t == typeof(long))
            {
                return ConfigurationOptionType.Integer;
            }

            if (t == typeof(float) || t == typeof(double))
            {
                return ConfigurationOptionType.Decimal;
            }

            if (t == typeof(bool))
            {
                return ConfigurationOptionType.Boolean;
            }

            if (t == typeof(string))
            {
                if (isPath)
                {
                    return ConfigurationOptionType.Path;
                }

                return ConfigurationOptionType.String;
            }

            throw new ArgumentOutOfRangeException("Option must be a primitive, string, or Enum type.");
        }
    }
}
