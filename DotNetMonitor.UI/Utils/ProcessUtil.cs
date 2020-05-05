using DotNetMonitor.Common.NativeMethod;
using DotNetMonitor.UI.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

        private static ProcessInfoViewModel BuildProcessInfo(Process process)
        {
            var result = new ProcessInfoViewModel();
            PopulateInfo(result, process);
            return result;
        }

        public static void PopulateInfo(ProcessInfoViewModel processInfo, Process process)
        {
            processInfo.Id = process.Id;
            processInfo.Name = process.ProcessName;
            processInfo.SessionId = process.SessionId;
            processInfo.WorkingSet = process.WorkingSet64;
            processInfo.PrivateMemorySize = process.PrivateMemorySize64;
            processInfo.Modules = GetProcessModuleInfos(process);
            processInfo.IsNetProcess = CheckDotNetProcess(processInfo);
            processInfo.IsX64 = CheckProcessBit(process);
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