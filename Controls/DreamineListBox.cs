using System.Drawing;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// \if KO
/// <para>내부 <see cref="ListBox"/>에 다크 테마, 포커스 테두리 및 자동 스크롤을 추가한 WinForms 래퍼입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a WinForms wrapper that adds dark-theme styling, a focus border, and automatic scrolling to an inner <see cref="ListBox"/>.</para>
/// \endif
/// </summary>
public class DreamineListBox : UserControl
{
    /// <summary>
    /// \if KO
    /// <para>inner 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the inner value.</para>
    /// \endif
    /// </summary>
    private readonly ListBox _inner;
    /// <summary>
    /// \if KO
    /// <para>is Focused 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the is focused value.</para>
    /// \endif
    /// </summary>
    private bool _isFocused;

    /// <summary>
    /// \if KO
    /// <para>내부 목록의 항목 컬렉션을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the item collection of the inner list.</para>
    /// \endif
    /// </summary>
    public ListBox.ObjectCollection Items => _inner.Items;

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
    /// <para>내부 목록의 데이터 원본을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the data source of the inner list.</para>
    /// \endif
    /// </summary>
    public object? DataSource
    {
        get => _inner.DataSource;
        set => _inner.DataSource = value;
    }

    /// <summary>
    /// \if KO
    /// <para>래퍼와 내부 목록의 전경색을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the foreground color of both the wrapper and inner list.</para>
    /// \endif
    /// </summary>
    public override Color ForeColor
    {
        get => base.ForeColor;
        set { base.ForeColor = value; _inner.ForeColor = value; }
    }

    /// <summary>
    /// \if KO
    /// <para>래퍼와 내부 목록의 글꼴을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the font of both the wrapper and inner list.</para>
    /// \endif
    /// </summary>
    public new Font Font
    {
        get => base.Font;
        set { base.Font = value; _inner.Font = value; }
    }

    /// <summary>
    /// \if KO
    /// <para>내부 목록의 선택 인덱스가 변경될 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Occurs when the selected index of the inner list changes.</para>
    /// \endif
    /// </summary>
    public event EventHandler? SelectedIndexChanged;
    /// <summary>
    /// \if KO
    /// <para>내부 목록 항목을 마우스로 두 번 클릭할 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Occurs when an item in the inner list is double-clicked with the mouse.</para>
    /// \endif
    /// </summary>
    public new event MouseEventHandler? DoubleClick
    {
        add => _inner.MouseDoubleClick += value;
        remove => _inner.MouseDoubleClick -= value;
    }

    /// <summary>
    /// \if KO
    /// <para>새 항목 알림 또는 선택 변경 후 목록 끝으로 자동 스크롤할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the list automatically scrolls to the end after item notification or selection changes.</para>
    /// \endif
    /// </summary>
    public bool AutoScrollToEnd { get; set; }

    /// <summary>
    /// \if KO
    /// <para>내부 목록, 이벤트 전달 및 Dreamine 테마 기본값을 구성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures the inner list, event forwarding, and Dreamine theme defaults.</para>
    /// \endif
    /// </summary>
    public DreamineListBox()
    {
        _inner = new ListBox
        {
            BorderStyle = BorderStyle.None,
            // IntegralHeight=true(기본값)면 마지막 줄이 잘릴 때 빈 띠를 남겨버려서
            // Dock=Fill로 크기를 강제로 맞추는 우리 사용 패턴과 충돌한다.
            IntegralHeight = false,
            BackColor = DreamineTheme.InputBackground,
            ForeColor = DreamineTheme.TextPrimary,
            Font = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point),
            Dock = DockStyle.Fill,
        };

        _inner.GotFocus += (_, _) => { _isFocused = true; Invalidate(); };
        _inner.LostFocus += (_, _) => { _isFocused = false; Invalidate(); };
        _inner.SelectedIndexChanged += (s, e) =>
        {
            SelectedIndexChanged?.Invoke(this, e);
            if (AutoScrollToEnd && _inner.Items.Count > 0)
                _inner.TopIndex = _inner.Items.Count - 1;
        };

        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer |
            ControlStyles.ResizeRedraw, true);

        BackColor = DreamineTheme.InputBackground;
        ForeColor = DreamineTheme.TextPrimary;
        Font = _inner.Font;
        Padding = new Padding(2);

        Controls.Add(_inner);
    }

    /// <summary>
    /// \if KO
    /// <para>항목 추가 후 자동 스크롤이 활성화되어 있으면 목록 끝으로 이동합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Moves to the end of the list after an item is added when automatic scrolling is enabled.</para>
    /// \endif
    /// </summary>
    public void NotifyItemAdded()
    {
        if (AutoScrollToEnd && _inner.Items.Count > 0)
            _inner.TopIndex = _inner.Items.Count - 1;
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
        var g = e.Graphics;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var bgBrush = new SolidBrush(BackColor);
        var borderColor = _isFocused ? DreamineTheme.BorderFocus : DreamineTheme.BorderNormal;
        using var pen = new Pen(borderColor, 1.5f);
        DreamineDrawHelper.FillRoundedRect(g, bgBrush, pen, rect, DreamineTheme.CornerRadiusSmall);
    }

    /// <summary>
    /// \if KO
    /// <para>목록 래퍼의 기본 크기를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the default size of the list wrapper.</para>
    /// \endif
    /// </summary>
    protected override Size DefaultSize => new(220, 100);
}
