using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace StreamerUpdate.AppContainer
{
  /// <summary>
  ///     Interaction logic for AppControl.xaml
  /// </summary>
  public partial class AppControl : UserControl, IDisposable
    {
        private const int SWP_NOOWNERZORDER = 0x200;
        private const int SWP_NOREDRAW = 0x8;
        private const int SWP_NOZORDER = 0x4;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int WS_EX_MDICHILD = 0x40;
        private const int SWP_FRAMECHANGED = 0x20;
        private const int SWP_NOACTIVATE = 0x10;
        private const int SWP_ASYNCWINDOWPOS = 0x4000;
        private const int SWP_NOMOVE = 0x2;
        private const int SWP_NOSIZE = 0x1;
        private const int GWL_STYLE = -16;
        private const int WS_VISIBLE = 0x10000000;
        private const int WS_CHILD = 0x40000000;
        private IntPtr _appWin = IntPtr.Zero;
        private Process _childp;

        private bool _iscreated;
        private bool _isdisposed;

        public AppControl()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     The name of the exe to launch
        /// </summary>
        public string ExeName { get; set; } = "";

        public void Dispose()
        {
            if (!_isdisposed)
                if (_iscreated && _appWin != IntPtr.Zero && !_childp.HasExited)
                {
                    // Stop the application
                    _childp.Kill();

                    // Clear internal handle
                    _appWin = IntPtr.Zero;
                }

            _isdisposed = true;
            GC.SuppressFinalize(this);
        }

        ~AppControl()
        {
            Dispose();
        }


        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);


        [DllImport("user32.dll", EntryPoint = "SetWindowLongA", SetLastError = true)]
        public static extern int SetWindowLongA([InAttribute] IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);


        /// <summary>
        ///     Force redraw of control when size changes
        /// </summary>
        /// <param name="e">Not used</param>
        protected void OnSizeChanged(object s, SizeChangedEventArgs e)
        {
            InvalidateVisual();
        }

        public void Go()
        {
            // If control needs to be initialized/created
            if (_iscreated == false)
            {
                // Mark that control is created
                _iscreated = true;

                // Initialize handle value to invalid
                _appWin = IntPtr.Zero;

                try
                {
                    var procInfo = new ProcessStartInfo(ExeName)
                    {
                        WorkingDirectory = Path.GetDirectoryName(ExeName),
                        UseShellExecute = false
                    };
                    // Start the process
                    _childp = Process.Start(procInfo);
                    _childp.WaitForInputIdle();
                    GetProcessHandle();
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message + "Error");
                }

                // Put it into this form
                var helper = new WindowInteropHelper(Window.GetWindow(AppContainer));

                SetParent(_appWin, helper.Handle);

                // Remove border and whatnot
                SetWindowLongA(_appWin, GWL_STYLE, WS_VISIBLE);

                // Move the window to overlay it on this window
                MoveWindow(_appWin, 0, 30, (int)ActualWidth, (int)ActualHeight, true);
                SizeChanged += OnSizeChanged;
                SizeChanged += OnResize;
            }
        }

        private void GetProcessHandle()
        {
            while (!_childp.HasExited)
            {
                _childp.Refresh();
                if (_childp.MainWindowHandle == IntPtr.Zero) continue;
                _appWin = _childp.MainWindowHandle;
                return;
            }
        }

        protected void OnResize(object s, SizeChangedEventArgs e)
        {
            if (_appWin != IntPtr.Zero) MoveWindow(_appWin, 0, 30, (int)ActualWidth, (int)ActualHeight, true);
        }

        public void Cleanup()
        {
            if (_childp != null)
                _childp.CloseMainWindow();
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct HWND__
        {
            /// int
            public int unused;
        }
    }
}