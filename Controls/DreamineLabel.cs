using System.Drawing;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>Dreamine 커스텀 Label. 다크 테마 기본값.</summary>
public class DreamineLabel : Label
{
    public DreamineLabel()
    {
        BackColor  = Color.Transparent;
        ForeColor  = DreamineTheme.TextPrimary;
        Font       = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point);
        AutoSize   = true;
    }
}
