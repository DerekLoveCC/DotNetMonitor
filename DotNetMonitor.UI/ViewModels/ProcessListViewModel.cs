using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace DotNetMonitor.UI.ViewModels
{
    public class ProcessListViewModel : BindableBase
    {
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

        internal void LoadProcesses()
        {
            var processInfoList = Process.GetProcesses().OrderBy(p => p.Id)
                                                        .Select(p => new ProcessInfoViewModel
                                                        {
                                                            Id = p.Id,
                                                            Name = p.ProcessName,
                                                        }).ToList();
            Processes = new ObservableCollection<ProcessInfoViewModel>(processInfoList);
        }
    }
}