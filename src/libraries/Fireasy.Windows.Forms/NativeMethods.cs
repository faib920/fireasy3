﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace Fireasy.Windows.Forms
{
    internal class NativeMethods
    {
        internal delegate bool EnumChildProc(IntPtr hWnd, IntPtr param);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal extern static IntPtr FindWindow(string className, string caption);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern uint SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc callback, IntPtr param);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal extern static IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool ScreenToClient(IntPtr hWnd, ref POINT pt);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder className, int maxCount);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder sb, int maxCount);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowLong(IntPtr hWnd, int Index);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public override string ToString()
            {
                return ("{left=" + left.ToString() + ", top=" + top.ToString() + ", right=" + right.ToString() + ", bottom=" + bottom.ToString() + "}");
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int x;
            public int y;
        }
        internal static int HIWORD(int n)
        {
            return (short)((n >> 16) & 0xffff);
        }

        internal static int HIWORD(IntPtr n)
        {
            return HIWORD(unchecked((int)(long)n));
        }

        internal static int LOWORD(int n)
        {
            return (short)(n & 0xffff);
        }

        internal static int LOWORD(IntPtr n)
        {
            return LOWORD(unchecked((int)(long)n));
        }

        internal static IntPtr MakeWParam(int lowWord, int highWord)
        {
            int wparam = highWord << 16;
            wparam |= (lowWord & 0xffff);

            return new IntPtr(wparam);
        }

        internal const int W_COMMAND = 0x111;
        internal const int W_USER = 0x400;
        internal const int W_REFLECT = W_USER + 0x1C00;

        internal const string CLS_BUTTON = "BUTTON";
        internal const string CLS_STATIC = "STATIC";

        internal const int SS_ICON = 3;

        internal const int GWL_STYLE = -16;
        internal const int GWL_ID = -12;

        //  Dialog Box Command IDs
        internal const int IDOK = 1;
        internal const int IDCANCEL = 2;
        internal const int IDABORT = 3;
        internal const int IDRETRY = 4;
        internal const int IDIGNORE = 5;
        internal const int IDYES = 6;
        internal const int IDNO = 7;
        internal const int IDCLOSE = 8;
        internal const int IDHELP = 9;
        internal const int IDTRYAGAIN = 10;
        internal const int IDCONTINUE = 11;

        // Button notification code
        internal const int BN_CLICKED = 0;

        [StructLayout(LayoutKind.Sequential)]
        internal struct MINMAXINFO
        {
            public Point reserved;
            public Size maxSize;
            public Point maxPosition;
            public Size minTrackSize;
            public Size maxTrackSize;
        }

        private static HandleRef HWND_TOPMOST = new HandleRef(null, new IntPtr(-1));

        [Flags]
        internal enum AnimationFlags : int
        {
            Roll = 0x0000,
            HorizontalPositive = 0x00001,
            HorizontalNegative = 0x00002,
            VerticalPositive = 0x00004,
            VerticalNegative = 0x00008,
            Center = 0x00010,
            Hide = 0x10000,
            Activate = 0x20000,
            Slide = 0x40000,
            Blend = 0x80000,
            Mask = 0xfffff,
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int AnimateWindow(HandleRef windowHandle, int time, AnimationFlags flags);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        internal static void SetTopMost(Control control)
        {
            try
            {
                var sp = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                sp.Demand();
                SetWindowPos(new HandleRef(control, control.Handle), HWND_TOPMOST, 0, 0, 0, 0, 0x13);
            }
            catch (SecurityException) { }
        }

        internal enum ShowWindowStyles : short
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }

        internal enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_CAPTION = 0x00C00000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,
            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,
            WS_TILED = 0x00000000,
            WS_ICONIC = 0x20000000,
            WS_SIZEBOX = 0x00040000,
            WS_POPUPWINDOW = 0x80880000,
            WS_OVERLAPPEDWINDOW = 0x00CF0000,
            WS_TILEDWINDOW = 0x00CF0000,
            WS_CHILDWINDOW = 0x40000000
        }

        internal enum WindowExStyles
        {
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_OVERLAPPEDWINDOW = 0x00000300,
            WS_EX_PALETTEWINDOW = 0x00000188,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_NOACTIVATE = 0x08000000
        }

        internal enum Msgs
        {
            W_NULL = 0x0000,
            W_CREATE = 0x0001,
            W_DESTROY = 0x0002,
            W_MOVE = 0x0003,
            W_SIZE = 0x0005,
            W_ACTIVATE = 0x0006,
            W_SETFOCUS = 0x0007,
            W_KILLFOCUS = 0x0008,
            W_ENABLE = 0x000A,
            W_SETREDRAW = 0x000B,
            W_SETTEXT = 0x000C,
            W_GETTEXT = 0x000D,
            W_GETTEXTLENGTH = 0x000E,
            W_PAINT = 0x000F,
            W_CLOSE = 0x0010,
            W_QUERYENDSESSION = 0x0011,
            W_QUIT = 0x0012,
            W_QUERYOPEN = 0x0013,
            W_ERASEBKGND = 0x0014,
            W_SYSCOLORCHANGE = 0x0015,
            W_ENDSESSION = 0x0016,
            W_SHOWWINDOW = 0x0018,
            W_WININICHANGE = 0x001A,
            W_SETTINGCHANGE = 0x001A,
            W_DEVMODECHANGE = 0x001B,
            W_ACTIVATEAPP = 0x001C,
            W_FONTCHANGE = 0x001D,
            W_TIMECHANGE = 0x001E,
            W_CANCELMODE = 0x001F,
            W_SETCURSOR = 0x0020,
            W_MOUSEACTIVATE = 0x0021,
            W_CHILDACTIVATE = 0x0022,
            W_QUEUESYNC = 0x0023,
            W_GETMINMAXINFO = 0x0024,
            W_PAINTICON = 0x0026,
            W_ICONERASEBKGND = 0x0027,
            W_NEXTDLGCTL = 0x0028,
            W_SPOOLERSTATUS = 0x002A,
            W_DRAWITEM = 0x002B,
            W_MEASUREITEM = 0x002C,
            W_DELETEITEM = 0x002D,
            W_VKEYTOITEM = 0x002E,
            W_CHARTOITEM = 0x002F,
            W_SETFONT = 0x0030,
            W_GETFONT = 0x0031,
            W_SETHOTKEY = 0x0032,
            W_GETHOTKEY = 0x0033,
            W_QUERYDRAGICON = 0x0037,
            W_COMPAREITEM = 0x0039,
            W_GETOBJECT = 0x003D,
            W_COMPACTING = 0x0041,
            W_COMMNOTIFY = 0x0044,
            W_WINDOWPOSCHANGING = 0x0046,
            W_WINDOWPOSCHANGED = 0x0047,
            W_POWER = 0x0048,
            W_COPYDATA = 0x004A,
            W_CANCELJOURNAL = 0x004B,
            W_NOTIFY = 0x004E,
            W_INPUTLANGCHANGEREQUEST = 0x0050,
            W_INPUTLANGCHANGE = 0x0051,
            W_TCARD = 0x0052,
            W_HELP = 0x0053,
            W_USERCHANGED = 0x0054,
            W_NOTIFYFORMAT = 0x0055,
            W_CONTEXTMENU = 0x007B,
            W_STYLECHANGING = 0x007C,
            W_STYLECHANGED = 0x007D,
            W_DISPLAYCHANGE = 0x007E,
            W_GETICON = 0x007F,
            W_SETICON = 0x0080,
            W_NCCREATE = 0x0081,
            W_NCDESTROY = 0x0082,
            W_NCCALCSIZE = 0x0083,
            W_NCHITTEST = 0x0084,
            W_NCPAINT = 0x0085,
            W_NCACTIVATE = 0x0086,
            W_GETDLGCODE = 0x0087,
            W_SYNCPAINT = 0x0088,
            W_NCMOUSEMOVE = 0x00A0,
            W_NCLBUTTONDOWN = 0x00A1,
            W_NCLBUTTONUP = 0x00A2,
            W_NCLBUTTONDBLCLK = 0x00A3,
            W_NCRBUTTONDOWN = 0x00A4,
            W_NCRBUTTONUP = 0x00A5,
            W_NCRBUTTONDBLCLK = 0x00A6,
            W_NCMBUTTONDOWN = 0x00A7,
            W_NCMBUTTONUP = 0x00A8,
            W_NCMBUTTONDBLCLK = 0x00A9,
            W_KEYDOWN = 0x0100,
            W_KEYUP = 0x0101,
            W_CHAR = 0x0102,
            W_DEADCHAR = 0x0103,
            W_SYSKEYDOWN = 0x0104,
            W_SYSKEYUP = 0x0105,
            W_SYSCHAR = 0x0106,
            W_SYSDEADCHAR = 0x0107,
            W_KEYLAST = 0x0108,
            W_IME_STARTCOMPOSITION = 0x010D,
            W_IME_ENDCOMPOSITION = 0x010E,
            W_IME_COMPOSITION = 0x010F,
            W_IME_KEYLAST = 0x010F,
            W_INITDIALOG = 0x0110,
            W_COMMAND = 0x0111,
            W_SYSCOMMAND = 0x0112,
            W_TIMER = 0x0113,
            W_HSCROLL = 0x0114,
            W_VSCROLL = 0x0115,
            W_INITMENU = 0x0116,
            W_INITMENUPOPUP = 0x0117,
            W_MENUSELECT = 0x011F,
            W_MENUCHAR = 0x0120,
            W_ENTERIDLE = 0x0121,
            W_MENURBUTTONUP = 0x0122,
            W_MENUDRAG = 0x0123,
            W_MENUGETOBJECT = 0x0124,
            W_UNINITMENUPOPUP = 0x0125,
            W_MENUCOMMAND = 0x0126,
            W_CTLCOLORMSGBOX = 0x0132,
            W_CTLCOLOREDIT = 0x0133,
            W_CTLCOLORLISTBOX = 0x0134,
            W_CTLCOLORBTN = 0x0135,
            W_CTLCOLORDLG = 0x0136,
            W_CTLCOLORSCROLLBAR = 0x0137,
            W_CTLCOLORSTATIC = 0x0138,
            W_MOUSEMOVE = 0x0200,
            W_LBUTTONDOWN = 0x0201,
            W_LBUTTONUP = 0x0202,
            W_LBUTTONDBLCLK = 0x0203,
            W_RBUTTONDOWN = 0x0204,
            W_RBUTTONUP = 0x0205,
            W_RBUTTONDBLCLK = 0x0206,
            W_MBUTTONDOWN = 0x0207,
            W_MBUTTONUP = 0x0208,
            W_MBUTTONDBLCLK = 0x0209,
            W_MOUSEWHEEL = 0x020A,
            W_PARENTNOTIFY = 0x0210,
            W_ENTERMENULOOP = 0x0211,
            W_EXITMENULOOP = 0x0212,
            W_NEXTMENU = 0x0213,
            W_SIZING = 0x0214,
            W_CAPTURECHANGED = 0x0215,
            W_MOVING = 0x0216,
            W_DEVICECHANGE = 0x0219,
            W_MDICREATE = 0x0220,
            W_MDIDESTROY = 0x0221,
            W_MDIACTIVATE = 0x0222,
            W_MDIRESTORE = 0x0223,
            W_MDINEXT = 0x0224,
            W_MDIMAXIMIZE = 0x0225,
            W_MDITILE = 0x0226,
            W_MDICASCADE = 0x0227,
            W_MDIICONARRANGE = 0x0228,
            W_MDIGETACTIVE = 0x0229,
            W_MDISETMENU = 0x0230,
            W_ENTERSIZEMOVE = 0x0231,
            W_EXITSIZEMOVE = 0x0232,
            W_DROPFILES = 0x0233,
            W_MDIREFRESHMENU = 0x0234,
            W_IME_SETCONTEXT = 0x0281,
            W_IME_NOTIFY = 0x0282,
            W_IME_CONTROL = 0x0283,
            W_IME_COMPOSITIONFULL = 0x0284,
            W_IME_SELECT = 0x0285,
            W_IME_CHAR = 0x0286,
            W_IME_REQUEST = 0x0288,
            W_IME_KEYDOWN = 0x0290,
            W_IME_KEYUP = 0x0291,
            W_MOUSEHOVER = 0x02A1,
            W_MOUSELEAVE = 0x02A3,
            W_CUT = 0x0300,
            W_COPY = 0x0301,
            W_PASTE = 0x0302,
            W_CLEAR = 0x0303,
            W_UNDO = 0x0304,
            W_RENDERFORMAT = 0x0305,
            W_RENDERALLFORMATS = 0x0306,
            W_DESTROYCLIPBOARD = 0x0307,
            W_DRAWCLIPBOARD = 0x0308,
            W_PAINTCLIPBOARD = 0x0309,
            W_VSCROLLCLIPBOARD = 0x030A,
            W_SIZECLIPBOARD = 0x030B,
            W_ASKCBFORMATNAME = 0x030C,
            W_CHANGECBCHAIN = 0x030D,
            W_HSCROLLCLIPBOARD = 0x030E,
            W_QUERYNEWPALETTE = 0x030F,
            W_PALETTEISCHANGING = 0x0310,
            W_PALETTECHANGED = 0x0311,
            W_HOTKEY = 0x0312,
            W_PRINT = 0x0317,
            W_PRINTCLIENT = 0x0318,
            W_HANDHELDFIRST = 0x0358,
            W_HANDHELDLAST = 0x035F,
            W_AFXFIRST = 0x0360,
            W_AFXLAST = 0x037F,
            W_PENWINFIRST = 0x0380,
            W_PENWINLAST = 0x038F,
            W_APP = 0x8000,
            W_USER = 0x0400
        }

        internal enum HitTest
        {
            HTERROR = -2,
            HTTRANSPARENT = -1,
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTSIZE = 4,
            HTMENU = 5,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTBORDER = 18,
            HTREDUCE = 8,
            HTZOOM = 9,
            HTSIZEFIRST = 10,
            HTSIZELAST = 17,
            HTOBJECT = 19,
            HTCLOSE = 20,
            HTHELP = 21
        }

        internal enum ScrollBars : uint
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        internal enum GetWindowLongIndex : int
        {
            GWL_STYLE = -16,
            GWL_EXSTYLE = -20
        }

        internal enum FlagsAnimateWindow : uint
        {
            AW_ACTIVATE = 0x20000,
            AW_BLEND = 0x80000,
            AW_CENTER = 0x10,
            AW_HIDE = 0x10000,
            AW_HOR_NEGATIVE = 2,
            AW_HOR_POSITIVE = 1,
            AW_SLIDE = 0x40000,
            AW_VER_NEGATIVE = 8,
            AW_VER_POSITIVE = 4
        }

        internal enum SystemParametersInfoActions : uint
        {
            GetAccessTimeout = 60,
            GetAnimation = 0x48,
            GetBeep = 1,
            GetBorder = 5,
            GetDefaultInputLang = 0x59,
            GetDragFullWindows = 0x26,
            GetFastTaskSwitch = 0x23,
            GetFilterKeys = 50,
            GetFontSmoothing = 0x4a,
            GetGridGranularity = 0x12,
            GetHighContrast = 0x42,
            GetIconMetrics = 0x2d,
            GetIconTitleLogFont = 0x1f,
            GetIconTitleWrap = 0x19,
            GetKeyboardDelay = 0x16,
            GetKeyboardPref = 0x44,
            GetKeyboardSpeed = 10,
            GetLowPowerActive = 0x53,
            GetLowPowerTimeout = 0x4f,
            GetMenuDropAlignment = 0x1b,
            GetMinimizedMetrics = 0x2b,
            GetMouse = 3,
            GetMouseHoverTime = 0x66,
            GetMouseKeys = 0x36,
            GetMouseTrails = 0x5e,
            GetNonClientMetrics = 0x29,
            GetPowerOffActive = 0x54,
            GetPowerOffTimeout = 80,
            GetScreenReader = 70,
            GetScreenSaveActive = 0x10,
            GetScreenSaveTimeout = 14,
            GetSerialKeys = 0x3e,
            GetShowSounds = 0x38,
            GetSoundsEntry = 0x40,
            GetStickyKeys = 0x3a,
            GetToggleKeys = 0x34,
            GetWindwosExtension = 0x5c,
            GetWorkArea = 0x30,
            IconHorizontalSpacing = 13,
            IconVerticalSpacing = 0x18,
            LangDriver = 12,
            ScreenSaverRunning = 0x61,
            SetAccessTimeout = 0x3d,
            SetAnimation = 0x49,
            SetBeep = 2,
            SetBorder = 6,
            SetCursors = 0x57,
            SetDefaultInputLang = 90,
            SetDeskPattern = 0x15,
            SetDeskWallPaper = 20,
            SetDoubleClickTime = 0x20,
            SetDoubleClkHeight = 30,
            SetDoubleClkWidth = 0x1d,
            SetDragFullWindows = 0x25,
            SetDragHeight = 0x4d,
            SetDragWidth = 0x4c,
            SetFastTaskSwitch = 0x24,
            SetFilterKeys = 0x33,
            SetFontSmoothing = 0x4b,
            SetGridGranularity = 0x13,
            SetHandHeld = 0x4e,
            SetHighContrast = 0x43,
            SetIconMetrics = 0x2e,
            SetIcons = 0x58,
            SetIconTitleLogFont = 0x22,
            SetIconTitleWrap = 0x1a,
            SetKeyboardDelay = 0x17,
            SetKeyboardPref = 0x45,
            SetKeyboardSpeed = 11,
            SetLangToggle = 0x5b,
            SetLowPowerActive = 0x55,
            SetLowPowerTimeout = 0x51,
            SetMenuDropAlignment = 0x1c,
            SetMinimizedMetrics = 0x2c,
            SetMouse = 4,
            SetMouseButtonSwap = 0x21,
            SetMouseKeys = 0x37,
            SetMouseTrails = 0x5d,
            SetNonClientMetrics = 0x2a,
            SetPenWindows = 0x31,
            SetPowerOffActive = 0x56,
            SetPowerOffTimeout = 0x52,
            SetScreenReader = 0x47,
            SetScreenSaveActive = 0x11,
            SetScreenSaveTimeout = 15,
            SetSerialKeys = 0x3f,
            SetShowSounds = 0x39,
            SetSoundsEntry = 0x41,
            SetStickyKeys = 0x3b,
            SetToggleKeys = 0x35,
            SetWorkArea = 0x2f
        }

        internal enum FlagsSetWindowPos : uint
        {
            SWP_ASYNCWINDOWPOS = 0x4000,
            SWP_DEFERERASE = 0x2000,
            SWP_DRAWFRAME = 0x20,
            SWP_FRAMECHANGED = 0x20,
            SWP_HIDEWINDOW = 0x80,
            SWP_NOACTIVATE = 0x10,
            SWP_NOCOPYBITS = 0x100,
            SWP_NOMOVE = 2,
            SWP_NOOWNERZORDER = 0x200,
            SWP_NOREDRAW = 8,
            SWP_NOREPOSITION = 0x200,
            SWP_NOSENDCHANGING = 0x400,
            SWP_NOSIZE = 1,
            SWP_NOZORDER = 4,
            SWP_SHOWWINDOW = 0x40
        }

        internal enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }
    }
}
