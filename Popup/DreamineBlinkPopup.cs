using System.Windows.Forms;

namespace Dreamine.UI.WinForms.Popup;

/// <summary>
/// \if KO
/// <para>WinForms 깜빡임 팝업을 비동기로 표시하는 진입점을 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides an entry point for asynchronously displaying WinForms blinking popups.</para>
/// \endif
/// </summary>
public static class DreamineBlinkPopup
{
    /// <summary>
    /// \if KO
    /// <para>지정한 옵션으로 깜빡임 팝업을 표시하고 닫힐 때까지 비동기로 기다립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Displays a blinking popup using the specified options and asynchronously waits until it closes.</para>
    /// \endif
    /// </summary>
    /// <param name="owner">
    /// \if KO
    /// <para>팝업 소유자이거나 소유자가 없으면 <see langword="null"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The popup owner, or <see langword="null"/> for no owner.</para>
    /// \endif
    /// </param>
    /// <param name="options">
    /// \if KO
    /// <para>팝업 콘텐츠와 표시 옵션입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The popup content and display options.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>팝업의 최종 WinForms 대화 상자 결과를 생성하는 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task that produces the popup's final WinForms dialog result.</para>
    /// \endif
    /// </returns>
    /// <exception cref="NullReferenceException">
    /// \if KO
    /// <para><paramref name="options"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="options"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    public static Task<DialogResult> ShowAsync(IWin32Window? owner, BlinkPopupOptions options)
    {
        var tcs = new TaskCompletionSource<DialogResult>();
        var form = new BlinkPopupForm(options);

        form.FormClosed += (_, _) =>
        {
            tcs.TrySetResult(form.Result);
            form.Dispose();
        };

        if (options.IsModal && owner is Form ownerForm)
        {
            // 모달처럼 동작하도록 owner를 비활성화했다가, 닫히면 복원한다.
            ownerForm.Enabled = false;
            form.FormClosed += (_, _) => ownerForm.Enabled = true;
        }

        form.Show(owner);
        return tcs.Task;
    }
}
