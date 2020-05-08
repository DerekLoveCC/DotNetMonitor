using DotNetMonitor.Common.NativeMethod;
using DotNetMonitor.UI.Utils;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DotNetMonitor.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(ProcessListViewModel processListViewModel)
        {
            ProcessListViewModel = processListViewModel;
            RefreshProcessListCommand = new DelegateCommand(OnRefreshProcessList);

            var s = Environment.Is64BitProcess;

            var isX64 = ProcessNativeMethods.Is64Bit(Process.GetCurrentProcess());
        }

        private ProcessListViewModel _processListViewModel;

        public ProcessListViewModel ProcessListViewModel
        {
            get { return _processListViewModel; }
            set
            {
                if (_processListViewModel != value)
                {
                    _processListViewModel = value;
                    RaisePropertyChanged(nameof(ProcessListViewModel));
                }
            }
        }

        internal async Task Initialize()
        {
            var loadProcessTask = ProcessListViewModel?.LoadProcessesAsync();
            var refreshInstanceTask = Task.Run(() => PerformanceCounterUtil.RefreshInstances());

            await Task.WhenAll(loadProcessTask, refreshInstanceTask);
        }

        public ICommand RefreshProcessListCommand { get; }

        private async void OnRefreshProcessList()
        {
            if (ProcessListViewModel?.Processes == null)
            {
                return;
            }

            ProcessListViewModel.PerformanceCounterViewModel?.Dispose();

            var loadProcessTask = ProcessListViewModel?.LoadProcessesAsync();
            var refreshInstanceTask = Task.Run(() => PerformanceCounterUtil.RefreshInstances());
            await Task.WhenAll(loadProcessTask, refreshInstanceTask);
        }
    }
}