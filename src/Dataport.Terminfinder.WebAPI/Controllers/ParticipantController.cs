using Microsoft.AspNetCore.Mvc;
using Dataport.Terminfinder.BusinessLayer;
using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.BusinessObject.Error;
using Dataport.Terminfinder.WebAPI.Constants;
using Dataport.Terminfinder.WebAPI.RequestContext;
using Dataport.Terminfinder.WebAPI.Swagger;

namespace Dataport.Terminfinder.WebAPI.Controllers;

/// <summary>
/// Participant Controller Class
/// </summary>
[Route("participant")]
public class ParticipantController : ApiControllerBase
{
    private readonly IAppointmentBusinessLayer _appointmentBusinessLayer;

    /// <summary>
    /// Participant Controller
    /// </summary>
    /// <param name="appointmentBusinessLayer"></param>
    /// <param name="requestContext"></param>
    /// <param name="logger"></param>
    /// <param name="localizer"></param>
    /// <exception cref="ArgumentNullException">appointmentBusinessLayer</exception>
    public ParticipantController(IAppointmentBusinessLayer appointmentBusinessLayer, IRequestContext requestContext,
        ILogger<ParticipantController> logger,
        IStringLocalizer<ParticipantController> localizer)
        : base(requestContext, logger, localizer)
    {
        Logger.LogDebug($"Enter {nameof(ParticipantController)}");

        _appointmentBusinessLayer = appointmentBusinessLayer ?? throw new ArgumentNullException(nameof(appointmentBusinessLayer));
    }

    /// <summary>
    /// Delete participant and votings.
    /// If the appointment is protected by a password, submit the credentials of the appointment via basic authentication http header.
    /// Please use the appointment id as username and use the password of the appointment as password in the basic authentication header.
    /// </summary>
    /// <param name="participantId">participantid to delete</param>
    /// <param name="customerId">id of the customer</param>
    /// <param name="appointmentId">id of the appointment</param>
    /// <returns>ActionResult</returns>
    /// <response code="200">Participant are deleted</response>
    /// <response code="400">customerId or appointmentId was null or empty or the appointment has not be started</response>
    /// <response code="401">the appointment is protected by a password and no password was submitted or the password was wrong</response>
    /// <response code="404">customerId was not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpDelete("{customerId}/{appointmentId}/{participantId}")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 401)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    [BasicAuthenticationOperation]
    public IActionResult Delete(string customerId, string appointmentId, string participantId)
    {
        Logger.LogDebug("Enter {NameofDelete}, Parameter: {CustomerId}, {AppointmentId}", nameof(Delete), customerId,
            appointmentId);

        if (!Guid.TryParse(customerId, out Guid customerIdGuid)
            || !Guid.TryParse(appointmentId, out Guid appointmentIdGuid)
            || !Guid.TryParse(participantId, out Guid participantIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        if (string.IsNullOrEmpty(participantId) || participantIdGuid == Guid.Empty)
        {
            throw CreateBadRequestException(ErrorType.NoInput);
        }

        Participant participant = new()
        {
            CustomerId = customerIdGuid, AppointmentId = appointmentIdGuid, ParticipantId = participantIdGuid
        };

        if (!_appointmentBusinessLayer.ParticipantToDeleteAreValid(participant))
        {
            throw CreateBadRequestException(ErrorType.ParticipantNotValid);
        }

        ValidateAppointmentRequestStatusIsStarted(customerIdGuid, appointmentIdGuid, _appointmentBusinessLayer);

        if (_appointmentBusinessLayer.ExistsParticipant(customerIdGuid, appointmentIdGuid, participantIdGuid))
        {
            _appointmentBusinessLayer.DeleteParticipiant(participant);
        }
        else
        {
            throw CreateNotFoundRequestException(ErrorType.ParticipantNotFound);
        }

        return Ok();
    }
}