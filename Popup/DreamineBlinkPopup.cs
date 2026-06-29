using System.Windows.Forms;

namespace Dreamine.UI.WinForms.Popup;

/// <summary>
/// WPF의 IPopupService.ShowBlinkAsync와 동일한 역할을 하는 WinForms용 깜빡임 팝업 서비스.
/// </summary>
public static class DreamineBlinkPopup
{
    /// <summary>깜빡임 팝업을 표시하고, 사용자가 닫을 때까지 비동기로 대기한다.</summary>
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
