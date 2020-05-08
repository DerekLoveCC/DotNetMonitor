using Autofac;
using DotNetMonitor.UI.Startup;
using DotNetMonitor.UI.Utils;
using DotNetMonitor.UI.Views;
using System.Windows;
using System.Windows.Threading;

namespace DotNetMonitor.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Unhandled exception happens. Error{e.Exception?.Message}");
            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            UIDispatcher.Dispatcher = Dispatcher;

            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Bootstrap();

            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.Show();
        }
    }
}