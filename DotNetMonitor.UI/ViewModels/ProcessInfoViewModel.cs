using DotNetMonitor.Common.NativeMethod;
using DotNetMonitor.UI.Utils;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DotNetMonitor.UI.ViewModels
{
    [DebuggerDisplay("Id={Id}, Name={Name}")]
    public class ProcessInfoViewModel : BindableBase, IDisposable
    {
        public ProcessInfoViewModel()
        {
            TrimWorksetCommand = new DelegateCommand(OnTrimWorkset);
            TracePerformanceCommand = new DelegateCommand(OnTracePerformance, () => IsNetProcess);
            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        private void OnRefresh()
        {
            var process = Process.GetProcessById(Id);
            ProcessUtil.PopulateInfo(this, process);
        }

        private void OnTrimWorkset()
        {
            var p = Process.GetProcessById(Id);
            var result = ProcessNativeMethods.EmptyWorkingSet(p);
            if (result)
            {
                WorkingSet = Process.GetProcessById(Id).WorkingSet64;
            }
            MessageBox.Show(result ? "Succeed" : "Failed");
        }

        private void OnTracePerformance()
        {
            Tracing = !Tracing;
            if (Tracing)
            {
                Task.Run(() => TracePerformance());
                TracePerformanceText = "Stop tracing perforamnce";
            }
            else
            {
                TracePerformanceText = "Start to trace perforamnce";
            }
        }

        public ICommand TrimWorksetCommand { get; }
        public ICommand TracePerformanceCommand { get; }
        public ICommand RefreshCommand { get; }

        public int Id { get; internal set; }
        public string Name { get; internal set; }

        public int SessionId { get; internal set; }
        public bool IsNetProcess { get; internal set; }
        public IList<ProcessModuleInfo> Modules { get; internal set; }

        public bool? IsX64 { get; internal set; }

        private long _workingSet;

        public long WorkingSet
        {
            get { return _workingSet; }
            set
            {
                if (_workingSet != value)
                {
                    _workingSet = value;
                    RaisePropertyChanged(nameof(WorkingSet));
                }
            }
        }

        public long PrivateMemorySize { get; internal set; }

        private string _tracePerformanceText = "Start to trace perforamnce";

        public string TracePerformanceText
        {
            get { return _tracePerformanceText; }
            set
            {
                if (_tracePerformanceText != value)
                {
                    _tracePerformanceText = value;
                    RaisePropertyChanged(nameof(TracePerformanceText));
                }
            }
        }

        private float _gen0Size;

        public float Gen0Size
        {
            get { return _gen0Size; }
            set
            {
                if (_gen0Size != value)
                {
                    _gen0Size = value;
                    RaisePropertyChanged(nameof(Gen0Size));
                }
            }
        }

        private float _gen1Size;

        public float Gen1Size
        {
            get { return _gen1Size; }
            set
            {
                if (_gen1Size != value)
                {
                    _gen1Size = value;
                    RaisePropertyChanged(nameof(Gen1Size));
                }
            }
        }

        private float _gen2Size;

        public float Gen2Size
        {
            get { return _gen2Size; }
            set
            {
                if (_gen2Size != value)
                {
                    _gen2Size = value;
                    RaisePropertyChanged(nameof(Gen2Size));
                }
            }
        }

        public bool Tracing { get; private set; }

        private void TracePerformance()
        {
            string instance = PerformanceCounterUtil.GetPerformanceCounterInstance(Id);
            if (instance == null)
            {
                return;
            }

            var category = ".NET CLR Memory";
            var gen0SizeCounter = new PerformanceCounter(category, "Gen 0 heap size", instance);
            var gen1SizeCounter = new PerformanceCounter(category, "Gen 1 heap size", instance);
            var gen2SizeCounter = new PerformanceCounter(category, "Gen 2 heap size", instance);
            const int sleepInterval = (int)(0.5 * 1000);
            while (Tracing)
            {
                Gen0Size = gen0SizeCounter.NextValue();
                Gen1Size = gen1SizeCounter.NextValue();
                Gen2Size = gen2SizeCounter.NextValue();

                Thread.Sleep(sleepInterval);
            }
        }

        public void Dispose()
        {
            Tracing = false;
        }
    }
}