using System.Drawing;
using System.Windows.Forms;
using Dreamine.UI.WinForms.Controls;

namespace Dreamine.UI.WinForms.MessageBox;

/// <summary>
/// \if KO
/// <para>드래그 가능한 사용자 지정 머리글, 메시지, 아이콘, 버튼과 선택적 타이머를 제공하는 내부 다크 테마 폼입니다.</para>
/// \endif
/// \if EN
/// <para>Provides the internal dark-theme form with a draggable custom header, message, icon, buttons, and optional timers.</para>
/// \endif
/// </summary>
internal sealed class DreamineMessageBoxForm : Form
{
    /// <summary>
    /// \if KO
    /// <para>message Label 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the message label value.</para>
    /// \endif
    /// </summary>
    private readonly Label _messageLabel;
    /// <summary>
    /// \if KO
    /// <para>button Panel 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the button panel value.</para>
    /// \endif
    /// </summary>
    private readonly FlowLayoutPanel _buttonPanel;
    /// <summary>
    /// \if KO
    /// <para>auto Click Timer 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the auto click timer value.</para>
    /// \endif
    /// </summary>
    private readonly System.Windows.Forms.Timer? _autoClickTimer;
    /// <summary>
    /// \if KO
    /// <para>enable Delay Timer 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the enable delay timer value.</para>
    /// \endif
    /// </summary>
    private readonly System.Windows.Forms.Timer? _enableDelayTimer;
    /// <summary>
    /// \if KO
    /// <para>auto Click 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the auto click value.</para>
    /// \endif
    /// </summary>
    private readonly DialogResult _autoClick;
    /// <summary>
    /// \if KO
    /// <para>auto Click Remaining Seconds 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the auto click remaining seconds value.</para>
    /// \endif
    /// </summary>
    private int _autoClickRemainingSeconds;
    /// <summary>
    /// \if KO
    /// <para>enable Delay Remaining Seconds 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the enable delay remaining seconds value.</para>
    /// \endif
    /// </summary>
    private int _enableDelayRemainingSeconds;
    /// <summary>
    /// \if KO
    /// <para>countdown Label 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the countdown label value.</para>
    /// \endif
    /// </summary>
    private Label? _countdownLabel;

    /// <summary>
    /// \if KO
    /// <para>사용자가 선택하거나 자동 선택된 최종 결과를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the final result selected by the user or automatic selection.</para>
    /// \endif
    /// </summary>
    public DialogResult Result { get; private set; } = DialogResult.None;

    /// <summary>
    /// \if KO
    /// <para>지정한 콘텐츠, 버튼, 자동 선택 및 버튼 활성화 지연으로 메시지 상자 폼을 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes the message-box form with the specified content, buttons, automatic selection, and button-enable delay.</para>
    /// \endif
    /// </summary>
    /// <param name="title">
    /// \if KO
    /// <para>머리글에 표시할 제목입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The title displayed in the header.</para>
    /// \endif
    /// </param>
    /// <param name="message">
    /// \if KO
    /// <para>본문에 표시할 메시지입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message displayed in the body.</para>
    /// \endif
    /// </param>
    /// <param name="icon">
    /// \if KO
    /// <para>메시지와 함께 표시할 표준 아이콘입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The standard icon displayed with the message.</para>
    /// \endif
    /// </param>
    /// <param name="buttons">
    /// \if KO
    /// <para>만들 표준 버튼 조합입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The standard button combination to create.</para>
    /// \endif
    /// </param>
    /// <param name="autoClick">
    /// \if KO
    /// <para>자동 선택 타이머 만료 시 사용할 결과입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The result used when the automatic-selection timer expires.</para>
    /// \endif
    /// </param>
    /// <param name="autoClickDelaySeconds">
    /// \if KO
    /// <para>자동 선택 전 대기할 초입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The number of seconds before automatic selection.</para>
    /// \endif
    /// </param>
    /// <param name="enableDelaySeconds">
    /// \if KO
    /// <para>버튼을 활성화하기 전 대기할 초입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The number of seconds before enabling the buttons.</para>
    /// \endif
    /// </param>
    public DreamineMessageBoxForm(
        string title,
        string message,
        MessageBoxIcon icon,
        MessageBoxButtons buttons,
        DialogResult autoClick,
        int autoClickDelaySeconds,
        int enableDelaySeconds)
    {
        _autoClick = autoClick;

        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = DreamineTheme.CardBackground;
        Size = new Size(420, 220);
        ShowInTaskbar = false;
        TopMost = true;
        // 자식 컨트롤이 Form 가장자리까지 꽉 채우면 WS_CLIPCHILDREN 때문에 그 부분의
        // 테두리가 안 그려진다. Padding으로 여백을 둬서 테두리가 끊김 없이 그려지게 한다.
        Padding = new Padding(2);

        var header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 36,
            BackColor = DreamineTheme.NavBackground
        };
        header.MouseDown += Header_MouseDown;

