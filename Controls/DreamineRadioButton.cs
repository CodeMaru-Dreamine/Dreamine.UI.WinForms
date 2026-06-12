using System.Drawing;
using System.Drawing.Drawing2D;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// Dreamine 커스텀 RadioButton. WPF DreamineRadioButton과 동일한 API.
/// GroupName 기반 상호 배타 선택 지원.
/// </summary>
public class DreamineRadioButton : Control
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
            if (value) UncheckSiblings();
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private string _content = string.Empty;
    public string Content
    {
        get => _content;
        set { _content = value; Invalidate(); }
    }

    public string GroupName { get; set; } = string.Empty;

    public event EventHandler? CheckedChanged;

    private const int BulletSize = 16;
    private const int BulletMargin = 2;

    public DreamineRadioButton()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer |
            ControlStyles.ResizeRedraw, true);

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
            IsChecked = true;
    }

    private void UncheckSiblings()
    {
        if (Parent == null || string.IsNullOrEmpty(GroupName)) return;
        foreach (Control c in Parent.Controls)
        {
            if (c != this && c is DreamineRadioButton rb &&
                rb.GroupName == GroupName && rb.IsChecked)
            {
                rb._isChecked = false;
                rb.Invalidate();
                rb.CheckedChanged?.Invoke(rb, EventArgs.Empty);
            }
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        int x = BulletMargin, y = (Height - BulletSize) / 2;
        var bulletRect = new Rectangle(x, y, BulletSize, BulletSize);

        // Outer circle
        var borderColor = _isChecked || _isHover ? DreamineTheme.BorderFocus : DreamineTheme.BorderNormal;
        using var bgBrush  = new SolidBrush(_isChecked ? DreamineTheme.AccentBlue : DreamineTheme.InputBackground);
        using var borderPen = new Pen(borderColor, 1.5f);
        g.FillEllipse(bgBrush, bulletRect);
        g.DrawEllipse(borderPen, new Rectangle(x, y, BulletSize - 1, BulletSize - 1));

        // Inner dot
        if (_isChecked)
        {
            int dotSize = 6, dotX = x + (BulletSize - dotSize) / 2, dotY = y + (BulletSize - dotSize) / 2;
            using var dotBrush = new SolidBrush(Color.White);
            g.FillEllipse(dotBrush, dotX, dotY, dotSize, dotSize);
        }

        // Label
        using var brush = new SolidBrush(Enabled ? ForeColor : DreamineTheme.TextSecondary);
        int textX = BulletMargin + BulletSize + 6;
        var sf = new StringFormat { LineAlignment = StringAlignment.Center };
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        g.DrawString(_content, Font, brush, new Rectangle(textX, 0, Width - textX, Height), sf);
    }

    protected override Size DefaultSize => new(160, 24);
}
