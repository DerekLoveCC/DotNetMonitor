using DotNetMonitor.Common;
using DotNetMonitor.SpyModule;
using System;
using System.Windows;
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
            Attach();
        }

        private void Attach()
        {
            var windowUnderCursor = NativeMethods.GetWindowUnderMouse();
            var windowInfo = new WindowInfo(windowUnderCursor);

            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                InjectorHelper.InjectLaunch(windowInfo,
                                typeof(ProcessInfoWindow).Assembly,
                                typeof(ProcessInfoWindow).FullName,
                                nameof(ProcessInfoWindow.ShowInfo));
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to attach:" + e);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void UpdateCursor()
        {
            _storedCursor = Cursor;
            Cursor = Cursors.Cross;
        }

        private void RestoreCursor()
        {
            ReleaseMouseCapture();

            Cursor = _storedCursor;
        }
    }
}