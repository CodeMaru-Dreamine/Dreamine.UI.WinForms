using System.Drawing;
using System.Drawing.Drawing2D;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// \if KO
/// <para>직접 그린 체크 표시, 텍스트 콘텐츠 및 상태 변경 이벤트를 제공하는 WinForms 체크 상자입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a WinForms check box with a custom-drawn mark, text content, and state-change event.</para>
/// \endif
/// </summary>
public class DreamineCheckBox : Control
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
    /// <para>is Checked 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the is checked value.</para>
    /// \endif
    /// </summary>
    private bool _isChecked;

    /// <summary>
    /// \if KO
    /// <para>체크 상태를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the checked state.</para>
    /// \endif
    /// </summary>
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
    /// <para>체크 표시 옆에 그릴 텍스트를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the text drawn next to the check mark.</para>
    /// \endif
    /// </summary>
    public string Content
    {
        get => _content;
        set { _content = value; Invalidate(); }
    }

    /// <summary>
    /// \if KO
    /// <para>체크 상태가 실제로 변경될 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Occurs when the checked state actually changes.</para>
    /// \endif
    /// </summary>
    public event EventHandler? CheckedChanged;

    /// <summary>
    /// \if KO
    /// <para>Box Size 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the box size value.</para>
    /// \endif
    /// </summary>
    private const int BoxSize = 16;
    /// <summary>
    /// \if KO
    /// <para>Box Margin 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the box margin value.</para>
    /// \endif
    /// </summary>
    private const int BoxMargin = 2;

    /// <summary>
    /// \if KO
    /// <para>사용자 지정 그리기 스타일과 Dreamine 테마 기본값을 구성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures custom-painting styles and Dreamine theme defaults.</para>
    /// \endif
    /// </summary>
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
    protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _isHover = true;  Invalidate(); }
    /// <summary>
    /// \if KO
    /// <para>포인터 진입 상태를 해제하고 다시 그리도록 요청합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Clears pointer-entry state and requests a redraw.</para>
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
    protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _isHover = false; Invalidate(); }

    /// <summary>
    /// \if KO
    /// <para>활성 컨트롤 안에서 왼쪽 버튼을 놓으면 체크 상태를 전환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Toggles the checked state when the left button is released inside an enabled control.</para>
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
        if (Enabled && e.Button == MouseButtons.Left && ClientRectangle.Contains(e.Location))
            IsChecked = !IsChecked;
    }

    /// <summary>
    /// \if KO
    /// <para>현재 체크, 호버 및 활성 상태에 맞게 상자, 체크 표시와 라벨을 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws the box, check mark, and label for the current checked, hover, and enabled state.</para>
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

    /// <summary>
    /// \if KO
    /// <para>체크 상자의 기본 크기를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the default size of the check box.</para>
    /// \endif
    /// </summary>
    protected override Size DefaultSize => new(160, 24);
}
