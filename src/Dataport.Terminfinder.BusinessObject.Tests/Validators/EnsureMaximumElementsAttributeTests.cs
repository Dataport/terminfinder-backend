using Dataport.Terminfinder.BusinessObject.Validators;
using System.ComponentModel.DataAnnotations;

namespace Dataport.Terminfinder.BusinessObject.Tests.Validators;

/// <summary>
/// Testclass for Business objects
/// </summary>
[TestClass]
public class EnsureMaximumElementsAttributeTests
{
    public const string ExectedEnsureMaximumElementsAttributeErrorMessage = "Error.";

    class EnsureMaximumElementsAttributeDtoTestClass : IValidatableObject
    {
        [EnsureMaximumElements(MaxElements = 100, ErrorMessage = ExectedEnsureMaximumElementsAttributeErrorMessage)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string[] Entries { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }

    [TestMethod]
    public void Constructor_MaxElementsValueIsTooGreat_throwException()
    {
        try
        {
            // ReSharper disable once ObjectCreationAsStatement
#pragma warning disable CA1806 // Do not ignore method results
            new EnsureMaximumElementsAttribute()
            {
                ErrorMessage = ExectedEnsureMaximumElementsAttributeErrorMessage,
                MaxElements = 10001
            };
#pragma warning restore CA1806 // Do not ignore method results
            Assert.Fail("A exception has been exprected");
        }
        catch (ArgumentException)
        {

        }
    }

    [TestMethod]
    public void Constructor_MaxElementsValueIsEqualMaximumMaxElementsValue_submittedMaxElementsIsUsed()
    {
        var sut = new EnsureMaximumElementsAttribute
        {
            ErrorMessage = ExectedEnsureMaximumElementsAttributeErrorMessage,
            MaxElements = 10000
        };
        Assert.AreEqual(10000, sut.MaxElements);
    }

    [TestMethod]
    public void IsValid_EntriesCollectionHasOneElement_returnNoValidationResult()
    {
        var entries = new string[1];
        CheckValidEnsureMaximumElementsValue(entries);
    }

    [TestMethod]
    public void IsValid_EntriesCollectionIsEmpty_returnValidationResult()
    {
        var entries = new string[101];
        CheckInvalidEnsureMaximumElementsValue(entries);
    }

    private static void CheckInvalidEnsureMaximumElementsValue(string[] entries)
    {
        var instance = new EnsureMaximumElementsAttributeDtoTestClass { Entries = entries };
        var validationResults = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true);

        Assert.IsFalse(actual);
        Assert.AreEqual(ExectedEnsureMaximumElementsAttributeErrorMessage, validationResults[0].ErrorMessage);
    }

    private static void CheckValidEnsureMaximumElementsValue(string[] entries)
    {
        var instance = new EnsureMaximumElementsAttributeDtoTestClass { Entries = entries };
        var validationResults = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true);

        Assert.IsTrue(actual);
        Assert.AreEqual(0, validationResults.Count);
    }
}