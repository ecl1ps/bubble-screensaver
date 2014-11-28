using System;
using System.Windows;
using System.Windows.Interop;

namespace Bubbles
{
    public partial class App : Application
    {
        private MainWindow winSaver;
 
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var settings = BubblesSettings.Load(BubblesSettings.SettingsFile);

            if (e.Args.Length == 0 || e.Args[0].ToLower().StartsWith("/s"))     
            {
                MainWindow win = new MainWindow(settings) { WindowState = WindowState.Maximized };
                win.Show();
            }
            // Preview mode--display in little window in Screen Saver dialog
            else if (e.Args[0].ToLower().StartsWith("/p"))
            {
                winSaver = new MainWindow(settings);

                string handle = e.Args[0].Contains(":") ? e.Args[0].Split(':')[1] : e.Args[1];
                IntPtr pPreviewHnd = new IntPtr(Convert.ToInt32(handle));
 
                RECT lpRect = new RECT();
                Win32API.GetClientRect(pPreviewHnd, ref lpRect);
 
                HwndSourceParameters sourceParams = new HwndSourceParameters("sourceParams")
                {
                    PositionX = 0,
                    PositionY = 0,
                    Height = lpRect.Bottom - lpRect.Top,
                    Width = lpRect.Right - lpRect.Left,
                    ParentWindow = pPreviewHnd,
                    WindowStyle = (int) (WindowStyles.WS_VISIBLE | WindowStyles.WS_CHILD | WindowStyles.WS_CLIPCHILDREN)
                };

                var winWpfContent = new HwndSource(sourceParams);
                winWpfContent.Disposed += winWPFContent_Disposed;
                winWpfContent.RootVisual = winSaver.MainGrid;
            }
            else if (e.Args[0].ToLower().StartsWith("/c"))     
            {
                SettingsWindow win = new SettingsWindow();
                win.Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
 
        /// <summary>
        /// Event that triggers when parent window is disposed--used when doing
        /// screen saver preview, so that we know when to exit.  If we didn't
        /// do this, Task Manager would get a new .scr instance every time
        /// we opened Screen Saver dialog or switched dropdown to this saver.
        /// </summary>
        ///<param name="sender"></param>
        ///<param name="e"></param>
        void winWPFContent_Disposed(object sender, EventArgs e)
        {
            winSaver.Close();
        }
    }
}
