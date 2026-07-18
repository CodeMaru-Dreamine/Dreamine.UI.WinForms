using System.Drawing;
using System.Windows.Forms;
using Dreamine.UI.WinForms.Controls;

namespace Dreamine.UI.WinForms.VirtualKeyboard;

/// <summary>
/// \if KO
/// <para>연결된 Dreamine 텍스트 상자를 직접 편집하는 WinForms 화면 키보드 폼입니다.</para>
/// \endif
/// \if EN
/// <para>Provides a WinForms on-screen keyboard form that directly edits an attached Dreamine text box.</para>
/// \endif
/// </summary>
internal sealed class DreamineVirtualKeyboardForm : Form
{
    /// <summary>
    /// \if KO
    /// <para>Key Background 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the key background value.</para>
    /// \endif
    /// </summary>
    private static readonly Color KeyBackground = Color.FromArgb(0xF1, 0x68, 0x5E);
    /// <summary>
    /// \if KO
    /// <para>Key Selected Background 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the key selected background value.</para>
    /// \endif
    /// </summary>
    private static readonly Color KeySelectedBackground = Color.FromArgb(0xF5, 0x95, 0x84);
    /// <summary>
    /// \if KO
    /// <para>Key Height 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the key height value.</para>
    /// \endif
    /// </summary>
    private const int KeyHeight = 54;
    /// <summary>
    /// \if KO
    /// <para>Key Margin 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the key margin value.</para>
    /// \endif
    /// </summary>
    private const int KeyMargin = 2;
    /// <summary>
    /// \if KO
    /// <para>Row Height 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the row height value.</para>
    /// \endif
    /// </summary>
    private const int RowHeight = 58;
    /// <summary>
    /// \if KO
    /// <para>Text Keyboard Width 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the text keyboard width value.</para>
    /// \endif
    /// </summary>
    private const int TextKeyboardWidth = 928;

    /// <summary>
    /// \if KO
    /// <para>target 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the target value.</para>
    /// \endif
    /// </summary>
    private readonly DreamineTextBox _target;
    /// <summary>
    /// \if KO
    /// <para>layout 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the layout value.</para>
    /// \endif
    /// </summary>
    private readonly VkLayout _layout;
    /// <summary>
    /// \if KO
    /// <para>hangul Composer 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the hangul composer value.</para>
    /// \endif
    /// </summary>
    private readonly HangulComposer _hangulComposer = new();
    /// <summary>
    /// \if KO
    /// <para>key Buttons 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the key buttons value.</para>
    /// \endif
    /// </summary>
    private readonly List<KeyButton> _keyButtons = [];
    /// <summary>
    /// \if KO
    /// <para>state Timer 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the state timer value.</para>
    /// \endif
    /// </summary>
    private readonly System.Windows.Forms.Timer _stateTimer;

    /// <summary>
    /// \if KO
    /// <para>shift 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the shift value.</para>
    /// \endif
    /// </summary>
    private bool _shift;
    /// <summary>
    /// \if KO
    /// <para>physical Shift 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the physical shift value.</para>
    /// \endif
    /// </summary>
    private bool _physicalShift;
    /// <summary>
    /// \if KO
    /// <para>korean 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the korean value.</para>
    /// \endif
    /// </summary>
    private bool _korean;
    /// <summary>
    /// \if KO
    /// <para>shift Button 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the shift button value.</para>
    /// \endif
    /// </summary>
    private DreamineButton? _shiftButton;
    /// <summary>
    /// \if KO
    /// <para>caps Button 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the caps button value.</para>
    /// \endif
    /// </summary>
    private DreamineButton? _capsButton;
    /// <summary>
    /// \if KO
    /// <para>language Button 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the language button value.</para>
    /// \endif
    /// </summary>
    private DreamineButton? _languageButton;
    /// <summary>
    /// \if KO
    /// <para>content Width 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the content width value.</para>
    /// \endif
    /// </summary>
    private int _contentWidth;
    /// <summary>
    /// \if KO
    /// <para>content Height 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the content height value.</para>
    /// \endif
    /// </summary>
    private int _contentHeight;

    /// <summary>
    /// \if KO
    /// <para>지정한 입력 대상과 레이아웃으로 키보드 UI 및 물리 키 상태 동기화를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes keyboard UI and physical-key-state synchronization for the specified input target and layout.</para>
    /// \endif
    /// </summary>
    /// <param name="target">
    /// \if KO
    /// <para>화면 키보드 입력을 받을 텍스트 상자입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The text box that receives on-screen keyboard input.</para>
    /// \endif
    /// </param>
    /// <param name="layout">
    /// \if KO
    /// <para>만들 키보드 레이아웃입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The keyboard layout to create.</para>
    /// \endif
    /// </param>
    /// <exception cref="NullReferenceException">
    /// \if KO
    /// <para><paramref name="target"/>이 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="target"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
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

