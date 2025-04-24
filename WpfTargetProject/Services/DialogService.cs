using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

public class DialogService : IDialogService
{
    private readonly MetroWindow _metroWindow;

    public DialogService(MetroWindow metroWindow)
    {
        _metroWindow = metroWindow;
    }

    public async Task ShowMessageAsync(string title, string message)
    {
        await _metroWindow.ShowMessageAsync(title, message);
    }
}
