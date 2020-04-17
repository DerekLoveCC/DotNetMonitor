using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DotNetMonitor.Common.NativeMethod
{
    public static class ProcessNativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms684139%28v=vs.85%29.aspx
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static bool Is64Bit(Process process)
        {
            if (!Environment.Is64BitOperatingSystem)
            {
                return false;
            }
            // if this method is not available in your version of .NET, use GetNativeSystemInfo via P/Invoke instead

            if (!IsWow64Process(process.Handle, out bool isWow64))
            {
                throw new Win32Exception();
            }
            return !isWow64;
        }
    }
}