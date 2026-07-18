using System.Drawing;
using System.Drawing.Drawing2D;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// \if KO
/// <para>다크 테마 드롭다운 그리기와 포커스 테두리를 제공하는 WinForms 콤보 상자 래퍼입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a WinForms combo-box wrapper with dark-theme drop-down drawing and a focus border.</para>
/// \endif
/// </summary>
public class DreamineComboBox : UserControl
{
    /// <summary>
    /// \if KO
    /// <para>inner 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the inner value.</para>
    /// \endif
    /// </summary>
    private readonly ComboBox _inner;
    /// <summary>
    /// \if KO
    /// <para>is Focused 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the is focused value.</para>
    /// \endif
    /// </summary>
    private bool _isFocused;

    // ── Properties ────────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>내부 콤보 상자의 항목 컬렉션을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the item collection of the inner combo box.</para>
    /// \endif
    /// </summary>
    public ComboBox.ObjectCollection Items => _inner.Items;

    /// <summary>
    /// \if KO
    /// <para>현재 선택한 항목을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the currently selected item.</para>
    /// \endif
    /// </summary>
    public object? SelectedItem
    {
        get => _inner.SelectedItem;
        set => _inner.SelectedItem = value;
    }

    /// <summary>
    /// \if KO
    /// <para>현재 선택한 항목의 인덱스를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the index of the currently selected item.</para>
    /// \endif
    /// </summary>
    public int SelectedIndex
    {
        get => _inner.SelectedIndex;
        set => _inner.SelectedIndex = value;
    }

    /// <summary>
    /// \if KO
    /// <para>래퍼와 내부 콤보 상자의 전경색을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the foreground color of both the wrapper and inner combo box.</para>
    /// \endif
    /// </summary>
    public override Color ForeColor
    {
        get => base.ForeColor;
        set { base.ForeColor = value; _inner.ForeColor = value; }
    }

    /// <summary>
    /// \if KO
    /// <para>래퍼와 내부 콤보 상자의 글꼴을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the font of both the wrapper and inner combo box.</para>
    /// \endif
    /// </summary>
    public new Font Font
    {
        get => base.Font;
        set { base.Font = value; _inner.Font = value; }
    }

    /// <summary>
    /// \if KO
    /// <para>내부 콤보 상자의 선택 인덱스가 변경될 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Occurs when the selected index of the inner combo box changes.</para>
    /// \endif
    /// </summary>
    public event EventHandler? SelectedIndexChanged;

    // ── Constructor ───────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>내부 콤보 상자, 사용자 지정 그리기 및 Dreamine 테마 기본값을 구성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures the inner combo box, owner drawing, and Dreamine theme defaults.</para>
    /// \endif
    /// </summary>
    public DreamineComboBox()
    {
        // _inner must be created first — SetStyle / property setters trigger
        // OnLayout and ForeColor/Font overrides before the field is otherwise set.
        _inner = new ComboBox
        {
            FlatStyle     = FlatStyle.Flat,
            BackColor     = DreamineTheme.InputBackground,
            ForeColor     = DreamineTheme.TextPrimary,
            Font          = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point),
            DropDownStyle = ComboBoxStyle.DropDownList,
        };

        _inner.GotFocus  += (_, _) => { _isFocused = true;  Invalidate(); };
        _inner.LostFocus += (_, _) => { _isFocused = false; Invalidate(); };
        _inner.SelectedIndexChanged += (s, e) => SelectedIndexChanged?.Invoke(this, e);
        _inner.DrawMode  = DrawMode.OwnerDrawFixed;
        _inner.DrawItem  += OnDrawItem;

        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer |
            ControlStyles.ResizeRedraw, true);

        BackColor = DreamineTheme.InputBackground;
        ForeColor = DreamineTheme.TextPrimary;
        Font      = _inner.Font;
        Height    = 36;

        Controls.Add(_inner);
    }

    /// <summary>
    /// \if KO
    /// <para>내부 콤보 상자를 래퍼의 현재 크기에 맞춰 배치합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Positions the inner combo box to fit the wrapper's current size.</para>
    /// \endif
    /// </summary>
    /// <param name="e">
    /// \if KO
    /// <para>레이아웃 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The layout event arguments.</para>
    /// \endif
    /// </param>
    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        if (_inner == null) return;
        _inner.SetBounds(0, (Height - _inner.PreferredHeight) / 2,
            Width, _inner.PreferredHeight);
    }

    /// <summary>
    /// \if KO
    /// <para>드롭다운 항목을 선택 및 호버 상태에 맞는 다크 테마로 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws a drop-down item using dark-theme colors for its selected and hover state.</para>
    /// \endif
    /// </summary>
    /// <param name="sender">
    /// \if KO
    /// <para>이벤트를 발생시킨 객체입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The object that raised the event.</para>
    /// \endif
    /// </param>
    /// <param name="e">
    /// \if KO
    /// <para>항목 그리기 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The item-drawing event arguments.</para>
    /// \endif
    /// </param>
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

    /// <summary>
    /// \if KO
    /// <para>현재 포커스 상태에 맞게 둥근 배경과 테두리를 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws the rounded background and border for the current focus state.</para>
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
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var bgBrush = new SolidBrush(BackColor);
        var borderColor = _isFocused ? DreamineTheme.BorderFocus : DreamineTheme.BorderNormal;
        using var pen = new Pen(borderColor, 1.5f);
        DreamineDrawHelper.FillRoundedRect(g, bgBrush, pen, rect, DreamineTheme.CornerRadiusSmall);
    }

    /// <summary>
    /// \if KO
    /// <para>콤보 상자 래퍼의 기본 크기를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the default size of the combo-box wrapper.</para>
    /// \endif
    /// </summary>
    protected override Size DefaultSize => new(200, 36);
}
