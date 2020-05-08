using DotNetMonitor.UI.ViewModels;
using System;
using System.Diagnostics;
using System.Reflection;

namespace DotNetMonitor.UI.Model
{
    public class DMPerformanceCounter
    {
        private readonly string _category;
        private readonly string _counterName;
        private string _instance;
        private readonly PerformanceCounterViewModel _viewModel;
        private readonly PropertyInfo _propertyInfo;
        private PerformanceCounter _performanceCounter;
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
                _propertyInfo.SetValue(_viewModel, _performanceCounter.NextValue());
            }
            catch (Exception)
            {
                _hasError = true;
            }
        }
    }
}