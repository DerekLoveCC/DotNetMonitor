using Autofac;
using DotNetMonitor.UI.Services;
using DotNetMonitor.UI.Startup;
using DotNetMonitor.UI.Utils;
using DotNetMonitor.UI.Views;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace DotNetMonitor.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        DialogBox.ShowMessage("Error", $"An unhandled exception has occurred.  Error: {e.Exception?.Message}.");
        e.Handled = true;
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        Thread.CurrentThread.Name = "UI";

        AppDomain.CurrentDomain.UnhandledException += HandleAppDomainException;

        UIDispatcher.Dispatcher = Dispatcher;

        var mainWindow = Bootstrapper.Container.Resolve<MainWindow>();
        mainWindow.Show();
    }

    private void HandleAppDomainException(object sender, UnhandledExceptionEventArgs e)
    {
        DialogBox.ShowMessage("Error", $"An unhandled exception has occurred.");
    }
}