using System;
using System.Collections.Generic;

namespace DotNetMonitor.Model
{
    public class ClrModel
    {
        public Version Version { get; set; }

        public int PointerSize { get; set; }

        public string Platform
        {
            get
            {
                return PointerSize == 8 ? "x64" : "x86";
            }
        }

        public bool ServerGC { get; set; }

        public int HeapCount { get; set; }

        public string DacLocation { get; set; }

        public IList<ClrObjectModel> ClrObjects { get; set; }
        public IList<ClrRootRefModel> RootRefs { get; set; }
    }
}