    /// <summary>
    /// \if KO
    /// <para>숫자, 소수점, 지우기, 삭제 및 Enter 키로 구성된 숫자 레이아웃을 만듭니다.</para>
    /// \endif
    /// \if EN
    /// <para>Builds the numeric layout containing digits, decimal point, clear, backspace, and Enter keys.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>완성된 숫자 키보드 루트 컨트롤입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The completed numeric-keyboard root control.</para>
    /// \endif
    /// </returns>
    private Control BuildNumericLayout()
    {
        var root = CreateRootPanel();
        AddRow(root, [KeySpec.Text("1"), KeySpec.Text("2"), KeySpec.Text("3"), KeySpec.Command("Backspace", 104, Backspace)]);
        AddRow(root, [KeySpec.Text("4"), KeySpec.Text("5"), KeySpec.Text("6"), KeySpec.Command("Enter", 104, Close)]);
        AddRow(root, [KeySpec.Text("7"), KeySpec.Text("8"), KeySpec.Text("9"), KeySpec.Text(".")]);
        AddRow(root, [KeySpec.Text("0", 148), KeySpec.Command("Clear", 148, Clear)]);
        return root;
    }

    /// <summary>
    /// \if KO
    /// <para>5행 QWERTY 텍스트 및 명령 키 레이아웃을 만듭니다.</para>
    /// \endif
    /// \if EN
    /// <para>Builds the five-row QWERTY layout of text and command keys.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>완성된 텍스트 키보드 루트 컨트롤입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The completed text-keyboard root control.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>키보드 행을 세로로 배치할 루트 흐름 패널을 만듭니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates the root flow panel that vertically arranges keyboard rows.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>구성된 루트 흐름 패널입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The configured root flow panel.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>키 사양 목록을 한 행으로 만들고 선택한 키 너비를 목표 너비까지 균등 확장합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates one row from key specifications and evenly expands selected keys to a target width.</para>
    /// \endif
    /// </summary>
    /// <param name="root">
    /// \if KO
    /// <para>행을 추가할 루트 흐름 패널입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The root flow panel to which the row is added.</para>
    /// \endif
    /// </param>
    /// <param name="specs">
    /// \if KO
    /// <para>행에 배치할 키 사양입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The key specifications to arrange in the row.</para>
    /// \endif
    /// </param>
    /// <param name="targetWidth">
    /// \if KO
    /// <para>선택적 목표 행 너비입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The optional target row width.</para>
    /// \endif
    /// </param>
    /// <param name="expandIndexes">
    /// \if KO
    /// <para>여분 너비를 분배할 키 인덱스입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The key indexes among which extra width is distributed.</para>
    /// \endif
    /// </param>
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

    /// <summary>
    /// \if KO
    /// <para>키 사양에 맞는 Dreamine 버튼과 클릭 동작을 만듭니다.</para>
    /// \endif
    /// \if EN
    /// <para>Creates a Dreamine button and click behavior for a key specification.</para>
    /// \endif
    /// </summary>
    /// <param name="spec">
    /// \if KO
    /// <para>만들 키의 종류, 레이블, 너비 및 동작입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The kind, label, width, and action of the key to create.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>구성된 Dreamine 키 버튼입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The configured Dreamine key button.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>현재 언어와 수정 키 상태에 따라 텍스트 키를 대상에 입력합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Enters a text key into the target according to the current language and modifier state.</para>
    /// \endif
    /// </summary>
    /// <param name="key">
    /// \if KO
    /// <para>입력할 기본 키 값입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The base key value to enter.</para>
    /// \endif
    /// </param>
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

    /// <summary>
    /// \if KO
    /// <para>한글 조합을 초기화하고 원시 텍스트를 대상의 현재 선택 위치에 삽입합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Resets Hangul composition and inserts raw text at the target's current selection.</para>
    /// \endif
    /// </summary>
    /// <param name="text">
    /// \if KO
    /// <para>삽입할 원시 텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The raw text to insert.</para>
    /// \endif
    /// </param>
    private void InsertRaw(string text)
    {
        _hangulComposer.Reset();
        _target.InsertText(text);
    }

