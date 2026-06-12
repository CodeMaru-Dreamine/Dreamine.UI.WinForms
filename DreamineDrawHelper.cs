using System.Drawing;
using System.Drawing.Drawing2D;

namespace Dreamine.UI.WinForms;

/// <summary>WinForms 커스텀 드로잉 유틸.</summary>
public static class DreamineDrawHelper
{
    /// <summary>둥근 모서리 GraphicsPath 생성.</summary>
    public static GraphicsPath RoundedRect(RectangleF r, float radius)
    {
        if (radius <= 0) { var p = new GraphicsPath(); p.AddRectangle(r); return p; }
        float d = radius * 2f;
        var path = new GraphicsPath();
        path.AddArc(r.X,              r.Y,              d, d, 180, 90);
        path.AddArc(r.Right - d,      r.Y,              d, d, 270, 90);
        path.AddArc(r.Right - d,      r.Bottom - d,     d, d,   0, 90);
        path.AddArc(r.X,              r.Bottom - d,     d, d,  90, 90);
        path.CloseFigure();
        return path;
    }

    public static GraphicsPath RoundedRect(Rectangle r, float radius)
        => RoundedRect((RectangleF)r, radius);

    /// <summary>단색 채우기 + 테두리 그리기.</summary>
    public static void FillRoundedRect(Graphics g, Brush fill, Pen? border,
        Rectangle rect, float radius)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = RoundedRect(rect, radius);
        g.FillPath(fill, path);
        if (border != null) g.DrawPath(border, path);
    }

    /// <summary>그라데이션 채우기 (위→아래).</summary>
    public static void FillRoundedGradient(Graphics g, Color top, Color bottom, Pen? border,
        Rectangle rect, float radius)
    {
        if (rect.Width <= 0 || rect.Height <= 0) return;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path  = RoundedRect(rect, radius);
        using var brush = new LinearGradientBrush(rect, top, bottom, LinearGradientMode.Vertical);
        g.FillPath(brush, path);
        if (border != null) g.DrawPath(border, path);
    }

    /// <summary>ShineColor 글로우 오버레이 (상단 반쪽 흰색 알파 그라데이션).</summary>
    public static void DrawShineOverlay(Graphics g, Color shineColor, Rectangle rect, float radius)
    {
        if (rect.Width <= 0 || rect.Height <= 0) return;
        var shineRect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height / 2);
        if (shineRect.Width <= 0 || shineRect.Height <= 0) return;
        using var path  = RoundedRect(shineRect, radius * 0.7f);
        var c1 = Color.FromArgb(80, shineColor);
        var c2 = Color.FromArgb(0,  shineColor);
        using var brush = new LinearGradientBrush(shineRect, c1, c2, LinearGradientMode.Vertical);
        g.FillPath(brush, path);
    }

    /// <summary>텍스트를 중앙 정렬로 그리기.</summary>
    public static void DrawCenteredText(Graphics g, string text, Font font, Color color, Rectangle rect)
    {
        using var brush = new SolidBrush(color);
        var sf = new StringFormat
        {
            Alignment     = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming      = StringTrimming.EllipsisCharacter
        };
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        g.DrawString(text, font, brush, rect, sf);
    }

    /// <summary>Blend 색상 (alpha 0~1).</summary>
    public static Color Blend(Color base_, Color overlay, float alpha)
    {
        int r = (int)(base_.R + (overlay.R - base_.R) * alpha);
        int g_ = (int)(base_.G + (overlay.G - base_.G) * alpha);
        int b = (int)(base_.B + (overlay.B - base_.B) * alpha);
        return Color.FromArgb(255,
            Math.Clamp(r, 0, 255),
            Math.Clamp(g_, 0, 255),
            Math.Clamp(b, 0, 255));
    }
}
