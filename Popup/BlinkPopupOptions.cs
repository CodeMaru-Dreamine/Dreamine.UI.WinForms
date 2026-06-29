using System.Drawing;

namespace Dreamine.UI.WinForms.Popup;

/// <summary>
/// WPF의 Dreamine.UI.Abstractions.Popup.BlinkPopupOptions에 대응하는 WinForms 전용 옵션.
/// (WPF 버전은 System.Windows.Window/Color에 종속돼 있어 WinForms에서 그대로 재사용할 수 없다.)
/// </summary>
public sealed class BlinkPopupOptions
{
    public string? Title { get; set; }
    public string? Message { get; set; }

    /// <summary>OK 버튼 텍스트(null/빈 문자열이면 버튼 숨김).</summary>
    public string? OkText { get; set; }

    /// <summary>Cancel 버튼 텍스트(null/빈 문자열이면 버튼 숨김).</summary>
    public string? CancelText { get; set; }

    /// <summary>모달 여부(true면 owner 창의 입력을 막는다).</summary>
    public bool IsModal { get; set; } = true;

    /// <summary>깜빡임 사용 여부.</summary>
    public bool UseBlink { get; set; } = true;

    /// <summary>깜빡임 1차 배경색.</summary>
    public Color Color1 { get; set; } = Color.Red;

    /// <summary>깜빡임 2차 배경색.</summary>
    public Color Color2 { get; set; } = Color.DarkRed;

    /// <summary>전경(텍스트) 색.</summary>
    public Color ForegroundColor { get; set; } = Color.Yellow;

    /// <summary>깜빡임 간격(ms). 왕복 한 번에 2배 소요.</summary>
    public int BlinkIntervalMs { get; set; } = 600;
}
