using System.Threading.Tasks;

namespace DotNetMonitor.UI.Services;

public interface IDialogService
{
    void ShowMessage(string title, string message);
    Task ShowMessageAsync(string title, string message);
}