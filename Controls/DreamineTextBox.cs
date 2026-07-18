using System.Drawing;
using System.Runtime.InteropServices;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// \if KO
/// <para>힌트, 읽기 전용 상태, 선택 범위 편집 및 포커스 테두리를 제공하는 WinForms 텍스트 입력 래퍼입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a WinForms text-input wrapper with hints, read-only state, selection editing, and a focus border.</para>
/// \endif
/// </summary>
public class DreamineTextBox : UserControl
{
    /// <summary>
    /// \if KO
    /// <para>EM SETCUEBANNER 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the em setcuebanner value.</para>
    /// \endif
    /// </summary>
    private const int EM_SETCUEBANNER = 0x1501;
    /// <summary>
    /// \if KO
    /// <para>Win32 메시지를 내부 텍스트 상자에 보내 힌트 배너를 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Sends a Win32 message to the inner text box to configure its cue banner.</para>
    /// \endif
    /// </summary>
    /// <param name="hWnd">
    /// \if KO
    /// <para>대상 창 핸들입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The target window handle.</para>
    /// \endif
    /// </param>
    /// <param name="msg">
    /// \if KO
    /// <para>전송할 Win32 메시지 식별자입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The Win32 message identifier to send.</para>
    /// \endif
    /// </param>
    /// <param name="wParam">
    /// \if KO
    /// <para>메시지의 정수 포인터 매개변수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message's pointer-sized integer parameter.</para>
    /// \endif
    /// </param>
    /// <param name="lParam">
    /// \if KO
    /// <para>메시지에 전달할 문자열입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The string passed with the message.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>메시지 처리 결과입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message-processing result.</para>
    /// \endif
    /// </returns>
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

    /// <summary>
    /// \if KO
    /// <para>inner 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the inner value.</para>
    /// \endif
    /// </summary>
    private readonly TextBox _inner;
    /// <summary>
    /// \if KO
    /// <para>is Focused 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the is focused value.</para>
    /// \endif
    /// </summary>
    private bool _isFocused;

    // ── Properties ────────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>내부 텍스트 상자의 현재 텍스트를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the current text of the inner text box.</para>
    /// \endif
    /// </summary>
    public new string Text
    {
        get => _inner.Text;
        set => _inner.Text = value;
    }

    /// <summary>
    /// \if KO
    /// <para>hint 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the hint value.</para>
    /// \endif
    /// </summary>
    private string _hint = string.Empty;
    /// <summary>
    /// \if KO
    /// <para>입력 전 표시할 네이티브 힌트 배너를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the native cue banner displayed before input.</para>
    /// \endif
    /// </summary>
    public string Hint
    {
        get => _hint;
        set
        {
            _hint = value;
            if (_inner.IsHandleCreated)
                SendMessage(_inner.Handle, EM_SETCUEBANNER, (IntPtr)1, _hint);
        }
    }

    /// <summary>
    /// \if KO
    /// <para>사용자가 텍스트를 편집할 수 없는지 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the user is prevented from editing the text.</para>
    /// \endif
    /// </summary>
    public bool IsReadOnly
    {
        get => _inner.ReadOnly;
        set { _inner.ReadOnly = value; Invalidate(); }
    }

    /// <summary>
    /// \if KO
    /// <para>선택 범위 또는 캐럿의 시작 인덱스를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the starting index of the selection or caret.</para>
    /// \endif
    /// </summary>
    public int SelectionStart
    {
        get => _inner.SelectionStart;
        set => _inner.SelectionStart = Math.Clamp(value, 0, _inner.TextLength);
    }

    /// <summary>
    /// \if KO
    /// <para>선택된 문자 수를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the number of selected characters.</para>
    /// \endif
    /// </summary>
    public int SelectionLength
    {
        get => _inner.SelectionLength;
        set => _inner.SelectionLength = Math.Clamp(value, 0, _inner.TextLength - _inner.SelectionStart);
    }

