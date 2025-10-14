using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Dataport.Terminfinder.WebAPI.Localisation.AttributeAdapters;

/// <inheritdoc />
public class AttributeAdapterNoClientValidation<T> : AttributeAdapterBase<T> where T : ValidationAttribute
{
    /// <inheritdoc />
    public AttributeAdapterNoClientValidation(T attribute, IStringLocalizer stringLocalizer)
        : base(attribute, stringLocalizer)
    {
    }

    /// <inheritdoc />
    public override void AddValidation(ClientModelValidationContext context)
    {
        // see here for an example: https://blogs.msdn.microsoft.com/mvpawardprogram/2017/01/03/asp-net-core-mvc/
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override string GetErrorMessage(ModelValidationContextBase validationContext)
    {
        var displayName = validationContext.ModelMetadata.GetDisplayName();
        return GetErrorMessage(validationContext.ModelMetadata, displayName);
    }
}