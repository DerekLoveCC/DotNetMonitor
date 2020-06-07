using DotNetMonitor.UI.Utils;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DotNetMonitor.UI.ViewModels
{
    public class ProcessListViewModel : BindableBase
    {
        public ProcessListViewModel()
        {
            RowDoulbeClickCommand = new DelegateCommand(OnRowDouleClick);
            PerformanceCounterViewModel = new PerformanceCounterViewModel();
        }

        private ObservableCollection<ProcessInfoViewModel> _processes;

        public ObservableCollection<ProcessInfoViewModel> Processes
        {
            get { return _processes; }
            set
            {
                if (_processes != value)
                {
                    _processes = value;
                    RaisePropertyChanged(nameof(Processes));
                }
            }
        }

        private ProcessInfoViewModel _selectedProcess;

        public ProcessInfoViewModel SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                if (_selectedProcess != value)
                {
                    _selectedProcess = value;
                    ProcessUtil.SetIsDotNetFlag(value);
                    RaisePropertyChanged(nameof(SelectedProcess));
                    PerformanceCounterViewModel.ChangeProcess(SelectedProcess);
                }
            }
        }

        internal void Select(int processId)
        {
            var process = Processes.FirstOrDefault(p => p.ProcessId == processId);
            if (process == null)
            {
                MessageBox.Show($"Cannot find process {processId}");
                return;
            }

            SelectedProcess = process;
        }

        private PerformanceCounterViewModel _performanceCounterViewModel;

        public PerformanceCounterViewModel PerformanceCounterViewModel
        {
            get { return _performanceCounterViewModel; }
            set
            {
                if (_performanceCounterViewModel != value)
                {
                    _performanceCounterViewModel = value;
                    RaisePropertyChanged(nameof(PerformanceCounterViewModel));
                }
            }
        }

        public ICommand RowDoulbeClickCommand { get; }

        private void OnRowDouleClick()
        {
        }

        internal async Task LoadProcessesAsync()
        {
            var processInfoList = await ProcessUtil.LoadProcessesAsync();
            Processes = new ObservableCollection<ProcessInfoViewModel>(processInfoList);
        }
    }
}