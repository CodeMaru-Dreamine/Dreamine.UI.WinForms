using System.Drawing;
using System.Windows.Forms;
using Dreamine.UI.WinForms.Controls;

namespace Dreamine.UI.WinForms.VirtualKeyboard;

/// <summary>
/// DreamineVirtualKeyboardAssist가 띄우는 WinForms 화면 키보드.
/// </summary>
internal sealed class DreamineVirtualKeyboardForm : Form
{
    private static readonly Color KeyBackground = Color.FromArgb(0xF1, 0x68, 0x5E);
    private static readonly Color KeySelectedBackground = Color.FromArgb(0xF5, 0x95, 0x84);
    private const int KeyHeight = 54;
    private const int KeyMargin = 2;
    private const int RowHeight = 58;
    private const int TextKeyboardWidth = 928;

    private readonly DreamineTextBox _target;
    private readonly VkLayout _layout;
    private readonly HangulComposer _hangulComposer = new();
    private readonly List<KeyButton> _keyButtons = [];
    private readonly System.Windows.Forms.Timer _stateTimer;

    private bool _shift;
    private bool _physicalShift;
    private bool _korean;
    private DreamineButton? _shiftButton;
    private DreamineButton? _capsButton;
    private DreamineButton? _languageButton;
    private int _contentWidth;
    private int _contentHeight;

    public DreamineVirtualKeyboardForm(DreamineTextBox target, VkLayout layout)
    {
        _target = target;
        _layout = layout;
        _korean = IsKoreanInputMode();

        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        BackColor = Color.Black;
        Padding = new Padding(18, 14, 18, 14);

        Controls.Add(layout == VkLayout.Numeric ? BuildNumericLayout() : BuildTextLayout());

        _stateTimer = new System.Windows.Forms.Timer { Interval = 180 };
        _stateTimer.Tick += (_, _) => SyncPhysicalKeyboardState();
        _stateTimer.Start();

        RefreshKeys();

        var contentWidth = _contentWidth == 0 ? 900 : _contentWidth;
        var contentHeight = _contentHeight == 0 ? (layout == VkLayout.Numeric ? 4 : 5) * RowHeight : _contentHeight;
        ClientSize = new Size(contentWidth + Padding.Horizontal, contentHeight + Padding.Vertical);
    }

    private Control BuildNumericLayout()
    {
        var root = CreateRootPanel();
        AddRow(root, [KeySpec.Text("1"), KeySpec.Text("2"), KeySpec.Text("3"), KeySpec.Command("Backspace", 104, Backspace)]);
        AddRow(root, [KeySpec.Text("4"), KeySpec.Text("5"), KeySpec.Text("6"), KeySpec.Command("Enter", 104, Close)]);
        AddRow(root, [KeySpec.Text("7"), KeySpec.Text("8"), KeySpec.Text("9"), KeySpec.Text(".")]);
        AddRow(root, [KeySpec.Text("0", 148), KeySpec.Command("Clear", 148, Clear)]);
        return root;
    }

    private Control BuildTextLayout()
    {
        var root = CreateRootPanel();

        AddRow(root,
        [
            KeySpec.Command("Esc", 54, Close),
            KeySpec.Text("`"), KeySpec.Text("1"), KeySpec.Text("2"), KeySpec.Text("3"), KeySpec.Text("4"),
            KeySpec.Text("5"), KeySpec.Text("6"), KeySpec.Text("7"), KeySpec.Text("8"), KeySpec.Text("9"),
            KeySpec.Text("0"), KeySpec.Text("-"), KeySpec.Text("="),
            KeySpec.Command("Backspace", 116, Backspace)
        ], TextKeyboardWidth, 14);

        AddRow(root,
        [
            KeySpec.Command("Tab", 100, () => InsertRaw("    ")),
            KeySpec.Text("q"), KeySpec.Text("w"), KeySpec.Text("e"), KeySpec.Text("r"), KeySpec.Text("t"),
            KeySpec.Text("y"), KeySpec.Text("u"), KeySpec.Text("i"), KeySpec.Text("o"), KeySpec.Text("p"),
            KeySpec.Text("["), KeySpec.Text("]"), KeySpec.Text("\\", 100)
        ], TextKeyboardWidth, 13);

        AddRow(root,
        [
            KeySpec.Command("Caps Lock", 116, ToggleCapsLock),
            KeySpec.Text("a"), KeySpec.Text("s"), KeySpec.Text("d"), KeySpec.Text("f"), KeySpec.Text("g"),
            KeySpec.Text("h"), KeySpec.Text("j"), KeySpec.Text("k"), KeySpec.Text("l"),
            KeySpec.Text(";"), KeySpec.Text("'"),
            KeySpec.Command("Enter", 116, Close)
        ], TextKeyboardWidth, 0, 12);

        AddRow(root,
        [
            KeySpec.Command("Shift", 138, ToggleShift),
            KeySpec.Text("z"), KeySpec.Text("x"), KeySpec.Text("c"), KeySpec.Text("v"), KeySpec.Text("b"),
            KeySpec.Text("n"), KeySpec.Text("m"), KeySpec.Text(","), KeySpec.Text("."), KeySpec.Text("/"),
            KeySpec.Command("◀", 58, MoveLeft),
            KeySpec.Command("▶", 58, MoveRight)
        ], TextKeyboardWidth, 0, 11, 12);

        AddRow(root,
        [
            KeySpec.Command("Ctrl", 124, () => { }),
            KeySpec.Command("Space", 696, () => InsertRaw(" ")),
            KeySpec.Command("abc", 72, ToggleLanguage)
        ], TextKeyboardWidth, 1);

        return root;
    }

