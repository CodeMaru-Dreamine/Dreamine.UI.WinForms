using System.Drawing.Drawing2D;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// Light bulb indicator control with a shared API across WPF, WinForms, Blazor, and MAUI.
/// </summary>
public class DreamineLightBulb : Control
{
    private bool _isOn;
    private float _diameter = 96f;

    public bool IsOn
    {
        get => _isOn;
        set { _isOn = value; Invalidate(); }
    }

    public float Diameter
    {
        get => _diameter;
        set { _diameter = Math.Max(32f, value); Invalidate(); }
    }

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

    private static void FillRib(Graphics g, Brush brush, float cx, float y, float width)
    {
        g.FillRectangle(brush, cx - width / 2f, y, width, 7f);
    }
}
