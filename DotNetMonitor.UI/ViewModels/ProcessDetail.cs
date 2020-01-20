using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DotNetMonitor.UI.ViewModels
{
    public class ProcessDetail
    {

        public ProcessDetail(int processId)
        {
            Id = processId;

            InjectCommand = new DelegateCommand<int?>(OnInject);
        }

        private void OnInject(int? processId)
        {
           
        }

        public int Id { get; }
        public string Name { get; private set; }

        public ICommand InjectCommand { get; }

        internal Task InitializeAsync()
        {
            return Task.Run(() =>
            {
                var process = Process.GetProcessById(Id);
                Name = process.ProcessName;

            });
        }
    }
}
