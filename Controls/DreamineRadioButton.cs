using System.Drawing;
using System.Drawing.Drawing2D;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// \if KO
/// <para>그룹 이름 기반 상호 배타 선택과 사용자 지정 그리기를 제공하는 WinForms 라디오 버튼입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a WinForms radio button with group-name-based mutual exclusion and custom drawing.</para>
/// \endif
/// </summary>
public class DreamineRadioButton : Control
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
    /// <para>라디오 버튼이 선택되어 있는지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the radio button is selected.</para>
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
            if (value) UncheckSiblings();
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
    /// <para>선택 원 옆에 그릴 텍스트를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the text drawn next to the selection circle.</para>
    /// \endif
    /// </summary>
    public string Content
    {
        get => _content;
        set { _content = value; Invalidate(); }
    }

    /// <summary>
    /// \if KO
    /// <para>상호 배타 선택에 사용할 그룹 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the group name used for mutual exclusion.</para>
    /// \endif
    /// </summary>
    public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// \if KO
    /// <para>선택 상태가 실제로 변경될 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Occurs when the selected state actually changes.</para>
    /// \endif
    /// </summary>
    public event EventHandler? CheckedChanged;

    /// <summary>
    /// \if KO
    /// <para>Bullet Size 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the bullet size value.</para>
    /// \endif
    /// </summary>
    private const int BulletSize = 16;
    /// <summary>
    /// \if KO
    /// <para>Bullet Margin 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the bullet margin value.</para>
    /// \endif
    /// </summary>
    private const int BulletMargin = 2;

    /// <summary>
    /// \if KO
    /// <para>사용자 지정 그리기 스타일과 Dreamine 테마 기본값을 구성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures custom-painting styles and Dreamine theme defaults.</para>
    /// \endif
    /// </summary>
    public DreamineRadioButton()
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
    /// <para>활성 컨트롤 안에서 왼쪽 버튼을 놓으면 이 항목을 선택합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Selects this item when the left button is released inside an enabled control.</para>
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
            IsChecked = true;
    }

    /// <summary>
    /// \if KO
    /// <para>같은 부모와 그룹 이름을 가진 다른 선택 항목을 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Clears other selected items that share the same parent and group name.</para>
    /// \endif
    /// </summary>
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

    /// <summary>
    /// \if KO
    /// <para>현재 선택, 호버 및 활성 상태에 맞게 원, 점과 라벨을 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws the circle, dot, and label for the current selected, hover, and enabled state.</para>
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

    /// <summary>
    /// \if KO
    /// <para>라디오 버튼의 기본 크기를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the default size of the radio button.</para>
    /// \endif
    /// </summary>
    protected override Size DefaultSize => new(160, 24);
}
