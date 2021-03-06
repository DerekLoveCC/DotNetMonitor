﻿using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;

namespace DotNetMonitor.Common
{
    public static partial class NativeMethods
    {
        public class ToolHelpHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private ToolHelpHandle()
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
}