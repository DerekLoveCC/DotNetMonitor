using DotNetMonitor.Common.NativeMethod;
using DotNetMonitor.UI.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetMonitor.UI.Utils
{
    public static class ProcessUtil
    {
        public static Task<IList<ProcessInfoViewModel>> LoadProcessesAsync()
        {
            return Task.Run(() => LoadProcesses());
        }

        public static IList<ProcessInfoViewModel> LoadProcesses()
        {
            var processInfoList = Process.GetProcesses().AsParallel()
                                                        .Select(p => BuildProcessInfo(p))
                                                        .OrderBy(p => p.Id);

            return new List<ProcessInfoViewModel>(processInfoList);
        }

        private static ProcessInfoViewModel BuildProcessInfo(Process p)
        {
            var result = new ProcessInfoViewModel
            {
                Id = p.Id,
                Name = p.ProcessName,
                SessionId = p.SessionId,
                WorkingSet = p.WorkingSet64,
                PrivateMemorySize = p.PrivateMemorySize64,
                Modules = GetProcessModuleInfos(p),
            };
            result.IsNetProcess = CheckDotNetProcess(result);
            result.IsX64 = CheckProcessBit(p);
            return result;
        }

        private static bool? CheckProcessBit(Process p)
        {
            try
            {
                return ProcessNativeMethods.Is64Bit(p);
            }
            catch
            {
                return null;
            }
        }

        private static IList<ProcessModuleInfo> GetProcessModuleInfos(Process process)
        {
            try
            {
                return process.Modules.OfType<ProcessModule>()
                                      .Select(m => new ProcessModuleInfo { Name = m.ModuleName })
                                      .ToList();
            }
            catch
            {
                return new List<ProcessModuleInfo>();
            }
        }

        private static bool CheckDotNetProcess(ProcessInfoViewModel process)
        {
            return process.Modules.Any(m => m.Name.Contains("clr.dll"));
        }
    }
}