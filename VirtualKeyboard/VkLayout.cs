namespace Dreamine.UI.WinForms.VirtualKeyboard;

/// <summary>
/// \if KO
/// <para>WinForms 가상 키보드에 사용할 레이아웃 종류를 지정합니다.</para>
/// \endif
/// \if EN
/// <para>Specifies the layout type used by the WinForms virtual keyboard.</para>
/// \endif
/// </summary>
public enum VkLayout
{
    /// <summary>
    /// \if KO
    /// <para>일반 텍스트 입력 레이아웃을 사용합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Uses the general text-input layout.</para>
    /// \endif
    /// </summary>
    Text,
    /// <summary>
    /// \if KO
    /// <para>숫자 입력 레이아웃을 사용합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Uses the numeric-input layout.</para>
    /// \endif
    /// </summary>
    Numeric,
    /// <summary>
    /// \if KO
    /// <para>암호 입력용 레이아웃을 사용합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Uses the password-input layout.</para>
    /// \endif
    /// </summary>
    Password,
}
