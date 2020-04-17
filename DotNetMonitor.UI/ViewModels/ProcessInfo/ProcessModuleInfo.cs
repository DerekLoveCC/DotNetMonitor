using System.Diagnostics;

namespace DotNetMonitor.UI.ViewModels.ProcessInfo
{
    [DebuggerDisplay("{Name}", Name = "Name")]
    public class ProcessModuleInfo
    {
        public string Name { get; set; }

    }
}