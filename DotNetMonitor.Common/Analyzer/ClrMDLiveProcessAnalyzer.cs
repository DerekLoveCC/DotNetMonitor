using Microsoft.Diagnostics.Runtime;
using System;

namespace DotNetMonitor.Common.Analyzer
{
    public class ClrMDLiveProcessAnalyzer : ClrMDAnalyzer, IProcessAnalyzer
    {
        private readonly int _processId;
        private DataTarget _dataTarget;

        public ClrMDLiveProcessAnalyzer(int processId)
        {
            _processId = processId;
        }

        #region ClrMDAnalyzer Override Methods

        public override void Dispose()
        {
            _dataTarget.Dispose();
        }

        #endregion ClrMDAnalyzer Override Methods

        #region IProcessAnalyzer Members

        public ProcessBasicInfo GetProcessBasicInfo()
        {
            var processBasicInfo = new ProcessBasicInfo();
            using (var runtime = CreateClrRuntime(_dataTarget))
            {
                processBasicInfo.ProcessId = _processId;
                processBasicInfo.GCMode = runtime.Heap.IsServer ? "Server" : "Workstation";
            }

            return processBasicInfo;
        }

        public void Init()
        {
            _dataTarget = DataTarget.AttachToProcess(_processId, false);
            VerifyDataTarget(_dataTarget);
        }

        #endregion IProcessAnalyzer Members
    }
}