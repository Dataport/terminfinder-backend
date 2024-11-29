using Dataport.Terminfinder.BusinessObject.Validators;
using Dataport.Terminfinder.WebAPI.Localisation;
using Dataport.Terminfinder.WebAPI.Localisation.AttributeAdapters;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Dataport.Terminfinder.WebAPI.Tests.Localisation;

[TestClass]
[ExcludeFromCodeCoverage]
public class CustomValidationAttributeAdapterProviderTests
{
    private IStringLocalizer localizer;

    [TestInitialize]
    public void Initialize()
    {
        // fake localizer
        var mockLocalize = new Mock<IStringLocalizer<CustomValidationAttributeAdapterProvider>>();
        localizer = mockLocalize.Object;
        localizer = Mock.Of<IStringLocalizer<CustomValidationAttributeAdapterProvider>>();
    }

    [TestMethod]
    public void GetAttributeAdapter_EnsureMinimumElementsAttribute_EnsureMinimumElementsAttributeAdapter()
    {
        var sut = new CustomValidationAttributeAdapterProvider();
        IAttributeAdapter result = sut.GetAttributeAdapter(new EnsureMinimumElementsAttribute(), localizer);
        Assert.IsInstanceOfType(result, typeof(AttributeAdapterNoClientValidation<EnsureMinimumElementsAttribute>));
    }

    [TestMethod]
    public void GetAttributeAdapter_EnsureMaximumElementsAttribute_EnsureMaximumElementsAttributeAdapter()
    {
        var sut = new CustomValidationAttributeAdapterProvider();
        IAttributeAdapter result = sut.GetAttributeAdapter(new EnsureMaximumElementsAttribute(), localizer);
        Assert.IsInstanceOfType(result, typeof(AttributeAdapterNoClientValidation<EnsureMaximumElementsAttribute>));
    }

    [TestMethod]
    public void GetAttributeAdapter_RequiredAttribute_RequiredAttributeAdapter()
    {
        var sut = new CustomValidationAttributeAdapterProvider();
        IAttributeAdapter result = sut.GetAttributeAdapter(new RequiredAttribute(), localizer);
        Assert.IsInstanceOfType(result, typeof(AttributeAdapterNoClientValidation<RequiredAttribute>));
    }

    [TestMethod]
    public void GetAttributeAdapter_StringLengthAttribute_StringLengthAttributeAdapter()
    {
        var sut = new CustomValidationAttributeAdapterProvider();
        IAttributeAdapter result = sut.GetAttributeAdapter(new StringLengthAttribute(1), localizer);
        Assert.IsInstanceOfType(result, typeof(AttributeAdapterNoClientValidation<StringLengthAttribute>));
    }

    [TestMethod]
    public void GetAttributeAdapter_MaxLengthAttribute_MaxLengthAttributeAdapter()
    {
        var sut = new CustomValidationAttributeAdapterProvider();
        IAttributeAdapter result = sut.GetAttributeAdapter(new MaxLengthAttribute(1), localizer);
        Assert.IsInstanceOfType(result, typeof(AttributeAdapterNoClientValidation<MaxLengthAttribute>));
    }

    [TestMethod]
    public void GetAttributeAdapter_DateTodayOrFutureAttribute_DateTodayOrFutureAttributeAdapter()
    {
        var sut = new CustomValidationAttributeAdapterProvider();
        IAttributeAdapter result = sut.GetAttributeAdapter(new DateTodayOrFutureAttribute(), localizer);
        Assert.IsInstanceOfType(result, typeof(AttributeAdapterNoClientValidation<DateTodayOrFutureAttribute>));
    }

    [TestMethod]
    public void GetAttributeAdapter_RegularExpressionAttribute_RegularExpressionAttributeAdapter()
    {
        var sut = new CustomValidationAttributeAdapterProvider();
        IAttributeAdapter result = sut.GetAttributeAdapter(new RegularExpressionAttribute(string.Empty), localizer);
        Assert.IsInstanceOfType(result, typeof(AttributeAdapterNoClientValidation<RegularExpressionAttribute>));
    }

    [TestMethod]
    public void GetAttributeAdapter_PasswordAttribute_PasswordAttributeAdapter()
    {
        var sut = new CustomValidationAttributeAdapterProvider();
        IAttributeAdapter result = sut.GetAttributeAdapter(new PasswordAttribute(), localizer);
        Assert.IsInstanceOfType(result, typeof(AttributeAdapterNoClientValidation<PasswordAttribute>));
    }

    [TestMethod]
    public void GetAttributeAdapter_UnknownAttribute_returnResultFromValidationAttributeAdapterProvider()
    {
        var sut = new CustomValidationAttributeAdapterProvider();
        IAttributeAdapter result = sut.GetAttributeAdapter(new UtAttribute(), localizer);
        Assert.IsNull(result);
    }

    class UtAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return null;
        }
    }
}