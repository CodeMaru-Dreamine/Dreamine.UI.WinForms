using System.Drawing;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// Dreamine 커스텀 DataGridView. 다크 테마(헤더/행/선택색)가 기본 적용된다.
/// WPF DreamineDataGrid + DataGridBehaviors.EnableClickToDeselect와 유사한 데모 목적의 API.
/// </summary>
public class DreamineDataGrid : DataGridView
{
    private int _lastClickedRow = -1;

    /// <summary>이미 선택된 행을 다시 클릭하면 선택을 해제할지 여부
    /// (WPF DataGridBehaviors.EnableClickToDeselect와 동일한 데모 목적).</summary>
    public bool EnableClickToDeselect { get; set; }

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