    /// <summary>
    /// \if KO
    /// <para>한글 조합을 초기화하고 대상의 선택 영역 또는 캐럿 앞 문자를 삭제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Resets Hangul composition and deletes the target's selection or character before the caret.</para>
    /// \endif
    /// </summary>
    private void Backspace()
    {
        _hangulComposer.Reset();
        _target.Backspace();
    }

    /// <summary>
    /// \if KO
    /// <para>한글 조합과 대상 텍스트를 모두 비웁니다.</para>
    /// \endif
    /// \if EN
    /// <para>Clears both Hangul composition and target text.</para>
    /// \endif
    /// </summary>
    private void Clear()
    {
        _hangulComposer.Reset();
        _target.Text = string.Empty;
    }

    /// <summary>
    /// \if KO
    /// <para>가상 Shift 상태를 전환하고 키 표시를 갱신합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Toggles virtual Shift state and refreshes key presentation.</para>
    /// \endif
    /// </summary>
    private void ToggleShift()
    {
        _shift = !_shift;
        RefreshKeys();
    }

    /// <summary>
    /// \if KO
    /// <para>한글 조합을 초기화하고 시스템 Caps Lock 키를 전환한 뒤 키 표시를 갱신합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Resets Hangul composition, toggles the system Caps Lock key, and refreshes key presentation.</para>
    /// \endif
    /// </summary>
    private void ToggleCapsLock()
    {
        _hangulComposer.Reset();
        SendKeys.SendWait("{CAPSLOCK}");
        RefreshKeys();
    }

    /// <summary>
    /// \if KO
    /// <para>영어와 한국어 입력 언어 및 IME 네이티브 모드를 전환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Toggles English and Korean input language and native IME mode.</para>
    /// \endif
    /// </summary>
    private void ToggleLanguage()
    {
        _hangulComposer.Reset();
        _korean = !_korean;
        ApplyInputLanguage(_korean);
        ImeHelper.SetNativeMode(_target.TextBoxHandle, _korean);
        RefreshKeys();
    }

    /// <summary>
    /// \if KO
    /// <para>대상 텍스트 상자의 캐럿을 왼쪽으로 한 칸 이동합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Moves the target text box's caret one position left.</para>
    /// \endif
    /// </summary>
    private void MoveLeft()
    {
        _target.SelectionStart = Math.Max(0, _target.SelectionStart - 1);
    }

    /// <summary>
    /// \if KO
    /// <para>대상 텍스트 상자의 캐럿을 오른쪽으로 한 칸 이동합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Moves the target text box's caret one position right.</para>
    /// \endif
    /// </summary>
    private void MoveRight()
    {
        _target.SelectionStart = Math.Min(_target.Text.Length, _target.SelectionStart + 1);
    }

    /// <summary>
    /// \if KO
    /// <para>물리 Shift, Caps Lock 및 현재 IME 언어 상태를 읽어 키보드 표시와 동기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Reads physical Shift, Caps Lock, and current IME language state and synchronizes keyboard presentation.</para>
    /// \endif
    /// </summary>
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

    /// <summary>
    /// \if KO
    /// <para>현재 Shift, Caps Lock 및 언어 상태에 맞게 모든 키 레이블과 선택 색상을 갱신합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Refreshes all key labels and selected colors for the current Shift, Caps Lock, and language state.</para>
    /// \endif
    /// </summary>
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

    /// <summary>
    /// \if KO
    /// <para>현재 언어와 수정 키 상태에서 기본 키가 생성할 텍스트를 계산합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Computes the text produced by a base key under the current language and modifier state.</para>
    /// \endif
    /// </summary>
    /// <param name="key">
    /// \if KO
    /// <para>변환할 기본 키 값입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The base key value to convert.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>화면에 표시하고 입력할 변환된 키 텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The converted key text to display and enter.</para>
    /// \endif
    /// </returns>
    /// <exception cref="NullReferenceException">
    /// \if KO
    /// <para><paramref name="key"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="key"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
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

