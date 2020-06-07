using DotNetMonitor.Common;
using DotNetMonitor.UI.Utils;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Data.SqlClient;
using System.Runtime;
using System.Threading.Tasks;
using System.Windows;
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
            CustomizeCommand = new DelegateCommand(OnCustomize);
            OnFindAction = new Action<WindowInfo>(OnFind);
        }

        private void OnFind(WindowInfo windowInfo)
        {
            if (windowInfo.HWnd != IntPtr.Zero)
            {
                NativeMethods.GetWindowThreadProcessId(windowInfo.HWnd, out int processId);

                ProcessListViewModel.Select(processId);
            }
        }

        private void OnCustomize()
        {
            MessageBox.Show("On Customize");
        }

        private void OnCompactMemory()
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
#pragma warning disable S1215 // "GC.Collect" should not be called
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
#pragma warning restore S1215 // "GC.Collect" should not be called
            //GC.AddMemoryPressure(1024);
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

        #region Binding command

        public ICommand RefreshProcessListCommand { get; }
        public ICommand CompactMemoryCommand { get; }

        public ICommand CustomizeCommand { get; }
        public Action<WindowInfo> OnFindAction { get; }

        #endregion Binding command

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