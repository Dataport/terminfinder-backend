using Dataport.Terminfinder.BusinessLayer;
using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.BusinessObject.Error;
using Dataport.Terminfinder.WebAPI.Constants;
using Dataport.Terminfinder.WebAPI.RequestContext;
using Dataport.Terminfinder.WebAPI.Swagger;
using Microsoft.AspNetCore.Mvc;

namespace Dataport.Terminfinder.WebAPI.Controllers;

/// <summary>
/// Suggested date Controller Class
/// </summary>
[Route("suggesteddate")]
public class SuggestedDateController : ApiControllerBase
{
    private readonly IAppointmentBusinessLayer _appointmentBusinessLayer;

    /// <summary>
    /// SuggestedDate Controller
    /// </summary>
    /// <param name="appointmentBusinessLayer"></param>
    /// <param name="requestContext"></param>
    /// <param name="logger"></param>
    /// <param name="localizer"></param>
    /// <exception cref="ArgumentNullException">appointmentBusinessLayer</exception>
    public SuggestedDateController(IAppointmentBusinessLayer appointmentBusinessLayer, IRequestContext requestContext,
        ILogger<SuggestedDateController> logger,
        IStringLocalizer<SuggestedDateController> localizer)
        : base(requestContext, logger, localizer)
    {
        Logger.LogDebug($"Enter {nameof(SuggestedDateController)}");

        _appointmentBusinessLayer = appointmentBusinessLayer ?? throw new ArgumentNullException(nameof(appointmentBusinessLayer));
    }

    /// <summary>
    /// Delete suggested date and votings.
    /// If the appointment is protected by a password, submit the credentials of the appointment via basic authentication http header.
    /// Please use the appointment id as username and use the password of the appointment as password in the basic authentication header.
    /// </summary>
    /// <remarks>If the suggestedDateId not exists, the request is ignored</remarks>
    /// <param name="suggestedDateId">delete suggested date</param>
    /// <param name="customerId">id of the customer</param>
    /// <param name="appointmentId">id of the appointment</param>
    /// <returns>ActionResult</returns>
    /// <response code="200">Suggested date deleted</response>
    /// <response code="400">customerId or appointmentId or suggestedDateId was null or empty</response>
    /// <response code="401">the appointment is protected by a password and no password was submitted or the password was wrong</response>
    /// <response code="404">customerId or appointmentId or suggestedDateId was not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpDelete("{customerId}/{appointmentId}/{suggestedDateId}")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 401)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    [BasicAuthenticationOperation]
    public IActionResult Delete(string customerId, string appointmentId, string suggestedDateId)
    {
        Logger.LogDebug("Enter {NameofDelete}, Parameter: {CustomerId}, {AppointmentId}", nameof(Delete),
            customerId, appointmentId);

        if (!Guid.TryParse(customerId, out var customerIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        if (!Guid.TryParse(appointmentId, out var appointmentIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        if (!Guid.TryParse(suggestedDateId, out var suggestedDateIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        var suggestedDate = new SuggestedDate
        {
            CustomerId = customerIdGuid, AppointmentId = appointmentIdGuid, SuggestedDateId = suggestedDateIdGuid
        };

        if (!_appointmentBusinessLayer.SuggestedDateToDeleteAreValid(suggestedDate))
        {
            throw CreateBadRequestException(ErrorType.NoInput);
        }

        ValidateAppointmentRequest(customerIdGuid, appointmentIdGuid, _appointmentBusinessLayer);

        if (_appointmentBusinessLayer.ExistsSuggestedDate(customerIdGuid, appointmentIdGuid, suggestedDateIdGuid))
        {
            _appointmentBusinessLayer.DeleteSuggestedDate(suggestedDate);
        }
        else
        {
            throw CreateNotFoundRequestException(ErrorType.SuggestedDateNotFound);
        }

        return Ok();
    }
}