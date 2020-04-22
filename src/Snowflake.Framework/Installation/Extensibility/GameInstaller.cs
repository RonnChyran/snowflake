﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Linq;
using Snowflake.Extensibility.Provisioning;
using Snowflake.Extensibility.Provisioning.Standalone;
using Snowflake.Model.Game;
using Snowflake.Filesystem;
using System.Threading;

namespace Snowflake.Installation.Extensibility
{
    /// <inheritdoc cref="IGameInstaller" />
    public abstract class GameInstaller
         : ProvisionedPlugin, IGameInstaller
    {
        protected GameInstaller(Type pluginType)
           : this(new StandalonePluginProvision(pluginType))
        {
        }

        /// <inheritdoc />
        protected GameInstaller(IPluginProvision provision)
            : base(provision)
        {
            this.SupportedPlatforms  = this.GetType()
                .GetCustomAttributes<SupportedPlatformAttribute>().Select(p => p.PlatformId).ToList();
        }

        /// <inheritdoc />
        public IEnumerable<PlatformId> SupportedPlatforms { get; }

        /// <inheritdoc />
        public abstract IEnumerable<IInstallable> GetInstallables(PlatformId platformId, IEnumerable<FileSystemInfo> fileEntries);

        /// <inheritdoc />
        public abstract IAsyncEnumerable<TaskResult<IFile>> Install(IGame game, IEnumerable<FileSystemInfo> files, CancellationToken cancellationToken = default);
    }
}
