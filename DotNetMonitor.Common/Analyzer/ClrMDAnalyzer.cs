using Microsoft.Diagnostics.Runtime;
using System;
using System.Linq;

namespace DotNetMonitor.Common.Analyzer
{
    public abstract class ClrMDAnalyzer : IDisposable
    {
        public abstract void Dispose();

        protected void VerifyDataTarget(DataTarget dataTarget)
        {
            var isTarget64Bit = dataTarget.DataReader.PointerSize == 8;

            if (isTarget64Bit != Environment.Is64BitProcess)
            {
                throw new Exception(string.Format("Architecture mismatch:  Process is {0} but target is {1}", Environment.Is64BitProcess ? "64 bit" : "32 bit", isTarget64Bit ? "64 bit" : "32 bit"));
            }

            if (!dataTarget.ClrVersions.Any())
            {
                throw new Exception("Please make sure it is .NET process");
            }
        }

        protected ClrRuntime CreateClrRuntime(DataTarget dataTarget)
        {
            var clrInfo = dataTarget.ClrVersions[0];
            return clrInfo.CreateRuntime();
        }
    }
}