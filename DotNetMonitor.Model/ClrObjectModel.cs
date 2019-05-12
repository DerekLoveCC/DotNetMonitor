namespace DotNetMonitor.Model
{
    public class ClrObjectModel
    {
        public string TypeName { get; set; }
        public ulong Size { get; set; }
        public int Gen { get; set; }
        public ulong InnerId { get; set; }
    }
}