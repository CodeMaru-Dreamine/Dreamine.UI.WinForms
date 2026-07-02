using System.Drawing;
using System.Runtime.InteropServices;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// Dreamine 커스텀 TextBox. WPF DreamineTextBox와 동일한 API:
/// Text, Hint, IsReadOnly. 다크 테마 + 포커스 테두리.
/// </summary>
public class DreamineTextBox : UserControl
{
    private const int EM_SETCUEBANNER = 0x1501;
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

    private readonly TextBox _inner;
    private bool _isFocused;

    // ── Properties ────────────────────────────────────────
    public new string Text
    {
        get => _inner.Text;
        set => _inner.Text = value;
    }

    private string _hint = string.Empty;
    public string Hint
    {
        get => _hint;
        set
        {
            _hint = value;
            if (_inner.IsHandleCreated)
                SendMessage(_inner.Handle, EM_SETCUEBANNER, (IntPtr)1, _hint);
        }
    }

    public bool IsReadOnly
    {
        get => _inner.ReadOnly;
        set { _inner.ReadOnly = value; Invalidate(); }
    }

    public int SelectionStart
    {
        get => _inner.SelectionStart;
        set => _inner.SelectionStart = Math.Clamp(value, 0, _inner.TextLength);
    }

    public int SelectionLength
    {
        get => _inner.SelectionLength;
        set => _inner.SelectionLength = Math.Clamp(value, 0, _inner.TextLength - _inner.SelectionStart);
    }

    public IntPtr TextBoxHandle => _inner.Handle;

    public override Color ForeColor
    {
        get => base.ForeColor;
        set { base.ForeColor = value; if (_inner != null) _inner.ForeColor = value; }
    }

    public new Font Font
    {
        get => base.Font;
        set { base.Font = value; if (_inner != null) _inner.Font = value; }
    }

    public new event EventHandler? TextChanged;

    public void InsertText(string text)
    {
        if (_inner.ReadOnly)
            return;

        _inner.SelectedText = text;
        _inner.Focus();
    }

    public void ReplaceTextTail(int replaceCount, string text)
    {
        if (_inner.ReadOnly)
            return;

        var currentText = _inner.Text ?? string.Empty;
        var selectionStart = Math.Clamp(_inner.SelectionStart, 0, currentText.Length);
        var selectionLength = Math.Clamp(_inner.SelectionLength, 0, currentText.Length - selectionStart);

        if (selectionLength > 0)
        {
            _inner.SelectedText = text;
        }
        else
        {
            var removeStart = Math.Max(0, selectionStart - replaceCount);
            removeStart = Math.Min(removeStart, currentText.Length);
            var removeLength = Math.Clamp(selectionStart - removeStart, 0, currentText.Length - removeStart);
            _inner.Text = currentText.Remove(removeStart, removeLength).Insert(removeStart, text);
            _inner.SelectionStart = removeStart + text.Length;
        }

        _inner.Focus();
    }

    public void Backspace()
    {
        if (_inner.ReadOnly)
            return;

        var currentText = _inner.Text ?? string.Empty;
        var selectionStart = Math.Clamp(_inner.SelectionStart, 0, currentText.Length);
        var selectionLength = Math.Clamp(_inner.SelectionLength, 0, currentText.Length - selectionStart);

        if (selectionLength > 0)
        {
            _inner.SelectedText = string.Empty;
        }
        else if (selectionStart > 0)
        {
            _inner.Text = currentText.Remove(selectionStart - 1, 1);
            _inner.SelectionStart = selectionStart - 1;
        }

        _inner.Focus();
    }

    public string GetTextBeforeCaret()
    {
        var currentText = _inner.Text ?? string.Empty;
        var caret = Math.Clamp(_inner.SelectionStart, 0, currentText.Length);
        return currentText[..caret];
    }

    // ── Constructor ───────────────────────────────────────
    public DreamineTextBox()
    {
        // _inner must be created first — SetStyle and property setters below
        // can trigger OnLayout / ForeColor / Font overrides before the field is set.
        _inner = new TextBox
        {
            BorderStyle = BorderStyle.None,
            BackColor   = DreamineTheme.InputBackground,
            ForeColor   = DreamineTheme.TextPrimary,
            Font        = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point),
            Dock        = DockStyle.Fill,
            Margin      = Padding.Empty,
        };

        _inner.GotFocus  += (_, _) => { _isFocused = true;  Invalidate(); };
        _inner.LostFocus += (_, _) => { _isFocused = false; Invalidate(); };
        _inner.TextChanged += (s, e) => TextChanged?.Invoke(this, e);
        _inner.HandleCreated += (_, _) =>
        {
            if (!string.IsNullOrEmpty(_hint))
                SendMessage(_inner.Handle, EM_SETCUEBANNER, (IntPtr)1, _hint);
        };

        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer |
            ControlStyles.ResizeRedraw, true);

        BackColor = DreamineTheme.InputBackground;
        ForeColor = DreamineTheme.TextPrimary;
        Font      = _inner.Font;
        Height    = 36;
        Padding   = new Padding(2);

        Controls.Add(_inner);
    }

    protected override Padding DefaultPadding => new Padding(6, 0, 6, 0);

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        if (_inner == null) return;
        _inner.SetBounds(6, (Height - _inner.PreferredHeight) / 2,
            Width - 12, _inner.PreferredHeight);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g    = e.Graphics;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);

        using var bgBrush = new SolidBrush(BackColor);
        var borderColor = _isFocused ? DreamineTheme.BorderFocus : DreamineTheme.BorderNormal;
        if (!Enabled) borderColor = Color.FromArgb(80, borderColor);
        using var pen = new Pen(borderColor, 1.5f);
        DreamineDrawHelper.FillRoundedRect(g, bgBrush, pen, rect, DreamineTheme.CornerRadiusSmall);
    }

    protected override Size DefaultSize => new(220, 36);
}
