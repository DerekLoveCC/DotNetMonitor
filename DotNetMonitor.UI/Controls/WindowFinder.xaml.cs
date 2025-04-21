using DotNetMonitor.Common;
using DotNetMonitor.SpyModule;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DotNetMonitor.UI.Controls
{
    /// <summary>
    /// Interaction logic for WindonwFinder.xaml
    /// </summary>
    public partial class WindowFinder : UserControl
    {
        private Cursor _storedCursor;

        public WindowFinder()
        {
            InitializeComponent();
        }

        public Action<WindowInfo> OnFindCommand
        {
            get { return (Action<WindowInfo>)GetValue(OnFindCommandProperty); }
            set { SetValue(OnFindCommandProperty, value); }
        }
        public static readonly DependencyProperty OnFindCommandProperty =
            DependencyProperty.Register("OnFindCommand", typeof(Action<WindowInfo>), typeof(WindowFinder), new PropertyMetadata(new Action<WindowInfo>(Attach)));

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
            var windowUnderCursor = NativeMethods.GetWindowUnderMouse();
            var windowInfo = new WindowInfo(windowUnderCursor);
            OnFindCommand?.Invoke(windowInfo);
        }

        private static void Attach(WindowInfo windowInfo)
        {
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

            try
            {
                var stream = Application.GetResourceStream(new Uri("pack://application:,,,/Images/location_select.cur", UriKind.RelativeOrAbsolute)).Stream;
                Cursor = new Cursor(stream);
            }
            catch
            {
                Cursor = Cursors.Cross;
            }
        }

        private void RestoreCursor()
        {
            ReleaseMouseCapture();

            Cursor = _storedCursor;
        }
    }
}