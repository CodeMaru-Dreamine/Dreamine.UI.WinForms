using System.Windows.Forms;

namespace Dreamine.UI.WinForms.MessageBox;

/// <summary>
/// WPF의 Dreamine.UI.Wpf.Controls.MessageBox.DreamineMessageBox와 동일한 API 형태를 제공하는
/// WinForms용 다크테마 커스텀 메시지박스.
/// </summary>
public static class DreamineMessageBox
{
    private static bool _isOpen;
    private static string? _lastTitle;
    private static string? _lastMessage;

    /// <summary>메시지박스를 모달로 표시하고, 사용자가 닫을 때까지 대기한다.</summary>
    public static DialogResult Show(
        string message,
        string title = "Information",
        MessageBoxButtons buttons = MessageBoxButtons.OK,
        MessageBoxIcon icon = MessageBoxIcon.None,
        DialogResult autoClick = DialogResult.None,
        int autoClickDelaySeconds = 0,
        int enableDelaySeconds = 0)
    {
        using var form = new DreamineMessageBoxForm(title, message, icon, buttons, autoClick, autoClickDelaySeconds, enableDelaySeconds);
        form.ShowDialog();
        return form.Result;
    }

    /// <summary>
    /// 메시지박스를 비동기(논블로킹)로 표시한다. 동일한 제목/메시지가 이미 열려 있으면 중복 표시를 막는다.
    /// </summary>
    public static void ShowAsync(
        string message,
        string title = "Information",
        MessageBoxButtons buttons = MessageBoxButtons.OK,
        MessageBoxIcon icon = MessageBoxIcon.None,
        Action<DialogResult>? callback = null,
        DialogResult autoClick = DialogResult.None,
        int autoClickDelaySeconds = 0,
        int enableDelaySeconds = 0)
    {
        if (_isOpen && _lastTitle == title && _lastMessage == message)
            return;

        _isOpen = true;
        _lastTitle = title;
        _lastMessage = message;

        var form = new DreamineMessageBoxForm(title, message, icon, buttons, autoClick, autoClickDelaySeconds, enableDelaySeconds);
        form.FormClosed += (_, _) =>
        {
            _isOpen = false;
            _lastTitle = null;
            _lastMessage = null;
            try { callback?.Invoke(form.Result); }
            finally { form.Dispose(); }
        };
        form.Show();
    }
}
