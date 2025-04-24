using Autofac;
using DotNetMonitor.UI.Startup;
using System.Threading.Tasks;

namespace DotNetMonitor.UI.Services;

public static class DialogBox
{
    public static async Task ShowMessageAsync(string title, string message)
    {
        var dialogService = Bootstrapper.Container.Resolve<IDialogService>();
        await dialogService?.ShowMessageAsync(title, message);
    }

    public static void ShowMessage(string title, string message)
    {
        var dialogService = Bootstrapper.Container.Resolve<IDialogService>();
        dialogService?.ShowMessage(title, message);
    }
}