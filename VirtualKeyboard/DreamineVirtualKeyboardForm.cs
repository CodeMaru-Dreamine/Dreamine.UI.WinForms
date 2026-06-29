using System.Drawing;
using System.Windows.Forms;
using Dreamine.UI.WinForms.Controls;

namespace Dreamine.UI.WinForms.VirtualKeyboard;

/// <summary>
/// DreamineVirtualKeyboardAssist가 띄우는 화면 키보드 팝업.
/// 영문 QWERTY + 숫자 레이아웃을 지원한다(한글 조합 입력은 지원하지 않는다).
/// </summary>
internal sealed class DreamineVirtualKeyboardForm : Form
{
    private readonly DreamineTextBox _target;
    private bool _shift;
    private readonly DreamineButton _shiftButton;

    public DreamineVirtualKeyboardForm(DreamineTextBox target, VkLayout layout)
    {
        _target = target;

        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        BackColor = DreamineTheme.CardBackground;
        Padding = new Padding(6);

        var grid = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };

        _shiftButton = MakeKey("Shift", 64, (_, _) =>
        {
            _shift = !_shift;
            _shiftButton.IsSelected = _shift;
            RefreshLetterCase(grid);
        });

        if (layout == VkLayout.Numeric)
        {
            foreach (var digit in "1234567890")
                grid.Controls.Add(MakeKey(digit.ToString(), 44, (_, _) => Insert(digit.ToString())));
            grid.Controls.Add(MakeKey(".", 44, (_, _) => Insert(".")));
            grid.Controls.Add(MakeKey("←", 44, (_, _) => Backspace()));
        }
        else
        {
            foreach (var row in new[] { "1234567890", "qwertyuiop", "asdfghjkl", "zxcvbnm" })
            {
                foreach (var ch in row)
                {
                    var c = ch;
                    grid.Controls.Add(MakeKey(c.ToString(), 40, (_, _) => Insert(_shift ? c.ToString().ToUpperInvariant() : c.ToString())));
                }
                if (row == "zxcvbnm")
                {
                    grid.Controls.Add(_shiftButton);
                    grid.Controls.Add(MakeKey("←", 50, (_, _) => Backspace()));
                }
            }
            grid.Controls.Add(MakeKey("Space", 200, (_, _) => Insert(" ")));
        }

        grid.Controls.Add(MakeKey("Enter", 70, (_, _) => Close()));
        grid.Controls.Add(MakeKey("✕", 44, (_, _) => Close()));

        Controls.Add(grid);

        int cols = layout == VkLayout.Numeric ? 6 : 11;
        Width = cols * 46 + 24;
        Height = layout == VkLayout.Numeric ? 110 : 220;
    }

    private DreamineButton MakeKey(string text, int width, EventHandler onClick)
    {
        var button = new DreamineButton
        {
            Content = text,
            Width = width,
            Height = 36,
            CornerRadius = 4,
            Margin = new Padding(2)
        };
        button.MouseUp += (s, e) =>
        {
            if (e.Button == MouseButtons.Left && button.ClientRectangle.Contains(e.Location))
                onClick(s, e);
        };
        return button;
    }

    private void RefreshLetterCase(Control container)
    {
        // 단순 데모 목적이므로 버튼 라벨은 그대로 두고, 입력값만 Insert 시점에 대소문자를 반영한다.
    }

    private void Insert(string text)
    {
        if (_target.IsDisposed) { Close(); return; }
        _target.Text += text;
    }

    private void Backspace()
    {
        if (_target.IsDisposed) { Close(); return; }
        if (_target.Text.Length > 0)
            _target.Text = _target.Text[..^1];
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var pen = new Pen(DreamineTheme.BorderNormal, 1f);
        e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
    }

    protected override bool ShowWithoutActivation => true;
}
