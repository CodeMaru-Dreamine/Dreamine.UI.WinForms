using System.Drawing;
using System.Drawing.Drawing2D;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// Dreamine 커스텀 CheckBox. WPF DreamineCheckBox와 동일한 API.
/// Content, IsChecked, CheckedChanged 이벤트.
/// </summary>
public class DreamineCheckBox : Control
{
    private bool _isHover;
    private bool _isChecked;

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (_isChecked == value) return;
            _isChecked = value;
            Invalidate();
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private string _content = string.Empty;
    public string Content
    {
        get => _content;
        set { _content = value; Invalidate(); }
    }

    public event EventHandler? CheckedChanged;

    private const int BoxSize = 16;
    private const int BoxMargin = 2;

    public DreamineCheckBox()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor, true);

        BackColor = Color.Transparent;
        ForeColor = DreamineTheme.TextPrimary;
        Font      = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point);
        Height    = 24;
        Cursor    = Cursors.Hand;
    }

    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _isHover = true;  Invalidate(); }
    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _isHover = false; Invalidate(); }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (Enabled && e.Button == MouseButtons.Left && ClientRectangle.Contains(e.Location))
            IsChecked = !IsChecked;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        int x = BoxMargin, y = (Height - BoxSize) / 2;
        var boxRect = new Rectangle(x, y, BoxSize, BoxSize);

        // Box background
        var bg = _isChecked
            ? DreamineTheme.AccentBlue
            : DreamineTheme.InputBackground;
        if (_isHover && !_isChecked)
            bg = DreamineDrawHelper.Blend(bg, Color.White, 0.1f);

        using var bgBrush  = new SolidBrush(bg);
        using var borderPen = new Pen(_isHover || _isChecked ? DreamineTheme.BorderFocus : DreamineTheme.BorderNormal, 1.5f);
        DreamineDrawHelper.FillRoundedRect(g, bgBrush, borderPen, boxRect, 3f);

        // Checkmark
        if (_isChecked)
        {
            using var ckPen = new Pen(Color.White, 2f) { LineJoin = LineJoin.Round };
            int cx = x + 3, cy = y + BoxSize / 2;
            g.DrawLines(ckPen, new[]
            {
                new Point(cx,     cy),
                new Point(cx + 3, cy + 3),
                new Point(cx + 8, cy - 3)
            });
        }

        // Label text
        using var brush = new SolidBrush(Enabled ? ForeColor : DreamineTheme.TextSecondary);
        int textX = BoxMargin + BoxSize + 6;
        var textRect = new Rectangle(textX, 0, Width - textX, Height);
        var sf = new StringFormat { LineAlignment = StringAlignment.Center };
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        g.DrawString(_content, Font, brush, textRect, sf);
    }

    protected override Size DefaultSize => new(160, 24);
}
