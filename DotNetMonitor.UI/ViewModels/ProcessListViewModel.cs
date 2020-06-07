using DotNetMonitor.UI.Utils;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
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

        public ICollectionView CollectionView
        {
            get
            {
                return Processes == null ? null : CollectionViewSource.GetDefaultView(Processes);
            }
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

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    RaisePropertyChanged(nameof(SearchText));
                    FilterProcesses(value);
                }
            }
        }

        private void FilterProcesses(string searchText)
        {
            var processList = Processes;
            if (processList == null)
            {
                return;
            }

            CollectionView.Filter = p => (p as ProcessInfoViewModel)?.Name.StartsWith(searchText, StringComparison.OrdinalIgnoreCase) == true;
        }

        internal void Select(int? processId)
        {
            if (processId == null)
            {
                return;
            }

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