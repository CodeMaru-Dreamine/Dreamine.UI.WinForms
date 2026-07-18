using System.Windows.Forms;

namespace Dreamine.UI.WinForms.MessageBox;

/// <summary>
/// \if KO
/// <para>모달 및 비차단 방식으로 표시할 수 있는 WinForms 다크 테마 메시지 상자를 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides a WinForms dark-theme message box that can be displayed modally or non-blocking.</para>
/// \endif
/// </summary>
public static class DreamineMessageBox
{
    /// <summary>
    /// \if KO
    /// <para>is Open 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the is open value.</para>
    /// \endif
    /// </summary>
    private static bool _isOpen;
    /// <summary>
    /// \if KO
    /// <para>last Title 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the last title value.</para>
    /// \endif
    /// </summary>
    private static string? _lastTitle;
    /// <summary>
    /// \if KO
    /// <para>last Message 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the last message value.</para>
    /// \endif
    /// </summary>
    private static string? _lastMessage;

    /// <summary>
    /// \if KO
    /// <para>메시지 상자를 모달로 표시하고 사용자가 닫을 때까지 기다립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Displays a message box modally and waits until the user closes it.</para>
    /// \endif
    /// </summary>
    /// <param name="message">
    /// \if KO
    /// <para>표시할 메시지입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message to display.</para>
    /// \endif
    /// </param>
    /// <param name="title">
    /// \if KO
    /// <para>메시지 상자 제목입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message-box title.</para>
    /// \endif
    /// </param>
    /// <param name="buttons">
    /// \if KO
    /// <para>표시할 표준 버튼 조합입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The standard button combination to display.</para>
    /// \endif
    /// </param>
    /// <param name="icon">
    /// \if KO
    /// <para>메시지와 함께 표시할 표준 아이콘입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The standard icon displayed with the message.</para>
    /// \endif
    /// </param>
    /// <param name="autoClick">
    /// \if KO
    /// <para>카운트다운 만료 시 자동 선택할 결과입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The result selected automatically when the countdown expires.</para>
    /// \endif
    /// </param>
    /// <param name="autoClickDelaySeconds">
    /// \if KO
    /// <para>자동 선택 전 대기할 초입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The number of seconds before automatic selection.</para>
    /// \endif
    /// </param>
    /// <param name="enableDelaySeconds">
    /// \if KO
    /// <para>버튼을 활성화하기 전 대기할 초입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The number of seconds before enabling the buttons.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>사용자가 선택하거나 자동 선택된 결과입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The result selected by the user or automatic selection.</para>
    /// \endif
    /// </returns>
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
    /// \if KO
    /// <para>메시지 상자를 비차단 방식으로 표시하고 같은 제목과 메시지의 중복 표시를 방지합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Displays a message box non-blocking and prevents duplicates with the same title and message.</para>
    /// \endif
    /// </summary>
    /// <param name="message">
    /// \if KO
    /// <para>표시할 메시지입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message to display.</para>
    /// \endif
    /// </param>
    /// <param name="title">
    /// \if KO
    /// <para>메시지 상자 제목입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message-box title.</para>
    /// \endif
    /// </param>
    /// <param name="buttons">
    /// \if KO
    /// <para>표시할 표준 버튼 조합입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The standard button combination to display.</para>
    /// \endif
    /// </param>
    /// <param name="icon">
    /// \if KO
    /// <para>메시지와 함께 표시할 표준 아이콘입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The standard icon displayed with the message.</para>
    /// \endif
    /// </param>
    /// <param name="callback">
    /// \if KO
    /// <para>폼이 닫힌 후 결과와 함께 호출할 선택적 콜백입니다.</para>
    /// \endif
    /// \if EN
    /// <para>An optional callback invoked with the result after the form closes.</para>
    /// \endif
    /// </param>
    /// <param name="autoClick">
    /// \if KO
    /// <para>카운트다운 만료 시 자동 선택할 결과입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The result selected automatically when the countdown expires.</para>
    /// \endif
    /// </param>
    /// <param name="autoClickDelaySeconds">
    /// \if KO
    /// <para>자동 선택 전 대기할 초입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The number of seconds before automatic selection.</para>
    /// \endif
    /// </param>
    /// <param name="enableDelaySeconds">
    /// \if KO
    /// <para>버튼을 활성화하기 전 대기할 초입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The number of seconds before enabling the buttons.</para>
    /// \endif
    /// </param>
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
