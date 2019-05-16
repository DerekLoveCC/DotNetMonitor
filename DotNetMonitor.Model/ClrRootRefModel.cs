namespace DotNetMonitor.Model
{
    public class ClrRootRefModel
    {
        public string Name { get; set; }

        public ulong Address { get; set; }

        public ClrObjectModel RootObject { get; set; }
        public ulong RootObjectAddress { get; set; }
        public string RootObjectTypeName { get; set; }

        public uint ObjectCount { get; set; }
    }
}