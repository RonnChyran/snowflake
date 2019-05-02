﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Snowflake.Filesystem;
using Snowflake.Model.Game;
using A = System.Collections.Generic;

namespace Snowflake.Installation.Tasks
{
    public sealed class CopyFileTask : InstallTaskAwaitable<IFile?>
    {
        public CopyFileTask(FileInfo source, IDirectory destinationDirectory)
        {
            this.Source = source;
            this.Destination = destinationDirectory;
        }
        private FileInfo Source { get; }
        private IDirectory Destination { get; }

        protected override string TaskName => "Copy";

        protected override async Task<IFile?> Execute()
        {
            return await this.Destination.CopyFromAsync(this.Source);
        }
    }
}
