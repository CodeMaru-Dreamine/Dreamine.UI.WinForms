using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Input;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// Dreamine 커스텀 버튼. WPF DreamineButton과 동일한 API:
/// Content, ShineColor, UseShadow, IsSelected, Command, CommandParameter.
/// </summary>
public class DreamineButton : Control
{
    private bool _isHover;
    private bool _isPressed;

    // ── Properties ────────────────────────────────────────
    private string _content = string.Empty;
    public string Content
    {
        get => _content;
        set { _content = value; Invalidate(); }
    }

    private Color _shineColor = Color.Empty;
    public Color ShineColor
    {
        get => _shineColor;
        set { _shineColor = value; Invalidate(); }
    }

    private Color _borderColor = DreamineTheme.BorderNormal;
    public Color BorderColor
    {
        get => _borderColor;
        set { _borderColor = value; Invalidate(); }
    }

    private int _cornerRadius = DreamineTheme.CornerRadius;
    public int CornerRadius
    {
        get => _cornerRadius;
        set { _cornerRadius = value; Invalidate(); }
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; Invalidate(); }
    }

    public ICommand? Command { get; set; }
    public object? CommandParameter { get; set; }

    // ── Constructor ───────────────────────────────────────
    public DreamineButton()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor, true);

        BackColor = DreamineTheme.NavBackground;
        ForeColor = DreamineTheme.TextPrimary;
        Font      = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point);
        Size      = new Size(100, 34);
        Cursor    = Cursors.Hand;
    }

    // ── Mouse Events ──────────────────────────────────────
    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _isHover = true;
        Invalidate();
    }
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHover = false;
        _isPressed = false;
        Invalidate();
    }
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left) { _isPressed = true; Invalidate(); }
    }
    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.Button == MouseButtons.Left)
        {
            _isPressed = false;
            Invalidate();
            if (ClientRectangle.Contains(e.Location))
                ExecuteCommand();
        }
    }

    private void ExecuteCommand()
    {
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }

    // ── Paint ─────────────────────────────────────────────
    protected override void OnPaint(PaintEventArgs e)
    {
        var g    = e.Graphics;
        var rect = new Rectangle(1, 1, Width - 2, Height - 2);

        // background color
        var bg = BackColor;
        if (_isPressed)
            bg = DreamineDrawHelper.Blend(bg, DreamineTheme.PressOverlay, 0.6f);
        else if (_isHover)
            bg = DreamineDrawHelper.Blend(bg, DreamineTheme.HoverOverlay, 0.4f);

        var topColor = _isSelected
            ? DreamineDrawHelper.Blend(bg, DreamineTheme.AccentBlue, 0.3f)
            : bg;

        // border
        var borderColor = _isSelected || _isHover
            ? DreamineTheme.BorderFocus
            : _borderColor;

        using var borderPen = new Pen(borderColor, 1f);
        DreamineDrawHelper.FillRoundedGradient(g, topColor, bg, borderPen, rect, _cornerRadius);

        // shine overlay
        if (_shineColor != Color.Empty)
            DreamineDrawHelper.DrawShineOverlay(g, _shineColor, rect, _cornerRadius);

        // selected indicator (bottom line)
        if (_isSelected)
        {
            using var selPen = new Pen(DreamineTheme.AccentBlue, 2f);
            int y = Height - 3;
            g.DrawLine(selPen, _cornerRadius, y, Width - _cornerRadius, y);
        }

        // text
        DreamineDrawHelper.DrawCenteredText(g, _content, Font, Enabled ? ForeColor : DreamineTheme.TextSecondary, rect);
    }
}
