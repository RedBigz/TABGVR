using System;
using System.Runtime.InteropServices;

namespace TABGVR;

public static class Native
{
    [DllImport("shlwapi.dll", CharSet = CharSet.Ansi)]
    public static extern int ShellMessageBox(IntPtr hAppInst, IntPtr hWnd, string lpcText, string lpcTitle,
        uint fuStyle);
}