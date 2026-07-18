using System.Windows.Forms;
using Dreamine.UI.WinForms.Controls;

namespace Dreamine.UI.WinForms.VirtualKeyboard;

/// <summary>
/// \if KO
/// <para><see cref="DreamineTextBox"/> 포커스에 화면 키보드의 표시와 수명 주기를 연결합니다.</para>
/// \endif
/// \if EN
/// <para>Associates on-screen keyboard display and lifetime with focus on a <see cref="DreamineTextBox"/>.</para>
/// \endif
/// </summary>
public static class DreamineVirtualKeyboardAssist
{
    /// <summary>
    /// \if KO
    /// <para>open 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the open value.</para>
    /// \endif
    /// </summary>
    private static readonly Dictionary<DreamineTextBox, DreamineVirtualKeyboardForm> _open = new();

    /// <summary>
    /// \if KO
    /// <para>지정한 텍스트 상자가 포커스를 받을 때 선택한 레이아웃의 화면 키보드를 표시하도록 연결합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Attaches display of an on-screen keyboard with the selected layout when the specified text box receives focus.</para>
    /// \endif
    /// </summary>
    /// <param name="textBox">
    /// \if KO
    /// <para>화면 키보드를 연결할 Dreamine 텍스트 상자입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The Dreamine text box to attach the on-screen keyboard to.</para>
    /// \endif
    /// </param>
    /// <param name="layout">
    /// \if KO
    /// <para>표시할 키보드 레이아웃입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The keyboard layout to display.</para>
    /// \endif
    /// </param>
    /// <exception cref="NullReferenceException">
    /// \if KO
    /// <para><paramref name="textBox"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="textBox"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    public static void Attach(DreamineTextBox textBox, VkLayout layout = VkLayout.Text)
    {
        textBox.Enter += (_, _) => Show(textBox, layout);
    }

    /// <summary>
    /// \if KO
    /// <para>열려 있는 화면 키보드를 닫고 지정한 텍스트 상자의 연결 상태를 제거합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Closes any open on-screen keyboard and removes tracking for the specified text box.</para>
    /// \endif
    /// </summary>
    /// <param name="textBox">
    /// \if KO
    /// <para>화면 키보드 연결을 해제할 텍스트 상자입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The text box whose on-screen keyboard is detached.</para>
    /// \endif
    /// </param>
    public static void Detach(DreamineTextBox textBox)
    {
        if (_open.TryGetValue(textBox, out var form))
        {
            form.Close();
            _open.Remove(textBox);
        }
    }

    /// <summary>
    /// \if KO
    /// <para>텍스트 상자 주변의 화면 작업 영역 안에 키보드 폼을 배치하여 표시합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Positions and displays the keyboard form within the screen work area near the text box.</para>
    /// \endif
    /// </summary>
    /// <param name="textBox">
    /// \if KO
    /// <para>입력 대상이자 배치 기준인 텍스트 상자입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The text box that is both the input target and positioning anchor.</para>
    /// \endif
    /// </param>
    /// <param name="layout">
    /// \if KO
    /// <para>키보드 폼에 사용할 레이아웃입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The layout used by the keyboard form.</para>
    /// \endif
    /// </param>
    private static void Show(DreamineTextBox textBox, VkLayout layout)
    {
        if (_open.ContainsKey(textBox))
            return;

        var screenLocation = textBox.Parent?.PointToScreen(textBox.Location) ?? Cursor.Position;
        var form = new DreamineVirtualKeyboardForm(textBox, layout);
        var workingArea = Screen.FromControl(textBox).WorkingArea;
        var x = Math.Clamp(screenLocation.X, workingArea.Left, Math.Max(workingArea.Left, workingArea.Right - form.Width));
        var y = screenLocation.Y + textBox.Height + 4;
        if (y + form.Height > workingArea.Bottom)
            y = Math.Max(workingArea.Top, screenLocation.Y - form.Height - 4);

        form.Location = new System.Drawing.Point(x, y);
        form.FormClosed += (_, _) => _open.Remove(textBox);

        _open[textBox] = form;
        form.Show();
    }
}
