using System.Drawing;
using System.Drawing.Drawing2D;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// \if KO
/// <para>켜짐, 맥동, 지름 및 모서리 배치를 지원하는 Dreamine WinForms LED 표시 컨트롤입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a Dreamine WinForms LED indicator with on, pulse, diameter, and corner-placement support.</para>
/// \endif
/// </summary>
public class DreamineCheckLed : Control
{
    /// <summary>
    /// \if KO
    /// <para>pulse Timer 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the pulse timer value.</para>
    /// \endif
    /// </summary>
    private System.Windows.Forms.Timer? _pulseTimer;
    /// <summary>
    /// \if KO
    /// <para>pulse Alpha 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the pulse alpha value.</para>
    /// \endif
    /// </summary>
    private float _pulseAlpha = 1f;
    /// <summary>
    /// \if KO
    /// <para>pulse Up 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the pulse up value.</para>
    /// \endif
    /// </summary>
    private bool  _pulseUp    = false;

    // ── Properties ────────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>is On 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the is on value.</para>
    /// \endif
    /// </summary>
    private bool _isOn = true;
    /// <summary>
    /// \if KO
    /// <para>LED가 켜져 있는지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the LED is on.</para>
    /// \endif
    /// </summary>
    public bool IsOn
    {
        get => _isOn;
        set { _isOn = value; Invalidate(); }
    }

    /// <summary>
    /// \if KO
    /// <para>is Pulse 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the is pulse value.</para>
    /// \endif
    /// </summary>
    private bool _isPulse;
    /// <summary>
    /// \if KO
    /// <para>불투명도 맥동 애니메이션을 사용할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the opacity pulse animation is enabled.</para>
    /// \endif
    /// </summary>
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

    /// <summary>
    /// \if KO
    /// <para>corner 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the corner value.</para>
    /// \endif
    /// </summary>
    private LedCorner _corner = LedCorner.TopRight;
    /// <summary>
    /// \if KO
    /// <para>컨트롤 영역에서 LED를 그릴 모서리를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the corner in the control bounds where the LED is drawn.</para>
    /// \endif
    /// </summary>
    public LedCorner Corner
    {
        get => _corner;
        set { _corner = value; Invalidate(); }
    }

    /// <summary>
    /// \if KO
    /// <para>diameter 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the diameter value.</para>
    /// \endif
    /// </summary>
    private float _diameter = 16f;
    /// <summary>
    /// \if KO
    /// <para>LED 원의 지름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the diameter of the LED circle.</para>
    /// \endif
    /// </summary>
    public float Diameter
    {
        get => _diameter;
        set { _diameter = value; Invalidate(); }
    }

    // ── Constructor ───────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>사용자 지정 그리기 스타일과 기본 LED 크기를 구성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures custom-painting styles and the default LED size.</para>
    /// \endif
    /// </summary>
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
    /// <summary>
    /// \if KO
    /// <para>맥동 타이머를 만들거나 다시 시작합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates or restarts the pulse timer.</para>
    /// \endif
    /// </summary>
    private void StartPulse()
    {
        if (_pulseTimer == null)
        {
            _pulseTimer = new System.Windows.Forms.Timer { Interval = 40 };
            _pulseTimer.Tick += OnPulseTick;
        }
        _pulseTimer.Start();
    }

    /// <summary>
    /// \if KO
    /// <para>맥동 타이머를 중지하고 불투명도를 초기 상태로 복원합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stops the pulse timer and restores opacity to its initial state.</para>
    /// \endif
    /// </summary>
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

    /// <summary>
    /// \if KO
    /// <para>타이머 틱마다 맥동 불투명도와 방향을 갱신합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Updates pulse opacity and direction on each timer tick.</para>
    /// \endif
    /// </summary>
    /// <param name="s">
    /// \if KO
    /// <para>이벤트를 발생시킨 타이머입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The timer that raised the event.</para>
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
    private void OnPulseTick(object? s, EventArgs e)
    {
        _pulseAlpha += _pulseUp ? 0.06f : -0.06f;
        if (_pulseAlpha >= 1f) { _pulseAlpha = 1f; _pulseUp = false; }
        if (_pulseAlpha <= 0.2f) { _pulseAlpha = 0.2f; _pulseUp = true; }
        Invalidate();
    }

    /// <summary>
    /// \if KO
    /// <para>맥동 타이머를 해제한 뒤 기본 컨트롤 리소스를 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Disposes the pulse timer before releasing base-control resources.</para>
    /// \endif
    /// </summary>
    /// <param name="disposing">
    /// \if KO
    /// <para>관리되는 리소스도 해제하면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> to dispose managed resources as well.</para>
    /// \endif
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing) { _pulseTimer?.Dispose(); _pulseTimer = null; }
        base.Dispose(disposing);
    }

    // ── Paint ─────────────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>현재 켜짐, 맥동 및 모서리 상태에 맞게 LED 광택과 링을 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws LED glow and rings for the current on, pulse, and corner state.</para>
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
