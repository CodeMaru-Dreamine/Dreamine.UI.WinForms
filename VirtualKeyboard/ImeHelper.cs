using System.Runtime.InteropServices;

namespace Dreamine.UI.WinForms.VirtualKeyboard;

internal static class ImeHelper
{
    private const uint IME_CMODE_NATIVE = 0x0001;

    [DllImport("imm32.dll")]
    private static extern IntPtr ImmGetContext(IntPtr hWnd);

    [DllImport("imm32.dll")]
    private static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

    [DllImport("imm32.dll")]
    private static extern bool ImmGetOpenStatus(IntPtr hIMC);

    [DllImport("imm32.dll")]
    private static extern bool ImmSetOpenStatus(IntPtr hIMC, bool open);

    [DllImport("imm32.dll")]
    private static extern bool ImmGetConversionStatus(IntPtr hIMC, out uint conversion, out uint sentence);

    [DllImport("imm32.dll")]
    private static extern bool ImmSetConversionStatus(IntPtr hIMC, uint conversion, uint sentence);

    public static bool IsNativeMode(IntPtr hwnd)
    {
        if (hwnd == IntPtr.Zero)
            return false;

        var context = ImmGetContext(hwnd);
        if (context == IntPtr.Zero)
            return false;

        try
        {
            if (!ImmGetOpenStatus(context))
                return false;

            return ImmGetConversionStatus(context, out var conversion, out _) &&
                   (conversion & IME_CMODE_NATIVE) != 0;
        }
        finally
        {
            ImmReleaseContext(hwnd, context);
        }
    }

    public static bool SetNativeMode(IntPtr hwnd, bool native)
    {
        if (hwnd == IntPtr.Zero)
            return false;

        var context = ImmGetContext(hwnd);
        if (context == IntPtr.Zero)
            return false;

        try
        {
            ImmSetOpenStatus(context, native);

            if (ImmGetConversionStatus(context, out var conversion, out var sentence))
            {
                conversion = native ? conversion | IME_CMODE_NATIVE : conversion & ~IME_CMODE_NATIVE;
                ImmSetConversionStatus(context, conversion, sentence);
            }

            return true;
        }
        finally
        {
            ImmReleaseContext(hwnd, context);
        }
    }
}
