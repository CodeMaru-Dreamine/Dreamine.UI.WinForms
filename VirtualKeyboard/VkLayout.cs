namespace Dreamine.UI.WinForms.VirtualKeyboard;

/// <summary>
/// WinForms 가상 키보드 레이아웃 종류.
/// (Dreamine.UI.Abstractions.VirtualKeyboard.VkLayout과 동일한 개념이지만,
/// WPF 어셈블리 의존을 피하기 위해 WinForms 쪽에 독립적으로 정의한다.)
/// </summary>
public enum VkLayout
{
    Text,
    Numeric,
    Password,
}
