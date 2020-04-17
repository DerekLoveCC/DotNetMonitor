using DotNetMonitor.Common.NativeMethod;
using DotNetMonitor.UI.ViewModels.ProcessInfo;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace DotNetMonitor.UI.ViewModels
{
    public class ProcessListViewModel : BindableBase
    {
        private ObservableCollection<ProcessDetailInfo> _processes;

        public ObservableCollection<ProcessDetailInfo> Processes
        {
            get { return _processes; }
            set
            {
                if (_processes != value)
                {
                    _processes = value;
                    RaisePropertyChanged(nameof(Processes));
                }
            }
        }

        private ProcessDetailInfo _selectedRow;

        public ProcessDetailInfo SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                _selectedRow = value;
                if (_selectedRow != value)
                {
                    RaisePropertyChanged(nameof(SelectedRow));
                }
            }
        }

        internal void LoadProcesses()
        {
            var processInfoList = Process.GetProcesses().OrderBy(p => p.Id)
                                                        .Select(p => BuildProcessInfoViewModel(p));

            Processes = new ObservableCollection<ProcessDetailInfo>(processInfoList);
        }

        private ProcessDetailInfo BuildProcessInfoViewModel(Process p)
        {
            var result = new ProcessDetailInfo
            {
                Id = p.Id,
                Name = p.ProcessName,
                SessionId = p.SessionId,
                Modules = GetProcessModuleInfos(p),
            };
            result.IsNetProcess = CheckDotNetProcess(result);
            result.IsX64 = CheckProcessBit(p);
            return result;
        }

        private bool? CheckProcessBit(Process p)
        {
            try
            {
                return ProcessNativeMethods.Is64Bit(p);
            }
            catch
            {
                return null;
            }
        }

        private IList<ProcessModuleInfo> GetProcessModuleInfos(Process process)
        {
            try
            {
                return process.Modules.OfType<ProcessModule>()
                                      .Select(m => new ProcessModuleInfo { Name = m.ModuleName })
                                      .ToList();
            }
            catch
            {
                return new List<ProcessModuleInfo>();
            }
        }

        private bool CheckDotNetProcess(ProcessDetailInfo process)
        {
            return process.Modules.Any(m => m.Name.Contains("clr.dll"));
        }
    }
}