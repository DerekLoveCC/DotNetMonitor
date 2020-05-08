using DotNetMonitor.UI.Model;
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
        private int? _processId;
        private ProcessInfoViewModel _processInfoViewModel;
        private Task _traceTask;
        private readonly object _startTraceLock = new object();

        internal void ChangeProcess(ProcessInfoViewModel processInfoViewModel)
        {
            _processInfoViewModel = processInfoViewModel;
            _processId = processInfoViewModel?.Id;

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

        public bool Tracing { get; private set; }

        public void StartTrace()
        {
            lock (_startTraceLock)
            {
                if (Tracing)
                {
                    return;
                }
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
        }

        private DMPerformanceCounter gen0SizeCounter;
        private DMPerformanceCounter gen1SizeCounter;
        private DMPerformanceCounter gen2SizeCounter;
        private DMPerformanceCounter lohSizeCounter;

        private DMPerformanceCounter allBytesInHeapCounter;
        private DMPerformanceCounter workSetCounter;
        private DMPerformanceCounter privateBytesCounter;

        private void UpdateCounterValues()
        {
            gen0SizeCounter.UpdateCounterValue();
            gen1SizeCounter.UpdateCounterValue();
            gen2SizeCounter.UpdateCounterValue();
            lohSizeCounter.UpdateCounterValue();
            allBytesInHeapCounter.UpdateCounterValue();

            workSetCounter.UpdateCounterValue();
            privateBytesCounter.UpdateCounterValue();
        }

        private void TracePerformance()
        {
            const int sleepInterval = (int)(0.5 * 1000);

            var category = ".NET CLR Memory";
            gen0SizeCounter = new DMPerformanceCounter(category, "Gen 0 heap size", nameof(Gen0SizeCounter), this);
            gen1SizeCounter = new DMPerformanceCounter(category, "Gen 1 heap size", nameof(Gen1SizeCounter), this);
            gen2SizeCounter = new DMPerformanceCounter(category, "Gen 2 heap size", nameof(Gen2SizeCounter), this);
            lohSizeCounter = new DMPerformanceCounter(category, "Large Object Heap Size", nameof(Gen0SizeCounter), this);
            allBytesInHeapCounter = new DMPerformanceCounter(category, "# Bytes in all Heaps", nameof(BytesInAllHeapsCounter), this);
            workSetCounter = new DMPerformanceCounter("Process", "Working Set", nameof(WorkingSetCounter), this);
            privateBytesCounter = new DMPerformanceCounter("Process", "Private Bytes", nameof(PrivateBytesCounter), this);

         

            try
            {
                var processId = _processId;
                var succeed = processId != null && SetInstance(processId.Value);
                if (!succeed)
                {
                    return;
                }

                while (Tracing)
                {
                    UpdateCounterValues();
                    if (processId == _processId)
                    {
                        Thread.Sleep(sleepInterval);
                    }
                    else
                    {
                        ClearCounters();

                        processId = _processId;
                        succeed = processId != null && SetInstance(processId.Value);
                        if (!succeed)
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                lock (_startTraceLock)
                {
                    Tracing = false;
                }
            }
        }

        private bool SetInstance(int processId)
        {
            string instance = PerformanceCounterUtil.GetPerformanceCounterInstance(processId);
            if (instance == null)
            {
                return false;
            }

            gen0SizeCounter.UpdateInstance(instance);
            gen1SizeCounter.UpdateInstance(instance);
            gen2SizeCounter.UpdateInstance(instance);
            lohSizeCounter.UpdateInstance(instance);
            allBytesInHeapCounter.UpdateInstance(instance);

            workSetCounter.UpdateInstance(instance);
            privateBytesCounter.UpdateInstance(instance);
            return true;
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