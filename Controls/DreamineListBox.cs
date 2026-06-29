using System.Drawing;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// Dreamine 커스텀 ListBox. 다크 테마 + 포커스 테두리.
/// 내부적으로 표준 <see cref="ListBox"/>를 감싸며, WPF DreamineListBox와 유사한 API를 제공한다.
/// </summary>
public class DreamineListBox : UserControl
{
    private readonly ListBox _inner;
    private bool _isFocused;

    public ListBox.ObjectCollection Items => _inner.Items;

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

    public object? DataSource
    {
        get => _inner.DataSource;
        set => _inner.DataSource = value;
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
    public new event MouseEventHandler? DoubleClick
    {
        add => _inner.MouseDoubleClick += value;
        remove => _inner.MouseDoubleClick -= value;
    }

    /// <summary>새 항목이 추가될 때마다 자동으로 맨 아래로 스크롤할지 여부
    /// (WPF AutoScrollListBoxBehavior.IsEnabled와 동일한 데모 목적).</summary>
    public bool AutoScrollToEnd { get; set; }

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

    /// <summary>항목 추가 후, AutoScrollToEnd가 켜져 있으면 맨 아래로 스크롤한다.</summary>
    public void NotifyItemAdded()
    {
        if (AutoScrollToEnd && _inner.Items.Count > 0)
            _inner.TopIndex = _inner.Items.Count - 1;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var bgBrush = new SolidBrush(BackColor);
        var borderColor = _isFocused ? DreamineTheme.BorderFocus : DreamineTheme.BorderNormal;
        using var pen = new Pen(borderColor, 1.5f);
        DreamineDrawHelper.FillRoundedRect(g, bgBrush, pen, rect, DreamineTheme.CornerRadiusSmall);
    }

    protected override Size DefaultSize => new(220, 100);
}
