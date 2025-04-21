using Prism.Mvvm;

namespace DotNetMonitor.UI.ViewModels
{
    public class ProcessDetailViewModel : BindableBase
    {
        private int? _processId;

        public ProcessDetailViewModel(int? processId)
        {
            _processId = processId;
        }
    }
}