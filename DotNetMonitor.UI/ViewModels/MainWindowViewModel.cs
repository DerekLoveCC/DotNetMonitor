using Prism.Mvvm;

namespace DotNetMonitor.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(ProcessListViewModel processListViewModel)
        {
            ProcessListViewModel = processListViewModel;
        }

        //public MainWindowViewModel(NavigationViewModel navigationViewModel,
        //                           ProcessDetailViewModel processDetailViewModel)
        //{
        //    NavigationViewModel = navigationViewModel;
        //    ProcessDetailViewModel = processDetailViewModel;
        //}

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

        private NavigationViewModel _navigationViewModel;

        public NavigationViewModel NavigationViewModel
        {
            get
            {
                return _navigationViewModel;
            }
            set
            {
                if (_navigationViewModel != value)
                {
                    _navigationViewModel = value;
                    RaisePropertyChanged(nameof(NavigationViewModel));
                }
            }
        }

        private ProcessDetailViewModel _processDetailViewModel;

        public ProcessDetailViewModel ProcessDetailViewModel
        {
            get
            {
                return _processDetailViewModel;
            }
            set
            {
                if (_processDetailViewModel != value)
                {
                    _processDetailViewModel = value;
                    RaisePropertyChanged(nameof(ProcessDetailViewModel));
                }
            }
        }

        internal void Initialize()
        {
            ProcessListViewModel?.LoadProcesses();
        }
    }
}