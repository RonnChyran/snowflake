﻿using HotChocolate.Types;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Snowflake.Filesystem;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphQL.Types;
using Zio;

namespace Snowflake.Framework.Remoting.GraphQL.Model.Filesystem.Contextual
{
    public sealed class OSFileInfoType
        : ObjectType<FileInfo>
    {
        protected override void Configure(IObjectTypeDescriptor<FileInfo> descriptor)
        {
            descriptor.Name("OSFileInfo")
                .Description("Describes a file in a realized filesystem.")
                .Interface<FileInfoInterface>();
            descriptor.Field("extension")
                .Description("The extension of the file.")
                .Type<StringType>()
                .Resolver(context => context.Parent<FileInfo>().Extension);
            descriptor.Field("name")
                .Description("The name of the file.")
                .Type<StringType>()
                .Resolver(context => context.Parent<FileInfo>().Name);
            descriptor.Field("osPath")
                .Description("The path of the file on the realized operating system.")
                .Type<OSFilePathType>()
                .Resolver(context => context.Parent<FileInfo>());
            descriptor.Field("lastModifiedTime")
                .Description("The last modified time of the file, in UTC.")
                .Type<DateTimeType>()
                .Resolver(context => context.Parent<FileInfo>().LastWriteTimeUtc);
            descriptor.Field("createdTime")
                .Description("The creation time of the file, in UTC.")
                .Type<DateTimeType>()
                .Resolver(context => context.Parent<FileInfo>().CreationTimeUtc);
            descriptor.Field("size")
                .Description("The size of the file, in bytes.")
                .Type<IntType>()
                .Resolver(context => context.Parent<FileInfo>().Length);
            descriptor.Field("path")
                .Description("The path to this file.")
                .Type<OSFilePathType>()
                .Resolver(context => context.Parent<FileInfo>());
            descriptor.Field("isHidden")
                .Description("Whether or not this file is hidden")
                .Type<BooleanType>()
                .Resolver(context => context.Parent<FileInfo>().Attributes.HasFlag(FileAttributes.Hidden));
        }
    }
}
