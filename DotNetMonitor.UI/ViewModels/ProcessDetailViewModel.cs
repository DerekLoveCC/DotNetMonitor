using DotNetMonitor.Common.Analyzer;
using Prism.Mvvm;
using System.Collections.Generic;

namespace DotNetMonitor.UI.ViewModels
{
    public class ProcessDetailViewModel : BindableBase
    {
        private IProcessAnalyzer _processAnalyzer;

        public ProcessDetailViewModel(int processId)
        {
            _processId = processId;
            _processAnalyzer = new ClrMDLiveProcessAnalyzer(processId);
        }

        #region Binding Properties

        private readonly int _processId;

        public int ProcessId
        { get { return _processId; } }

        private string _gcMode;

        public string GCMode
        {
            get { return _gcMode; }
            set { SetProperty(ref _gcMode, value); }
        }

        private IList<ProcessModuleInfo> _modules;

        public IList<ProcessModuleInfo> Modules
        {
            get { return _modules; }
            set { SetProperty(ref _modules, value); }
        }

        #endregion Binding Properties

        internal void Init()
        {
            _processAnalyzer.Init();
            var processBasicInfo = _processAnalyzer.GetProcessBasicInfo();
            GCMode = processBasicInfo.GCMode;
            Modules = _processAnalyzer.GetModules();
        }
    }
}