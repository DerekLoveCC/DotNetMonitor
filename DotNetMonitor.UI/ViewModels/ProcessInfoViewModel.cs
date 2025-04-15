using DotNetMonitor.Common.NativeMethod;
using DotNetMonitor.UI.Utils;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DotNetMonitor.UI.ViewModels
{
    [DebuggerDisplay("Id={ProcessId}, Name={Name}")]
    public class ProcessInfoViewModel : BindableBase
    {
        public ProcessInfoViewModel(int? pid = null)
        {
            ProcessId = pid;

            TrimWorksetCommand = new DelegateCommand(OnTrimWorkset);
            RefreshCommand = new DelegateCommand(OnRefresh);
            KillCommand = new DelegateCommand<ProcessInfoViewModel>(OnKill);
            ExplorerFolderCommand = new DelegateCommand<ProcessInfoViewModel>(OnExplorerFolder, CanExecuateExplorerFolder);
        }

        private bool CanExecuateExplorerFolder(ProcessInfoViewModel process)
        {
            return !string.IsNullOrWhiteSpace(this.ExecutablePath);
        }

        private void OnExplorerFolder(ProcessInfoViewModel process)
        {
            var folder = Path.GetDirectoryName(process.ExecutablePath);

            Process.Start("explorer.exe", folder);
        }

        private void OnKill(ProcessInfoViewModel p)
        {
            Process.GetProcessById(p.ProcessId.Value)?.Kill();
        }

        private void OnRefresh()
        {
            var process = Process.GetProcessById(ProcessId.Value);
            ProcessUtil.PopulateInfo(this, process);
        }

        private void OnTrimWorkset()
        {
            var p = Process.GetProcessById(ProcessId.Value);
            var result = ProcessNativeMethods.EmptyWorkingSet(p);
            MessageBox.Show(result ? "Succeed" : "Failed");
        }

        public ICommand TrimWorksetCommand { get; }

        public ICommand RefreshCommand { get; }
        public ICommand KillCommand { get; }

        public ICommand ExplorerFolderCommand { get; }

        public int? ProcessId { get; internal set; }
        public string Name { get; internal set; }

        public string CommandLine { get; set; }

        public string Description { get; set; }
        public string ExecutablePath { get; set; }

        public int? SessionId { get; internal set; }
        public bool? IsNetProcess { get; internal set; }
        public IntPtr? Handle { get; set; }
        public IList<ProcessModuleInfo> Modules { get; internal set; }

        public bool? IsX64 { get; internal set; }

        public string Error { get; set; }
    }
}