using System.Diagnostics;
using System.Threading.Tasks;

namespace DotNetMonitor.UI.ViewModels
{
    public class ProcessDetail
    {
        public ProcessDetail(int processId)
        {
            Id = processId;
        }

        public int Id { get; }

        internal Task InitializeAsync()
        {
            return Task.Run(() =>
            {
                var process = Process.GetProcessById(Id);
            });
        }
    }
}