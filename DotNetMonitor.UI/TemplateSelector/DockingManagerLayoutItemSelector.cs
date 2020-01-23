using DotNetMonitor.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DotNetMonitor.UI.TemplateSelector
{
    public class DockingManagerLayoutItemSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ProcessDetail)
            {
                var foundTempalte = Application.Current.MainWindow.TryFindResource("ProcessDocumentPaneTemplate") as DataTemplate;
                return foundTempalte;
            }

            return base.SelectTemplate(item, container);
        }
    }
}