    /// <summary>
    /// \if KO
    /// <para>내부 네이티브 텍스트 상자 창 핸들을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the native window handle of the inner text box.</para>
    /// \endif
    /// </summary>
    public IntPtr TextBoxHandle => _inner.Handle;

    /// <summary>
    /// \if KO
    /// <para>래퍼와 내부 텍스트 상자의 전경색을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the foreground color of both the wrapper and inner text box.</para>
    /// \endif
    /// </summary>
    public override Color ForeColor
    {
        get => base.ForeColor;
        set { base.ForeColor = value; if (_inner != null) _inner.ForeColor = value; }
    }

    /// <summary>
    /// \if KO
    /// <para>래퍼와 내부 텍스트 상자의 글꼴을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the font of both the wrapper and inner text box.</para>
    /// \endif
    /// </summary>
    public new Font Font
    {
        get => base.Font;
        set { base.Font = value; if (_inner != null) _inner.Font = value; }
    }

    /// <summary>
    /// \if KO
    /// <para>내부 텍스트가 변경될 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Occurs when the inner text changes.</para>
    /// \endif
    /// </summary>
    public new event EventHandler? TextChanged;

    /// <summary>
    /// \if KO
    /// <para>읽기 전용이 아니면 현재 선택 위치에 텍스트를 삽입하고 입력 포커스를 복원합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Inserts text at the current selection and restores input focus when the control is not read-only.</para>
    /// \endif
    /// </summary>
    /// <param name="text">
    /// \if KO
    /// <para>삽입할 텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The text to insert.</para>
    /// \endif
    /// </param>
    public void InsertText(string text)
    {
        if (_inner.ReadOnly)
            return;

        _inner.SelectedText = text;
        _inner.Focus();
    }

    /// <summary>
    /// \if KO
    /// <para>선택 영역을 교체하거나 캐럿 앞의 지정 문자 수를 새 텍스트로 교체합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Replaces the selection or a specified number of characters before the caret with new text.</para>
    /// \endif
    /// </summary>
    /// <param name="replaceCount">
    /// \if KO
    /// <para>선택 영역이 없을 때 캐럿 앞에서 교체할 문자 수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The number of characters to replace before the caret when there is no selection.</para>
    /// \endif
    /// </param>
    /// <param name="text">
    /// \if KO
    /// <para>교체 위치에 삽입할 텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The text to insert at the replacement position.</para>
    /// \endif
    /// </param>
    public void ReplaceTextTail(int replaceCount, string text)
    {
        if (_inner.ReadOnly)
            return;

        var currentText = _inner.Text ?? string.Empty;
        var selectionStart = Math.Clamp(_inner.SelectionStart, 0, currentText.Length);
        var selectionLength = Math.Clamp(_inner.SelectionLength, 0, currentText.Length - selectionStart);

        if (selectionLength > 0)
        {
            _inner.SelectedText = text;
        }
        else
        {
            var removeStart = Math.Max(0, selectionStart - replaceCount);
            removeStart = Math.Min(removeStart, currentText.Length);
            var removeLength = Math.Clamp(selectionStart - removeStart, 0, currentText.Length - removeStart);
            _inner.Text = currentText.Remove(removeStart, removeLength).Insert(removeStart, text);
            _inner.SelectionStart = removeStart + text.Length;
        }

        _inner.Focus();
    }

    /// <summary>
    /// \if KO
    /// <para>선택 영역 또는 캐럿 앞의 문자 하나를 삭제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Deletes the selection or one character before the caret.</para>
    /// \endif
    /// </summary>
    public void Backspace()
    {
        if (_inner.ReadOnly)
            return;

        var currentText = _inner.Text ?? string.Empty;
        var selectionStart = Math.Clamp(_inner.SelectionStart, 0, currentText.Length);
        var selectionLength = Math.Clamp(_inner.SelectionLength, 0, currentText.Length - selectionStart);

        if (selectionLength > 0)
        {
            _inner.SelectedText = string.Empty;
        }
        else if (selectionStart > 0)
        {
            _inner.Text = currentText.Remove(selectionStart - 1, 1);
            _inner.SelectionStart = selectionStart - 1;
        }

        _inner.Focus();
    }

