using System.Drawing;
using System.Drawing.Drawing2D;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// Dreamine 커스텀 TabControl. WPF DreamineTabControl과 동일한 다크 테마.
/// 선택된 탭: 파란 하단 밑줄 + 흰색 텍스트.
/// </summary>
public class DreamineTabControl : TabControl
{
    public DreamineTabControl()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer |
            ControlStyles.ResizeRedraw, true);

        DrawMode  = TabDrawMode.OwnerDrawFixed;
        ItemSize  = new Size(120, 36);
        Padding   = new Point(12, 6);
        BackColor = DreamineTheme.AppBackground;
        Font      = new Font("Segoe UI", 10f, FontStyle.Bold, GraphicsUnit.Point);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(DreamineTheme.AppBackground);

        // Header bar background
        var headerRect = new Rectangle(0, 0, Width, ItemSize.Height + 1);
        using var headerBrush = new SolidBrush(DreamineTheme.BarBackground);
        g.FillRectangle(headerBrush, headerRect);

        // Header bar bottom border
        using var dividerPen = new Pen(DreamineTheme.BorderNormal, 1f);
        g.DrawLine(dividerPen, 0, ItemSize.Height, Width, ItemSize.Height);

        // Draw tab items
        for (int i = 0; i < TabCount; i++)
            DrawTabItem(g, i);

        // Content area
        if (SelectedTab != null)
        {
            var contentRect = new Rectangle(0, ItemSize.Height + 1, Width, Height - ItemSize.Height - 1);
            using var contentBrush = new SolidBrush(DreamineTheme.AppBackground);
            g.FillRectangle(contentBrush, contentRect);
        }
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        DrawTabItem(e.Graphics, e.Index);
    }

    private void DrawTabItem(Graphics g, int index)
    {
        if (index < 0 || index >= TabCount) return;
        var tabRect  = GetTabRect(index);
        bool selected = SelectedIndex == index;

        // Hover detection
        var mousePos = PointToClient(Cursor.Position);
        bool hovered = tabRect.Contains(mousePos) && !selected;

        // Background
        var bg = hovered
            ? Color.FromArgb(0xFF, 0x16, 0x20, 0x40)
            : DreamineTheme.BarBackground;

        using var bgBrush = new SolidBrush(bg);
        g.FillRectangle(bgBrush, tabRect);

        // Selected: blue underline
        if (selected)
        {
            using var selPen = new Pen(DreamineTheme.AccentBlue, 2.5f);
            g.DrawLine(selPen, tabRect.Left + 4, tabRect.Bottom - 1, tabRect.Right - 4, tabRect.Bottom - 1);
        }

        // Label
        var textColor = selected ? DreamineTheme.TextPrimary
            : hovered ? Color.FromArgb(0xFF, 0xCC, 0xE4, 0xFF)
            : DreamineTheme.TextSecondary;

        using var brush = new SolidBrush(textColor);
        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        g.DrawString(TabPages[index].Text, Font, brush, tabRect, sf);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        Invalidate(); // 호버 갱신
    }
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        Invalidate();
    }
}