    private FlowLayoutPanel CreateRootPanel()
    {
        return new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Color.Black,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            AutoScroll = false,
        };
    }

    private void AddRow(FlowLayoutPanel root, IReadOnlyList<KeySpec> specs, int? targetWidth = null, params int[] expandIndexes)
    {
        var arrangedSpecs = specs.ToArray();
        var rowWidth = arrangedSpecs.Sum(spec => spec.Width + (KeyMargin * 2));
        if (targetWidth is { } width && expandIndexes.Length > 0 && rowWidth < width)
        {
            var validIndexes = expandIndexes
                .Where(index => index >= 0 && index < arrangedSpecs.Length)
                .Distinct()
                .ToArray();

            if (validIndexes.Length > 0)
            {
                var extra = width - rowWidth;
                var share = extra / validIndexes.Length;
                var remainder = extra % validIndexes.Length;

                for (var i = 0; i < validIndexes.Length; i++)
                {
                    var index = validIndexes[i];
                    var spec = arrangedSpecs[index];
                    arrangedSpecs[index] = spec with { Width = spec.Width + share + (i == validIndexes.Length - 1 ? remainder : 0) };
                }
            }

            rowWidth = width;
        }

        var row = new FlowLayoutPanel
        {
            Width = rowWidth,
            Height = RowHeight,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Black,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
        };

        foreach (var spec in arrangedSpecs)
        {
            var button = MakeKey(spec);
            row.Controls.Add(button);
        }

        root.Controls.Add(row);
        _contentWidth = Math.Max(_contentWidth, rowWidth);
        _contentHeight += RowHeight;
    }

    private DreamineButton MakeKey(KeySpec spec)
    {
        var button = new DreamineButton
        {
            Content = spec.Label,
            Width = spec.Width,
            Height = KeyHeight,
            CornerRadius = 4,
            Margin = new Padding(KeyMargin),
            BackColor = KeyBackground,
            ForeColor = Color.White,
            BorderColor = KeyBackground,
            Font = new Font("Segoe UI", spec.Width > 90 ? 12f : 14f, FontStyle.Regular, GraphicsUnit.Point)
        };

        if (spec.Kind == KeyKind.Text)
            _keyButtons.Add(new KeyButton(button, spec.Label));

        if (spec.Label == "Shift")
            _shiftButton = button;
        else if (spec.Label == "Caps Lock")
            _capsButton = button;
        else if (spec.Label is "abc" or "가")
            _languageButton = button;

        button.MouseUp += (_, e) =>
        {
            if (e.Button != MouseButtons.Left || !button.ClientRectangle.Contains(e.Location))
                return;

            if (_target.IsDisposed)
            {
                Close();
                return;
            }

            _target.Focus();
            if (spec.Kind == KeyKind.Text)
                InsertKey(spec.Label);
            else
                spec.Action?.Invoke();
        };

        return button;
    }

    private void InsertKey(string key)
    {
        var text = GetKeyText(key);
        if (_korean && HangulComposer.IsComposableJamo(text))
        {
            var edit = _hangulComposer.Input(text, _target.GetTextBeforeCaret());
            _target.ReplaceTextTail(edit.ReplaceCount, edit.Text);
        }
        else
        {
            _hangulComposer.Reset();
            _target.InsertText(text);
        }

        if (_shift)
        {
            _shift = false;
            RefreshKeys();
        }
    }

    private void InsertRaw(string text)
    {
        _hangulComposer.Reset();
        _target.InsertText(text);
    }

    private void Backspace()
    {
        _hangulComposer.Reset();
        _target.Backspace();
    }

    private void Clear()
    {
        _hangulComposer.Reset();
        _target.Text = string.Empty;
    }

    private void ToggleShift()
    {
        _shift = !_shift;
        RefreshKeys();
    }

    private void ToggleCapsLock()
    {
        _hangulComposer.Reset();
        SendKeys.SendWait("{CAPSLOCK}");
        RefreshKeys();
    }

    private void ToggleLanguage()
    {
        _hangulComposer.Reset();
        _korean = !_korean;
        ApplyInputLanguage(_korean);
        ImeHelper.SetNativeMode(_target.TextBoxHandle, _korean);
        RefreshKeys();
    }

    private void MoveLeft()
    {
        _target.SelectionStart = Math.Max(0, _target.SelectionStart - 1);
    }

    private void MoveRight()
    {
        _target.SelectionStart = Math.Min(_target.Text.Length, _target.SelectionStart + 1);
    }

    private void SyncPhysicalKeyboardState()
    {
        if (IsDisposed)
            return;

        var caps = Control.IsKeyLocked(Keys.CapsLock);
        _physicalShift = (ModifierKeys & Keys.Shift) == Keys.Shift;
        _korean = IsKoreanInputMode();
        if (_capsButton != null)
            _capsButton.IsSelected = caps;

        RefreshKeys();
    }

    private void RefreshKeys()
    {
        var caps = Control.IsKeyLocked(Keys.CapsLock);

        foreach (var key in _keyButtons)
            key.Button.Content = GetKeyText(key.BaseText);

        if (_shiftButton != null)
        {
            _shiftButton.IsSelected = EffectiveShift;
            _shiftButton.BackColor = EffectiveShift ? KeySelectedBackground : KeyBackground;
        }

        if (_capsButton != null)
        {
            _capsButton.IsSelected = caps;
            _capsButton.BackColor = caps ? KeySelectedBackground : KeyBackground;
        }

        if (_languageButton != null)
        {
            _languageButton.Content = _korean ? "가" : "abc";
            _languageButton.IsSelected = _korean;
            _languageButton.BackColor = _korean ? KeySelectedBackground : KeyBackground;
        }
    }

    private string GetKeyText(string key)
    {
        if (_korean && KoreanKeys.TryGetValue(key, out var korean))
            return EffectiveShift && KoreanShiftKeys.TryGetValue(key, out var koreanShift) ? koreanShift : korean;

        if (ShiftKeys.TryGetValue(key, out var shifted) && EffectiveShift)
            return shifted;

        if (key.Length == 1 && char.IsLetter(key[0]))
        {
            var upper = EffectiveShift ^ Control.IsKeyLocked(Keys.CapsLock);
            return upper ? key.ToUpperInvariant() : key.ToLowerInvariant();
        }

        return key;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _stateTimer.Stop();
        _stateTimer.Dispose();
        base.OnFormClosed(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var pen = new Pen(Color.Gray, 4f);
        e.Graphics.DrawRectangle(pen, 1, 1, Width - 3, Height - 3);
    }

    protected override bool ShowWithoutActivation => true;

    private bool EffectiveShift => _shift || _physicalShift;

    private bool IsKoreanInputMode()
    {
        return InputLanguage.CurrentInputLanguage.Culture.Name.Equals("ko-KR", StringComparison.OrdinalIgnoreCase) &&
               ImeHelper.IsNativeMode(_target.TextBoxHandle);
    }

    private static void ApplyInputLanguage(bool korean)
    {
        var cultureName = korean ? "ko-KR" : "en-US";
        foreach (InputLanguage language in InputLanguage.InstalledInputLanguages)
        {
            if (language.Culture.Name.Equals(cultureName, StringComparison.OrdinalIgnoreCase))
            {
                InputLanguage.CurrentInputLanguage = language;
                return;
            }
        }
    }

    private static readonly Dictionary<string, string> ShiftKeys = new()
    {
        ["`"] = "~", ["1"] = "!", ["2"] = "@", ["3"] = "#", ["4"] = "$",
        ["5"] = "%", ["6"] = "^", ["7"] = "&", ["8"] = "*", ["9"] = "(",
        ["0"] = ")", ["-"] = "_", ["="] = "+", ["["] = "{", ["]"] = "}",
        ["\\"] = "|", [";"] = ":", ["'"] = "\"", [","] = "<", ["."] = ">",
        ["/"] = "?",
    };

    private static readonly Dictionary<string, string> KoreanKeys = new()
    {
        ["q"] = "ㅂ", ["w"] = "ㅈ", ["e"] = "ㄷ", ["r"] = "ㄱ", ["t"] = "ㅅ",
        ["y"] = "ㅛ", ["u"] = "ㅕ", ["i"] = "ㅑ", ["o"] = "ㅐ", ["p"] = "ㅔ",
        ["a"] = "ㅁ", ["s"] = "ㄴ", ["d"] = "ㅇ", ["f"] = "ㄹ", ["g"] = "ㅎ",
        ["h"] = "ㅗ", ["j"] = "ㅓ", ["k"] = "ㅏ", ["l"] = "ㅣ",
        ["z"] = "ㅋ", ["x"] = "ㅌ", ["c"] = "ㅊ", ["v"] = "ㅍ", ["b"] = "ㅠ",
        ["n"] = "ㅜ", ["m"] = "ㅡ",
    };

    private static readonly Dictionary<string, string> KoreanShiftKeys = new()
    {
        ["q"] = "ㅃ", ["w"] = "ㅉ", ["e"] = "ㄸ", ["r"] = "ㄲ", ["t"] = "ㅆ",
        ["o"] = "ㅒ", ["p"] = "ㅖ",
    };

    private sealed record KeyButton(DreamineButton Button, string BaseText);
    private sealed record KeySpec(KeyKind Kind, string Label, int Width, Action? Action)
    {
        public static KeySpec Text(string label, int width = 54) => new(KeyKind.Text, label, width, null);
        public static KeySpec Command(string label, int width, Action action) => new(KeyKind.Command, label, width, action);
    }

    private enum KeyKind
    {
        Text,
        Command
    }
}
