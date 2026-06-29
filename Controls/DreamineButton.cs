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

    /// <summary>
    /// 모서리를 채울 때 쓸 "실제로 보이는" 배경색을 찾는다. 바로 위 부모가
    /// Color.Transparent(예: 버튼을 담는 FlowLayoutPanel)처럼 알파값이 불완전하면
    /// 그 색은 그대로 채워도 의미가 없으므로(그냥 아무것도 안 그려진 효과),
    /// 불투명한 배경을 가진 조상을 찾을 때까지 위로 올라간다.
    /// </summary>
    private Color GetEffectiveParentBackColor()
    {
        for (Control? p = Parent; p != null; p = p.Parent)
        {
            if (p.BackColor.A == 255)
                return p.BackColor;
        }
        return FindForm()?.BackColor ?? DreamineTheme.AppBackground;
    }

    // ── Paint ─────────────────────────────────────────────
    protected override void OnPaint(PaintEventArgs e)
    {
        var g    = e.Graphics;
        var rect = new Rectangle(1, 1, Width - 2, Height - 2);

        // 둥근 사각형 바깥쪽(네 귀퉁이)을 부모의 실제 배경색으로 먼저 채운다.
        // Win32 Region으로 잘라내는 방식은 안티앨리어싱이 없어 작은 반경에서 오히려
        // 더 거칠게 보이므로, 대신 "부모 배경색 채우기 + AA 처리된 둥근 도형"으로
        // 귀퉁이가 부모 배경에 자연스럽게 섞여 보이도록 한다(부모가 단색 배경일 때 효과적).
        using (var cornerBrush = new SolidBrush(GetEffectiveParentBackColor()))
            g.FillRectangle(cornerBrush, 0, 0, Width, Height);

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
