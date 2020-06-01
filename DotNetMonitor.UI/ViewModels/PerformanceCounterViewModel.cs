using DotNetMonitor.UI.Model;
using DotNetMonitor.UI.Utils;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetMonitor.UI.ViewModels
{
    public class PerformanceCounterViewModel : BindableBase, IDisposable
    {
        private int? _processId;
        private readonly object _startTraceLock = new object();

        public PerformanceCounterViewModel()
        {
            Counters = new ObservableCollection<DMPerformanceCounter>
            {
                 new DMPerformanceCounter(".NET CLR Memory", "Gen 0 heap size"),
                 new DMPerformanceCounter(".NET CLR Memory", "Gen 1 heap size"),
                 new DMPerformanceCounter(".NET CLR Memory", "Gen 2 heap size"),
                 new DMPerformanceCounter(".NET CLR Memory", "Large Object Heap Size"),
                 new DMPerformanceCounter(".NET CLR Memory", "# Bytes in all Heaps"),
                 new DMPerformanceCounter("Process", "Working Set"),
                 new DMPerformanceCounter("Process", "Private Bytes"),
        };
        }

        internal void ChangeProcess(ProcessInfoViewModel processInfoViewModel)
        {
            _processId = processInfoViewModel?.ProcessId;

            StartTrace();
        }

        private void ClearCounters()
        {
            foreach (var counter in Counters)
            {
                counter.CounterValue = null;
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
                var traceTask = Task.Run(() =>
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

        private void UpdateCounterValues()
        {
            foreach (var counter in Counters)
            {
                counter.UpdateCounterValue();
            }
        }

        private void TracePerformance()
        {
            const int sleepInterval = (int)(0.5 * 1000);
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

            foreach (var counter in Counters)
            {
                counter.UpdateInstance(instance);
            }
            return true;
        }



        #region Binding Properties

        private ObservableCollection<DMPerformanceCounter> _counters;
        private bool disposedValue;

        public ObservableCollection<DMPerformanceCounter> Counters
        {
            get { return _counters; }
            set
            {
                if (_counters != value)
                {
                    _counters = value;
                    RaisePropertyChanged(nameof(Counters));
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Tracing = false;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PerformanceCounterViewModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion Binding Properties
    }
}