namespace DotNetMonitor.Common
{
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    public static class Injector
    {
        private static string GetSuffix(WindowInfo windowInfo)
        {
            return windowInfo.IsOwningProcess64Bit ? "64" : "32";
        }

        public static void Launch(WindowInfo windowInfo, Assembly assembly, string className, string methodName)
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(location) ?? string.Empty;
            var file = Path.Combine(directory, $"DotNetMonitor.ManagedInjectorLauncher{GetSuffix(windowInfo)}.exe");

            if (!File.Exists(file))
            {
                string message = $"Cannot find launcher {file}";
                throw new FileNotFoundException(message, file);
            }

            var startInfo = new ProcessStartInfo(file, $"{windowInfo.HWnd} \"{assembly.Location}\" \"{className}\" \"{methodName}\"")
            {
                Verb = windowInfo.IsOwningProcessElevated ? "runas" : null
            };

            using (var process = Process.Start(startInfo))
            {
                process?.WaitForExit();
            }
        }
    }
}