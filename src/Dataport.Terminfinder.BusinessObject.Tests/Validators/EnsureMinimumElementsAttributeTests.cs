using Dataport.Terminfinder.BusinessObject.Validators;
using System.ComponentModel.DataAnnotations;

namespace Dataport.Terminfinder.BusinessObject.Tests.Validators;

/// <summary>
/// Testclass for Business objects
/// </summary>
[TestClass]
public class EnsureMinimumElementsAttributeTests
{
    public const string ExectedEnsureMinimumElementsAttributeErrorMessage = "Error.";

    class EnsureMinimumElementsAttributeDtoTestClass : IValidatableObject
    {
        [EnsureMinimumElements(ErrorMessage = ExectedEnsureMinimumElementsAttributeErrorMessage)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string[] Entries { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }

    [TestMethod]
    public void Constructor_MinElementsValueIsTooSmall_throwException()
    {
        try
        {
            // ReSharper disable once ObjectCreationAsStatement
#pragma warning disable CA1806 // Do not ignore method results
            new EnsureMinimumElementsAttribute
            {
                ErrorMessage = ExectedEnsureMinimumElementsAttributeErrorMessage,
                MinElements = 0
            };
#pragma warning restore CA1806 // Do not ignore method results
            Assert.Fail("A exception has been exprected");
        }
        catch (ArgumentException)
        {

        }
    }

    [TestMethod]
    public void Constructor_MinElementsValueIsEqualMinimalMinElementsValue_submittedMinElementsIsUsed()
    {
        var sut = new EnsureMinimumElementsAttribute
        {
            ErrorMessage = ExectedEnsureMinimumElementsAttributeErrorMessage,
            MinElements = 1
        };
        Assert.AreEqual(1, sut.MinElements);
    }

    [TestMethod]
    public void IsValid_EntriesCollectionHasOneElement_returnNoValidationResult()
    {
        var entries = new string[1];
        CheckValidEnsureMinimumElementsValue(entries);
    }

    [TestMethod]
    public void IsValid_EntriesCollectionIsEmpty_returnValidationResult()
    {
        var entries = Array.Empty<string>();
        CheckInvalidEnsureMinimumElementsValue(entries);
    }

    private static void CheckInvalidEnsureMinimumElementsValue(string[] entries)
    {
        var instance = new EnsureMinimumElementsAttributeDtoTestClass { Entries = entries };
        var validationResults = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true);

        Assert.IsFalse(actual);
        Assert.AreEqual(1, validationResults.Count);
        Assert.AreEqual(ExectedEnsureMinimumElementsAttributeErrorMessage, validationResults[0].ErrorMessage);
    }

    private static void CheckValidEnsureMinimumElementsValue(string[] entries)
    {
        var instance = new EnsureMinimumElementsAttributeDtoTestClass { Entries = entries };
        var validationResults = new List<ValidationResult>();
        var actual = Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, true);

        Assert.IsTrue(actual);
        Assert.AreEqual(0, validationResults.Count);
    }
}