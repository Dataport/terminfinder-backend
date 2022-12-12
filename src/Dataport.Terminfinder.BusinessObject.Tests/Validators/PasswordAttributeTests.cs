using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dataport.Terminfinder.BusinessObject.Validators;

namespace Dataport.Terminfinder.BusinessObject.Tests.Validators;

/// <summary>
/// Testclass for Business objects
/// </summary>
[TestClass]
[ExcludeFromCodeCoverage]
public class PasswordAttributeTests
{
    public const string ExectedPasswordAttributeErrorMessage = "Error.";

    class PasswordAttributeTestsDtoTestClass : IValidatableObject
    {
        [Password(ErrorMessage = ExectedPasswordAttributeErrorMessage)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }

    [TestMethod]
    public void IsValid_null_returnNoValidationResult()
    {
        CheckValidPasswordValue(null);
    }

    [TestMethod]
    public void IsValid_ValidPassword_returnNoValidationResult()
    {
        string validPassword = "Blafasel1!";
        CheckValidPasswordValue(validPassword);
    }

    [TestMethod]
    public void IsValid_InvalidPassword_returnValidationResult()
    {
        string invalidPassword = string.Empty;
        CheckInvalidPasswordValue(invalidPassword);
    }

    private static void CheckInvalidPasswordValue(string password)
    {
        var instance = new PasswordAttributeTestsDtoTestClass { Password = password };
        var validationResults = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true);

        Assert.IsFalse(actual);
        Assert.AreEqual(1, validationResults.Count);
        Assert.AreEqual(ExectedPasswordAttributeErrorMessage, validationResults[0].ErrorMessage);
    }

    private static void CheckValidPasswordValue(string password)
    {
        var instance = new PasswordAttributeTestsDtoTestClass { Password = password };
        var validationResults = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true);

        Assert.IsTrue(actual);
        Assert.AreEqual(0, validationResults.Count);
    }
}