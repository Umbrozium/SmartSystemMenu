using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Text.RegularExpressions;
using SmartSystemMenu.Native;
using SmartSystemMenu.Native.Enums;
using SmartSystemMenu.Native.Structs;
using static SmartSystemMenu.Native.Gdi32;
using static SmartSystemMenu.Native.User32;
using static SmartSystemMenu.Native.Kernel32;
using static SmartSystemMenu.Native.Constants;

namespace SmartSystemMenu.Utils
{
    static class WindowUtils
    {
        private static readonly Regex HwndWrapperRegex =
            new Regex(
                @"^HwndWrapper\[(?<process>[^;]+);;[0-9a-fA-F\-]{36}\]$",
                RegexOptions.Compiled);

        public static string NormalizeClassName(string className)
        {
            if (string.IsNullOrEmpty(className))
                return className;

            var match = HwndWrapperRegex.Match(className);
            if (!match.Success)
                return className;

            return $"HwndWrapper[{match.Groups["process"].Value};;*]";
        }

        public static string GetClassName(IntPtr hWnd)
        {
            var sb = new StringBuilder(256);
            User32.GetClassName(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public static bool PrintWindow(IntPtr hWnd, out Bitmap bitmap)
        {
            GetWindowRect(hWnd, out var rect);
            bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);
            var hdc = graphics.GetHdc();
            var result = User32.PrintWindow(hWnd, hdc, 0);
            graphics.ReleaseHdc(hdc);
            return result;
        }

        public static IntPtr GetParentWindow(IntPtr hWnd)
        {
            IntPtr parent;
            var result = hWnd;
            while ((parent = GetParent(result)) != IntPtr.Zero)
                result = parent;
            return result;
        }

        public static int GetProcessId(IntPtr hWnd)
        {
            GetWindowThreadProcessId(hWnd, out var pid);
            return pid;
        }
    }
}
