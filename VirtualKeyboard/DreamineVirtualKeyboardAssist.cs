using System.Windows.Forms;
using Dreamine.UI.WinForms.Controls;

namespace Dreamine.UI.WinForms.VirtualKeyboard;

/// <summary>
/// WPF의 DreamineVirtualKeyboardAssist.UseVirtualKeyBoard 첨부 속성과 동일한 역할을 하는
/// WinForms용 헬퍼. <see cref="DreamineTextBox"/>가 포커스를 받으면 화면 키보드를 띄운다.
/// </summary>
public static class DreamineVirtualKeyboardAssist
{
    private static readonly Dictionary<DreamineTextBox, DreamineVirtualKeyboardForm> _open = new();

    /// <summary>지정한 텍스트박스에 화면 키보드를 연결한다.</summary>
    public static void Attach(DreamineTextBox textBox, VkLayout layout = VkLayout.Text)
    {
        textBox.Enter += (_, _) => Show(textBox, layout);
    }

    /// <summary>화면 키보드를 연결 해제한다.</summary>
    public static void Detach(DreamineTextBox textBox)
    {
        if (_open.TryGetValue(textBox, out var form))
        {
            form.Close();
            _open.Remove(textBox);
        }
    }

    private static void Show(DreamineTextBox textBox, VkLayout layout)
    {
        if (_open.ContainsKey(textBox))
            return;

        var screenLocation = textBox.Parent?.PointToScreen(textBox.Location) ?? Cursor.Position;
        var form = new DreamineVirtualKeyboardForm(textBox, layout);
        form.Location = new System.Drawing.Point(screenLocation.X, screenLocation.Y + textBox.Height + 4);
        form.FormClosed += (_, _) => _open.Remove(textBox);

        _open[textBox] = form;
        form.Show();
    }
}
