using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Input;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// \if KO
/// <para>둥근 모서리, 광택, 선택 상태 및 명령 실행을 지원하는 Dreamine WinForms 버튼입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a Dreamine WinForms button with rounded corners, shine, selection state, and command execution.</para>
/// \endif
/// </summary>
public class DreamineButton : Control
{
    /// <summary>
    /// \if KO
    /// <para>is Hover 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the is hover value.</para>
    /// \endif
    /// </summary>
    private bool _isHover;
    /// <summary>
    /// \if KO
    /// <para>is Pressed 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the is pressed value.</para>
    /// \endif
    /// </summary>
    private bool _isPressed;

    // ── Properties ────────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>content 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the content value.</para>
    /// \endif
    /// </summary>
    private string _content = string.Empty;
    /// <summary>
    /// \if KO
    /// <para>버튼에 표시할 텍스트 콘텐츠를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the text content displayed by the button.</para>
    /// \endif
    /// </summary>
    public string Content
    {
        get => _content;
        set { _content = value; Invalidate(); }
    }

    /// <summary>
    /// \if KO
    /// <para>shine Color 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the shine color value.</para>
    /// \endif
    /// </summary>
    private Color _shineColor = Color.Empty;
    /// <summary>
    /// \if KO
    /// <para>상단 광택 오버레이 색상을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the upper shine-overlay color.</para>
    /// \endif
    /// </summary>
    public Color ShineColor
    {
        get => _shineColor;
        set { _shineColor = value; Invalidate(); }
    }

    /// <summary>
    /// \if KO
    /// <para>border Color 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the border color value.</para>
    /// \endif
    /// </summary>
    private Color _borderColor = DreamineTheme.BorderNormal;
    /// <summary>
    /// \if KO
    /// <para>기본 테두리 색상을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the normal border color.</para>
    /// \endif
    /// </summary>
    public Color BorderColor
    {
        get => _borderColor;
        set { _borderColor = value; Invalidate(); }
    }

    /// <summary>
    /// \if KO
    /// <para>corner Radius 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the corner radius value.</para>
    /// \endif
    /// </summary>
    private int _cornerRadius = DreamineTheme.CornerRadius;
    /// <summary>
    /// \if KO
    /// <para>버튼 모서리 반지름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the button corner radius.</para>
    /// \endif
    /// </summary>
    public int CornerRadius
    {
        get => _cornerRadius;
        set { _cornerRadius = value; Invalidate(); }
    }

    /// <summary>
    /// \if KO
    /// <para>is Selected 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the is selected value.</para>
    /// \endif
    /// </summary>
    private bool _isSelected;
    /// <summary>
    /// \if KO
    /// <para>버튼이 선택 상태로 강조되는지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the button is highlighted as selected.</para>
    /// \endif
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; Invalidate(); }
    }

    /// <summary>
    /// \if KO
    /// <para>유효한 클릭 후 실행할 명령을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the command executed after a valid click.</para>
    /// \endif
    /// </summary>
    public ICommand? Command { get; set; }
    /// <summary>
    /// \if KO
    /// <para>명령의 실행 가능 여부 확인과 실행에 전달할 매개변수를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the parameter passed to command executability checks and execution.</para>
    /// \endif
    /// </summary>
    public object? CommandParameter { get; set; }

    // ── Constructor ───────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>사용자 지정 그리기 스타일과 Dreamine 테마 기본값을 구성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures custom-painting styles and Dreamine theme defaults.</para>
    /// \endif
    /// </summary>
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
    /// <summary>
    /// \if KO
    /// <para>포인터 진입 상태를 기록하고 다시 그리도록 요청합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Records pointer-entry state and requests a redraw.</para>
    /// \endif
    /// </summary>
    /// <param name="e">
    /// \if KO
    /// <para>마우스 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The mouse event arguments.</para>
    /// \endif
    /// </param>
    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _isHover = true;
        Invalidate();
    }
    /// <summary>
    /// \if KO
    /// <para>포인터와 누름 상태를 해제하고 다시 그리도록 요청합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Clears pointer and pressed state and requests a redraw.</para>
    /// \endif
    /// </summary>
    /// <param name="e">
    /// \if KO
    /// <para>마우스 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The mouse event arguments.</para>
    /// \endif
    /// </param>
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHover = false;
        _isPressed = false;
        Invalidate();
    }
    /// <summary>
    /// \if KO
    /// <para>왼쪽 버튼 누름 상태를 기록합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Records a left-button pressed state.</para>
    /// \endif
    /// </summary>
    /// <param name="e">
    /// \if KO
    /// <para>마우스 버튼 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The mouse-button event arguments.</para>
    /// \endif
    /// </param>
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left) { _isPressed = true; Invalidate(); }
    }
    /// <summary>
    /// \if KO
    /// <para>왼쪽 버튼을 컨트롤 안에서 놓으면 연결된 명령을 실행합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Executes the associated command when the left button is released inside the control.</para>
    /// \endif
    /// </summary>
    /// <param name="e">
    /// \if KO
    /// <para>마우스 버튼 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The mouse-button event arguments.</para>
    /// \endif
    /// </param>
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

    /// <summary>
    /// \if KO
    /// <para>명령이 현재 매개변수로 실행 가능하면 명령을 호출합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Invokes the command when it can execute with the current parameter.</para>
    /// \endif
    /// </summary>
    private void ExecuteCommand()
    {
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }

    /// <summary>
    /// \if KO
    /// <para>둥근 모서리 밖을 채울 첫 번째 불투명 조상 배경색을 찾습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Finds the first opaque ancestor background color used to fill outside rounded corners.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>불투명 조상 또는 폼의 배경색이며 찾을 수 없으면 애플리케이션 기본 배경색입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The opaque ancestor or form background, or the application default background when none is found.</para>
    /// \endif
    /// </returns>
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
    /// <summary>
    /// \if KO
    /// <para>현재 호버, 누름 및 선택 상태에 맞게 버튼 배경, 테두리, 광택과 텍스트를 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws the button background, border, shine, and text for the current hover, pressed, and selected state.</para>
    /// \endif
    /// </summary>
    /// <param name="e">
    /// \if KO
    /// <para>컨트롤 그리기 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The control paint event arguments.</para>
    /// \endif
    /// </param>
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
