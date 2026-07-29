using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Tests;

public sealed class DreamineThemeTests
{
    [Fact]
    public void ThemeColors_AreOpaqueWhereSurfaceColorsAreExpected()
    {
        Assert.Equal(255, DreamineTheme.AppBackground.A);
        Assert.Equal(255, DreamineTheme.CardBackground.A);
        Assert.Equal(255, DreamineTheme.BorderFocus.A);
        Assert.Equal(255, DreamineTheme.TextPrimary.A);
    }

    [Fact]
    public void RadiusConstants_ArePositiveAndOrdered()
    {
        Assert.True(DreamineTheme.CornerRadius > 0);
        Assert.True(DreamineTheme.CornerRadiusSmall > 0);
        Assert.True(DreamineTheme.CornerRadius >= DreamineTheme.CornerRadiusSmall);
    }
}
