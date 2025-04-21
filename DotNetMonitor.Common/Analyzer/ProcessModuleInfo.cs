using System.Diagnostics;

namespace DotNetMonitor.Common.Analyzer
{
    [DebuggerDisplay("{Name}", Name = "Name")]
    public class ProcessModuleInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }

    }
}