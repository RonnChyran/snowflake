﻿using System;
using Snowflake.Filesystem;
using Snowflake.Model.Records.File;

namespace Snowflake.Model.Records
{
    public class FileRecord : IFileRecord
    {
        /// <inheritdoc/>
        public IMetadataCollection Metadata { get; }

        /// <inheritdoc/>
        public Guid RecordID => File.FileGuid;

        public IFile File { get; }

        /// <inheritdoc/>
        public string MimeType { get; }

        internal FileRecord(IFile file, string mimeType, IMetadataCollection metadataCollection)
        {
            this.MimeType = mimeType;
            this.File = file;
            this.Metadata = metadataCollection;
        }
    }
}
