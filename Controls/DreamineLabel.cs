using System.Drawing;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// \if KO
/// <para>Dreamine 다크 테마 기본값을 적용한 WinForms 레이블입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a WinForms label initialized with Dreamine dark-theme defaults.</para>
/// \endif
/// </summary>
public class DreamineLabel : Label
{
    /// <summary>
    /// \if KO
    /// <para>투명 배경, 기본 전경색, 글꼴 및 자동 크기 조정을 구성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures a transparent background, default foreground color, font, and automatic sizing.</para>
    /// \endif
    /// </summary>
    public DreamineLabel()
    {
        BackColor  = Color.Transparent;
        ForeColor  = DreamineTheme.TextPrimary;
        Font       = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point);
        AutoSize   = true;
    }
}
