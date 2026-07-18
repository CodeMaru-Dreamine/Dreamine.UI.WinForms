using System.Drawing;

namespace Dreamine.UI.WinForms.Popup;

/// <summary>
/// \if KO
/// <para>WinForms 깜빡임 팝업의 콘텐츠, 모달 동작, 색상 및 애니메이션을 구성합니다.</para>
/// \endif
/// \if EN
/// <para>Configures the content, modal behavior, colors, and animation of a WinForms blinking popup.</para>
/// \endif
/// </summary>
public sealed class BlinkPopupOptions
{
    /// <summary>
    /// \if KO
    /// <para>팝업 제목을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the popup title.</para>
    /// \endif
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// \if KO
    /// <para>팝업 메시지를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the popup message.</para>
    /// \endif
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// \if KO
    /// <para>확인 버튼 텍스트를 가져오거나 설정합니다. <see langword="null"/> 또는 빈 문자열이면 버튼을 숨깁니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the OK button text. A <see langword="null"/> or empty value hides the button.</para>
    /// \endif
    /// </summary>
    public string? OkText { get; set; }

    /// <summary>
    /// \if KO
    /// <para>취소 버튼 텍스트를 가져오거나 설정합니다. <see langword="null"/> 또는 빈 문자열이면 버튼을 숨깁니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the Cancel button text. A <see langword="null"/> or empty value hides the button.</para>
    /// \endif
    /// </summary>
    public string? CancelText { get; set; }

    /// <summary>
    /// \if KO
    /// <para>팝업이 열려 있는 동안 소유자 폼 입력을 막을지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether owner-form input is disabled while the popup is open.</para>
    /// \endif
    /// </summary>
    public bool IsModal { get; set; } = true;

    /// <summary>
    /// \if KO
    /// <para>배경 깜빡임 효과를 사용할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the background blinking effect is enabled.</para>
    /// \endif
    /// </summary>
    public bool UseBlink { get; set; } = true;

    /// <summary>
    /// \if KO
    /// <para>깜빡임 애니메이션의 첫 번째 배경색을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the first background color of the blinking animation.</para>
    /// \endif
    /// </summary>
    public Color Color1 { get; set; } = Color.Red;

    /// <summary>
    /// \if KO
    /// <para>깜빡임 애니메이션의 두 번째 배경색을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the second background color of the blinking animation.</para>
    /// \endif
    /// </summary>
    public Color Color2 { get; set; } = Color.DarkRed;

    /// <summary>
    /// \if KO
    /// <para>팝업 텍스트의 전경색을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the foreground color of the popup text.</para>
    /// \endif
    /// </summary>
    public Color ForegroundColor { get; set; } = Color.Yellow;

    /// <summary>
    /// \if KO
    /// <para>한 방향 깜빡임 전환 간격을 밀리초 단위로 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the one-way blink transition interval in milliseconds.</para>
    /// \endif
    /// </summary>
    public int BlinkIntervalMs { get; set; } = 600;
}
