using System.Drawing;
using System.Windows.Forms;
using Dreamine.UI.WinForms.Controls;

namespace Dreamine.UI.WinForms.MessageBox;

/// <summary>
/// DreamineMessageBox가 내부적으로 사용하는 다크테마 커스텀 메시지박스 폼.
/// 표준 타이틀바 없이 직접 그린 헤더(드래그 가능) + 메시지 + 버튼으로 구성된다.
/// </summary>
internal sealed class DreamineMessageBoxForm : Form
{
    private readonly Label _messageLabel;
    private readonly FlowLayoutPanel _buttonPanel;
    private readonly System.Windows.Forms.Timer? _autoClickTimer;
    private readonly System.Windows.Forms.Timer? _enableDelayTimer;
    private readonly DialogResult _autoClick;
    private int _autoClickRemainingSeconds;
    private int _enableDelayRemainingSeconds;
    private Label? _countdownLabel;

    public DialogResult Result { get; private set; } = DialogResult.None;

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

    private void SetButtonsEnabled(bool enabled)
    {
        foreach (Control c in _buttonPanel.Controls)
            if (c is DreamineButton btn) btn.Enabled = enabled;
    }

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

    private void EnableDelayTimer_Tick(object? sender, EventArgs e)
    {
        _enableDelayRemainingSeconds--;
        if (_enableDelayRemainingSeconds <= 0)
        {
            _enableDelayTimer?.Stop();
            SetButtonsEnabled(true);
        }
    }

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
    private const int WM_NCLBUTTONDOWN = 0xA1;
    private const int HT_CAPTION = 0x2;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ReleaseCapture();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    private void Header_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        ReleaseCapture();
        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        // BlinkPopupForm과 동일한 이유로 Form 자체는 사각형 모서리를 유지한다
        // (Region 클리핑은 안티앨리어싱이 없어 더 거칠게 보임).
        using var pen = new Pen(DreamineTheme.BorderNormal, 1.5f);
        e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
    }

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
