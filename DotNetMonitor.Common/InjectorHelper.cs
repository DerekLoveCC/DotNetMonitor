namespace DotNetMonitor.Common
{
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    public static class InjectorHelper
    {
        private static string GetSuffix(bool isX64)
        {
            return isX64 ? "64" : "32";
        }

        public static void InjectLaunch(WindowInfo windowInfo, Assembly assembly, string className, string methodName)
        {
            string file = GetProcessExeFile(windowInfo.IsOwningProcess64Bit);

            var argument = $"{InjectAction.Inject} {windowInfo.HWnd} \"{assembly.Location}\" \"{className}\" \"{methodName}\"";
            var startInfo = new ProcessStartInfo(file, argument)
            {
                Verb = windowInfo.IsOwningProcessElevated ? "runas" : null
            };

            using (var process = Process.Start(startInfo))
            {
                process?.WaitForExit();
            }
        }

        public static Process GetLaunchProcess(InjectAction injectAction, bool isX64, int processId)
        {
            string file = GetProcessExeFile(isX64);

            var argument = $"{injectAction} {processId}";
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = file,
                    Arguments = argument,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            return proc;
        }

        private static string GetProcessExeFile(bool isX64)
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(location) ?? string.Empty;
            var file = Path.Combine(directory, $"DotNetMonitor.ManagedInjectorLauncher{GetSuffix(isX64)}.exe");

            if (!File.Exists(file))
            {
                string message = $"Cannot find launcher {file}";
                throw new FileNotFoundException(message, file);
            }

            return file;
        }
    }
}