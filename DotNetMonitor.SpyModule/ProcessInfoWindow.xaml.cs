using System.Windows;

namespace DotNetMonitor.SpyModule
{
    /// <summary>
    /// Interaction logic for ProcessInfoWindow.xaml
    /// </summary>
    public partial class ProcessInfoWindow : Window
    {
        public ProcessInfoWindow()
        {
            InitializeComponent();
        }

        public static void ShowInfo()
        {
            var window = new ProcessInfoWindow();
            window.Show();
        }
    }
}