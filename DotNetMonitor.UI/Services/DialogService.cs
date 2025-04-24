using DotNetMonitor.UI.Views;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace DotNetMonitor.UI.Services;

public class DialogService : IDialogService
{
    private readonly MainWindow _metroWindow;

    public DialogService(MainWindow metroWindow)
    {
        _metroWindow = metroWindow;
    }

    public void ShowMessage(string title, string message)
    {
        _metroWindow.ShowMessageAsync(title, message);
    }

    public async Task ShowMessageAsync(string title, string message)
    {
        await _metroWindow.ShowMessageAsync(title, message);
    }
}