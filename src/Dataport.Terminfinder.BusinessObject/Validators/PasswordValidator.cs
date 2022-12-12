using System.Text.RegularExpressions;

namespace Dataport.Terminfinder.BusinessObject.Validators;

/// <summary>
/// A class to validate the password
/// </summary>
public static class PasswordValidator
{
    private static readonly int MinLengthPassword = 8;

    // Max length of the password because of bcrypt - https://www.mscharhag.com/software-development/bcrypt-maximum-password-length
    private static readonly int MaxLengthPassword = 30;
    private static readonly Regex RegexContainsUpperCaseLetter = new ("[A-Z]");

    private static readonly Regex RegexContainsDigit = new ("\\d");

    // Characters of the ASCII code table from 33 to 126 (except digits or letters)
    private static readonly Regex RegexContainsSpecialCharacters = new ("[\\!\"#\\$%&'\\(\\)\\*\\+,-\\./:;<=>\\?@\\[\\\\\\]\\^_`{\\|}~]");

    /// <summary>
    /// Check if the submitted password is valid
    /// </summary>
    /// <param name="password">the password to check</param>
    /// <returns>true if the password is valid. otherwise false</returns>
    public static bool IsValid(string password)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));

        if (password.Length >= MinLengthPassword && password.Length <= MaxLengthPassword)
        {
            return RegexContainsUpperCaseLetter.IsMatch(password)
                   && RegexContainsDigit.IsMatch(password)
                   && RegexContainsSpecialCharacters.IsMatch(password);
        }

        return false;
    }
}