    /// <summary>
    /// \if KO
    /// <para>폼이 닫힐 때 물리 키 상태 타이머를 중지하고 해제합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stops and disposes the physical-key-state timer when the form closes.</para>
    /// \endif
    /// </summary>
    /// <param name="e">
    /// \if KO
    /// <para>폼 닫힘 이벤트 인수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The form-closed event arguments.</para>
    /// \endif
    /// </param>
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _stateTimer.Stop();
        _stateTimer.Dispose();
        base.OnFormClosed(e);
    }

    /// <summary>
    /// \if KO
    /// <para>화면 키보드 폼 가장자리에 회색 테두리를 그립니다.</para>
    /// \endif
    /// \if EN
    /// <para>Draws a gray border around the on-screen keyboard form.</para>
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
        using var pen = new Pen(Color.Gray, 4f);
        e.Graphics.DrawRectangle(pen, 1, 1, Width - 3, Height - 3);
    }

    /// <summary>
    /// \if KO
    /// <para>폼을 활성화하지 않고 표시할지 여부를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets whether the form is shown without activation.</para>
    /// \endif
    /// </summary>
    protected override bool ShowWithoutActivation => true;

    /// <summary>
    /// \if KO
    /// <para>가상 또는 물리 Shift 키가 눌린 유효 상태인지 여부를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets whether either virtual or physical Shift is effectively pressed.</para>
    /// \endif
    /// </summary>
    private bool EffectiveShift => _shift || _physicalShift;

    /// <summary>
    /// \if KO
    /// <para>현재 입력 언어가 한국어이고 대상의 IME가 네이티브 모드인지 확인합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Determines whether the current input language is Korean and the target IME is in native mode.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>한국어 네이티브 입력 모드이면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when native Korean input mode is active.</para>
    /// \endif
    /// </returns>
    private bool IsKoreanInputMode()
    {
        return InputLanguage.CurrentInputLanguage.Culture.Name.Equals("ko-KR", StringComparison.OrdinalIgnoreCase) &&
               ImeHelper.IsNativeMode(_target.TextBoxHandle);
    }

    /// <summary>
    /// \if KO
    /// <para>설치된 입력 언어에서 영어 또는 한국어를 찾아 현재 언어로 적용합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Finds English or Korean among installed input languages and applies it as the current language.</para>
    /// \endif
    /// </summary>
    /// <param name="korean">
    /// \if KO
    /// <para>한국어를 적용하려면 <see langword="true"/>, 영어를 적용하려면 <see langword="false"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> to apply Korean; <see langword="false"/> to apply English.</para>
    /// \endif
    /// </param>
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

    /// <summary>
    /// \if KO
    /// <para>Shift Keys 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the shift keys value.</para>
    /// \endif
    /// </summary>
    private static readonly Dictionary<string, string> ShiftKeys = new()
    {
        ["`"] = "~", ["1"] = "!", ["2"] = "@", ["3"] = "#", ["4"] = "$",
        ["5"] = "%", ["6"] = "^", ["7"] = "&", ["8"] = "*", ["9"] = "(",
        ["0"] = ")", ["-"] = "_", ["="] = "+", ["["] = "{", ["]"] = "}",
        ["\\"] = "|", [";"] = ":", ["'"] = "\"", [","] = "<", ["."] = ">",
        ["/"] = "?",
    };

    /// <summary>
    /// \if KO
    /// <para>Korean Keys 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the korean keys value.</para>
    /// \endif
    /// </summary>
    private static readonly Dictionary<string, string> KoreanKeys = new()
    {
        ["q"] = "ㅂ", ["w"] = "ㅈ", ["e"] = "ㄷ", ["r"] = "ㄱ", ["t"] = "ㅅ",
        ["y"] = "ㅛ", ["u"] = "ㅕ", ["i"] = "ㅑ", ["o"] = "ㅐ", ["p"] = "ㅔ",
        ["a"] = "ㅁ", ["s"] = "ㄴ", ["d"] = "ㅇ", ["f"] = "ㄹ", ["g"] = "ㅎ",
        ["h"] = "ㅗ", ["j"] = "ㅓ", ["k"] = "ㅏ", ["l"] = "ㅣ",
        ["z"] = "ㅋ", ["x"] = "ㅌ", ["c"] = "ㅊ", ["v"] = "ㅍ", ["b"] = "ㅠ",
        ["n"] = "ㅜ", ["m"] = "ㅡ",
    };

    /// <summary>
    /// \if KO
    /// <para>Korean Shift Keys 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the korean shift keys value.</para>
    /// \endif
    /// </summary>
    private static readonly Dictionary<string, string> KoreanShiftKeys = new()
    {
        ["q"] = "ㅃ", ["w"] = "ㅉ", ["e"] = "ㄸ", ["r"] = "ㄲ", ["t"] = "ㅆ",
        ["o"] = "ㅒ", ["p"] = "ㅖ",
    };

    /// <summary>
    /// \if KO
    /// <para>화면 버튼과 변환 전 기본 키 텍스트의 연결을 나타냅니다.</para>
    /// \endif
    /// \if EN
    /// <para>Represents the association between a visual button and its unconverted base-key text.</para>
    /// \endif
    /// </summary>
    /// <param name="Button">
    /// \if KO
    /// <para>화면에 표시된 Dreamine 버튼입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The Dreamine button displayed on screen.</para>
    /// \endif
    /// </param>
    /// <param name="BaseText">
    /// \if KO
    /// <para>언어 및 수정 키 변환 전 기본 텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The base text before language and modifier conversion.</para>
    /// \endif
    /// </param>
    private sealed record KeyButton(DreamineButton Button, string BaseText);

    /// <summary>
    /// \if KO
    /// <para>키보드 키의 종류, 레이블, 너비 및 선택적 명령을 나타냅니다.</para>
    /// \endif
    /// \if EN
    /// <para>Represents a keyboard key's kind, label, width, and optional command.</para>
    /// \endif
    /// </summary>
    /// <param name="Kind">
    /// \if KO
    /// <para>텍스트 또는 명령 키 종류입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The text or command key kind.</para>
    /// \endif
    /// </param>
    /// <param name="Label">
    /// \if KO
    /// <para>키에 표시할 기본 레이블입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The base label displayed on the key.</para>
    /// \endif
    /// </param>
    /// <param name="Width">
    /// \if KO
    /// <para>키 너비입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The key width.</para>
    /// \endif
    /// </param>
    /// <param name="Action">
    /// \if KO
    /// <para>명령 키를 클릭할 때 실행할 선택적 동작입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The optional action invoked when a command key is clicked.</para>
    /// \endif
    /// </param>
    private sealed record KeySpec(KeyKind Kind, string Label, int Width, Action? Action)
    {
        /// <summary>
        /// \if KO
        /// <para>지정한 레이블과 너비로 텍스트 입력 키 사양을 만듭니다.</para>
        /// \endif
        /// \if EN
        /// <para>Creates a text-input key specification with the specified label and width.</para>
        /// \endif
        /// </summary>
        /// <param name="label">
        /// \if KO
        /// <para>키의 기본 레이블과 입력 값입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The key's base label and input value.</para>
        /// \endif
        /// </param>
        /// <param name="width">
        /// \if KO
        /// <para>키 너비입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The key width.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>새 텍스트 키 사양입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A new text-key specification.</para>
        /// \endif
        /// </returns>
        public static KeySpec Text(string label, int width = 54) => new(KeyKind.Text, label, width, null);
        /// <summary>
        /// \if KO
        /// <para>지정한 레이블, 너비 및 동작으로 명령 키 사양을 만듭니다.</para>
        /// \endif
        /// \if EN
        /// <para>Creates a command-key specification with the specified label, width, and action.</para>
        /// \endif
        /// </summary>
        /// <param name="label">
        /// \if KO
        /// <para>키에 표시할 레이블입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The label displayed on the key.</para>
        /// \endif
        /// </param>
        /// <param name="width">
        /// \if KO
        /// <para>키 너비입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The key width.</para>
        /// \endif
        /// </param>
        /// <param name="action">
        /// \if KO
        /// <para>키를 클릭할 때 실행할 동작입니다.</para>
        /// \endif
        /// \if EN
        /// <para>The action invoked when the key is clicked.</para>
        /// \endif
        /// </param>
        /// <returns>
        /// \if KO
        /// <para>새 명령 키 사양입니다.</para>
        /// \endif
        /// \if EN
        /// <para>A new command-key specification.</para>
        /// \endif
        /// </returns>
        /// \fn KeySpec Command(string label, int width, Action action)
        public static KeySpec Command(string label, int width, Action action) => new(KeyKind.Command, label, width, action);
    }

    /// <summary>
    /// \if KO
    /// <para>가상 키가 직접 텍스트를 입력하는지 명령을 실행하는지 지정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Specifies whether a virtual key enters text directly or executes a command.</para>
    /// \endif
    /// </summary>
    private enum KeyKind
    {
        /// <summary>
        /// \if KO
        /// <para>키가 변환된 텍스트를 입력함을 나타냅니다.</para>
        /// \endif
        /// \if EN
        /// <para>Indicates that the key enters converted text.</para>
        /// \endif
        /// </summary>
        Text,
        /// <summary>
        /// \if KO
        /// <para>키가 연결된 동작을 실행함을 나타냅니다.</para>
        /// \endif
        /// \if EN
        /// <para>Indicates that the key executes an associated action.</para>
        /// \endif
        /// </summary>
        Command
    }
}
