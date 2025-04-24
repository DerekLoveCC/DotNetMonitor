using CSharpExtensionMethods;
using Microsoft.Diagnostics.Runtime;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

        public IList<ProcessModuleInfo> GetModules()
        {
            var fileNameToModuleMap = new Dictionary<string, ProcessModuleInfo>();

            foreach (var module in _dataTarget.EnumerateModules())
            {
                fileNameToModuleMap[module.FileName] = new ProcessModuleInfo
                {
                    Name = Path.GetFileName(module.FileName),
                    Version = module.Version.ToString(),
                    Path = module.FileName
                };
            }

            using (var runtime = CreateClrRuntime(_dataTarget))
            {
                foreach (var module in runtime.EnumerateModules())
                {
                    if (!fileNameToModuleMap.HasKey(module.AssemblyName))
                    {
                        fileNameToModuleMap[module.AssemblyName] = new ProcessModuleInfo
                        {
                            Name = Path.GetFileName(module.AssemblyName),
                            Version = GetFileVersion(module.AssemblyName),
                            Path = module.AssemblyName
                        };
                    }
                }
            }

            return fileNameToModuleMap.Values.ToList();
        }

        private string GetFileVersion(string fileName)
        {
            var version = FileVersionInfo.GetVersionInfo(fileName);

            return version.ToString();
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