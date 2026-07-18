using System.Drawing;
using System.Drawing.Drawing2D;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// \if KO
/// <para>클릭 가능한 머리글과 접기/펼치기 콘텐츠 패널을 제공하는 WinForms 확장 컨트롤입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a WinForms expander with a clickable header and collapsible content panel.</para>
/// \endif
/// </summary>
public class DreamineExpander : UserControl
{
    /// <summary>
    /// \if KO
    /// <para>header Panel 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the header panel value.</para>
    /// \endif
    /// </summary>
    private readonly Panel   _headerPanel;
    /// <summary>
    /// \if KO
    /// <para>header Label 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the header label value.</para>
    /// \endif
    /// </summary>
    private readonly Label   _headerLabel;
    /// <summary>
    /// \if KO
    /// <para>arrow Label 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the arrow label value.</para>
    /// \endif
    /// </summary>
    private readonly Label   _arrowLabel;
    /// <summary>
    /// \if KO
    /// <para>content Panel 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the content panel value.</para>
    /// \endif
    /// </summary>
    private readonly Panel   _contentPanel;
    /// <summary>
    /// \if KO
    /// <para>expanded Height 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the expanded height value.</para>
    /// \endif
    /// </summary>
    private int _expandedHeight;

    /// <summary>
    /// \if KO
    /// <para>Header Height 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the header height value.</para>
    /// \endif
    /// </summary>
    private const int HeaderHeight = 36;

    // ── Properties ────────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>header 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the header value.</para>
    /// \endif
    /// </summary>
    private string _header = string.Empty;
    /// <summary>
    /// \if KO
    /// <para>머리글에 표시할 텍스트를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the text displayed in the header.</para>
    /// \endif
    /// </summary>
    public string Header
    {
        get => _header;
        set { _header = value; _headerLabel.Text = value; }
    }

    /// <summary>
    /// \if KO
    /// <para>is Expanded 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the is expanded value.</para>
    /// \endif
    /// </summary>
    private bool _isExpanded = true;
    /// <summary>
    /// \if KO
    /// <para>콘텐츠 패널이 펼쳐져 있는지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the content panel is expanded.</para>
    /// \endif
    /// </summary>
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value) return;
            _isExpanded = value;
            ApplyExpandState();
            ExpandedChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// \if KO
    /// <para>호출자가 자식 컨트롤을 추가할 내부 콘텐츠 패널을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the inner content panel to which callers add child controls.</para>
    /// \endif
    /// </summary>
    public Panel Content => _contentPanel;

    /// <summary>
    /// \if KO
    /// <para>확장 상태가 실제로 변경될 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Occurs when the expansion state actually changes.</para>
    /// \endif
    /// </summary>
    public event EventHandler? ExpandedChanged;

    // ── Constructor ───────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>머리글, 화살표, 콘텐츠 패널 및 Dreamine 테마 기본값을 구성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures the header, arrow, content panel, and Dreamine theme defaults.</para>
    /// \endif
    /// </summary>
    public DreamineExpander()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer |
            ControlStyles.ResizeRedraw, true);

        BackColor = DreamineTheme.CardBackground;
        ForeColor = DreamineTheme.TextPrimary;
        Font      = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point);

        // Header
        _headerPanel = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = HeaderHeight,
            BackColor = DreamineTheme.CardBackground,
            Cursor    = Cursors.Hand,
        };

        _arrowLabel = new Label
        {
            Text      = "▼",
            ForeColor = DreamineTheme.TextSecondary,
            Font      = new Font("Segoe UI", 8f, FontStyle.Regular, GraphicsUnit.Point),
            AutoSize  = false,
            Width     = 20,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock      = DockStyle.Left,
            BackColor = Color.Transparent,
        };

        _headerLabel = new Label
        {
            Text      = _header,
            ForeColor = DreamineTheme.TextPrimary,
            Font      = new Font("Segoe UI", 10f, FontStyle.Bold, GraphicsUnit.Point),
            AutoSize  = false,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(4, 0, 0, 0),
            BackColor = Color.Transparent,
        };

        _headerPanel.Controls.Add(_headerLabel);
        _headerPanel.Controls.Add(_arrowLabel);

        _headerPanel.Click  += OnHeaderClick;
        _headerLabel.Click  += OnHeaderClick;
        _arrowLabel.Click   += OnHeaderClick;

        // Content
        _contentPanel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = DreamineTheme.CardBackground,
            Padding   = new Padding(8),
        };

        Controls.Add(_contentPanel);
        Controls.Add(_headerPanel);

        _expandedHeight = 120;
        Height = HeaderHeight + _expandedHeight;
    }

    /// <summary>
    /// \if KO
    /// <para>머리글 클릭에 응답하여 확장 상태를 전환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Toggles expansion in response to a header click.</para>
    /// \endif
    /// </summary>
    /// <param name="s">
    /// \if KO
    /// <para>이벤트를 발생시킨 머리글 요소입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The header element that raised the event.</para>
    /// \endif
    /// </param>
    /// <param name="e">
    /// \if KO
    /// <para>이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The event arguments.</para>
    /// \endif
    /// </param>
    private void OnHeaderClick(object? s, EventArgs e)
    {
        IsExpanded = !IsExpanded;
    }

    /// <summary>
    /// \if KO
    /// <para>현재 확장 상태에 맞게 화살표, 콘텐츠 표시 및 컨트롤 높이를 적용합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Applies the arrow, content visibility, and control height for the current expansion state.</para>
    /// \endif
    /// </summary>
    private void ApplyExpandState()
    {
        if (_isExpanded)
        {
            _arrowLabel.Text = "▼";
            _contentPanel.Visible = true;
            Height = HeaderHeight + _expandedHeight;
        }
        else
        {
            _arrowLabel.Text = "▶";
            _expandedHeight = Height - HeaderHeight;
            _contentPanel.Visible = false;
            Height = HeaderHeight;
        }
    }

    /// <summary>
    /// \if KO
    /// <para>둥근 배경과 테두리 및 펼쳐진 머리글 구분선을 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws the rounded background and border and the expanded-header divider.</para>
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
        using var bgBrush  = new SolidBrush(BackColor);
        using var borderPen = new Pen(DreamineTheme.BorderNormal, 1f);
        DreamineDrawHelper.FillRoundedRect(g, bgBrush, borderPen, rect, DreamineTheme.CornerRadius);

        // Header separator line
        if (_isExpanded)
        {
            using var divPen = new Pen(DreamineTheme.BorderNormal, 1f);
            g.DrawLine(divPen, 0, HeaderHeight, Width, HeaderHeight);
        }
    }

    /// <summary>
    /// \if KO
    /// <para>확장 컨트롤의 기본 크기를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the default size of the expander.</para>
    /// \endif
    /// </summary>
    protected override Size DefaultSize => new(400, 120);
}
