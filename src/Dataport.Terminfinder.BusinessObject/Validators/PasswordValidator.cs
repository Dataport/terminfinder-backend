using System.Text.RegularExpressions;

namespace Dataport.Terminfinder.BusinessObject.Validators;

/// <summary>
/// A class to validate the password
/// </summary>
public static class PasswordValidator
{
    private const int RegexTimeoutMs = 100;

    // Max length of the password because of bcrypt - https://www.mscharhag.com/software-development/bcrypt-maximum-password-length
    private const int MaxLengthPassword = 30;
    private const int MinLengthPassword = 8;

    private static readonly Regex RegexContainsUpperCaseLetter =
        new("[A-Z]", RegexOptions.NonBacktracking, TimeSpan.FromMilliseconds(RegexTimeoutMs));

    private static readonly Regex RegexContainsDigit =
        new("\\d", RegexOptions.NonBacktracking, TimeSpan.FromMilliseconds(RegexTimeoutMs));

    // Characters of the ASCII code table from 33 to 126 (except digits or letters)
    private static readonly Regex RegexContainsSpecialCharacters =
        new("[\\!\"#\\$%&'\\(\\)\\*\\+,-\\./:;<=>\\?@\\[\\\\\\]\\^_`{\\|}~]", RegexOptions.NonBacktracking,
            TimeSpan.FromMilliseconds(RegexTimeoutMs));

    /// <summary>
    /// Check if the submitted password is valid
    /// </summary>
    /// <param name="password">the password to check</param>
    /// <returns>true if the password is valid. otherwise false</returns>
    public static bool IsValid(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        if (password.Length >= MinLengthPassword && password.Length <= MaxLengthPassword)
        {
            return RegexContainsUpperCaseLetter.IsMatch(password)
                   && RegexContainsDigit.IsMatch(password)
                   && RegexContainsSpecialCharacters.IsMatch(password);
        }

        return false;
    }
}