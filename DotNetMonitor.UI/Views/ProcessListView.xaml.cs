using System.Windows.Controls;

namespace DotNetMonitor.UI.Views
{
    /// <summary>
    /// Interaction logic for ProcessListView.xaml
    /// </summary>
    public partial class ProcessListView : UserControl
    {
        public ProcessListView()
        {
            InitializeComponent();
            ProcessListDataGrid.SelectionChanged += ProcessListDataGrid_SelectionChanged;
        }

        private void ProcessListDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProcessListDataGrid.ScrollIntoView(ProcessListDataGrid.SelectedValue);
        }
    }
}