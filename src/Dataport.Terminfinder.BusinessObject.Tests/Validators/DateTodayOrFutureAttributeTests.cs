using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dataport.Terminfinder.BusinessObject.Validators;

namespace Dataport.Terminfinder.BusinessObject.Tests.Validators;

/// <summary>
/// Testclass for Business objects
/// </summary>
[TestClass]
[ExcludeFromCodeCoverage]
public class DateTodayOrFutureAttributeTests
{
    public const string ExectedDateTodayOrFutureAttributeErrorMessage = "Error.";

    class DateTodayOrFutureAttributeDtoTestClass : IValidatableObject
    {
        [DateTodayOrFuture(ErrorMessage = ExectedDateTodayOrFutureAttributeErrorMessage)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public DateTime Entry { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }

    [TestMethod]
    public void IsValid_DateMin_returnValidationResult()
    {
        var entry = DateTime.MinValue;
        CheckValidDateTodayOrFutureValue(entry);
    }

    [TestMethod]
    public void IsValid_DateNow_returnValidationResult()
    {
        var entry = DateTime.Now;
        CheckValidDateTodayOrFutureValue(entry);
    }

    [TestMethod]
    public void IsValid_DateNowMinusOneDay_returnValidationResult()
    {
        var entry = DateTime.Now.AddDays(-1);
        CheckInvalidDateTodayOrFutureValue(entry);
    }

    private static void CheckValidDateTodayOrFutureValue(DateTime dt)
    {
        var instance = new DateTodayOrFutureAttributeDtoTestClass { Entry = dt };
        var validationResults = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true);

        Assert.IsTrue(actual);
    }

    private static void CheckInvalidDateTodayOrFutureValue(DateTime dt)
    {
        var instance = new DateTodayOrFutureAttributeDtoTestClass { Entry = dt };
        var validationResults = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true);

        Assert.IsFalse(actual);
        Assert.AreEqual(ExectedDateTodayOrFutureAttributeErrorMessage, validationResults[0].ErrorMessage);
    }
}