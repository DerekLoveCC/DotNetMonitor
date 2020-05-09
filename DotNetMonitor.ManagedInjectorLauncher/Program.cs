// (c) Copyright Cory Plotts.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using DotNetMonitor.Common;
using DotNetMonitorManagedInjector;
using System;
using System.Diagnostics;

namespace DotNetMonitorManagedInjectorLauncher
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (!Enum.TryParse(args[0], true, out InjectAction injectAction))
            {
                Console.WriteLine("Unknown inject action");
                return;
            }
            switch (injectAction)
            {
                case InjectAction.Inject:
                    Inject(args);
                    break;

                case InjectAction.ProcessInfo:
                    PrintInjectInfo(args);
                    break;
            }
        }

        private static void PrintInjectInfo(string[] args)
        {
            if (!int.TryParse(args[1], out int processId))
            {
                return;
            }

            var process = Process.GetProcessById(processId);

            try
            {
                foreach (ProcessModule module in process.Modules)
                {
                    Console.WriteLine(module.ModuleName);
                }
            }
            catch
            {
            }
        }

        private static void Inject(string[] args)
        {
            var windowHandle = (IntPtr)long.Parse(args[1]);
            var assemblyName = args[2];
            var className = args[3];
            var methodName = args[4];

            var injectorData = new InjectorData
            {
                AssemblyName = assemblyName,
                ClassName = className,
                MethodName = methodName
            };

            Injector.Launch(windowHandle, injectorData);

            //check to see that it was injected, and if not, retry with the main window handle.
            var process = GetProcessFromWindowHandle(windowHandle);
            if (process != null && !CheckInjectedStatus(process) && process.MainWindowHandle != windowHandle)
            {
                Injector.LogMessage("Could not inject with current handle... retrying with MainWindowHandle", true);
                Injector.Launch(process.MainWindowHandle, injectorData);
                CheckInjectedStatus(process);
            }
        }

        private static Process GetProcessFromWindowHandle(IntPtr windowHandle)
        {
            NativeMethods.GetWindowThreadProcessId(windowHandle, out int processId);
            if (processId == 0)
            {
                Injector.LogMessage(string.Format("could not get process for window handle {0}", windowHandle), true);
                return null;
            }

            var process = Process.GetProcessById(processId);
            if (process == null)
            {
                Injector.LogMessage(string.Format("could not get process for PID = {0}", processId), true);
                return null;
            }
            return process;
        }

        private static bool CheckInjectedStatus(Process process)
        {
            bool containsFile = false;
            process.Refresh();
            foreach (ProcessModule module in process.Modules)
            {
                if (module.FileName.Contains("ManagedInjector"))
                {
                    containsFile = true;
                }
            }
            if (containsFile)
            {
                Injector.LogMessage($"Successfully injected for process {process.ProcessName} (PID = {process.Id})", true);
            }
            else
            {
                Injector.LogMessage($"Failed to inject for process {process.ProcessName} (PID = {process.Id})", true);
            }

            return containsFile;
        }
    }
}