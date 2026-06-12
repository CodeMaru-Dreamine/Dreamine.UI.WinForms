using System.Drawing;
using System.Drawing.Drawing2D;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// Dreamine 커스텀 Expander (접기/펼치기 패널).
/// WPF DreamineExpander와 동일한 API: Header, IsExpanded, Content.
/// </summary>
public class DreamineExpander : UserControl
{
    private readonly Panel   _headerPanel;
    private readonly Label   _headerLabel;
    private readonly Label   _arrowLabel;
    private readonly Panel   _contentPanel;
    private int _expandedHeight;

    private const int HeaderHeight = 36;

    // ── Properties ────────────────────────────────────────
    private string _header = string.Empty;
    public string Header
    {
        get => _header;
        set { _header = value; _headerLabel.Text = value; }
    }

    private bool _isExpanded = true;
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

    public Panel Content => _contentPanel;

    public event EventHandler? ExpandedChanged;

    // ── Constructor ───────────────────────────────────────
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

    private void OnHeaderClick(object? s, EventArgs e)
    {
        IsExpanded = !IsExpanded;
    }

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

    protected override Size DefaultSize => new(400, 120);
}
