using System.Drawing;
using System.Runtime.InteropServices;
using Dreamine.UI.WinForms;

namespace Dreamine.UI.WinForms.Controls;

/// <summary>
/// \if KO
/// <para>암호 마스킹, 힌트 텍스트 및 포커스 테두리를 제공하는 WinForms 암호 입력 래퍼입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a WinForms password-input wrapper with masking, hint text, and a focus border.</para>
/// \endif
/// </summary>
public class DreaminePasswordBox : UserControl
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
    /// <para>마스킹된 암호 텍스트를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the masked password text.</para>
    /// \endif
    /// </summary>
    public string Password
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
    /// <para>내부 암호 텍스트가 변경될 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Occurs when the inner password text changes.</para>
    /// \endif
    /// </summary>
    public event EventHandler? PasswordChanged;

    // ── Constructor ───────────────────────────────────────
    /// <summary>
    /// \if KO
    /// <para>내부 암호 상자, 힌트 처리 및 Dreamine 테마 기본값을 구성합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Configures the inner password box, hint handling, and Dreamine theme defaults.</para>
    /// \endif
    /// </summary>
    public DreaminePasswordBox()
    {
        // _inner must be created first — SetStyle and property setters below
        // can trigger OnLayout / ForeColor / Font overrides before the field is set.
        _inner = new TextBox
        {
            BorderStyle          = BorderStyle.None,
            BackColor            = DreamineTheme.InputBackground,
            ForeColor            = DreamineTheme.TextPrimary,
            Font                 = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point),
            UseSystemPasswordChar = true,
        };

        _inner.GotFocus  += (_, _) => { _isFocused = true;  Invalidate(); };
        _inner.LostFocus += (_, _) => { _isFocused = false; Invalidate(); };
        _inner.TextChanged += (s, e) => PasswordChanged?.Invoke(this, e);
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

        Controls.Add(_inner);
    }

    /// <summary>
    /// \if KO
    /// <para>내부 암호 상자를 래퍼의 현재 크기에 맞춰 배치합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Positions the inner password box to fit the wrapper's current size.</para>
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
    /// <para>현재 포커스 상태에 맞게 둥근 배경과 테두리를 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws the rounded background and border for the current focus state.</para>
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
        using var pen = new Pen(borderColor, 1.5f);
        DreamineDrawHelper.FillRoundedRect(g, bgBrush, pen, rect, DreamineTheme.CornerRadiusSmall);
    }

    /// <summary>
    /// \if KO
    /// <para>암호 입력 래퍼의 기본 크기를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the default size of the password-input wrapper.</para>
    /// \endif
    /// </summary>
    protected override Size DefaultSize => new(220, 36);
}
