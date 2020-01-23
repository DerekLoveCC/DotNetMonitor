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

            //containerBuilder.RegisterType<AStockDbContext>().AsSelf();
            //containerBuilder.RegisterType<StockRepository>().As<IStockRepository>();
            //containerBuilder.RegisterType<StockHolderRepository>().As<IStockHolderRepository>();
            //containerBuilder.RegisterType<ExchangeCenterRepository>().As<IExchangeCenterRepository>();
            //containerBuilder.RegisterType<LookupDataService>().AsImplementedInterfaces();

            containerBuilder.RegisterType<MainWindowViewModel>().AsSelf();
            containerBuilder.RegisterType<NavigationViewModel>().AsSelf();
            containerBuilder.RegisterType<ProcessDetailViewModel>().AsSelf();
            containerBuilder.RegisterType<ProcessListViewModel>().AsSelf();
            containerBuilder.RegisterType<MainWindow>().AsSelf();

            //containerBuilder.RegisterType<MainViewModel>().AsSelf();
            //containerBuilder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();
            //containerBuilder.RegisterType<StockDetailViewModel>().Keyed<IDetailViewModel>(nameof(StockDetailViewModel));
            //containerBuilder.RegisterType<StockHolderDetailViewModel>().Keyed<IDetailViewModel>(nameof(StockHolderDetailViewModel));
            //containerBuilder.RegisterType<ExchangeCenterDetailViewModel>().Keyed<IDetailViewModel>(nameof(ExchangeCenterDetailViewModel));

            containerBuilder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            //containerBuilder.RegisterType<MessageDialogService>().As<IMessageDialogService>();

            return containerBuilder.Build();
        }
    }
}