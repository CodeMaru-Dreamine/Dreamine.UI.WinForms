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

    private static string Compose(params string[] keys)
    {
        var composer = new HangulComposer();
        var text = string.Empty;

        foreach (var key in keys)
        {
            var edit = composer.Input(key, text);
            text = edit.ReplaceCount == 0
                ? text + edit.Text
                : text[..^edit.ReplaceCount] + edit.Text;
        }

        return text;
    }
}
