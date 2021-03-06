﻿using Snowflake.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snowflake.Orchestration.Projections
{
#pragma warning disable CS0618
    public sealed class DirectoryProjection : IDirectoryProjection
    {
        private DirectoryProjection? Parent { get; }

        private Dictionary<string, FileSystemInfo> ProjectionState { get; }

        private Dictionary<string, DirectoryProjection> Children { get; }

        public DirectoryProjection()
        {
            this.Parent = null;
            this.ProjectionState = new();
            this.Children = new();
        }

        private DirectoryProjection(DirectoryProjection parent)
        {
            this.ProjectionState = new();
            this.Children = new();
            this.Parent = parent;
        }

        public IDirectoryProjection Enter(string directoryName)
        {
            string realName = Path.GetFileName(directoryName);
            if (!Filesystem.Directory.IsValidFileName(realName))
                throw new DirectoryNotFoundException($"Name {realName} is invalid.");
            if (this.ProjectionState.ContainsKey(realName))
            {
                throw new ArgumentException($"Can not enter projected item {realName}.");
            }

            if (this.Children.TryGetValue(realName, out var projection))
                return projection;

            projection = new(this);
            this.Children.Add(realName, projection);
            return projection;
        }

        public IDirectoryProjection Exit()
        {
            if (this.Parent == null)
            {
                throw new InvalidOperationException("Can not exit out of the root directory of a projection.");
            }
            return this.Parent;
        }

        public IDirectoryProjection Project(string name, IFile file)
        {
            string realName = Path.GetFileName(name);
            if (!Filesystem.Directory.IsValidFileName(realName))
                throw new DirectoryNotFoundException($"Name {realName} is invalid.");
            if (this.Children.ContainsKey(realName))
            {
                throw new ArgumentException("Can not project into an existing projection level.");
            }
            this.ProjectionState[realName] = file.UnsafeGetPath();
            return this;
        }

        public IDirectoryProjection Project(string name, IDirectory dir)
        {
            string realName = Path.GetFileName(name);
            if (!Filesystem.Directory.IsValidFileName(realName))
                throw new DirectoryNotFoundException($"Name {realName} is invalid.");
            if (this.Children.ContainsKey(realName))
            {
                throw new ArgumentException("Can not project into an existing projection level.");
            }
            this.ProjectionState[realName] = dir.UnsafeGetPath();
            return this;
        }

        public IReadOnlyDirectory Mount(IDisposableDirectory autoDisposingDirectory)
            => this.Mount(autoDisposingDirectory, "");

        public IReadOnlyDirectory Mount(IDisposableDirectory autoDisposingDirectory, string mountRoot)
        {
            IDirectory mountDir;
            if ((mountRoot == "/" || mountRoot == "") &&
                !autoDisposingDirectory.EnumerateFiles().Any() &&
                !autoDisposingDirectory.EnumerateDirectories().Any())
            {
                mountDir = autoDisposingDirectory;
            }
            // ContainsFile checks for directories as well.
            else if (!autoDisposingDirectory.ContainsFile(mountRoot))
            {
                mountDir = autoDisposingDirectory.OpenDirectory(mountRoot);
            }
            else
            {
                throw new IOException("Can not mount projection on an non-empty directory or already existing mount point.");
            }

            if (this.Parent != null)
            {
                throw new InvalidOperationException("Can not mount subtreee of projection.");
            }

            var activeDir = mountDir;
            foreach (var (name, file) in this.ProjectionState)
            {
                switch (file)
                {
                    case DirectoryInfo dirInfo:
                        activeDir.LinkFrom(dirInfo, name);
                        break;
                    case FileInfo fileInfo:
                        activeDir.LinkFrom(fileInfo, name);
                        break;
                }
            }

            Stack<(string, DirectoryProjection)> projectionsToProcess = new(this.Children.Select((kvp) => (kvp.Key, kvp.Value)));
            while (projectionsToProcess.Count > 0)
            {
                var (projectionName, projectionLevel) = projectionsToProcess.Pop();
                activeDir = activeDir.OpenDirectory(projectionName);
                foreach (var (name, file) in projectionLevel.ProjectionState)
                {
                    switch (file)
                    {
                        case DirectoryInfo dirInfo:
                            activeDir.LinkFrom(dirInfo, name);
                            break;
                        case FileInfo fileInfo:
                            activeDir.LinkFrom(fileInfo, name);
                            break;
                    }
                }
                foreach (var (name, childProjection) in projectionLevel.Children)
                {
                    projectionsToProcess.Push((name, childProjection));
                }
            }
            return mountDir.AsReadOnly();
        }
    }
#pragma warning restore CS0618
}
