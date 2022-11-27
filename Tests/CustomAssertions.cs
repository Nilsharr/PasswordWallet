using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace PasswordWallet.UnitTests;

public static class CustomAssertions
{
    public static AndConstraint<StringAssertions> BeHexString(this StringAssertions text, string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion.BecauseOf(because, becauseArgs).ForCondition(IsHexString(text.Subject))
            .FailWith("Expected string to be a hex string{reason}.");

        return new AndConstraint<StringAssertions>(text);
    }

    private static bool IsHexString(string text)
    {
        // ReSharper disable once ForCanBeConvertedToForeach
        // ReSharper disable once LoopCanBeConvertedToQuery
        for (var i = 0; i < text.Length; i++)
        {
            if (!Uri.IsHexDigit(text[i]))
            {
                return false;
            }
        }

        return true;
    }
}