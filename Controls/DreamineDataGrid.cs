using System.Drawing;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// \if KO
/// <para>다크 테마와 선택 행 재클릭 해제를 지원하는 Dreamine WinForms 데이터 그리드입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a Dreamine WinForms data grid with dark-theme styling and optional deselection by reclicking a selected row.</para>
/// \endif
/// </summary>
public class DreamineDataGrid : DataGridView
{
    /// <summary>
    /// \if KO
    /// <para>last Clicked Row 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the last clicked row value.</para>
    /// \endif
    /// </summary>
    private int _lastClickedRow = -1;

    /// <summary>
    /// \if KO
    /// <para>이미 선택된 행을 다시 클릭하면 선택을 해제할지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether clicking an already selected row clears the selection.</para>
    /// \endif
    /// </summary>
    public bool EnableClickToDeselect { get; set; }

    /// <summary>
    /// \if KO
    /// <para>표 동작과 헤더, 행, 교대 행 및 선택 색상의 다크 테마 기본값을 구성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures grid behavior and dark-theme defaults for headers, rows, alternating rows, and selection.</para>
    /// \endif
    /// </summary>
    public DreamineDataGrid()
    {
        BorderStyle = BorderStyle.None;
        BackgroundColor = DreamineTheme.InputBackground;
        GridColor = DreamineTheme.BorderNormal;
        RowHeadersVisible = false;
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        AllowUserToResizeRows = false;
        ReadOnly = true;
        SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        MultiSelect = false;
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        ColumnHeadersHeight = 32;
        EnableHeadersVisualStyles = false;
        CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        Font = new Font("Segoe UI", 9.5f);

        ColumnHeadersDefaultCellStyle.BackColor = DreamineTheme.NavBackground;
        ColumnHeadersDefaultCellStyle.ForeColor = DreamineTheme.TextPrimary;
        ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        ColumnHeadersDefaultCellStyle.SelectionBackColor = DreamineTheme.NavBackground;

        DefaultCellStyle.BackColor = DreamineTheme.InputBackground;
        DefaultCellStyle.ForeColor = DreamineTheme.TextPrimary;
        DefaultCellStyle.SelectionBackColor = DreamineTheme.AccentBlue;
        DefaultCellStyle.SelectionForeColor = Color.White;

        AlternatingRowsDefaultCellStyle.BackColor = DreamineTheme.CardBackground;
        AlternatingRowsDefaultCellStyle.ForeColor = DreamineTheme.TextPrimary;
        AlternatingRowsDefaultCellStyle.SelectionBackColor = DreamineTheme.AccentBlue;
        AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

        CellClick += DreamineDataGrid_CellClick;
    }

    /// <summary>
    /// \if KO
    /// <para>선택된 행을 다시 클릭하면 구성에 따라 현재 선택을 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Clears the current selection when the selected row is clicked again and the option is enabled.</para>
    /// \endif
    /// </summary>
    /// <param name="sender">
    /// \if KO
    /// <para>이벤트를 발생시킨 데이터 그리드입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The data grid that raised the event.</para>
    /// \endif
    /// </param>
    /// <param name="e">
    /// \if KO
    /// <para>클릭된 셀의 행 및 열 정보입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The row and column information of the clicked cell.</para>
    /// \endif
    /// </param>
    private void DreamineDataGrid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (!EnableClickToDeselect || e.RowIndex < 0)
            return;

        if (_lastClickedRow == e.RowIndex)
        {
            ClearSelection();
            CurrentCell = null;
            _lastClickedRow = -1;
        }
        else
        {
            _lastClickedRow = e.RowIndex;
        }
    }
}
