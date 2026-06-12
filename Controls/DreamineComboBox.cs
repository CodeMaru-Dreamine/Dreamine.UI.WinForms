using System.Drawing;
using System.Drawing.Drawing2D;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// Dreamine 커스텀 ComboBox. WPF DreamineComboBox와 동일한 API:
/// Items, SelectedItem, SelectedIndex. 다크 테마 + 드롭다운 색상.
/// </summary>
public class DreamineComboBox : UserControl
{
    private readonly ComboBox _inner;
    private bool _isFocused;

    // ── Properties ────────────────────────────────────────
    public ComboBox.ObjectCollection Items => _inner.Items;

    public object? SelectedItem
    {
        get => _inner.SelectedItem;
        set => _inner.SelectedItem = value;
    }

    public int SelectedIndex
    {
        get => _inner.SelectedIndex;
        set => _inner.SelectedIndex = value;
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

    public event EventHandler? SelectedIndexChanged;

    // ── Constructor ───────────────────────────────────────
    public DreamineComboBox()
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

        _inner = new ComboBox
        {
            FlatStyle   = FlatStyle.Flat,
            BackColor   = DreamineTheme.InputBackground,
            ForeColor   = DreamineTheme.TextPrimary,
            Font        = Font,
            DropDownStyle = ComboBoxStyle.DropDownList,
        };

        _inner.GotFocus  += (_, _) => { _isFocused = true;  Invalidate(); };
        _inner.LostFocus += (_, _) => { _isFocused = false; Invalidate(); };
        _inner.SelectedIndexChanged += (s, e) => SelectedIndexChanged?.Invoke(this, e);
        _inner.DrawMode = DrawMode.OwnerDrawFixed;
        _inner.DrawItem += OnDrawItem;

        Controls.Add(_inner);
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        _inner.SetBounds(0, (Height - _inner.PreferredHeight) / 2,
            Width, _inner.PreferredHeight);
    }

    private void OnDrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0) return;
        var isSelected = (e.State & DrawItemState.Selected) != 0;
        var isHover    = (e.State & DrawItemState.HotLight) != 0;

        var bg = isSelected
            ? DreamineTheme.AccentBlue
            : isHover
                ? Color.FromArgb(0xFF, 0x1E, 0x4A, 0x80)
                : DreamineTheme.InputBackground;

        using var bgBrush = new SolidBrush(bg);
        e.Graphics.FillRectangle(bgBrush, e.Bounds);

        string? text = _inner.Items[e.Index]?.ToString();
        if (!string.IsNullOrEmpty(text))
        {
            using var textBrush = new SolidBrush(DreamineTheme.TextPrimary);
            var sf = new StringFormat { LineAlignment = StringAlignment.Center };
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.DrawString(text, e.Font!, textBrush,
                new Rectangle(e.Bounds.X + 6, e.Bounds.Y, e.Bounds.Width - 6, e.Bounds.Height), sf);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g    = e.Graphics;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var bgBrush = new SolidBrush(BackColor);
        var borderColor = _isFocused ? DreamineTheme.BorderFocus : DreamineTheme.BorderNormal;
        using var pen = new Pen(borderColor, 1.5f);
        DreamineDrawHelper.FillRoundedRect(g, bgBrush, pen, rect, DreamineTheme.CornerRadiusSmall);
    }

    protected override Size DefaultSize => new(200, 36);
}
