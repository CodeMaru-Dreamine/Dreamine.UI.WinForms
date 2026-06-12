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

    public override Color ForeColor
    {
        get => base.ForeColor;
        set { base.ForeColor = value; _inner.ForeColor = value; }
    }

    public new Font Font
    {
        get => base.Font;
        set { base.Font = value; _inner.Font = value; }
    }

    public new event EventHandler? TextChanged;

    // ── Constructor ───────────────────────────────────────
    public DreamineTextBox()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer |
            ControlStyles.ResizeRedraw, true);

        BackColor = DreamineTheme.InputBackground;
        ForeColor = DreamineTheme.TextPrimary;
        Font      = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point);
        Height    = 36;
        Padding   = new Padding(2);

        _inner = new TextBox
        {
            BorderStyle = BorderStyle.None,
            BackColor   = DreamineTheme.InputBackground,
            ForeColor   = DreamineTheme.TextPrimary,
            Font        = Font,
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

        Controls.Add(_inner);
    }

    protected override Padding DefaultPadding => new Padding(6, 0, 6, 0);

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
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
