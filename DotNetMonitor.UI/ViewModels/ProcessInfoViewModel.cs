using DotNetMonitor.Common.NativeMethod;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DotNetMonitor.UI.ViewModels
{
    [DebuggerDisplay("Id={Id}, Name={Name}")]
    public class ProcessInfoViewModel : BindableBase
    {
        public ProcessInfoViewModel()
        {
            TrimWorksetCommand = new DelegateCommand(OnTrimWorkset);
            TracePerformanceCommand = new DelegateCommand(OnTracePerformance, () => IsNetProcess);
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
            while (Tracing)
            {
                //var ctr1 = new PerformanceCounter("Process", "Private Bytes", Process.GetCurrentProcess().ProcessName);
                //var ctr2 = new PerformanceCounter(".NET CLR Memory", "# Gen 0 Collections", Process.GetCurrentProcess().ProcessName);
                //var ctr3 = new PerformanceCounter(".NET CLR Memory", "# Gen 1 Collections", Process.GetCurrentProcess().ProcessName);
                //var ctr4 = new PerformanceCounter(".NET CLR Memory", "# Gen 2 Collections", Process.GetCurrentProcess().ProcessName);
                var gen0Size = new PerformanceCounter(".NET CLR Memory", "Gen 0 heap size", Process.GetCurrentProcess().ProcessName);
                var gen1Size = new PerformanceCounter(".NET CLR Memory", "Gen 1 heap size", Process.GetCurrentProcess().ProcessName);
                var gen2Size = new PerformanceCounter(".NET CLR Memory", "Gen 2 heap size", Process.GetCurrentProcess().ProcessName);
                Gen0Size = gen0Size.NextValue();
                Gen1Size = gen1Size.NextValue();
                Gen2Size = gen2Size.NextValue();
                //...
                //Debug.WriteLine("ctr1 = " + ctr1.NextValue());
                //Debug.WriteLine("ctr2 = " + ctr2.NextValue());
                //Debug.WriteLine("ctr3 = " + ctr3.NextValue());
                //Debug.WriteLine("ctr4 = " + ctr4.NextValue());
                //Debug.WriteLine("ctr5 = " + ctr5.NextValue());
                Thread.Sleep(1 * 1000);
            }
        }
    }
}