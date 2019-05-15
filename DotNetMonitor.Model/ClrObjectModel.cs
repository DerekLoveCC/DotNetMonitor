using System.Collections.Generic;

namespace DotNetMonitor.Model
{
    public class ClrObjectModel
    {
        public uint RefCount { get; set; }
        public string TypeName { get; set; }
        public ulong TotalSize { get; set; }
        public ulong OwnSize { get; set; }
        public int Gen { get; set; }
        public ulong InnerId { get; set; }

        public uint ChildCount { get; set; }

        public IList<ClrObjectModel> ReferencedObjects { get; set; }
    }
}