using Prism.Mvvm;

namespace DotNetMonitor.UI.ViewModels
{
    public class ProcessInfoViewModel : BindableBase
    {
        public int Id { get; internal set; }
        public string Name { get; internal set; }
    }
}