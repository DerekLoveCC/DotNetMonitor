using Prism.Events;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DotNetMonitor.UI.ViewModels
{
    public class NavigationViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        public NavigationViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
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

        public void LoadProcesses()
        {
            ProcessList = Process.GetProcesses().OrderBy(p => p.Id)
                                                .Select(p => new NavigationItemViewModel(_eventAggregator)
                                                {
                                                    Id = p.Id,
                                                    DisplayMember = p.ProcessName,
                                                }).ToList();
        }
    }
}