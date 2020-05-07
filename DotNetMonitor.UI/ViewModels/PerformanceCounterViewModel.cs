using DotNetMonitor.UI.Utils;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetMonitor.UI.ViewModels
{
    public class PerformanceCounterViewModel : BindableBase, IDisposable
    {
        private int _processId;
        private ProcessInfoViewModel _processInfoViewModel;
        private Task _traceTask;

        internal async Task ChangeProcess(ProcessInfoViewModel processInfoViewModel)
        {
            await StopTrace();
            _processInfoViewModel = processInfoViewModel;
            _processId = processInfoViewModel.Id;
            ClearCounters();

            StartTrace();
        }

        private void ClearCounters()
        {
            var properties = GetType().GetProperties().Where(p => p.Name.EndsWith("Counter"));
            foreach (var property in properties)
            {
                property.SetValue(this, null);
            }
        }

        public bool Tracing { get; private set; } = true;

        public void StartTrace()
        {
            Tracing = true;
            _traceTask = Task.Run(() =>
            {
                try
                {
                    TracePerformance();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            });
        }

        private async Task StopTrace()
        {
            Tracing = false;
            if (_traceTask != null)
            {
                try
                {
                    await _traceTask;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void TracePerformance()
        {
            string instance = PerformanceCounterUtil.GetPerformanceCounterInstance(_processId);
            if (instance == null)
            {
                return;
            }

            const int sleepInterval = (int)(0.5 * 1000);

            var category = ".NET CLR Memory";
            var gen0SizeCounter = new PerformanceCounter(category, "Gen 0 heap size", instance);
            var gen1SizeCounter = new PerformanceCounter(category, "Gen 1 heap size", instance);
            var gen2SizeCounter = new PerformanceCounter(category, "Gen 2 heap size", instance);
            var lohSizeCounter = new PerformanceCounter(category, "Large Object Heap Size", instance);
            var allBytesInHeapCounter = new PerformanceCounter(category, "# Bytes in all Heaps", instance);

            var workSetCounter = new PerformanceCounter("Process", "Working Set", instance);
            var privateBytesCounter = new PerformanceCounter("Process", "Private Bytes", instance);
            while (Tracing)
            {
                WorkingSetCounter = workSetCounter.NextValue();
                PrivateBytesCounter = privateBytesCounter.NextValue();

                if (_processInfoViewModel.IsNetProcess)
                {
                    BytesInAllHeapsCounter = allBytesInHeapCounter.NextValue();
                    Gen0SizeCounter = gen0SizeCounter.NextValue();
                    Gen1SizeCounter = gen1SizeCounter.NextValue();
                    Gen2SizeCounter = gen2SizeCounter.NextValue();
                    LohSizeCounter = lohSizeCounter.NextValue();
                }

                Thread.Sleep(sleepInterval);
            }
        }

        public void Dispose()
        {
            Tracing = false;
        }

        #region Binding Properties

        private float? _gen0SizeCounter;

        public float? Gen0SizeCounter
        {
            get { return _gen0SizeCounter; }
            set
            {
                if (_gen0SizeCounter != value)
                {
                    _gen0SizeCounter = value;
                    RaisePropertyChanged(nameof(Gen0SizeCounter));
                }
            }
        }

        private float? _gen1SizeCounter;

        public float? Gen1SizeCounter
        {
            get { return _gen1SizeCounter; }
            set
            {
                if (_gen1SizeCounter != value)
                {
                    _gen1SizeCounter = value;
                    RaisePropertyChanged(nameof(Gen1SizeCounter));
                }
            }
        }

        private float? _gen2SizeCounter;

        public float? Gen2SizeCounter
        {
            get { return _gen2SizeCounter; }
            set
            {
                if (_gen2SizeCounter != value)
                {
                    _gen2SizeCounter = value;
                    RaisePropertyChanged(nameof(Gen2SizeCounter));
                }
            }
        }

        private float? _lohSizeCounter;

        public float? LohSizeCounter
        {
            get { return _lohSizeCounter; }
            set
            {
                if (_lohSizeCounter != value)
                {
                    _lohSizeCounter = value;
                    RaisePropertyChanged(nameof(LohSizeCounter));
                }
            }
        }

        private float? _bytesInAllHeapsCounter;

        public float? BytesInAllHeapsCounter
        {
            get { return _bytesInAllHeapsCounter; }
            set
            {
                if (_bytesInAllHeapsCounter != value)
                {
                    _bytesInAllHeapsCounter = value;
                    RaisePropertyChanged(nameof(BytesInAllHeapsCounter));
                }
            }
        }

        private float? _workingSetCounter;

        public float? WorkingSetCounter
        {
            get { return _workingSetCounter; }
            set
            {
                if (_workingSetCounter != value)
                {
                    _workingSetCounter = value;
                    RaisePropertyChanged(nameof(WorkingSetCounter));
                }
            }
        }

        private float? _privateBytesCounter;

        public float? PrivateBytesCounter
        {
            get { return _privateBytesCounter; }
            set
            {
                if (_privateBytesCounter != value)
                {
                    _privateBytesCounter = value;
                    RaisePropertyChanged(nameof(PrivateBytesCounter));
                }
            }
        }

        #endregion Binding Properties
    }
}