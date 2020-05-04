using Prism.Mvvm;
using System.Threading.Tasks;

namespace DotNetMonitor.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(ProcessListViewModel processListViewModel)
        {
            ProcessListViewModel = processListViewModel;
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
            await ProcessListViewModel?.LoadProcesses();
        }
    }
}