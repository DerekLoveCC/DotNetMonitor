using DotNetMonitor.Common;
using Microsoft.Diagnostics.Runtime;
using System.Diagnostics;
using System.Linq;

namespace DotNetMonitor64
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var processId = args[0].ToNullableInt();
            if (processId == null)
            {
                processId = Process.GetProcessesByName(args[0]).FirstOrDefault()?.Id;
            }
            using (DataTarget dataTarget = DataTarget.AttachToProcess(processId.Value, 500, AttachFlag.NonInvasive))
            {
                foreach (var clrVersion in dataTarget.ClrVersions)
                {
                    var runTime = clrVersion.CreateRuntime();
                }
            }
        }
    }
}