﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Snowflake.Configuration;
using Snowflake.Extensibility.Configuration;
using Snowflake.Filesystem;

namespace Snowflake.Extensibility.Provisioning
{
    /// <summary>
    /// The plugin provisions provided by the plugin manager
    /// </summary>
    public interface IPluginProvision
    {
        /// <summary>
        /// Gets the logger for the plugin
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Gets the plugin's properties
        /// </summary>
        IPluginProperties Properties { get; }

        /// <summary>
        /// Gets the plugin's configuration store
        /// </summary>
        IPluginConfigurationStore ConfigurationStore { get; }

        /// <summary>
        /// Gets the plugin's name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the author of the plugin.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Gets a short description of the plugin
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the version of the plugin
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Gets this plugin's content directory, or the root location of the plugin.
        /// </summary>
        DirectoryInfo ContentDirectory { get; }

        /// <summary>
        /// Gets the plugin's data directory
        /// </summary>
        IDirectory DataDirectory { get; }

        /// <summary>
        /// Gets the plugin's resource directory
        /// </summary>
        IDirectory ResourceDirectory { get; }

        /// <summary>
        /// Gets the resource directory common to the plugin's module.
        /// </summary>
        IDirectory CommonResourceDirectory { get; }
    }
}
