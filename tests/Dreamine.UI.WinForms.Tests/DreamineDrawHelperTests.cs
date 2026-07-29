using System.Drawing;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Tests;

public sealed class DreamineDrawHelperTests
{
    [Fact]
    public void Blend_ReturnsExpectedInterpolatedColor()
    {
        var result = DreamineDrawHelper.Blend(
            Color.FromArgb(10, 20, 30),
            Color.FromArgb(110, 220, 230),
            0.5f);

        Assert.Equal(Color.FromArgb(255, 60, 120, 130), result);
    }

    [Fact]
    public void RoundedRect_WithZeroRadius_PreservesBounds()
    {
        using var path = DreamineDrawHelper.RoundedRect(new Rectangle(10, 20, 30, 40), 0);

        Assert.Equal(new RectangleF(10, 20, 30, 40), path.GetBounds());
    }

    [Fact]
    public void DrawingHelpers_RenderWithoutThrowing()
    {
        using var image = new Bitmap(120, 80);
        using var graphics = Graphics.FromImage(image);
        using var fill = new SolidBrush(DreamineTheme.CardBackground);
        using var border = new Pen(DreamineTheme.BorderNormal);
        using var font = new Font("Arial", 9f);

        DreamineDrawHelper.FillRoundedRect(graphics, fill, border, new Rectangle(4, 4, 80, 40), 6);
        DreamineDrawHelper.FillRoundedGradient(graphics, DreamineTheme.CardBackground, DreamineTheme.NavBackground, border, new Rectangle(8, 8, 96, 48), 8);
        DreamineDrawHelper.DrawShineOverlay(graphics, Color.White, new Rectangle(8, 8, 96, 48), 8);
        DreamineDrawHelper.DrawCenteredText(graphics, "Dreamine", font, DreamineTheme.TextPrimary, new Rectangle(0, 0, 120, 80));

        Assert.NotEqual(Color.Empty, image.GetPixel(10, 10));
    }
}
