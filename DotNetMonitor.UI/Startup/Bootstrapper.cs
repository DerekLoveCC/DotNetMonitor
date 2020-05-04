using Autofac;
using DotNetMonitor.UI.ViewModels;
using DotNetMonitor.UI.Views;
using Prism.Events;

namespace DotNetMonitor.UI.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<MainWindowViewModel>().AsSelf();
            containerBuilder.RegisterType<ProcessListViewModel>().AsSelf();
            containerBuilder.RegisterType<MainWindow>().AsSelf();

            containerBuilder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            return containerBuilder.Build();
        }
    }
}