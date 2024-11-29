using Dataport.Terminfinder.BusinessLayer;
using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.BusinessObject.Error;
using Dataport.Terminfinder.WebAPI.Constants;
using Dataport.Terminfinder.WebAPI.RequestContext;
using Microsoft.AspNetCore.Mvc;

namespace Dataport.Terminfinder.WebAPI.Controllers;

/// <summary>
/// App Controller Class
/// </summary>
[Route("customer")]
public class CustomerController : ApiControllerBase
{
    private readonly ICustomerBusinessLayer _customerBusinessLayer;

    /// <summary>
    /// App Controller
    /// </summary>
    /// <param name="customerBusinessLayer"></param>
    /// <param name="requestContext"></param>
    /// <param name="logger"></param>
    /// <param name="localizer"></param>
    /// <exception cref="ArgumentNullException">customerBusinessLayer</exception>
    public CustomerController(ICustomerBusinessLayer customerBusinessLayer, IRequestContext requestContext,
        ILogger<CustomerController> logger, IStringLocalizer<CustomerController> localizer)
        : base(requestContext, logger, localizer)
    {
        Logger.LogDebug($"Enter {nameof(CustomerController)}");

        _customerBusinessLayer = customerBusinessLayer ?? throw new ArgumentNullException(nameof(customerBusinessLayer));
    }

    /// <summary>
    /// Get customer information
    /// </summary>
    /// <remarks></remarks>
    /// <param name="customerId">id of the customer</param>
    /// <returns>customer information</returns>
    /// <response code="200">Customers returned</response>
    /// <response code="400">customerid was null or empty</response>
    /// <response code="404">customerid was not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet("{customerId}")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(typeof(Customer), 200)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    public IActionResult GetCustomer(string customerId)
    {
        try
        {
            Logger.LogDebug($"Enter {nameof(GetCustomer)}");

            if (string.IsNullOrEmpty(customerId))
            {
                string errorMessage = $"ErrorCode{string.Format("{0:d4}", (int)ErrorType.CustomerIdNotFound)}";
                string language = Thread.CurrentThread.CurrentCulture.Name;

                ErrorResult errorObject = new()
                {
                    Code = ((int)ErrorType.CustomerIdNotFound).ToString(),
                    Message = Localizer[errorMessage],
                    Language = language
                };

                Logger.LogInformation("Error {NameofGetCustomer}: {ErrorObjectCode},{ErrorObjectMessage}",
                    nameof(GetCustomer), errorObject.Code, errorObject.Message);
                return BadRequest(errorObject);
            }

            if (!Guid.TryParse(customerId, out Guid customerIdGuid))
            {
                string errorMessage = $"ErrorCode{string.Format("{0:d4}", (int)ErrorType.WrongInputOrNotAllowed)}";
                string language = Thread.CurrentThread.CurrentCulture.Name;

                ErrorResult errorObject = new()
                {
                    Code = ((int)ErrorType.CustomerIdNotFound).ToString(),
                    Message = Localizer[errorMessage],
                    Language = language
                };

                Logger.LogInformation("Error {NameofGetCustomer}: {ErrorObjectCode},{ErrorObjectMessage}",
                    nameof(GetCustomer), errorObject.Code, errorObject.Message);
                return BadRequest(errorObject);
            }

            // check status type of customer
            Customer customer = _customerBusinessLayer.GetCustomer(customerIdGuid);
            if (customer == null)
            {
                string errorMessage = $"ErrorCode{string.Format("{0:d4}", (int)ErrorType.CustomerNotFound)}";
                string language = Thread.CurrentThread.CurrentCulture.Name;

                ErrorResult errorObject = new()
                {
                    Code = ((int)ErrorType.CustomerNotFound).ToString(),
                    Message = Localizer[errorMessage],
                    Language = language
                };

                Logger.LogInformation("Error {NameofGetCustomer}: {ErrorObjectCode},{ErrorObjectMessage}",
                    nameof(GetCustomer), errorObject.Code, errorObject.Message);

                return NotFound(errorObject);
            }

            if (customer.Status == AppointmentStatusType.Started.ToString())
            {
                return Ok(customer);
            }

            string errorMessageWithCode = $"ErrorCode{string.Format("{0:d4}", (int)ErrorType.CustomerNotValid)}";
            string languageName = Thread.CurrentThread.CurrentCulture.Name;

            ErrorResult errorResultObject = new()
            {
                Code = ((int)ErrorType.CustomerNotValid).ToString(),
                Message = Localizer[errorMessageWithCode],
                Language = languageName
            };

            Logger.LogInformation("Error {NameofGetCustomer}: {errorResultObjectCode},{errorResultObjectMessage}",
                nameof(GetCustomer), errorResultObject.Code, errorResultObject.Message);

            return NotFound(errorResultObject);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            string errorMessage = $"ErrorCode{string.Format("{0:d4}", (int)ErrorType.GeneralError)}";
            string language = Thread.CurrentThread.CurrentCulture.Name;

            ErrorResult errorObject = new()
            {
                Code = ((int)ErrorType.GeneralError).ToString(),
                Message = Localizer[errorMessage],
                Language = language
            };
            Logger.LogError(e, "Error {NameofGetCustomer}: {ErrorObjectCode},{ErrorObjectMessage}",
                nameof(GetCustomer), errorObject.Code, errorObject.Message);

            return StatusCode(500, errorObject);
        }
        finally
        {
            Logger.LogDebug($"Leave {nameof(GetCustomer)}");
        }
    }
}