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


        private NavigationItemViewModel _selectedProcess;

        public NavigationItemViewModel SelectedProcess
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

        private IList<NavigationItemViewModel> _processList;

        public IList<NavigationItemViewModel> ProcessList
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
            ProcessList = System.Diagnostics.Process.GetProcesses().OrderBy(p => p.Id).Select(p => new NavigationItemViewModel
            {
                Id = p.Id,
                DisplayMember = p.ProcessName,
            }).ToList();
        }

    }
}