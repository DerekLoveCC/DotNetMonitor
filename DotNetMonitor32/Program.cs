using DotNetMonitor.Common;
using System;
using System.Diagnostics;
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
                //ProcessInfoCollector.Test(processId.Value);
                var processInfoCollector = new ProcessInfoCollector(processId.Value);
                var model = processInfoCollector.Collect();
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error(ex);
                Console.Read();
            }
        }
    }
}