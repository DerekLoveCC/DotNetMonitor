using DotNetMonitor.Common;
using DotNetMonitor.Model;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
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
            if (processId == null)
            {
                return;
            }
            var clrModels = new List<ClrModel>();
            using (DataTarget dataTarget = DataTarget.AttachToProcess(processId.Value, 500, AttachFlag.NonInvasive))
            {
                foreach (var clrVersion in dataTarget.ClrVersions)
                {
                    var runTime = clrVersion.CreateRuntime();

                    clrModels.Add(CreateClrModel(clrVersion, runTime));

                }
            }
        }

        private static ClrModel CreateClrModel(ClrInfo clrVersion, ClrRuntime runtime)
        {
            return new ClrModel
            {
                Version = CreateVersion(clrVersion),
                PointerSize = runtime.PointerSize,
            };
        }

        private static Version CreateVersion(ClrInfo clrVersion)
        {
            return new Version(clrVersion.Version.Major, clrVersion.Version.Minor, clrVersion.Version.Patch, clrVersion.Version.Revision);
        }
    }
}