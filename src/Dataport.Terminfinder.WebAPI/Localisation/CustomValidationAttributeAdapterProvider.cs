using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Dataport.Terminfinder.BusinessObject.Validators;
using Dataport.Terminfinder.WebAPI.Localisation.AttributeAdapters;

namespace Dataport.Terminfinder.WebAPI.Localisation;

/// <inheritdoc />
public class CustomValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
{
    private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();

    /// <inheritdoc />
    public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
    {
        if (attribute is EnsureMinimumElementsAttribute ensureMinimumElementAttribute)
        {
            return new AttributeAdapterNoClientValidation<EnsureMinimumElementsAttribute>(ensureMinimumElementAttribute,
                stringLocalizer);
        }

        if (attribute is EnsureMaximumElementsAttribute ensureMaximumElementAttribute)
        {
            return new AttributeAdapterNoClientValidation<EnsureMaximumElementsAttribute>(ensureMaximumElementAttribute,
                stringLocalizer);
        }

        if (attribute is RequiredAttribute requiredAttribute)
        {
            return new AttributeAdapterNoClientValidation<RequiredAttribute>(requiredAttribute, stringLocalizer);
        }

        if (attribute is StringLengthAttribute stringLengthAttribute)
        {
            return new AttributeAdapterNoClientValidation<StringLengthAttribute>(stringLengthAttribute,
                stringLocalizer);
        }

        if (attribute is MaxLengthAttribute maxLengthAttribute)
        {
            return new AttributeAdapterNoClientValidation<MaxLengthAttribute>(maxLengthAttribute, stringLocalizer);
        }

        if (attribute is DateTodayOrFutureAttribute dateTodayOrFutureAttribute)
        {
            return new AttributeAdapterNoClientValidation<DateTodayOrFutureAttribute>(dateTodayOrFutureAttribute,
                stringLocalizer);
        }

        if (attribute is RegularExpressionAttribute regularExpressionAttribute)
        {
            return new AttributeAdapterNoClientValidation<RegularExpressionAttribute>(regularExpressionAttribute,
                stringLocalizer);
        }

        if (attribute is PasswordAttribute passwordAttribute)
        {
            return new AttributeAdapterNoClientValidation<PasswordAttribute>(passwordAttribute, stringLocalizer);
        }

        return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
    }
}