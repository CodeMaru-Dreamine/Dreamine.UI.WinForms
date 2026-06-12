using System.Drawing;

namespace Dreamine.UI.WinForms;

/// <summary>Dreamine 다크 테마 색상/크기 상수.</summary>
public static class DreamineTheme
{
    // ── Backgrounds ───────────────────────────────────────
    public static readonly Color AppBackground    = Color.FromArgb(0xFF, 0x1A, 0x1A, 0x2E);
    public static readonly Color CardBackground   = Color.FromArgb(0xFF, 0x0F, 0x1E, 0x3A);
    public static readonly Color InputBackground  = Color.FromArgb(0xFF, 0x16, 0x20, 0x40);
    public static readonly Color NavBackground    = Color.FromArgb(0xFF, 0x0D, 0x1B, 0x3E);
    public static readonly Color BarBackground    = Color.FromArgb(0xFF, 0x0A, 0x15, 0x25);

    // ── Borders ───────────────────────────────────────────
    public static readonly Color BorderNormal     = Color.FromArgb(0xFF, 0x2D, 0x4A, 0x6E);
    public static readonly Color BorderFocus      = Color.FromArgb(0xFF, 0x1E, 0x90, 0xFF);

    // ── Foregrounds ───────────────────────────────────────
    public static readonly Color TextPrimary      = Color.White;
    public static readonly Color TextSecondary    = Color.FromArgb(0xFF, 0x88, 0x99, 0xAA);
    public static readonly Color TextHint         = Color.FromArgb(0xFF, 0x55, 0x66, 0x77);
    public static readonly Color TextAccent       = Color.FromArgb(0xFF, 0x4F, 0xC3, 0xF7);

    // ── Accent ────────────────────────────────────────────
    public static readonly Color AccentBlue       = Color.FromArgb(0xFF, 0x1E, 0x90, 0xFF);
    public static readonly Color AccentCyan       = Color.FromArgb(0xFF, 0x00, 0xBC, 0xD4);
    public static readonly Color AccentGreen      = Color.FromArgb(0xFF, 0x4C, 0xAF, 0x50);
    public static readonly Color AccentWarn       = Color.FromArgb(0xFF, 0xB8, 0x5C, 0x00);
    public static readonly Color AccentDanger     = Color.FromArgb(0xFF, 0x8B, 0x1A, 0x1A);

    // ── LED ───────────────────────────────────────────────
    public static readonly Color LedOnOuter       = Color.FromArgb(0xFF, 0x1F, 0xD3, 0x6B);
    public static readonly Color LedOnInner       = Color.FromArgb(0xFF, 0xA7, 0xF0, 0xC1);

    // ── Button hover/press ────────────────────────────────
    public static Color HoverOverlay  => Color.FromArgb(40, 255, 255, 255);
    public static Color PressOverlay  => Color.FromArgb(60, 0, 0, 0);

    // ── CornerRadius ──────────────────────────────────────
    public const int CornerRadius     = 6;
    public const int CornerRadiusSmall = 4;
}
