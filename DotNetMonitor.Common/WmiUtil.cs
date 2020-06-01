using System.Collections.Generic;
using System.Management;

namespace DotNetMonitor.Common
{
    public static class WmiUtil
    {
        public static IList<ManagementObject> QueryProcesses()
        {
            var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Process");

            var processes = new List<ManagementObject>();

            foreach (ManagementObject wmi in searcher.Get())
            {
                processes.Add(wmi);
            }

            return processes;
        }
    }
}