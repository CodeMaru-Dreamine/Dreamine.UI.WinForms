using System.Runtime.InteropServices;

namespace Dreamine.UI.WinForms.VirtualKeyboard;

/// <summary>
/// \if KO
/// <para>Win32 IMM API를 사용하여 지정한 창의 한글 네이티브 입력 모드를 조회하고 설정합니다.</para>
/// \endif
/// \if EN
/// <para>Uses the Win32 IMM API to query and configure native Hangul input mode for a window.</para>
/// \endif
/// </summary>
internal static class ImeHelper
{
    /// <summary>
    /// \if KO
    /// <para>IME CMODE NATIVE 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the ime cmode native value.</para>
    /// \endif
    /// </summary>
    private const uint IME_CMODE_NATIVE = 0x0001;

    /// <summary>
    /// \if KO
    /// <para>지정한 창의 입력 메서드 컨텍스트를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Retrieves the input-method context of the specified window.</para>
    /// \endif
    /// </summary>
    /// <param name="hWnd">
    /// \if KO
    /// <para>대상 창 핸들입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The target window handle.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>입력 메서드 컨텍스트 핸들이며 실패하면 0입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The input-method context handle, or zero on failure.</para>
    /// \endif
    /// </returns>
    [DllImport("imm32.dll")]
    private static extern IntPtr ImmGetContext(IntPtr hWnd);

    /// <summary>
    /// \if KO
    /// <para>이전에 가져온 입력 메서드 컨텍스트를 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Releases a previously retrieved input-method context.</para>
    /// \endif
    /// </summary>
    /// <param name="hWnd">
    /// \if KO
    /// <para>컨텍스트를 소유한 창 핸들입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The handle of the window that owns the context.</para>
    /// \endif
    /// </param>
    /// <param name="hIMC">
    /// \if KO
    /// <para>해제할 입력 메서드 컨텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The input-method context to release.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>해제에 성공하면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the context is released successfully.</para>
    /// \endif
    /// </returns>
    [DllImport("imm32.dll")]
    private static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

    /// <summary>
    /// \if KO
    /// <para>입력 메서드 컨텍스트가 열려 있는지 조회합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Queries whether an input-method context is open.</para>
    /// \endif
    /// </summary>
    /// <param name="hIMC">
    /// \if KO
    /// <para>조회할 입력 메서드 컨텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The input-method context to query.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>입력 메서드가 열려 있으면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> if the input method is open.</para>
    /// \endif
    /// </returns>
    [DllImport("imm32.dll")]
    private static extern bool ImmGetOpenStatus(IntPtr hIMC);

    /// <summary>
    /// \if KO
    /// <para>입력 메서드 컨텍스트의 열림 상태를 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Sets the open state of an input-method context.</para>
    /// \endif
    /// </summary>
    /// <param name="hIMC">
    /// \if KO
    /// <para>변경할 입력 메서드 컨텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The input-method context to modify.</para>
    /// \endif
    /// </param>
    /// <param name="open">
    /// \if KO
    /// <para>입력 메서드를 열려면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> to open the input method.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>요청이 성공하면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the request succeeds.</para>
    /// \endif
    /// </returns>
    [DllImport("imm32.dll")]
    private static extern bool ImmSetOpenStatus(IntPtr hIMC, bool open);

    /// <summary>
    /// \if KO
    /// <para>입력 메서드 컨텍스트의 변환 및 문장 모드를 조회합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Retrieves conversion and sentence modes from an input-method context.</para>
    /// \endif
    /// </summary>
    /// <param name="hIMC">
    /// \if KO
    /// <para>조회할 입력 메서드 컨텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The input-method context to query.</para>
    /// \endif
    /// </param>
    /// <param name="conversion">
    /// \if KO
    /// <para>변환 모드 플래그를 받습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Receives the conversion-mode flags.</para>
    /// \endif
    /// </param>
    /// <param name="sentence">
    /// \if KO
    /// <para>문장 모드 플래그를 받습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Receives the sentence-mode flags.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>상태 조회에 성공하면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the status is retrieved successfully.</para>
    /// \endif
    /// </returns>
    [DllImport("imm32.dll")]
    private static extern bool ImmGetConversionStatus(IntPtr hIMC, out uint conversion, out uint sentence);

    /// <summary>
    /// \if KO
    /// <para>입력 메서드 컨텍스트의 변환 및 문장 모드를 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Sets conversion and sentence modes on an input-method context.</para>
    /// \endif
    /// </summary>
    /// <param name="hIMC">
    /// \if KO
    /// <para>변경할 입력 메서드 컨텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The input-method context to modify.</para>
    /// \endif
    /// </param>
    /// <param name="conversion">
    /// \if KO
    /// <para>적용할 변환 모드 플래그입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The conversion-mode flags to apply.</para>
    /// \endif
    /// </param>
    /// <param name="sentence">
    /// \if KO
    /// <para>적용할 문장 모드 플래그입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The sentence-mode flags to apply.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>상태 설정에 성공하면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the status is set successfully.</para>
    /// \endif
    /// </returns>
    [DllImport("imm32.dll")]
    private static extern bool ImmSetConversionStatus(IntPtr hIMC, uint conversion, uint sentence);

    /// <summary>
    /// \if KO
    /// <para>지정한 창의 IME가 열려 있고 네이티브 변환 모드인지 확인합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Determines whether the specified window's IME is open and in native conversion mode.</para>
    /// \endif
    /// </summary>
    /// <param name="hwnd">
    /// \if KO
    /// <para>확인할 창 핸들입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The window handle to inspect.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>네이티브 입력 모드이면 <see langword="true"/>, 그렇지 않거나 조회에 실패하면 <see langword="false"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> in native input mode; otherwise, or when the query fails, <see langword="false"/>.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>지정한 창의 IME 열림 상태와 네이티브 변환 플래그를 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures the IME open state and native conversion flag of the specified window.</para>
    /// \endif
    /// </summary>
    /// <param name="hwnd">
    /// \if KO
    /// <para>변경할 창 핸들입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The window handle to modify.</para>
    /// \endif
    /// </param>
    /// <param name="native">
    /// \if KO
    /// <para>네이티브 입력을 활성화하려면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> to enable native input.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>유효한 입력 메서드 컨텍스트를 얻어 설정을 시도했으면 <see langword="true"/>, 그렇지 않으면 <see langword="false"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> if a valid input-method context was obtained and configuration attempted; otherwise, <see langword="false"/>.</para>
    /// \endif
    /// </returns>
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
