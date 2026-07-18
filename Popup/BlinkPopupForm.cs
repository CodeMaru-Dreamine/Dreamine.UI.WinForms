using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dreamine.UI.WinForms.Controls;

namespace Dreamine.UI.WinForms.Popup;

/// <summary>
/// \if KO
/// <para>배경 깜빡임과 선택적 확인 및 취소 버튼을 렌더링하는 내부 알림 폼입니다.</para>
/// \endif
/// \if EN
/// <para>Provides the internal notification form that renders a blinking background and optional OK and Cancel buttons.</para>
/// \endif
/// </summary>
internal sealed class BlinkPopupForm : Form
{
    /// <summary>
    /// \if KO
    /// <para>options 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the options value.</para>
    /// \endif
    /// </summary>
    private readonly BlinkPopupOptions _options;
    /// <summary>
    /// \if KO
    /// <para>blink Timer 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the blink timer value.</para>
    /// \endif
    /// </summary>
    private readonly System.Windows.Forms.Timer? _blinkTimer;
    /// <summary>
    /// \if KO
    /// <para>blink Phase 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the blink phase value.</para>
    /// \endif
    /// </summary>
    private bool _blinkPhase;
    /// <summary>
    /// \if KO
    /// <para>blink Buttons 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the blink buttons value.</para>
    /// \endif
    /// </summary>
    private readonly List<DreamineButton> _blinkButtons = new();

    /// <summary>
    /// \if KO
    /// <para>사용자가 선택한 대화 상자 결과를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the dialog result selected by the user.</para>
    /// \endif
    /// </summary>
    public DialogResult Result { get; private set; } = DialogResult.Cancel;

    /// <summary>
    /// \if KO
    /// <para>지정한 옵션으로 폼 콘텐츠, 버튼 및 선택적 깜빡임 타이머를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes form content, buttons, and the optional blink timer using the specified options.</para>
    /// \endif
    /// </summary>
    /// <param name="options">
    /// \if KO
    /// <para>팝업 콘텐츠와 표시 옵션입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The popup content and display options.</para>
    /// \endif
    /// </param>
    /// <exception cref="NullReferenceException">
    /// \if KO
    /// <para><paramref name="options"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="options"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
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

    /// <summary>
    /// \if KO
    /// <para>폼 가장자리에 반투명 테두리를 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws a translucent border around the form edge.</para>
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
        // Form 자체를 Region으로 둥글게 잘라내면 GDI Region 특성상 안티앨리어싱이 없어
        // 오히려 거칠게 보인다(데스크톱이 뒤로 보여야 해서 버튼처럼 "부모색 채우기"로
        // 대체할 수도 없음). 그래서 Form은 사각형을 유지하고, 차분한 반투명 흰색 테두리만
        // 그려서 깔끔하게 마무리한다.
        using var pen = new Pen(Color.FromArgb(90, 255, 255, 255), 1.5f);
        e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
    }

    /// <summary>
    /// \if KO
    /// <para>관리되는 깜빡임 타이머를 해제한 뒤 기본 폼 리소스를 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Disposes the managed blink timer before releasing base-form resources.</para>
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
            _blinkTimer?.Dispose();
        base.Dispose(disposing);
    }
}
