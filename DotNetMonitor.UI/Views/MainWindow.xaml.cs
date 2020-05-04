using DotNetMonitor.UI.ViewModels;
using MahApps.Metro.Controls;
using System.Windows;

namespace DotNetMonitor.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel _mainViewModel;

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            _mainViewModel = viewModel;
            DataContext = viewModel;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await _mainViewModel?.Initialize();
        }
    }
}