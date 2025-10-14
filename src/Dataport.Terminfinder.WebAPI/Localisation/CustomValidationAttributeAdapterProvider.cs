using Dataport.Terminfinder.BusinessObject.Validators;
using Dataport.Terminfinder.WebAPI.Localisation.AttributeAdapters;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Dataport.Terminfinder.WebAPI.Localisation;

/// <inheritdoc />
public class CustomValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
{
    private readonly ValidationAttributeAdapterProvider _baseProvider = new();

    /// <inheritdoc />
    public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
    {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
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