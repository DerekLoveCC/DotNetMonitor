using Autofac;
using DotNetMonitor.UI.Services;
using DotNetMonitor.UI.ViewModels;
using DotNetMonitor.UI.Views;
using Prism.Events;

namespace DotNetMonitor.UI.Startup
{
    public static class Bootstrapper
    {
        private static IContainer _instance = CreateContainer();

        public static IContainer Container
        {
            get
            {
                return _instance;
            }
        }

        private static IContainer CreateContainer()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<MainWindowViewModel>().AsSelf();
            containerBuilder.RegisterType<ProcessListViewModel>().AsSelf();
            containerBuilder.RegisterType<MainWindow>().SingleInstance();

            containerBuilder.RegisterType<DialogService>().As<IDialogService>().SingleInstance();

            containerBuilder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            return containerBuilder.Build();
        }
    }
}