using System.Windows.Threading;

namespace DotNetMonitor.UI.Utils
{
    public static class UIDispatcher
    {
        public static Dispatcher Dispatcher { get; internal set; }
    }
}