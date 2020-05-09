using DotNetMonitor.Common;
using DotNetMonitor.UI.ViewModels;
using System;
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

        private static ProcessInfoViewModel BuildProcessInfo(Process process)
        {
            var result = new ProcessInfoViewModel(process.Id);
            PopulateInfo(result, process);
            return result;
        }

        public static void PopulateInfo(ProcessInfoViewModel processInfo, Process process)
        {
            processInfo.Id = process.Id;
            processInfo.Name = process.ProcessName;
            processInfo.SessionId = process.SessionId;
            processInfo.IsX64 = CheckProcessBit(process);
            processInfo.Modules = GetProcessModuleInfos(process, processInfo.IsX64);
            processInfo.IsNetProcess = CheckDotNetProcess(processInfo);
        }

        private static bool? CheckProcessBit(Process p)
        {
            try
            {
                return WindowInfo.IsProcess64Bit(p);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        private static IList<ProcessModuleInfo> GetProcessModuleInfos(Process process, bool? isX64)
        {
            try
            {
                if (isX64 == null || (Environment.Is64BitProcess && isX64 == true) || (!Environment.Is64BitProcess && isX64 == false))
                {
                    return process.Modules.OfType<ProcessModule>()
                                          .Select(m => new ProcessModuleInfo { Name = m.ModuleName })
                                          .ToList();
                }
                else
                {
                    var modules = new List<ProcessModuleInfo>();
                    var proc = InjectorHelper.GetLaunchProcess(InjectAction.ProcessInfo, isX64.Value, process.Id);
                    proc.Start();
                    while (!proc.StandardOutput.EndOfStream)
                    {
                        string line = proc.StandardOutput.ReadLine();
                        modules.Add(new ProcessModuleInfo { Name = line });
                    }

                    return modules;
                }
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