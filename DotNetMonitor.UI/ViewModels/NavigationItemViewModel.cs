using DotNetMonitor.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Windows.Input;

namespace DotNetMonitor.UI.ViewModels
{
    public class NavigationItemViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        public NavigationItemViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            OpenProcessDetailCommand = new DelegateCommand(ObeyOpenProcessDetail);
        }

        private void ObeyOpenProcessDetail()
        {
            _eventAggregator.GetEvent<LoadProcessDetailEvent>().Publish(Id);
        }

        public string DisplayMember { get; set; }
        public int Id { get; set; }

        public ICommand OpenProcessDetailCommand { get; set; }
    }
}