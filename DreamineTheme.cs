using System.Drawing;

namespace Dreamine.UI.WinForms;

/// <summary>
/// \if KO
/// <para>Dreamine WinForms 다크 테마의 공통 색상과 모서리 크기를 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides shared colors and corner dimensions for the Dreamine WinForms dark theme.</para>
/// \endif
/// </summary>
public static class DreamineTheme
{
    // ── Backgrounds ───────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>애플리케이션 최상위 배경색입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The top-level application background color.</para>
    /// \endif
    /// </summary>
    public static readonly Color AppBackground    = Color.FromArgb(0xFF, 0x1A, 0x1A, 0x2E);
    /// <summary>
    /// \if KO
    /// <para>카드 및 패널 배경색입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The card and panel background color.</para>
    /// \endif
    /// </summary>
    public static readonly Color CardBackground   = Color.FromArgb(0xFF, 0x0F, 0x1E, 0x3A);
    /// <summary>
    /// \if KO
    /// <para>입력 컨트롤 배경색입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The input-control background color.</para>
    /// \endif
    /// </summary>
    public static readonly Color InputBackground  = Color.FromArgb(0xFF, 0x16, 0x20, 0x40);
    /// <summary>
    /// \if KO
    /// <para>탐색 영역 배경색입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The navigation-area background color.</para>
    /// \endif
    /// </summary>
    public static readonly Color NavBackground    = Color.FromArgb(0xFF, 0x0D, 0x1B, 0x3E);
    /// <summary>
    /// \if KO
    /// <para>도구 및 상태 표시줄 배경색입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The toolbar and status-bar background color.</para>
    /// \endif
    /// </summary>
    public static readonly Color BarBackground    = Color.FromArgb(0xFF, 0x0A, 0x15, 0x25);

    // ── Borders ───────────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>기본 테두리 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The normal border color.</para>
    /// \endif
    /// </summary>
    public static readonly Color BorderNormal     = Color.FromArgb(0xFF, 0x2D, 0x4A, 0x6E);
    /// <summary>
    /// \if KO
    /// <para>포커스된 컨트롤의 테두리 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The border color of a focused control.</para>
    /// \endif
    /// </summary>
    public static readonly Color BorderFocus      = Color.FromArgb(0xFF, 0x1E, 0x90, 0xFF);

    // ── Foregrounds ───────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>주요 본문 텍스트 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The primary body-text color.</para>
    /// \endif
    /// </summary>
    public static readonly Color TextPrimary      = Color.White;
    /// <summary>
    /// \if KO
    /// <para>보조 설명 텍스트 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The secondary descriptive-text color.</para>
    /// \endif
    /// </summary>
    public static readonly Color TextSecondary    = Color.FromArgb(0xFF, 0x88, 0x99, 0xAA);
    /// <summary>
    /// \if KO
    /// <para>입력 힌트 텍스트 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The input hint-text color.</para>
    /// \endif
    /// </summary>
    public static readonly Color TextHint         = Color.FromArgb(0xFF, 0x55, 0x66, 0x77);
    /// <summary>
    /// \if KO
    /// <para>강조 텍스트 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The accent-text color.</para>
    /// \endif
    /// </summary>
    public static readonly Color TextAccent       = Color.FromArgb(0xFF, 0x4F, 0xC3, 0xF7);

    // ── Accent ────────────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>기본 파란 강조 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The primary blue accent color.</para>
    /// \endif
    /// </summary>
    public static readonly Color AccentBlue       = Color.FromArgb(0xFF, 0x1E, 0x90, 0xFF);
    /// <summary>
    /// \if KO
    /// <para>청록 강조 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The cyan accent color.</para>
    /// \endif
    /// </summary>
    public static readonly Color AccentCyan       = Color.FromArgb(0xFF, 0x00, 0xBC, 0xD4);
    /// <summary>
    /// \if KO
    /// <para>성공 상태 강조 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The success-state accent color.</para>
    /// \endif
    /// </summary>
    public static readonly Color AccentGreen      = Color.FromArgb(0xFF, 0x4C, 0xAF, 0x50);
    /// <summary>
    /// \if KO
    /// <para>경고 상태 강조 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The warning-state accent color.</para>
    /// \endif
    /// </summary>
    public static readonly Color AccentWarn       = Color.FromArgb(0xFF, 0xB8, 0x5C, 0x00);
    /// <summary>
    /// \if KO
    /// <para>위험 상태 강조 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The danger-state accent color.</para>
    /// \endif
    /// </summary>
    public static readonly Color AccentDanger     = Color.FromArgb(0xFF, 0x8B, 0x1A, 0x1A);

    // ── LED ───────────────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>켜진 LED의 바깥쪽 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The outer color of an illuminated LED.</para>
    /// \endif
    /// </summary>
    public static readonly Color LedOnOuter       = Color.FromArgb(0xFF, 0x1F, 0xD3, 0x6B);
    /// <summary>
    /// \if KO
    /// <para>켜진 LED의 안쪽 색상입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The inner color of an illuminated LED.</para>
    /// \endif
    /// </summary>
    public static readonly Color LedOnInner       = Color.FromArgb(0xFF, 0xA7, 0xF0, 0xC1);

    // ── Button hover/press ────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>마우스 호버 상태에 합성할 반투명 오버레이 색상을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the translucent overlay color blended for a pointer-hover state.</para>
    /// \endif
    /// </summary>
    public static Color HoverOverlay  => Color.FromArgb(40, 255, 255, 255);
    /// <summary>
    /// \if KO
    /// <para>누름 상태에 합성할 반투명 오버레이 색상을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the translucent overlay color blended for a pressed state.</para>
    /// \endif
    /// </summary>
    public static Color PressOverlay  => Color.FromArgb(60, 0, 0, 0);

    // ── CornerRadius ──────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>일반 컨트롤에 사용할 기본 모서리 반지름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The default corner radius used by regular controls.</para>
    /// \endif
    /// </summary>
    public const int CornerRadius     = 6;
    /// <summary>
    /// \if KO
    /// <para>작은 컨트롤에 사용할 모서리 반지름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The corner radius used by small controls.</para>
    /// \endif
    /// </summary>
    public const int CornerRadiusSmall = 4;
}
