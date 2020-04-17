using System.Collections.Generic;
using System.Diagnostics;

namespace DotNetMonitor.UI.ViewModels.ProcessInfo
{
    [DebuggerDisplay("Id={Id}, Name={Name}")]
    public class ProcessDetailInfo
    {
        public int Id { get; internal set; }
        public string Name { get; internal set; }
        public int SessionId { get; internal set; }
        public bool IsNetProcess { get; internal set; }
        public IList<ProcessModuleInfo> Modules { get; internal set; }

        public bool? IsX64 { get; internal set; }
    }
}