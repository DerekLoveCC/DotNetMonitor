using DotNetMonitor.UI.Event;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace DotNetMonitor.UI.ViewModels
{
    public class ProcessDetailViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        public ProcessDetailViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<LoadProcessDetailEvent>().Subscribe(HandleLoadProcessDetailEvent);
            LoadedProcesses = new ObservableCollection<ProcessDetail>();
        }

        private void HandleLoadProcessDetailEvent(int processId)
        {
            LoadedProcesses.Add(new ProcessDetail
            {
                 Id = processId,
                 Name = "Dummyname",
            });
        }

        public int Id { get; set; }
        public string Name { get; set; }

        private ObservableCollection<ProcessDetail> _loadedProcesses;

        public ObservableCollection<ProcessDetail> LoadedProcesses
        {
            get
            {
                return _loadedProcesses;
            }
            set
            {
                if (_loadedProcesses != value)
                {
                    _loadedProcesses = value;
                    RaisePropertyChanged(nameof(LoadedProcesses));
                }
            }
        }
    }
}