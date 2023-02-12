// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace Fireasy.Windows.Forms
{
    internal class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern bool MessageBeep(MessageBoxIcon mbi);

        internal const int W_COMMAND = 0x111;
        internal const int W_PAINT = 15;
        internal const int W_NCPAINT = 0x85;
        internal const int W_USER = 0x400;
        internal const int W_REFLECT = W_USER + 0x1C00;
        internal const int CBN_DROPDOWN = 7;

        internal static int HIWORD(int n)
        {
            return (short)((n >> 16) & 0xffff);
        }
    }
}
