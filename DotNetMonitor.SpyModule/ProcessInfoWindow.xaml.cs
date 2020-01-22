using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