    /// <summary>
    /// \if KO
    /// <para>내부 텍스트 상자에서 캐럿 앞의 텍스트를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the text before the caret from the inner text box.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>캐럿 앞의 텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The text before the caret.</para>
    /// \endif
    /// </returns>
    public string GetTextBeforeCaret()
    {
        var currentText = _inner.Text ?? string.Empty;
        var caret = Math.Clamp(_inner.SelectionStart, 0, currentText.Length);
        return currentText[..caret];
    }

    // ── Constructor ───────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>내부 텍스트 상자, 힌트 처리, 이벤트 전달 및 Dreamine 테마 기본값을 구성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures the inner text box, hint handling, event forwarding, and Dreamine theme defaults.</para>
    /// \endif
    /// </summary>
    public DreamineTextBox()
    {
        // _inner must be created first — SetStyle and property setters below
        // can trigger OnLayout / ForeColor / Font overrides before the field is set.
        _inner = new TextBox
        {
            BorderStyle = BorderStyle.None,
            BackColor   = DreamineTheme.InputBackground,
            ForeColor   = DreamineTheme.TextPrimary,
            Font        = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point),
            Dock        = DockStyle.Fill,
            Margin      = Padding.Empty,
        };

        _inner.GotFocus  += (_, _) => { _isFocused = true;  Invalidate(); };
        _inner.LostFocus += (_, _) => { _isFocused = false; Invalidate(); };
        _inner.TextChanged += (s, e) => TextChanged?.Invoke(this, e);
        _inner.HandleCreated += (_, _) =>
        {
            if (!string.IsNullOrEmpty(_hint))
                SendMessage(_inner.Handle, EM_SETCUEBANNER, (IntPtr)1, _hint);
        };

        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer |
            ControlStyles.ResizeRedraw, true);

        BackColor = DreamineTheme.InputBackground;
        ForeColor = DreamineTheme.TextPrimary;
        Font      = _inner.Font;
        Height    = 36;
        Padding   = new Padding(2);

        Controls.Add(_inner);
    }

    /// <summary>
    /// \if KO
    /// <para>텍스트 입력 래퍼의 기본 내부 여백을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the default inner padding of the text-input wrapper.</para>
    /// \endif
    /// </summary>
    protected override Padding DefaultPadding => new Padding(6, 0, 6, 0);

    /// <summary>
    /// \if KO
    /// <para>내부 텍스트 상자를 래퍼의 현재 크기에 맞춰 배치합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Positions the inner text box to fit the wrapper's current size.</para>
    /// \endif
    /// </summary>
    /// <param name="e">
    /// \if KO
    /// <para>레이아웃 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The layout event arguments.</para>
    /// \endif
    /// </param>
    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        if (_inner == null) return;
        _inner.SetBounds(6, (Height - _inner.PreferredHeight) / 2,
            Width - 12, _inner.PreferredHeight);
    }

    /// <summary>
    /// \if KO
    /// <para>현재 포커스와 활성 상태에 맞게 둥근 배경과 테두리를 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws the rounded background and border for the current focus and enabled state.</para>
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
        var g    = e.Graphics;
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);

        using var bgBrush = new SolidBrush(BackColor);
        var borderColor = _isFocused ? DreamineTheme.BorderFocus : DreamineTheme.BorderNormal;
        if (!Enabled) borderColor = Color.FromArgb(80, borderColor);
        using var pen = new Pen(borderColor, 1.5f);
        DreamineDrawHelper.FillRoundedRect(g, bgBrush, pen, rect, DreamineTheme.CornerRadiusSmall);
    }

    /// <summary>
    /// \if KO
    /// <para>텍스트 입력 래퍼의 기본 크기를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the default size of the text-input wrapper.</para>
    /// \endif
    /// </summary>
    protected override Size DefaultSize => new(220, 36);
}
