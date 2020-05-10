namespace FreeEcho
{
    namespace FEMouseMoveWindow
    {
        /// <summary>
        /// POINT
        /// </summary>
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        struct POINT
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// RECT
        /// </summary>
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>
        /// 低レベルのマウス入力イベント情報
        /// </summary>
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public System.IntPtr dwExtraInfo;
        }

        /// <summary>
        /// ネイティブメソッド
        /// </summary>
        class NativeMethods
        {
            public delegate System.IntPtr MouseHookCallback(int nCode, uint msg, ref MSLLHOOKSTRUCT msllhookstruct);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr SetWindowsHookEx(int idHook, MouseHookCallback lpfn, System.IntPtr hMod, uint dwThreadId);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr CallNextHookEx(System.IntPtr hhk, int nCode, uint msg, ref MSLLHOOKSTRUCT msllhookstruct);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(System.IntPtr hhk);
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern System.IntPtr GetForegroundWindow();
            [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
            public static extern bool GetWindowRect(System.IntPtr hwnd, out RECT lpRect);
        }

        /// <summary>
        /// マウスでウィンドウ移動検知のイベントデータ
        /// </summary>
        public class WindowMoveDetectionMouseEventArgs
        {
        }

        /// <summary>
        /// マウスでウィンドウ移動開始のイベントデータ
        /// </summary>
        public class StartWindowMoveMouseEventArgs
        {
        }

        /// <summary>
        /// マウスでウィンドウ移動停止のイベントデータ
        /// </summary>
        public class StopWindowMoveMouseEventArgs
        {
        }

        /// <summary>
        /// マウスでウィンドウ移動検知
        /// </summary>
        public class MouseMoveWindow
        {
            /// <summary>
            /// フックを実行しているか
            /// </summary>
            public bool IsHooking
            {
                get;
                private set;
            } = false;
            /// <summary>
            /// ウィンドウが移動中
            /// </summary>
            public bool MovingWindow
            {
                get;
                set;
            } = false;
            /// <summary>
            /// フックプロシージャのハンドル
            /// </summary>
            private System.IntPtr Handle;
            /// <summary>
            /// マウスの左ボタンが押されているか
            /// </summary>
            private bool MouseDown = false;
            /// <summary>
            /// 前回調べた時のウィンドウの位置
            /// </summary>
            private RECT PreviousWindowPosition;

            /// <summary>
            /// マウスでウィンドウが移動された時のイベントのデリゲート
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void MovedEventHandler(
                object sender,
                WindowMoveDetectionMouseEventArgs e
                );
            /// <summary>
            /// マウスでウィンドウが移動された時のイベント
            /// </summary>
            public event MovedEventHandler Moved;
            /// <summary>
            /// マウスでウィンドウが移動された時のイベントを実行
            /// </summary>
            private void DoMoved()
            {
                Moved?.Invoke(this, new WindowMoveDetectionMouseEventArgs());
            }

            /// <summary>
            /// マウスでウィンドウ移動開始のイベントのデリゲート
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void MoveStartEventHandler(
                object sender,
                StartWindowMoveMouseEventArgs e
                );
            /// <summary>
            /// マウスでウィンドウ移動開始のイベント
            /// </summary>
            public event MoveStartEventHandler MoveStart;
            /// <summary>
            /// マウスでウィンドウ移動開始のイベントを実行
            /// </summary>
            private void DoMoveStart()
            {
                MoveStart?.Invoke(this, new StartWindowMoveMouseEventArgs());
            }

            /// <summary>
            /// マウスでウィンドウ移動停止のイベントのデリゲート
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public delegate void MoveStopEventHandler(
                object sender,
                StopWindowMoveMouseEventArgs e
                );
            /// <summary>
            /// マウスでウィンドウ移動停止のイベント
            /// </summary>
            public event MoveStopEventHandler MoveStop;
            /// <summary>
            /// マウスでウィンドウ移動停止のイベントを実行
            /// </summary>
            private void DoMoveStop()
            {
                MoveStop?.Invoke(this, new StopWindowMoveMouseEventArgs());
            }

            /// <summary>
            /// フックチェーンにインストールするフックプロシージャのイベント
            /// </summary>
            private event NativeMethods.MouseHookCallback HookCallback;

            /// <summary>
            /// マウスでウィンドウ移動の検知開始
            /// </summary>
            /// <exception cref="System.ComponentModel.Win32Exception"></exception>
            public void Start()
            {
                if (IsHooking == false)
                {
                    HookCallback = HookProcedure;
                    Handle = NativeMethods.SetWindowsHookEx(14, HookCallback, System.Runtime.InteropServices.Marshal.GetHINSTANCE(System.Reflection.Assembly.GetEntryAssembly().GetModules()[0]), 0);      // 14 = WH_MOUSE_LL
                    if (Handle == System.IntPtr.Zero)
                    {
                        throw new System.ComponentModel.Win32Exception();
                    }
                    IsHooking = true;
                }
            }

            /// <summary>
            /// マウスでウィンドウ移動の検知停止
            /// </summary>
            public void Stop()
            {
                if (IsHooking)
                {
                    if (Handle != System.IntPtr.Zero)
                    {
                        IsHooking = false;

                        NativeMethods.UnhookWindowsHookEx(Handle);
                        Handle = System.IntPtr.Zero;
                        HookCallback -= HookProcedure;
                    }
                }
            }

            /// <summary>
            /// フックプロシージャ
            /// </summary>
            /// <param name="nCode">フックコード</param>
            /// <param name="msg">フックプロシージャに渡す値</param>
            /// <param name="s">フックプロシージャに渡す値</param>
            /// <returns>フックチェーン内の次のフックプロシージャの戻り値</returns>
            private System.IntPtr HookProcedure(
                int nCode,
                uint msg,
                ref MSLLHOOKSTRUCT s
                )
            {
                if (0 <= nCode)
                {
                    switch (msg)
                    {
                        case 0x0201:        // WM_LBUTTONDOWN
                            MouseDown = true;
                            break;
                        case 0x0202:        // WM_LBUTTONUP
                            MouseDown = false;
                            if (MovingWindow)
                            {
                                MovingWindow = false;
                                DoMoveStop();
                                PreviousWindowPosition.Left = -1;
                                PreviousWindowPosition.Top = -1;
                                PreviousWindowPosition.Right = -1;
                                PreviousWindowPosition.Bottom = -1;
                            }
                            break;
                        case 0x0200:      // WM_MOUSEMOVE
                            if (MouseDown)
                            {
                                if (NativeMethods.GetWindowRect(NativeMethods.GetForegroundWindow(), out RECT window_rect))
                                {
                                    if (MovingWindow == false)
                                    {
                                        PreviousWindowPosition.Right = window_rect.Right - window_rect.Left;
                                        PreviousWindowPosition.Bottom = window_rect.Bottom - window_rect.Top;
                                        MovingWindow = true;
                                        DoMoveStart();
                                    }
                                    if (((window_rect.Left != PreviousWindowPosition.Left) || (window_rect.Top != PreviousWindowPosition.Top)) && ((PreviousWindowPosition.Right == (window_rect.Right - window_rect.Left)) && (PreviousWindowPosition.Bottom == (window_rect.Bottom - window_rect.Top))))
                                    {
                                        PreviousWindowPosition.Left = window_rect.Left;
                                        PreviousWindowPosition.Top = window_rect.Top;
                                        DoMoved();
                                    }
                                }
                            }
                            break;
                    }
                }

                return (NativeMethods.CallNextHookEx(Handle, nCode, msg, ref s));
            }
        }
    }
}
