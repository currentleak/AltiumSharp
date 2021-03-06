﻿using OpenMcdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AltiumSharp
{
    public static class CFItemExtensions
    {
        public static CFItem TryGetChild(this CFItem item, string name)
        {
            return (CFItem)(item as CFStorage)?.TryGetStorage(name) ?? (item as CFStorage)?.TryGetStream(name);
        }

        public static CFItem GetChild(this CFItem item, string name)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (item is CFStorage storage)
            {
                return TryGetChild(item, name) ?? throw new ArgumentException($"Item '{name}' doesn't exists within storage '{storage.Name}'.", nameof(name));
            }
            else
            {
                throw new InvalidOperationException($"Item '{item.Name}' is a stream and cannot have child items.");
            }
        }

        public static IEnumerable<CFItem> Children(this CFItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var result = new List<CFItem>();
            if (item is CFStorage storage)
            {
                storage.VisitEntries(childItem => result.Add(childItem), false);
            }
            else
            {
                throw new InvalidOperationException($"Item '{item.Name}' is a stream and cannot have child items.");
            }
            return result;
        }
    }

    public static class CFStreamExtensions
    {
        public static MemoryStream GetMemoryStream(this CFStream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            return new MemoryStream(stream.GetData());
        }

        public static BinaryReader GetBinaryReader(this CFStream stream, Encoding encoding)
        {
            return new BinaryReader(stream.GetMemoryStream(), encoding, false);
        }

        public static BinaryReader GetBinaryReader(this CFStream stream)
        {
            return GetBinaryReader(stream, Encoding.UTF8);
        }
    }

    public static class CompoundFileExtensions
    {
        private static readonly Regex PathElementSplitter = new Regex(@"(?<!\\)\/+", RegexOptions.Compiled);

        public static CFItem TryGetItem(this CompoundFile cf, string path)
        {
            if (cf == null) throw new ArgumentNullException(nameof(cf));

            var pathElements = PathElementSplitter.Split(path);
            CFItem item = cf.RootStorage;
            foreach (var pathElement in pathElements)
            {
                item = item.TryGetChild(pathElement);
                if (item == null) break;
            }
            return item;
        }

        public static CFItem GetItem(this CompoundFile cf, string path)
        {
            return TryGetItem(cf, path) ?? throw new ArgumentException($"Storage or stream with path '{path}' doesn't exist.", nameof(path));
        }

        public static CFStorage TryGetStorage(this CompoundFile cf, string path)
        {
            return TryGetItem(cf, path) as CFStorage;
        }

        public static CFStorage GetStorage(this CompoundFile cf, string path)
        {
            return TryGetStorage(cf, path) ?? throw new ArgumentException($"Storage with path '{path}' doesn't exist.", nameof(path));
        }

        public static CFStream TryGetStream(this CompoundFile cf, string path)
        {
            return TryGetItem(cf, path) as CFStream;
        }

        public static CFStream GetStream(this CompoundFile cf, string path)
        {
            return TryGetStream(cf, path) ?? throw new ArgumentException($"Stream with path '{path}' doesn't exist.", nameof(path));
        }
    }
}
