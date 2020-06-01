using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace DotNetMonitor.Common.NativeMethod
{
    public static class ProcessNativeMethods
    {
        [DllImport("psapi")]
        private static extern bool EmptyWorkingSet(IntPtr hProcess);

        [DllImport("psapi.dll")]
        private static extern uint GetModuleFileNameEx(IntPtr hProcess,
                                               IntPtr hModule, [Out] StringBuilder lpBaseName,
                                               [In][MarshalAs(UnmanagedType.U4)] uint nSize);

        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool EnumProcessModulesEx(IntPtr hProcess,
                                                     [Out] IntPtr lphModule,
                                                     uint cb,
                                                     [MarshalAs(UnmanagedType.U4)] out uint lpcbNeeded,
                                                     uint flag);


        public static IList<string> GetProcessModules(IntPtr processHandle)
        {
            var result = new List<string>();
            // Setting up the variable for the second argument for EnumProcessModules
            var hMods = new IntPtr[1024];

            GCHandle gch = GCHandle.Alloc(hMods, GCHandleType.Pinned); // Don't forget to free this later
            try
            {
                IntPtr pModules = gch.AddrOfPinnedObject();

                // Setting up the rest of the parameters for EnumProcessModules
                uint uiSize = (uint)(Marshal.SizeOf(typeof(IntPtr)) * (hMods.Length));

                if (EnumProcessModulesEx(processHandle, pModules, uiSize, out uint cbNeeded, 0x03))
                {
                    int uiTotalNumberofModules = (int)(cbNeeded / (Marshal.SizeOf(typeof(IntPtr))));

                    for (int i = 0; i < uiTotalNumberofModules; i++)
                    {
                        StringBuilder strbld = new StringBuilder(1024);
                        GetModuleFileNameEx(processHandle, hMods[i], strbld, (uint)(strbld.Capacity));
                        result.Add(strbld.ToString());
                    }
                }

                return result;
            }
            finally
            {
                // Must free the GCHandle object
                gch.Free();
            }
        }

        public static IList<string> GetProcessModules(Process p)
        {
            return GetProcessModules(p.Handle);
        }

        public static bool EmptyWorkingSet(Process process)
        {
            return EmptyWorkingSet(process.Handle);
        }

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern ProcessHandle OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        public static ProcessHandle OpenProcess(Process proc, ProcessAccessFlags flags)
        {
            return OpenProcess(proc.Id, flags);
        }

        public static ProcessHandle OpenProcess(int processId, ProcessAccessFlags flags)
        {
            return OpenProcess(flags, false, processId);
        }
    }
}