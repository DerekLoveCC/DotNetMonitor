using DotNetMonitor.Common.NativeMethod;
using DotNetMonitor.UI.Utils;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DotNetMonitor.UI.ViewModels
{
    public class ProcessListViewModel : BindableBase
    {
        public ProcessListViewModel()
        {
            RowDoulbeClickCommand = new DelegateCommand(OnRowDouleClick);
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

        private ProcessInfoViewModel _selectedRow;

        public ProcessInfoViewModel SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                if (_selectedRow != value)
                {
                    _selectedRow = value;
                    RaisePropertyChanged(nameof(SelectedRow));
                }
            }
        }

        public ICommand RowDoulbeClickCommand { get; }

        private void OnRowDouleClick()
        {

        }

        internal async Task LoadProcesses()
        {
            var processInfoList = await ProcessUtil.LoadProcessesAsync();
            Processes = new ObservableCollection<ProcessInfoViewModel>(processInfoList);
        }
    }
}