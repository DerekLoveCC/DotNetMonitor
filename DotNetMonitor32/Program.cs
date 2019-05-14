using DotNetMonitor.Common;
using DotNetMonitor.Model;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DotNetMonitor64
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
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
                //Test(processId.Value);
                var processInfoCollector = new ProcessInfoCollector(processId.Value);
                processInfoCollector.Collect();
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error(ex);
                Console.Read();
            }
        }

        private static void Test(int processId)
        {
            using (DataTarget dataTarget = DataTarget.AttachToProcess(processId, 500, AttachFlag.NonInvasive))
            {
                foreach (var clrVersion in dataTarget.ClrVersions)
                {
                    var runtime = clrVersion.CreateRuntime();
                    //PrintAppDomains(runtime, sw);
                    //PrintModules(runtime, sw);
                    //PrintThreadS(runtime, sw);
                    //PrintSegments(runtime, sw);
                    //PrintGCHandles(runtime, sw);
                    //PrintHeapSegments(runtime, sw);
                    //PrintLogicHeapBalance(runtime, sw);
                    PrintManagedObjectsBySegment(runtime);
                    //PrintManagedObjects(runtime, sw);
                }
            }
        }

       
    }
}