        var titleLabel = new Label
        {
            Text = title,
            ForeColor = DreamineTheme.TextPrimary,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(12, 0, 0, 0)
        };
        titleLabel.MouseDown += Header_MouseDown;
        header.Controls.Add(titleLabel);

        var closeButton = new Label
        {
            Text = "✕",
            ForeColor = DreamineTheme.TextSecondary,
            Font = new Font("Segoe UI", 10f),
            Dock = DockStyle.Right,
            Width = 36,
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand
        };
        closeButton.Click += (_, _) => { Result = DialogResult.Cancel; Close(); };
        header.Controls.Add(closeButton);

        var iconBox = new PictureBox
        {
            Dock = DockStyle.Left,
            Width = 56,
            SizeMode = PictureBoxSizeMode.CenterImage,
            Image = GetIconBitmap(icon)
        };

        _messageLabel = new Label
        {
            Text = message,
            ForeColor = DreamineTheme.TextPrimary,
            Font = new Font("Segoe UI", 10f),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(8, 16, 16, 16)
        };

        var messageRow = new Panel { Dock = DockStyle.Fill };
        messageRow.Controls.Add(_messageLabel);
        messageRow.Controls.Add(iconBox);

        _buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 56,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 0, 12, 12)
        };

        BuildButtons(buttons);

        Controls.Add(_buttonPanel);
        Controls.Add(messageRow);
        Controls.Add(header);

        if (autoClickDelaySeconds > 0 && autoClick != DialogResult.None)
        {
            _autoClickRemainingSeconds = autoClickDelaySeconds;
            _countdownLabel = new Label
            {
                Text = $"({_autoClickRemainingSeconds}s)",
                ForeColor = DreamineTheme.TextSecondary,
                Font = new Font("Segoe UI", 8f),
                AutoSize = true,
                Dock = DockStyle.Left,
                Padding = new Padding(12, 0, 0, 0)
            };
            _buttonPanel.Controls.Add(_countdownLabel);

            _autoClickTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _autoClickTimer.Tick += AutoClickTimer_Tick;
            _autoClickTimer.Start();
        }

        if (enableDelaySeconds > 0)
        {
            SetButtonsEnabled(false);
            _enableDelayRemainingSeconds = enableDelaySeconds;
            _enableDelayTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _enableDelayTimer.Tick += EnableDelayTimer_Tick;
            _enableDelayTimer.Start();
        }
    }

    /// <summary>
    /// \if KO
    /// <para>표준 버튼 조합에 대응하는 Dreamine 버튼을 만들고 결과 완료 동작을 연결합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates Dreamine buttons for a standard button combination and attaches result-completion behavior.</para>
    /// \endif
    /// </summary>
    /// <param name="buttons">
    /// \if KO
    /// <para>만들 표준 메시지 상자 버튼 조합입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The standard message-box button combination to create.</para>
    /// \endif
    /// </param>
    private void BuildButtons(MessageBoxButtons buttons)
    {
        var specs = buttons switch
        {
            MessageBoxButtons.OKCancel => new[] { (DialogResult.Cancel, "취소"), (DialogResult.OK, "확인") },
            MessageBoxButtons.YesNo => new[] { (DialogResult.No, "아니오"), (DialogResult.Yes, "예") },
            MessageBoxButtons.YesNoCancel => new[] { (DialogResult.Cancel, "취소"), (DialogResult.No, "아니오"), (DialogResult.Yes, "예") },
            MessageBoxButtons.RetryCancel => new[] { (DialogResult.Cancel, "취소"), (DialogResult.Retry, "재시도") },
            MessageBoxButtons.AbortRetryIgnore => new[] { (DialogResult.Ignore, "무시"), (DialogResult.Retry, "재시도"), (DialogResult.Abort, "중단") },
            _ => new[] { (DialogResult.OK, "확인") }
        };

        foreach (var (result, text) in specs)
        {
            var button = new DreamineButton
            {
                Content = text,
                Width = 96,
                Height = 32,
                CornerRadius = 6,
                Margin = new Padding(6, 12, 0, 0)
            };
            var captured = result;
            button.MouseUp += (_, e) =>
            {
                if (e.Button == MouseButtons.Left && button.ClientRectangle.Contains(e.Location))
                {
                    Result = captured;
                    Close();
                }
            };
            _buttonPanel.Controls.Add(button);
        }
    }

    /// <summary>
    /// \if KO
    /// <para>폼의 모든 Dreamine 버튼 활성 상태를 일괄 변경합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Changes the enabled state of all Dreamine buttons in the form.</para>
    /// \endif
    /// </summary>
    /// <param name="enabled">
    /// \if KO
    /// <para>버튼을 활성화하려면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> to enable the buttons.</para>
    /// \endif
    /// </param>
    private void SetButtonsEnabled(bool enabled)
    {
        foreach (Control c in _buttonPanel.Controls)
            if (c is DreamineButton btn) btn.Enabled = enabled;
    }

    /// <summary>
    /// \if KO
    /// <para>자동 선택 카운트다운을 갱신하고 만료 시 구성된 결과로 폼을 닫습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Updates the automatic-selection countdown and closes the form with the configured result when it expires.</para>
    /// \endif
    /// </summary>
    /// <param name="sender">
    /// \if KO
    /// <para>이벤트를 발생시킨 타이머입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The timer that raised the event.</para>
    /// \endif
    /// </param>
    /// <param name="e">
    /// \if KO
    /// <para>이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The event arguments.</para>
    /// \endif
    /// </param>
    private void AutoClickTimer_Tick(object? sender, EventArgs e)
    {
        _autoClickRemainingSeconds--;
        if (_countdownLabel != null)
            _countdownLabel.Text = $"({_autoClickRemainingSeconds}s)";

        if (_autoClickRemainingSeconds <= 0)
        {
            _autoClickTimer?.Stop();
            Result = _autoClick;
            Close();
        }
    }

    /// <summary>
    /// \if KO
    /// <para>버튼 활성화 지연 카운트다운을 갱신하고 만료 시 버튼을 활성화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Updates the button-enable countdown and enables buttons when it expires.</para>
    /// \endif
    /// </summary>
    /// <param name="sender">
    /// \if KO
    /// <para>이벤트를 발생시킨 타이머입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The timer that raised the event.</para>
    /// \endif
    /// </param>
    /// <param name="e">
    /// \if KO
    /// <para>이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The event arguments.</para>
    /// \endif
    /// </param>
    private void EnableDelayTimer_Tick(object? sender, EventArgs e)
    {
        _enableDelayRemainingSeconds--;
        if (_enableDelayRemainingSeconds <= 0)
        {
            _enableDelayTimer?.Stop();
            SetButtonsEnabled(true);
        }
    }

    /// <summary>
    /// \if KO
    /// <para>표준 메시지 상자 아이콘에 대응하는 비트맵을 만듭니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a bitmap corresponding to a standard message-box icon.</para>
    /// \endif
    /// </summary>
    /// <param name="icon">
    /// \if KO
    /// <para>변환할 표준 메시지 상자 아이콘입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The standard message-box icon to convert.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>새 아이콘 비트맵이거나 아이콘이 없으면 <see langword="null"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A new icon bitmap, or <see langword="null"/> when no icon is requested.</para>
    /// \endif
    /// </returns>
    private static Bitmap? GetIconBitmap(MessageBoxIcon icon)
    {
        Icon? sysIcon = icon switch
        {
            MessageBoxIcon.Information => SystemIcons.Information,
            MessageBoxIcon.Warning => SystemIcons.Warning,
            MessageBoxIcon.Error => SystemIcons.Error,
            MessageBoxIcon.Question => SystemIcons.Question,
            _ => null
        };
        return sysIcon?.ToBitmap();
    }

    // ── Draggable header (Win32 캡션 드래그 트릭) ───────────
    /// <summary>
    /// \if KO
    /// <para>WM NCLBUTTONDOWN 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the wm nclbuttondown value.</para>
    /// \endif
    /// </summary>
    private const int WM_NCLBUTTONDOWN = 0xA1;
    /// <summary>
    /// \if KO
    /// <para>HT CAPTION 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the ht caption value.</para>
    /// \endif
    /// </summary>
    private const int HT_CAPTION = 0x2;

    /// <summary>
    /// \if KO
    /// <para>현재 스레드의 마우스 캡처를 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Releases mouse capture from the current thread.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>호출이 성공하면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the call succeeds.</para>
    /// \endif
    /// </returns>
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    /// <summary>
    /// \if KO
    /// <para>창에 동기 Win32 메시지를 전송합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Sends a synchronous Win32 message to a window.</para>
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
    /// <param name="Msg">
    /// \if KO
    /// <para>메시지 식별자입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The message identifier.</para>
    /// \endif
    /// </param>
    /// <param name="wParam">
    /// \if KO
    /// <para>첫 번째 메시지 매개변수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The first message parameter.</para>
    /// \endif
    /// </param>
    /// <param name="lParam">
    /// \if KO
    /// <para>두 번째 메시지 매개변수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The second message parameter.</para>
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
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    /// <summary>
    /// \if KO
    /// <para>머리글의 왼쪽 마우스 누름을 비클라이언트 캡션 드래그로 변환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Converts a left mouse press on the header into a non-client caption drag.</para>
    /// \endif
    /// </summary>
    /// <param name="sender">
    /// \if KO
    /// <para>이벤트를 발생시킨 머리글 요소입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The header element that raised the event.</para>
    /// \endif
    /// </param>
    /// <param name="e">
    /// \if KO
    /// <para>마우스 버튼 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The mouse-button event arguments.</para>
    /// \endif
    /// </param>
    private void Header_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        ReleaseCapture();
        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
    }

    /// <summary>
    /// \if KO
    /// <para>메시지 상자 폼 가장자리에 Dreamine 테두리를 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws the Dreamine border around the message-box form edge.</para>
    /// \endif
    /// </summary>
    /// <param name="e">
    /// \if KO
    /// <para>폼 그리기 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The form paint event arguments.</para>
    /// \endif
    /// </param>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        // BlinkPopupForm과 동일한 이유로 Form 자체는 사각형 모서리를 유지한다
        // (Region 클리핑은 안티앨리어싱이 없어 더 거칠게 보임).
        using var pen = new Pen(DreamineTheme.BorderNormal, 1.5f);
        e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
    }

    /// <summary>
    /// \if KO
    /// <para>자동 선택 및 활성화 지연 타이머를 해제한 뒤 기본 폼 리소스를 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Disposes automatic-selection and enable-delay timers before releasing base-form resources.</para>
    /// \endif
    /// </summary>
    /// <param name="disposing">
    /// \if KO
    /// <para>관리되는 리소스도 해제하면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> to dispose managed resources as well.</para>
    /// \endif
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _autoClickTimer?.Dispose();
            _enableDelayTimer?.Dispose();
        }
        base.Dispose(disposing);
    }
}
