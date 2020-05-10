namespace FEMouseMoveWindowTest
{
    public partial class MainWindow : System.Windows.Window
    {
        private FreeEcho.FEMouseMoveWindow.MouseMoveWindow MouseMoveWindow = new FreeEcho.FEMouseMoveWindow.MouseMoveWindow();

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                Loaded += MainWindow_Loaded;
                Closed += MainWindow_Closed;
                MouseMoveWindow.MoveStart += MouseMoveWindow_MoveStart;
                MouseMoveWindow.Moved += MouseMoveWindow_Moved;
                MouseMoveWindow.MoveStop += MouseMoveWindow_MoveStop;
            }
            catch
            {
            }
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                System.Windows.Interop.HwndSource source = (System.Windows.Interop.HwndSource)System.Windows.Interop.HwndSource.FromVisual(this);
                MouseMoveWindow.Start();
            }
            catch
            {
            }
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            try
            {
                MouseMoveWindow.Stop();
            }
            catch
            {
            }
        }

        /// <summary>
        /// マウスでウィンドウ移動が開始された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseMoveWindow_MoveStart(object sender, FreeEcho.FEMouseMoveWindow.StartWindowMoveMouseEventArgs e)
        {
            try
            {
                LabelInformation.Content = "マウスでウィンドウ移動が開始された";
            }
            catch
            {
            }
        }

        /// <summary>
        /// マウスでウィンドウが移動された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseMoveWindow_Moved(object sender, FreeEcho.FEMouseMoveWindow.WindowMoveDetectionMouseEventArgs e)
        {
            try
            {
                LabelInformation.Content = "マウスでウィンドウが移動された";
            }
            catch
            {
            }
        }

        /// <summary>
        /// マウスでウィンドウ移動が停止された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseMoveWindow_MoveStop(object sender, FreeEcho.FEMouseMoveWindow.StopWindowMoveMouseEventArgs e)
        {
            try
            {
                LabelInformation.Content = "マウスでウィンドウ移動が停止された";
            }
            catch
            {
            }
        }
    }
}
