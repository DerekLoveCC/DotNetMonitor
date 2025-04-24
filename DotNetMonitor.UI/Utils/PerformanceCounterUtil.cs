using CSharpExtensionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DotNetMonitor.UI.Utils
{
    public static class PerformanceCounterUtil
    {
        private static IDictionary<int, string> _pidToInstance = new Dictionary<int, string>();

        public static void RefreshInstances()
        {
            _pidToInstance.Clear();

            const string processCategory = "Process";
            var cat = new PerformanceCounterCategory(processCategory);

            foreach (string instance in cat.GetInstanceNames())
            {
                try
                {
                    using (var cnt = new PerformanceCounter(processCategory, "ID Process", instance, true))
                    {
                        int val = (int)cnt.RawValue;
                        if (!_pidToInstance.ContainsKey(val))
                        {
                            _pidToInstance.Add(val, instance);
                        }
                    }
                }
                catch { }
            }
        }

        public static string GetPerformanceCounterInstance(int pid)
        {
            if (_pidToInstance.ContainsKey(pid))
            {
                return _pidToInstance[pid];
            }

            return null;
        }

        public static IList<PerformanceCounterCategory> GetDotNetPerformanceCategories()
        {
            return [.. PerformanceCounterCategory.GetCategories().Where(predicate: static c => c.CategoryName.StartsWithIgnoreCase(".Net"))];
        }
    }
}