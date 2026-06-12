using System.Drawing;
using System.Drawing.Drawing2D;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// Dreamine LED 인디케이터. WPF DreamineCheckLed와 동일한 API:
/// IsOn, IsPulse, Corner, Diameter.
/// </summary>
public class DreamineCheckLed : Control
{
    private System.Windows.Forms.Timer? _pulseTimer;
    private float _pulseAlpha = 1f;
    private bool  _pulseUp    = false;

    // ── Properties ────────────────────────────────────────
    private bool _isOn = true;
    public bool IsOn
    {
        get => _isOn;
        set { _isOn = value; Invalidate(); }
    }

    private bool _isPulse;
    public bool IsPulse
    {
        get => _isPulse;
        set
        {
            _isPulse = value;
            if (value) StartPulse();
            else       StopPulse();
            Invalidate();
        }
    }

    private LedCorner _corner = LedCorner.TopRight;
    public LedCorner Corner
    {
        get => _corner;
        set { _corner = value; Invalidate(); }
    }

    private float _diameter = 16f;
    public float Diameter
    {
        get => _diameter;
        set { _diameter = value; Invalidate(); }
    }

    // ── Constructor ───────────────────────────────────────
    public DreamineCheckLed()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor, true);

        BackColor = Color.Transparent;
        Size = new Size(24, 24);
    }

    // ── Pulse ─────────────────────────────────────────────
    private void StartPulse()
    {
        _pulseTimer ??= new System.Windows.Forms.Timer { Interval = 40 };
        _pulseTimer.Tick += OnPulseTick;
        _pulseTimer.Start();
    }

    private void StopPulse()
    {
        if (_pulseTimer != null)
        {
            _pulseTimer.Stop();
            _pulseTimer.Tick -= OnPulseTick;
        }
        _pulseAlpha = 1f;
        Invalidate();
    }

    private void OnPulseTick(object? s, EventArgs e)
    {
        _pulseAlpha += _pulseUp ? 0.06f : -0.06f;
        if (_pulseAlpha >= 1f) { _pulseAlpha = 1f; _pulseUp = false; }
        if (_pulseAlpha <= 0.2f) { _pulseAlpha = 0.2f; _pulseUp = true; }
        Invalidate();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) { _pulseTimer?.Dispose(); _pulseTimer = null; }
        base.Dispose(disposing);
    }

    // ── Paint ─────────────────────────────────────────────
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        float d = _diameter;
        float x, y;

        switch (_corner)
        {
            case LedCorner.TopLeft:     x = 0;             y = 0;              break;
            case LedCorner.TopRight:    x = Width  - d;    y = 0;              break;
            case LedCorner.BottomLeft:  x = 0;             y = Height - d;     break;
            case LedCorner.BottomRight: x = Width  - d;    y = Height - d;     break;
            default:                    x = (Width - d)/2; y = (Height - d)/2; break;
        }

        var ledRect = new RectangleF(x, y, d, d);

        if (_isOn)
        {
            float alpha = _isPulse ? _pulseAlpha : 1f;

            // Outer glow ring
            float glowD = d + 4;
            var glowRect = new RectangleF(x - 2, y - 2, glowD, glowD);
            using var glowBrush = new PathGradientBrush(DreamineDrawHelper.RoundedRect(glowRect, glowD / 2))
            {
                CenterColor    = Color.FromArgb((int)(80 * alpha), DreamineTheme.LedOnOuter),
                SurroundColors = new[] { Color.FromArgb(0, DreamineTheme.LedOnOuter) }
            };
            g.FillEllipse(glowBrush, glowRect);

            // Outer ring
            using var outerBrush = new PathGradientBrush(DreamineDrawHelper.RoundedRect(ledRect, d / 2))
            {
                CenterColor    = Color.FromArgb((int)(220 * alpha), DreamineTheme.LedOnInner),
                SurroundColors = new[] { Color.FromArgb((int)(160 * alpha), DreamineTheme.LedOnOuter) }
            };
            g.FillEllipse(outerBrush, ledRect);

            // Inner highlight
            float innerD = d * 0.45f;
            float innerX = x + (d - innerD) / 2, innerY = y + (d - innerD) / 2;
            using var innerBrush = new SolidBrush(Color.FromArgb((int)(120 * alpha), Color.White));
            g.FillEllipse(innerBrush, innerX, innerY, innerD, innerD);
        }
        else
        {
            // Off: dim ring only
            using var ringPen = new Pen(Color.FromArgb(80, DreamineTheme.LedOnOuter), 1.5f);
            g.DrawEllipse(ringPen, ledRect.X, ledRect.Y, ledRect.Width - 1, ledRect.Height - 1);
            using var centerBrush = new SolidBrush(Color.FromArgb(30, DreamineTheme.CardBackground));
            g.FillEllipse(centerBrush, ledRect);
        }
    }
}
