using System.Collections.Generic;

namespace DotNetMonitor.Common.Analyzer
{
    public interface IProcessAnalyzer
    {
        void Init();

        ProcessBasicInfo GetProcessBasicInfo();

        IList<ProcessModuleInfo> GetModules();
    }
}