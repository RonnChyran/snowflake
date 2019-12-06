﻿using Snowflake.Configuration.Input;
using Snowflake.Configuration.Serialization;
using Snowflake.Configuration.Serialization.Serializers.Implementations;
using Snowflake.Execution.Extensibility;
using Snowflake.Execution.Saving;
using Snowflake.Filesystem;
using Snowflake.Model.Game;
using Snowflake.Model.Game.LibraryExtensions;
using Snowflake.Plugin.Emulators.RetroArch;
using Snowflake.Plugin.Emulators.RetroArch.Adapters.Higan.Configuration;
using Snowflake.Plugin.Emulators.RetroArch.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snowflake.Adapters.Higan
{
    public class HiganGameEmulation : GameEmulation
    {
        public HiganGameEmulation(IGame game, Guid guid) : base(game, guid)
        {
            this.Scratch = this.Game.WithFiles().GetRuntimeLocation();
        }

        private IDirectory Scratch { get; }

        public override async Task PersistSaveGame(IDirectory targetDirectory)
        {
            var saveDirectory = this.Scratch.OpenDirectory("save");
            foreach (var file in saveDirectory.EnumerateFilesRecursive())
            {
                await targetDirectory.CopyFromAsync(file);
            }
        }

        public override Task RestoreSaveGame(SaveGame targetDirectory)
        {
            throw new NotImplementedException();
        }

        public override CancellationToken RunGame()
        {
            throw new NotImplementedException();
        }

        public override Task SetupEnvironment()
        {
            throw new NotImplementedException();
        }

        public async override Task CompileConfiguration()
        {
            var serializer = new SimpleCfgConfigurationSerializer();
            var tokenizer = new ConfigurationTraversalContext();
            var config = this.Game.WithConfigurations()
                .GetProfile<HiganRetroArchConfiguration>(nameof(HiganGameEmulation),
                this.ConfigurationProfile).Configuration;

            var node = tokenizer.TraverseCollection(config);
            var retroArchNode = node["#retroarch"];
            StringBuilder configContents = new StringBuilder();

            configContents.Append(serializer.Transform(retroArchNode));

            var configFile = this.Scratch.OpenDirectory("config")
                    .OpenFile("retroarch.cfg");

            foreach (var port in this.ControllerPorts)
            {
                var template = new InputTemplate<RetroPadTemplate>(port.LayoutMapping, port.PortIndex);
                var inputNode = tokenizer.TraverseInputTemplate(template, this.InputMappings, port.PortIndex);
                configContents.Append(serializer.Transform(retroArchNode));
            }
            configFile.WriteAllText(configContents.ToString());
        }

        protected override void TeardownGame()
        {
            throw new NotImplementedException();
        }
    }
}
