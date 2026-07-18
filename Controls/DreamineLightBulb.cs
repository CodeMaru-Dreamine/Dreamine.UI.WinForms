using System.Drawing.Drawing2D;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// \if KO
/// <para>플랫폼 간 공통 API로 상태를 직접 그리는 WinForms 전구 표시 컨트롤입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a WinForms light-bulb indicator that draws its state directly through a cross-platform API.</para>
/// \endif
/// </summary>
public class DreamineLightBulb : Control
{
    /// <summary>
    /// \if KO
    /// <para>is On 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the is on value.</para>
    /// \endif
    /// </summary>
    private bool _isOn;
    /// <summary>
    /// \if KO
    /// <para>diameter 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the diameter value.</para>
    /// \endif
    /// </summary>
    private float _diameter = 96f;

    /// <summary>
    /// \if KO
    /// <para>전구가 켜져 있는지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the light bulb is on.</para>
    /// \endif
    /// </summary>
    public bool IsOn
    {
        get => _isOn;
        set { _isOn = value; Invalidate(); }
    }

    /// <summary>
    /// \if KO
    /// <para>최소 32픽셀로 제한되는 전구 유리 기준 지름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the bulb-glass reference diameter, clamped to a minimum of 32 pixels.</para>
    /// \endif
    /// </summary>
    public float Diameter
    {
        get => _diameter;
        set { _diameter = Math.Max(32f, value); Invalidate(); }
    }

    /// <summary>
    /// \if KO
    /// <para>사용자 지정 그리기 스타일과 전구의 기본 크기를 구성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures custom-painting styles and the bulb's default size.</para>
    /// \endif
    /// </summary>
    public DreamineLightBulb()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor, true);

        BackColor = Color.Transparent;
        Size = new Size(150, 180);
    }

    /// <summary>
    /// \if KO
    /// <para>현재 켜짐 상태와 컨트롤 크기에 맞게 전구 유리, 필라멘트 및 소켓을 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws the bulb glass, filament, and socket for the current on state and control size.</para>
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
        base.OnPaint(e);

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var d = Math.Min(_diameter, Math.Min(Width * .85f, Height * .62f));
        var cx = Width / 2f;
        var top = 4f;

        var glassFill = _isOn ? Color.FromArgb(255, 214, 102) : Color.FromArgb(42, 100, 116, 139);
        var glassStroke = _isOn ? Color.FromArgb(255, 196, 0) : Color.FromArgb(102, 117, 139);
        var filament = _isOn ? Color.FromArgb(122, 75, 0) : Color.FromArgb(100, 116, 139);
        var baseFill = Color.FromArgb(112, 128, 152);
        using var glassPath = CreateGlassPath(cx, top, d);

        if (_isOn)
        {
            using var glow = new SolidBrush(Color.FromArgb(70, 255, 214, 102));
            g.FillEllipse(glow, cx - d * .62f, top + d * .50f - d * .62f, d * 1.24f, d * 1.24f);
        }

        using var fill = new SolidBrush(glassFill);
        using var stroke = new Pen(glassStroke, 4);
        g.FillPath(fill, glassPath);
        g.DrawPath(stroke, glassPath);

        using var filamentPen = new Pen(filament, 4)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round
        };
        g.DrawBezier(filamentPen, cx - d * .22f, top + d * .56f, cx - d * .12f, top + d * .38f, cx - d * .02f, top + d * .72f, cx + d * .10f, top + d * .53f);
        g.DrawBezier(filamentPen, cx + d * .10f, top + d * .53f, cx + d * .16f, top + d * .44f, cx + d * .21f, top + d * .49f, cx + d * .25f, top + d * .55f);

        var neckTop = top + d * .92f;
        using var baseBrush = new SolidBrush(baseFill);
        g.FillPolygon(baseBrush, new[]
        {
            new PointF(cx - d * .30f, neckTop),
            new PointF(cx + d * .30f, neckTop),
            new PointF(cx + d * .20f, neckTop + d * .26f),
            new PointF(cx - d * .20f, neckTop + d * .26f)
        });
        FillRib(g, baseBrush, cx, neckTop + d * .10f, d * .44f);
        FillRib(g, baseBrush, cx, neckTop + d * .22f, d * .36f);
        FillRib(g, baseBrush, cx, neckTop + d * .34f, d * .27f);
    }

    /// <summary>
    /// \if KO
    /// <para>전구 유리 외곽선을 나타내는 그래픽 경로를 만듭니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a graphics path representing the outline of the bulb glass.</para>
    /// \endif
    /// </summary>
    /// <param name="cx">
    /// \if KO
    /// <para>전구 중심의 X 좌표입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The X coordinate of the bulb center.</para>
    /// \endif
    /// </param>
    /// <param name="top">
    /// \if KO
    /// <para>전구 위쪽 Y 좌표입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The top Y coordinate of the bulb.</para>
    /// \endif
    /// </param>
    /// <param name="d">
    /// \if KO
    /// <para>기준 지름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The reference diameter.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>호출자가 해제해야 하는 닫힌 전구 유리 경로입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A closed bulb-glass path that the caller must dispose.</para>
    /// \endif
    /// </returns>
    private static GraphicsPath CreateGlassPath(float cx, float top, float d)
    {
        var path = new GraphicsPath();
        path.StartFigure();
        path.AddBezier(cx, top + d * .02f, cx - d * .36f, top + d * .02f, cx - d * .52f, top + d * .27f, cx - d * .52f, top + d * .54f);
        path.AddBezier(cx - d * .52f, top + d * .54f, cx - d * .52f, top + d * .74f, cx - d * .35f, top + d * .87f, cx - d * .25f, top + d * .96f);
        path.AddLine(cx - d * .25f, top + d * .96f, cx + d * .25f, top + d * .96f);
        path.AddBezier(cx + d * .25f, top + d * .96f, cx + d * .35f, top + d * .87f, cx + d * .52f, top + d * .74f, cx + d * .52f, top + d * .54f);
        path.AddBezier(cx + d * .52f, top + d * .54f, cx + d * .52f, top + d * .27f, cx + d * .36f, top + d * .02f, cx, top + d * .02f);
        path.CloseFigure();
        return path;
    }

    /// <summary>
    /// \if KO
    /// <para>전구 소켓의 가로 홈 하나를 채웁니다.</para>
    /// \endif
    /// \if EN
    /// <para>Fills one horizontal rib of the bulb socket.</para>
    /// \endif
    /// </summary>
    /// <param name="g">
    /// \if KO
    /// <para>홈을 그릴 그래픽 컨텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The graphics context on which the rib is drawn.</para>
    /// \endif
    /// </param>
    /// <param name="brush">
    /// \if KO
    /// <para>홈을 채울 브러시입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The brush used to fill the rib.</para>
    /// \endif
    /// </param>
    /// <param name="cx">
    /// \if KO
    /// <para>홈 중심의 X 좌표입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The X coordinate of the rib center.</para>
    /// \endif
    /// </param>
    /// <param name="y">
    /// \if KO
    /// <para>홈의 Y 좌표입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The Y coordinate of the rib.</para>
    /// \endif
    /// </param>
    /// <param name="width">
    /// \if KO
    /// <para>홈 너비입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The rib width.</para>
    /// \endif
    /// </param>
    private static void FillRib(Graphics g, Brush brush, float cx, float y, float width)
    {
        g.FillRectangle(brush, cx - width / 2f, y, width, 7f);
    }
}
