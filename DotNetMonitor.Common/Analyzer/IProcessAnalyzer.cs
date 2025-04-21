using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetMonitor.Common.Analyzer
{
    public interface IProcessAnalyzer
    {
        void Init();

        ProcessBasicInfo GetProcessBasicInfo();
    }
}
