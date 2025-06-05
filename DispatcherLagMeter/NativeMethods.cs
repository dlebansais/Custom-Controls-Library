namespace CustomControls;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Contracts;

/// <summary>
/// Contains native methods.
/// </summary>
internal static class NativeMethods
{
    /// <summary>
    /// Windows Win32 messages.
    /// </summary>
    private enum WindowsMessage
    {
        /// <summary>Sent after a window has been moved.</summary>
        WM_MOVE = 0x0003,
        /// <summary>
        /// Sent to a window when the size or position of the window is about to change.
        /// An application can use this message to override the window's default maximized size and position,
        /// or its default minimum or maximum tracking size.
        /// </summary>
        WM_GETMINMAXINFO = 0x0024,
        /// <summary>
        /// Sent to a window whose size, position, or place in the Z order is about to change as a result
        /// of a call to the SetWindowPos function or another window-management function.
        /// </summary>
        WM_WINDOWPOSCHANGING = 0x0046,
        /// <summary>
        /// Sent to a window whose size, position, or place in the Z order has changed as a result of a
        /// call to the SetWindowPos function or another window-management function.
        /// </summary>
        WM_WINDOWPOSCHANGED = 0x0047,
        /// <summary>
        /// Sent to a window that the user is moving. By processing this message, an application can monitor
        /// the position of the drag rectangle and, if needed, change its position.
        /// </summary>
        WM_MOVING = 0x0216,
        /// <summary>
        /// Sent once to a window after it enters the moving or sizing modal loop. The window enters the
        /// moving or sizing modal loop when the user clicks the window's title bar or sizing border, or
        /// when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam
        /// parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete
        /// when DefWindowProc returns.
        /// <para />
        /// The system sends the WM_ENTERSIZEMOVE message regardless of whether the dragging of full windows
        /// is enabled.
        /// </summary>
        WM_ENTERSIZEMOVE = 0x0231,
        /// <summary>
        /// Sent once to a window once it has exited moving or sizing modal loop. The window enters the
        /// moving or sizing modal loop when the user clicks the window's title bar or sizing border, or
        /// when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the
        /// wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is
        /// complete when DefWindowProc returns.
        /// </summary>
        WM_EXITSIZEMOVE = 0x0232,
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern IntPtr DefWindowProc(
        IntPtr hWnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern void ClientToScreen(IntPtr hWnd, ref NativePoint lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern void GetClientRect(IntPtr hWnd, ref NativeRect lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern void GetWindowRect(IntPtr hWnd, ref NativeRect lpRect);

    [StructLayout(LayoutKind.Sequential)]
    private struct WINDOWPOS
    {
        public IntPtr Hwnd;
        public IntPtr HwndInsertAfter;
        public int X;
        public int Y;
        public int CX;
        public int CY;
        public int Flags;
    }

    /// <summary>
    /// Gets the client area coordinates.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <param name="windowPosition">The client coordinates upon return.</param>
    /// <param name="clientAreaOffsetX">The offset to apply along the X axis.</param>
    /// <param name="clientAreaOffsetY">The offset to apply along the Y axis.</param>
    public static void GetClientArea(Window window, out Point windowPosition, out int clientAreaOffsetX, out int clientAreaOffsetY)
    {
        IntPtr Hwnd = new WindowInteropHelper(window).Handle;
        HwndSource HwndSource = HwndSource.FromHwnd(Hwnd);
        NativePoint lpPoint = default;
        ClientToScreen(HwndSource.Handle, ref lpPoint);
        NativeRect rcRect = default;
        GetClientRect(HwndSource.Handle, ref rcRect);
        NativeRect rcWinRect = default;
        GetWindowRect(HwndSource.Handle, ref rcWinRect);

        windowPosition = new Point(lpPoint.X, lpPoint.Y);
        clientAreaOffsetX = ((rcWinRect.Right - rcWinRect.Left) - (rcRect.Right - rcRect.Left)) / 2;
        clientAreaOffsetY = (rcWinRect.Bottom - rcWinRect.Top) - (rcRect.Bottom - rcRect.Top);
    }

    /// <summary>
    ///  Installs a hook for the WM_WINDOWPOSCHANGED message on a window.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <param name="hookHandler">The message hander.</param>
    public static void InstallHook(Window window, Action hookHandler)
    {
        HookHandler = hookHandler;

        IntPtr Hwnd = new WindowInteropHelper(window).Handle;
        HwndSource HwndSource = HwndSource.FromHwnd(Hwnd);
        HwndSource.AddHook(WndProc);
    }

    private static Action HookHandler = () => { };

    private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == (uint)WindowsMessage.WM_WINDOWPOSCHANGED)
        {
            _ = DefWindowProc(hwnd, msg, wParam, lParam);

            WINDOWPOS wp = Marshal.PtrToStructure<WINDOWPOS>(lParam);
            const int SWP_NOMOVE = 0x0002;

            if ((wp.Flags & SWP_NOMOVE) == 0)
                HookHandler();
        }

        return IntPtr.Zero;
    }
}
