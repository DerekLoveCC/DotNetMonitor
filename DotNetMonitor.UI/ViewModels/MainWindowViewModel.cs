using Prism.Commands;
using Prism.Mvvm;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DotNetMonitor.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(ProcessListViewModel processListViewModel)
        {
            ProcessListViewModel = processListViewModel;
            RefreshProcessListCommand = new DelegateCommand(OnRefreshProcessList);
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

        public ICommand RefreshProcessListCommand { get; }

        private async void OnRefreshProcessList()
        {
            if (ProcessListViewModel?.Processes == null)
            {
                return;
            }

            foreach (var process in ProcessListViewModel?.Processes)
            {
                process.Dispose();
            }

            await ProcessListViewModel?.LoadProcesses();
        }
    }
}