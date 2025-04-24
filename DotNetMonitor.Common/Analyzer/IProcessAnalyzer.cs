using System.Collections.Generic;

namespace DotNetMonitor.Common.Analyzer
{
    public interface IProcessAnalyzer
    {
        IList<ProcessModuleInfo> GetModules();

        ProcessBasicInfo GetProcessBasicInfo();

        void Init();
    }
}