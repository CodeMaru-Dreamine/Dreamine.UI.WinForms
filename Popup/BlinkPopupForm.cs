using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dreamine.UI.WinForms.Controls;

namespace Dreamine.UI.WinForms.Popup;

/// <summary>DreamineBlinkPopup이 내부적으로 사용하는, 배경이 깜빡이는 알림 팝업 폼.</summary>
internal sealed class BlinkPopupForm : Form
{
    private readonly BlinkPopupOptions _options;
    private readonly System.Windows.Forms.Timer? _blinkTimer;
    private bool _blinkPhase;
    private readonly List<DreamineButton> _blinkButtons = new();

    public DialogResult Result { get; private set; } = DialogResult.Cancel;

    public BlinkPopupForm(BlinkPopupOptions options)
    {
        _options = options;

        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(480, 260);
        ShowInTaskbar = false;
        TopMost = true;
        BackColor = options.Color1;
        // 자식 컨트롤이 Form 가장자리까지 꽉 채우면 WS_CLIPCHILDREN 때문에 그 부분의
        // 테두리가 안 그려진다(자식이 덮은 영역은 Form의 OnPaint가 그릴 수 없음).
        // Padding으로 살짝 여백을 둬서 가장자리 전체에 테두리가 끊김 없이 그려지게 한다.
        Padding = new Padding(2);

        var titleLabel = new Label
        {
            Text = options.Title ?? string.Empty,
            ForeColor = options.ForegroundColor,
            Font = new Font("Segoe UI", 20f, FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = 64,
            TextAlign = ContentAlignment.MiddleCenter
        };

        var messageLabel = new Label
        {
            Text = options.Message ?? string.Empty,
            ForeColor = options.ForegroundColor,
            Font = new Font("Segoe UI", 12f),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 64,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 0, 16, 16),
            BackColor = Color.Transparent
        };

        if (!string.IsNullOrEmpty(options.OkText))
        {
            var okButton = new DreamineButton { Content = options.OkText, Width = 110, Height = 36, CornerRadius = 6, Margin = new Padding(8, 12, 0, 0) };
            okButton.MouseUp += (_, e) =>
            {
                if (e.Button == MouseButtons.Left && okButton.ClientRectangle.Contains(e.Location))
                {
                    Result = DialogResult.OK;
                    Close();
                }
            };
            buttonPanel.Controls.Add(okButton);
            _blinkButtons.Add(okButton);
        }

        if (!string.IsNullOrEmpty(options.CancelText))
        {
            var cancelButton = new DreamineButton { Content = options.CancelText, Width = 110, Height = 36, CornerRadius = 6, Margin = new Padding(8, 12, 0, 0) };
            cancelButton.MouseUp += (_, e) =>
            {
                if (e.Button == MouseButtons.Left && cancelButton.ClientRectangle.Contains(e.Location))
                {
                    Result = DialogResult.Cancel;
                    Close();
                }
            };
            buttonPanel.Controls.Add(cancelButton);
            _blinkButtons.Add(cancelButton);
        }

        Controls.Add(buttonPanel);
        Controls.Add(messageLabel);
        Controls.Add(titleLabel);

        if (options.UseBlink)
        {
            var buttonBaseColor = DreamineTheme.NavBackground;

            _blinkTimer = new System.Windows.Forms.Timer { Interval = options.BlinkIntervalMs };
            _blinkTimer.Tick += (_, _) =>
            {
                _blinkPhase = !_blinkPhase;
                var phaseColor = _blinkPhase ? options.Color2 : options.Color1;
                BackColor = phaseColor;

                // 버튼도 같은 박자로 살짝 색이 묻어나게 해서 배경 점멸과 시각적으로 맞물리게 한다.
                var tinted = DreamineDrawHelper.Blend(buttonBaseColor, phaseColor, 0.35f);
                foreach (var btn in _blinkButtons)
                    btn.BackColor = tinted;
            };
            _blinkTimer.Start();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        // Form 자체를 Region으로 둥글게 잘라내면 GDI Region 특성상 안티앨리어싱이 없어
        // 오히려 거칠게 보인다(데스크톱이 뒤로 보여야 해서 버튼처럼 "부모색 채우기"로
        // 대체할 수도 없음). 그래서 Form은 사각형을 유지하고, 차분한 반투명 흰색 테두리만
        // 그려서 깔끔하게 마무리한다.
        using var pen = new Pen(Color.FromArgb(90, 255, 255, 255), 1.5f);
        e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _blinkTimer?.Dispose();
        base.Dispose(disposing);
    }
}
