using DotNetMonitor.UI.ViewModels;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Reflection;

namespace DotNetMonitor.UI.Model
{
    public class DMPerformanceCounter : BindableBase
    {
        private readonly string _category;
        private string _instance;
        private readonly PerformanceCounterViewModel _viewModel;
        private readonly PropertyInfo _propertyInfo;
        public PerformanceCounter _performanceCounter;
        private bool _hasError;

        public DMPerformanceCounter(string category,
                                    string counterName,
                                    string property,
                                    PerformanceCounterViewModel viewModel)
        {
            _category = category;
            _counterName = counterName;
            _viewModel = viewModel;

            _propertyInfo = viewModel.GetType().GetProperty(property);
        }

        public DMPerformanceCounter(string category, string counterName)
        {
            _category = category;
            _counterName = counterName;
        }

        #region Bindable Properties

        private string _counterName;

        public string CounterName
        {
            get { return _counterName; }
            set
            {
                if (_counterName != value)
                {
                    _counterName = value;
                    RaisePropertyChanged(nameof(CounterName));
                }
            }
        }

        private float? _counterValue;

        public float? CounterValue
        {
            get { return _counterValue; }
            set
            {
                if (_counterValue != value)
                {
                    _counterValue = value;
                    RaisePropertyChanged(nameof(CounterValue));
                }
            }
        }

        #endregion Bindable Properties

        public void UpdateInstance(string instance)
        {
            if (_instance == instance)
            {
                return;
            }
            _hasError = false;
            _instance = instance;
            _performanceCounter?.Close();
            _performanceCounter?.Dispose();

            _performanceCounter = new PerformanceCounter(_category, _counterName, _instance, true);
        }

        public void UpdateCounterValue()
        {
            if (_hasError)
            {
                return;
            }
            try
            {
                CounterValue = _performanceCounter.NextValue();
            }
            catch (Exception)
            {
                _hasError = true;
            }
        }
    }
}