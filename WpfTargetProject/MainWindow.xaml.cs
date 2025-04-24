using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using WpfTargetProject.ViewModels;

namespace WpfTargetProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            // Register the dialog service
            var dialogService = new DialogService(this);
            var viewModel = new MainViewModel(dialogService);
            DataContext = viewModel;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ShowMessageAsync("Hello", "This is a message box");
        }
    }
}