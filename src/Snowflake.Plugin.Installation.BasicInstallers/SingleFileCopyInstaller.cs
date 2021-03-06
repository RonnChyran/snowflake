﻿using System;
using System.Collections.Generic;
using System.IO;
using Snowflake.Installation;
using Snowflake.Model.Game;
using System.Linq;
using Snowflake.Installation.Tasks;
using Snowflake.Model.Game.LibraryExtensions;
using Snowflake.Installation.Extensibility;
using Snowflake.Extensibility.Provisioning;
using Snowflake.Extensibility;
using Snowflake.Services;
using Snowflake.Filesystem;
using Snowflake.Romfile;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Snowflake.Plugin.Installation.BasicInstallers
{
    [Plugin("BasicCopyInstaller")]
    [SupportedPlatform("ATARI_2600")]
    [SupportedPlatform("ATARI_5200")]
    [SupportedPlatform("ATARI_7800")]
    [SupportedPlatform("NINTENDO_GB")]
    [SupportedPlatform("NINTENDO_GBC")]
    [SupportedPlatform("NINTENDO_GBA")]
    [SupportedPlatform("NINTENDO_NDS")]
    [SupportedPlatform("NINTENDO_NES")]
    [SupportedPlatform("NINTENDO_FDS")]
    [SupportedPlatform("NINTENDO_SNES")]
    [SupportedPlatform("NINTENDO_N64")]
    [SupportedPlatform("NINTENDO_N64DD")]
    [SupportedPlatform("SEGA_GEN")]
    [SupportedPlatform("SEGA_GG")]
    [SupportedPlatform("SEGA_SMS")]
    public class SingleFileCopyInstaller
        : GameInstaller
    {
        public SingleFileCopyInstaller(IStoneProvider stone) : base(typeof(SingleFileCopyInstaller))
        {
            this.StoneProvider = stone;
        }

        public SingleFileCopyInstaller(IPluginProvision provision, IStoneProvider stone) : base(provision)
        {
            this.StoneProvider = stone;
        }

        public IStoneProvider StoneProvider { get; }

        public override IEnumerable<IInstallable> GetInstallables(PlatformId platformId, IEnumerable<FileSystemInfo> fileEntries)
        {
            foreach (var entry in fileEntries)
            {
                if (entry is FileInfo file) {
                    using var stream = file.OpenRead();
                    string mimetype = this.StoneProvider.GetStoneMimetype(platformId, stream, file.Extension);
                    if (mimetype == String.Empty) continue;
                    string serial = this.StoneProvider.GetFileSignature(mimetype, stream)?
                        .GetSerial(stream);
                    if (serial is null || serial is "")
                    {
                        yield return new Installable(new[] { file }, file, this);
                        continue;
                    }
                    yield return new Installable(new[] { file }, $"{file.Name} ({serial})", this);
                }
            }
        }

        public override async IAsyncEnumerable<TaskResult<IFile>> Install(IGame game, IEnumerable<FileSystemInfo> files,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var platform =  game.Record.PlatformID;
           
            foreach (var file in files.Select(f => f as FileInfo))
            {
                if (file == null) continue;
                using var stream = file.OpenRead();
                string mimetype = this.StoneProvider.GetStoneMimetype(platform, stream, file.Extension);
                if (mimetype == String.Empty) continue;

                var copiedFile = await new CopyFileTask(file, game.WithFiles().ProgramRoot);
                yield return copiedFile;
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
                if (copiedFile.Error == null)
                {
                    game.WithFiles().RegisterFile(await copiedFile, mimetype);
                }
                else
                {
                    yield return await new FailureTask<IFile>($"Failed to install {file.Name} to game {game.Record.RecordID}", copiedFile.Error);
                    yield break;
                }
            }
        }
    }
}
