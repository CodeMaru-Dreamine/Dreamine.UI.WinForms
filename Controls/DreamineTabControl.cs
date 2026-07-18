using System.Drawing;
using System.Drawing.Drawing2D;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// \if KO
/// <para>다크 테마 머리글과 선택 밑줄을 직접 그리는 WinForms 탭 컨트롤입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a WinForms tab control that owner-draws dark-theme headers and a selection underline.</para>
/// \endif
/// </summary>
public class DreamineTabControl : TabControl
{
    /// <summary>
    /// \if KO
    /// <para>사용자 지정 그리기 스타일, 탭 크기 및 Dreamine 테마 기본값을 구성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures custom-painting styles, tab dimensions, and Dreamine theme defaults.</para>
    /// \endif
    /// </summary>
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

    /// <summary>
    /// \if KO
    /// <para>머리글 표시줄, 모든 탭 항목 및 콘텐츠 배경을 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws the header bar, all tab items, and the content background.</para>
    /// \endif
    /// </summary>
    /// <param name="e">
    /// \if KO
    /// <para>컨트롤 그리기 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The control paint event arguments.</para>
    /// \endif
    /// </param>
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

    /// <summary>
    /// \if KO
    /// <para>프레임워크 그리기 요청의 지정 탭 항목을 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws the specified tab item for a framework owner-draw request.</para>
    /// \endif
    /// </summary>
    /// <param name="e">
    /// \if KO
    /// <para>탭 항목 그리기 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The tab-item drawing event arguments.</para>
    /// \endif
    /// </param>
    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        DrawTabItem(e.Graphics, e.Index);
    }

    /// <summary>
    /// \if KO
    /// <para>지정한 인덱스의 탭을 선택 및 호버 상태에 맞게 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws the tab at the specified index for its selected and hover state.</para>
    /// \endif
    /// </summary>
    /// <param name="g">
    /// \if KO
    /// <para>탭을 그릴 그래픽 컨텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The graphics context on which the tab is drawn.</para>
    /// \endif
    /// </param>
    /// <param name="index">
    /// \if KO
    /// <para>그릴 탭의 인덱스입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The index of the tab to draw.</para>
    /// \endif
    /// </param>
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

    /// <summary>
    /// \if KO
    /// <para>포인터 이동 시 탭 호버 표시를 갱신하도록 다시 그리기를 요청합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Requests a redraw on pointer movement to update tab hover feedback.</para>
    /// \endif
    /// </summary>
    /// <param name="e">
    /// \if KO
    /// <para>마우스 이동 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The mouse-move event arguments.</para>
    /// \endif
    /// </param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        Invalidate(); // 호버 갱신
    }
    /// <summary>
    /// \if KO
    /// <para>포인터가 떠날 때 탭 호버 표시를 지우도록 다시 그리기를 요청합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Requests a redraw when the pointer leaves to clear tab hover feedback.</para>
    /// \endif
    /// </summary>
    /// <param name="e">
    /// \if KO
    /// <para>마우스 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The mouse event arguments.</para>
    /// \endif
    /// </param>
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        Invalidate();
    }
}
