using CSharpExtensionMethods;
using DotNetMonitor.Common;
using DotNetMonitor.Common.NativeMethod;
using DotNetMonitor.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Xml.XPath;

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
            var processInfoList = WmiUtil.QueryProcesses().AsParallel()
                                                          .Select(mgmtObj => BuildProcessInfo(mgmtObj))
                                                          .OrderBy(p => p.ProcessId);

            //var processInfoList = Process.GetProcesses().AsParallel()
            //                                            .Select(p => BuildProcessInfo(p))
            //                                            .OrderBy(p => p.ProcessId);

            return new List<ProcessInfoViewModel>(processInfoList);
        }

        private static ProcessInfoViewModel BuildProcessInfo(Process process)
        {
            var result = new ProcessInfoViewModel(process.Id);
            PopulateInfo(result, process);
            return result;
        }

        private static ProcessInfoViewModel BuildProcessInfo(ManagementObject mgmtObj)
        {
            var result = new ProcessInfoViewModel();
            foreach (var property in mgmtObj.Properties)
            {
                switch (property.Name)
                {
                    case nameof(ProcessInfoViewModel.CommandLine):
                        result.CommandLine = property.Value?.ToString();
                        break;

                    case nameof(ProcessInfoViewModel.ProcessId):
                        result.ProcessId = property.Value.ToNullableInt().GetValueOrDefault();
                        break;

                    case nameof(ProcessInfoViewModel.SessionId):
                        result.SessionId = property.Value.ToNullableInt();
                        break;

                    case nameof(ProcessInfoViewModel.Name):
                        result.Name = property.Value?.ToString();
                        break;

                    case nameof(ProcessInfoViewModel.Description):
                        result.Description = property.Value?.ToString();
                        break;

                    case nameof(ProcessInfoViewModel.ExecutablePath):
                        result.ExecutablePath = property.Value?.ToString();
                        break;
                }
            }
            if (result.ProcessId != null)
            {
                result.IsX64 = CheckProcessBit(result.ProcessId.Value, out string error);
                result.Error = error;
            }

            return result;
        }


        public static void SetIsDotNetFlag(ProcessInfoViewModel processInfo)
        {
            if (processInfo == null || processInfo.IsNetProcess != null)
            {
                return;
            }

            try
            {
                processInfo.Handle = Process.GetProcessById(processInfo.ProcessId.Value).Handle;
            }
            catch
            {
                processInfo.IsNetProcess = false;
                return;
            }

            if (processInfo.Handle != null)
            {
                processInfo.Modules = GetProcessModuleInfos(processInfo);
                processInfo.IsNetProcess = CheckDotNetProcess(processInfo);
            }
        }

        private static IList<ProcessModuleInfo> GetProcessModuleInfos(ProcessInfoViewModel result)
        {
            if (result.Handle == null)
            {
                return new List<ProcessModuleInfo>();
            }

            var modules = ProcessNativeMethods.GetProcessModules(result.Handle.Value);
            return modules.Select(m => new ProcessModuleInfo { Name = m }).ToList();
        }

        public static void PopulateInfo(ProcessInfoViewModel processInfo, Process process)
        {
            processInfo.ProcessId = process.Id;
            processInfo.Name = process.ProcessName;
            processInfo.SessionId = process.SessionId;
            processInfo.IsX64 = CheckProcessBit(process, out string error);
            processInfo.Error = error;
            processInfo.Modules = GetProcessModuleInfos(process, processInfo);
            processInfo.IsNetProcess = CheckDotNetProcess(processInfo);
        }

        private static bool? CheckProcessBit(Process p, out string error)
        {
            return CheckProcessBit(p.Id, out error);
        }

        private static bool? CheckProcessBit(int processId, out string error)
        {
            error = null;
            try
            {
                return WindowInfo.IsProcess64Bit(processId);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                Debug.WriteLine(ex);
                return null;
            }
        }

        private static IList<ProcessModuleInfo> GetProcessModuleInfos(Process process, ProcessInfoViewModel processInfoViewModel)
        {
            var isX64 = processInfoViewModel.IsX64;
            if (isX64 == null)
            {
                return new List<ProcessModuleInfo>();
            }

            try
            {
                if ((Environment.Is64BitProcess && isX64 == true) || (!Environment.Is64BitProcess && isX64 == false))
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
            catch (Exception ex)
            {
                processInfoViewModel.Error = ex.Message;

                return new List<ProcessModuleInfo>();
            }
        }

        private static bool CheckDotNetProcess(ProcessInfoViewModel process)
        {
            return process.Modules.Any(m => m.Name.Contains("clr.dll"));
        }
    }
}