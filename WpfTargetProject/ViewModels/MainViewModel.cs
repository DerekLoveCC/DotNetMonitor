using Prism.Commands;
using Prism.Mvvm;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfTargetProject.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;

        public MainViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            ShowDialogCommand = new DelegateCommand(async () => await ShowDialog());
        }

        public ICommand ShowDialogCommand { get; }

        private async Task ShowDialog()
        {
            await _dialogService.ShowMessageAsync("Hello", "This is a message from the ViewModel!");
        }
    }
}