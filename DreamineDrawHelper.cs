using System.Drawing;
using System.Drawing.Drawing2D;

namespace Dreamine.UI.WinForms;

/// <summary>
/// \if KO
/// <para>Dreamine WinForms 컨트롤의 둥근 도형, 그라데이션, 텍스트 및 색상 합성을 지원합니다.</para>
/// \endif
/// \if EN
/// <para>Provides rounded-shape, gradient, text, and color-composition helpers for Dreamine WinForms controls.</para>
/// \endif
/// </summary>
public static class DreamineDrawHelper
{
    /// <summary>
    /// \if KO
    /// <para>지정한 사각형과 반지름으로 둥근 사각형 경로를 만듭니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a rounded-rectangle path for the specified rectangle and radius.</para>
    /// \endif
    /// </summary>
    /// <param name="r">
    /// \if KO
    /// <para>경로의 외곽 사각형입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The bounding rectangle of the path.</para>
    /// \endif
    /// </param>
    /// <param name="radius">
    /// \if KO
    /// <para>모서리 반지름입니다. 0 이하는 직각 사각형을 만듭니다.</para>
    /// \endif
    /// \if EN
    /// <para>The corner radius. A non-positive value creates a square-cornered rectangle.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>호출자가 해제해야 하는 새 그래픽 경로입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A new graphics path that the caller must dispose.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>정수 좌표 사각형과 반지름으로 둥근 사각형 경로를 만듭니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a rounded-rectangle path from an integer-coordinate rectangle and radius.</para>
    /// \endif
    /// </summary>
    /// <param name="r">
    /// \if KO
    /// <para>경로의 외곽 사각형입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The bounding rectangle of the path.</para>
    /// \endif
    /// </param>
    /// <param name="radius">
    /// \if KO
    /// <para>모서리 반지름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The corner radius.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>호출자가 해제해야 하는 새 그래픽 경로입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A new graphics path that the caller must dispose.</para>
    /// \endif
    /// </returns>
    public static GraphicsPath RoundedRect(Rectangle r, float radius)
        => RoundedRect((RectangleF)r, radius);

    /// <summary>
    /// \if KO
    /// <para>둥근 사각형을 단색으로 채우고 선택적 테두리를 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Fills a rounded rectangle with a solid brush and draws an optional border.</para>
    /// \endif
    /// </summary>
    /// <param name="g">
    /// \if KO
    /// <para>그리기 대상 그래픽 컨텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The target graphics context.</para>
    /// \endif
    /// </param>
    /// <param name="fill">
    /// \if KO
    /// <para>내부를 채울 브러시입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The brush used to fill the interior.</para>
    /// \endif
    /// </param>
    /// <param name="border">
    /// \if KO
    /// <para>테두리 펜이거나 테두리를 생략하려면 <see langword="null"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The border pen, or <see langword="null"/> to omit the border.</para>
    /// \endif
    /// </param>
    /// <param name="rect">
    /// \if KO
    /// <para>그릴 외곽 사각형입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The bounding rectangle to draw.</para>
    /// \endif
    /// </param>
    /// <param name="radius">
    /// \if KO
    /// <para>모서리 반지름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The corner radius.</para>
    /// \endif
    /// </param>
    public static void FillRoundedRect(Graphics g, Brush fill, Pen? border,
        Rectangle rect, float radius)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = RoundedRect(rect, radius);
        g.FillPath(fill, path);
        if (border != null) g.DrawPath(border, path);
    }

    /// <summary>
    /// \if KO
    /// <para>둥근 사각형을 위에서 아래 방향의 색상 그라데이션으로 채웁니다.</para>
    /// \endif
    /// \if EN
    /// <para>Fills a rounded rectangle with a top-to-bottom color gradient.</para>
    /// \endif
    /// </summary>
    /// <param name="g">
    /// \if KO
    /// <para>그리기 대상 그래픽 컨텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The target graphics context.</para>
    /// \endif
    /// </param>
    /// <param name="top">
    /// \if KO
    /// <para>그라데이션의 위쪽 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The top gradient color.</para>
    /// \endif
    /// </param>
    /// <param name="bottom">
    /// \if KO
    /// <para>그라데이션의 아래쪽 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The bottom gradient color.</para>
    /// \endif
    /// </param>
    /// <param name="border">
    /// \if KO
    /// <para>테두리 펜이거나 생략하려면 <see langword="null"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The border pen, or <see langword="null"/> to omit it.</para>
    /// \endif
    /// </param>
    /// <param name="rect">
    /// \if KO
    /// <para>그릴 외곽 사각형입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The bounding rectangle to draw.</para>
    /// \endif
    /// </param>
    /// <param name="radius">
    /// \if KO
    /// <para>모서리 반지름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The corner radius.</para>
    /// \endif
    /// </param>
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

    /// <summary>
    /// \if KO
    /// <para>사각형 상단 절반에 투명 광택 그라데이션을 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws a translucent shine gradient over the upper half of a rectangle.</para>
    /// \endif
    /// </summary>
    /// <param name="g">
    /// \if KO
    /// <para>그리기 대상 그래픽 컨텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The target graphics context.</para>
    /// \endif
    /// </param>
    /// <param name="shineColor">
    /// \if KO
    /// <para>광택의 기준 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The base color of the shine.</para>
    /// \endif
    /// </param>
    /// <param name="rect">
    /// \if KO
    /// <para>광택을 적용할 사각형입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The rectangle to which the shine is applied.</para>
    /// \endif
    /// </param>
    /// <param name="radius">
    /// \if KO
    /// <para>광택 경로의 기준 모서리 반지름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The reference corner radius of the shine path.</para>
    /// \endif
    /// </param>
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

    /// <summary>
    /// \if KO
    /// <para>지정한 사각형 안에 텍스트를 가로 및 세로 중앙 정렬로 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws text horizontally and vertically centered within the specified rectangle.</para>
    /// \endif
    /// </summary>
    /// <param name="g">
    /// \if KO
    /// <para>그리기 대상 그래픽 컨텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The target graphics context.</para>
    /// \endif
    /// </param>
    /// <param name="text">
    /// \if KO
    /// <para>그릴 텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The text to draw.</para>
    /// \endif
    /// </param>
    /// <param name="font">
    /// \if KO
    /// <para>텍스트 글꼴입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The text font.</para>
    /// \endif
    /// </param>
    /// <param name="color">
    /// \if KO
    /// <para>텍스트 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The text color.</para>
    /// \endif
    /// </param>
    /// <param name="rect">
    /// \if KO
    /// <para>텍스트 배치 영역입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The text layout rectangle.</para>
    /// \endif
    /// </param>
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

    /// <summary>
    /// \if KO
    /// <para>지정한 보간 비율로 기준 색상과 오버레이 색상을 혼합합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Blends a base color with an overlay color using the specified interpolation ratio.</para>
    /// \endif
    /// </summary>
    /// <param name="base_">
    /// \if KO
    /// <para>혼합의 시작 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The starting color of the blend.</para>
    /// \endif
    /// </param>
    /// <param name="overlay">
    /// \if KO
    /// <para>혼합할 오버레이 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The overlay color to blend.</para>
    /// \endif
    /// </param>
    /// <param name="alpha">
    /// \if KO
    /// <para>오버레이 보간 비율입니다. 일반적인 범위는 0부터 1까지입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The overlay interpolation ratio; the conventional range is zero through one.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>각 RGB 채널을 바이트 범위로 제한한 불투명 혼합 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The opaque blended color with each RGB channel clamped to the byte range.</para>
    /// \endif
    /// </returns>
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
