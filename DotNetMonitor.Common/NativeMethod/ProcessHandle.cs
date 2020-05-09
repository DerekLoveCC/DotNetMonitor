using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;

namespace DotNetMonitor.Common.NativeMethod
{
    public class ProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private ProcessHandle()
            : base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        override protected bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(handle);
        }
    }
}