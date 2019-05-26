using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetMonitor.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            PopulateProcess();
        }


        private ProcessViewModel _selectedProcess;

        public ProcessViewModel SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                if (_selectedProcess != value)
                {
                    _selectedProcess = value;

                    RaisePropertyChanged(nameof(SelectedProcess));
                }
            }
        }

        private IList<ProcessViewModel> _processList;

        public IList<ProcessViewModel> ProcessList
        {
            get
            {
                return _processList;
            }
            set
            {
                if (_processList != value)
                {
                    _processList = value;
                    RaisePropertyChanged(nameof(ProcessList));
                }
            }
        }

        private void PopulateProcess()
        {
            ProcessList = System.Diagnostics.Process.GetProcesses().Select(p => new ProcessViewModel
            {
                Id = p.Id,
                Name = p.ProcessName,
            }).ToList();
        }

    }
}