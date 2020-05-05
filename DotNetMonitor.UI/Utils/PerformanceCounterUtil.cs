using System.Diagnostics;

namespace DotNetMonitor.UI.Utils
{
    public static class PerformanceCounterUtil
    {
        public static string GetPerformanceCounterInstance(int pid)
        {
            const string processCategory = "Process";
            var cat = new PerformanceCounterCategory(processCategory);

            string[] instances = cat.GetInstanceNames();
            foreach (string instance in instances)
            {
                using (var cnt = new PerformanceCounter(processCategory, "ID Process", instance, true))
                {
                    int val = (int)cnt.RawValue;
                    if (val == pid)
                    {
                        return instance;
                    }
                }
            }

            return null;
        }
    }
}