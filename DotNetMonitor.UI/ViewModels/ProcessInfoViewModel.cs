using DotNetMonitor.Common.NativeMethod;
using DotNetMonitor.UI.Utils;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace DotNetMonitor.UI.ViewModels
{
    [DebuggerDisplay("Id={Id}, Name={Name}")]
    public class ProcessInfoViewModel : BindableBase
    {
        public ProcessInfoViewModel(int pid)
        {
            TrimWorksetCommand = new DelegateCommand(OnTrimWorkset);
            //TracePerformanceCommand = new DelegateCommand(OnTracePerformance, () => IsNetProcess);
            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        private void OnRefresh()
        {
            var process = Process.GetProcessById(Id);
            ProcessUtil.PopulateInfo(this, process);
        }

        private void OnTrimWorkset()
        {
            var p = Process.GetProcessById(Id);
            var result = ProcessNativeMethods.EmptyWorkingSet(p);
            MessageBox.Show(result ? "Succeed" : "Failed");
        }

        public ICommand TrimWorksetCommand { get; }
        public ICommand TracePerformanceCommand { get; }
        public ICommand RefreshCommand { get; }

        public int Id { get; internal set; }
        public string Name { get; internal set; }

        public int SessionId { get; internal set; }
        public bool IsNetProcess { get; internal set; }
        public IList<ProcessModuleInfo> Modules { get; internal set; }

        public bool? IsX64 { get; internal set; }

        private string _tracePerformanceText = "Start to trace perforamnce";

        public string TracePerformanceText
        {
            get { return _tracePerformanceText; }
            set
            {
                if (_tracePerformanceText != value)
                {
                    _tracePerformanceText = value;
                    RaisePropertyChanged(nameof(TracePerformanceText));
                }
            }
        }
    }
}