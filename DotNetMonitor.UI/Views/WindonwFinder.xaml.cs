using System.Windows.Controls;
using System.Windows.Input;

namespace DotNetMonitor.UI.Views
{
    /// <summary>
    /// Interaction logic for WindonwFinder.xaml
    /// </summary>
    public partial class WindonwFinder : UserControl
    {
        private Cursor _storedCursor;

        public WindonwFinder()
        {
            InitializeComponent();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            StartSnoopTargetsSearch();
            e.Handled = true;
            base.OnPreviewMouseLeftButtonDown(e);
        }

        private void StartSnoopTargetsSearch()
        {
            CaptureMouse();
            Keyboard.Focus(btnSeacher);
            UpdateCursor();
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            RestoreCursor();
            base.OnPreviewMouseLeftButtonUp(e);
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            RestoreCursor();
            base.OnPreviewKeyUp(e);
        }

        private void UpdateCursor()
        {
            _storedCursor = Cursor;
            Cursor = Cursors.Cross;
        }

        private void RestoreCursor()
        {
            ReleaseMouseCapture();

            Cursor = _storedCursor ?? Cursors.Arrow;
        }
    }
}