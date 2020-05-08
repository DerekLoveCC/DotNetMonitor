using DotNetMonitor.UI.Utils;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Runtime;
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
            CompactMemoryCommand = new DelegateCommand(OnCompactMemory);
        }

        private void OnCompactMemory()
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
#pragma warning disable S1215 // "GC.Collect" should not be called
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
#pragma warning restore S1215 // "GC.Collect" should not be called
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
        public ICommand CompactMemoryCommand { get; }

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