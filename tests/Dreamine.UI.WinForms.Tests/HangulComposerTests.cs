using Dreamine.UI.WinForms.VirtualKeyboard;

namespace Dreamine.UI.WinForms.Tests;

public sealed class HangulComposerTests
{
    [Fact]
    public void IsComposableJamo_RecognizesSingleKoreanJamoOnly()
    {
        Assert.True(HangulComposer.IsComposableJamo("ㅎ"));
        Assert.True(HangulComposer.IsComposableJamo("ㅏ"));
        Assert.False(HangulComposer.IsComposableJamo("하"));
        Assert.False(HangulComposer.IsComposableJamo("ab"));
    }

    [Fact]
    public void Input_ComposesSimpleSyllable()
    {
        var result = Compose("ㅎ", "ㅏ");

        Assert.Equal("하", result);
    }

    [Fact]
    public void Input_ComposesTrailingConsonant()
    {
        var result = Compose("ㅎ", "ㅏ", "ㄴ");

        Assert.Equal("한", result);
    }

    [Fact]
    public void Input_SplitsTrailingConsonantWhenVowelFollows()
    {
        var result = Compose("ㄱ", "ㅏ", "ㄱ", "ㅣ");

        Assert.Equal("가기", result);
    }

    [Fact]
    public void Input_ComposesCompoundVowel()
    {
        var result = Compose("ㄱ", "ㅗ", "ㅏ");

        Assert.Equal("과", result);
    }

    [Fact]
    public void Input_ComposesCompoundTrailingConsonant()
    {
        var result = Compose("ㄱ", "ㅏ", "ㄹ", "ㄱ");

        Assert.Equal("갉", result);
    }

    [Fact]
    public void Input_SplitsCompoundTrailingConsonantWhenVowelFollows()
    {
        var result = Compose("ㄱ", "ㅏ", "ㄹ", "ㄱ", "ㅏ");

        Assert.Equal("갈가", result);
    }

    [Fact]
    public void Input_StartsNewSyllableAfterCompletedSyllable()
    {
        var result = Compose("ㅎ", "ㅏ", "ㄴ", "ㄱ", "ㅡ", "ㄹ");

        Assert.Equal("한글", result);
    }

    [Fact]
    public void Input_NonComposableTextResetsComposition()
    {
        var composer = new HangulComposer();
        var text = string.Empty;

        foreach (var key in new[] { "ㅎ", "ㅏ" })
        {
            var edit = composer.Input(key, text);
            text = Apply(text, edit);
        }

        var latin = composer.Input("A", text);
        text = Apply(text, latin);
        var jamo = composer.Input("ㄴ", text);
        text = Apply(text, jamo);

        Assert.Equal("하Aㄴ", text);
    }

    [Fact]
    public void Reset_ClearsPendingComposition()
    {
        var composer = new HangulComposer();

        var first = composer.Input("ㄱ", string.Empty);
        composer.Reset();
        var second = composer.Input("ㅏ", string.Empty);

        Assert.Equal(new HangulEdit(0, "ㄱ"), first);
        Assert.Equal(new HangulEdit(0, "ㅏ"), second);
    }

    private static string Compose(params string[] keys)
    {
        var composer = new HangulComposer();
        var text = string.Empty;

        foreach (var key in keys)
        {
            var edit = composer.Input(key, text);
            text = Apply(text, edit);
        }

        return text;
    }

    private static string Apply(string text, HangulEdit edit)
    {
        return edit.ReplaceCount == 0
            ? text + edit.Text
            : text[..^edit.ReplaceCount] + edit.Text;
    }
}
