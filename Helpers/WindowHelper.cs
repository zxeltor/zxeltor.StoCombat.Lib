// Copyright (c) 2024, Todd Taylor (https://github.com/zxeltor)
// All rights reserved.
// 
// This source code is licensed under the Apache-2.0-style license found in the
// LICENSE file in the root directory of this source tree.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace zxeltor.StoCombat.Lib.Helpers;

public static class WindowHelper
{
    #region Static Fields and Constants

    private const int GwlExStyle = -20;
    private const int WsExTransparent = 0x00000020;

    #endregion

    #region Public Members

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr FindWindow(string strClassName, string strWindowName);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

    public static bool IsOpen(this Window window)
    {
        return Application.Current.Windows.Cast<Window>().Any(x => x == window);
    }

    public static void SetWindowExTransparent(IntPtr hwnd)
    {
        var extendedStyle = GetWindowLong(hwnd, GwlExStyle);
        SetWindowLong(hwnd, GwlExStyle, extendedStyle | WsExTransparent);
    }

    public static bool TryGetApplicationWindowLocation(string applicationName, out Rect rectOfAppWindow)
    {
        rectOfAppWindow = new Rect();

        try
        {
            var processes = Process.GetProcesses().Where(proc => proc.MainWindowHandle != 0).ToList();
            var stoProc = processes.FirstOrDefault(proc =>
                proc.ProcessName.Equals("gameclient", StringComparison.CurrentCultureIgnoreCase));

            if (stoProc != null)
            {
                var ptr = stoProc.MainWindowHandle;
                var notepadRect = new Rect();
                GetWindowRect(ptr, ref notepadRect);
                return GetWindowRect(ptr, ref rectOfAppWindow);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return false;
    }

    #endregion

    #region Other Members

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

    #endregion
}