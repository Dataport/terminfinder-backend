using Dataport.Terminfinder.BusinessLayer;
using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.BusinessObject.Error;
using Dataport.Terminfinder.Common.Extension;
using Dataport.Terminfinder.WebAPI.Constants;
using Dataport.Terminfinder.WebAPI.RequestContext;
using Dataport.Terminfinder.WebAPI.Swagger;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Dataport.Terminfinder.WebAPI.Controllers;

/// <summary>
/// Voting Controller Class
/// </summary>
[Route("votings")]
public class VotingController : ApiControllerBase
{
    private readonly IAppointmentBusinessLayer _appointmentBusinessLayer;

    /// <summary>
    /// Voting Controller
    /// </summary>
    /// <param name="appointmentBusinessLayer"></param>
    /// <param name="requestContext"></param>
    /// <param name="logger"></param>
    /// <param name="localizer"></param>
    /// <exception cref="ArgumentNullException">appointmentBusinessLayer</exception>
    public VotingController(IAppointmentBusinessLayer appointmentBusinessLayer, IRequestContext requestContext,
        ILogger<VotingController> logger,
        IStringLocalizer<VotingController> localizer)
        : base(requestContext, logger, localizer)
    {
        Logger.LogDebug($"Enter {nameof(VotingController)}");

        _appointmentBusinessLayer = appointmentBusinessLayer ?? throw new ArgumentNullException(nameof(appointmentBusinessLayer));
    }

    /// <summary>
    /// Get participants and votings.
    /// If the appointment is protected by a password, submit the credentials of the appointment via basic authentication http header.
    /// Please use the appointment id as username and use the password of the appointment as password in the basic authentication header.
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="appointmentId">id of the appointment</param>
    /// <returns>all participants and votings of the appointment</returns>
    /// <response code="200">Appointments returned</response>
    /// <response code="400">customerId or appointmentId was null or empty</response>
    /// <response code="401">the appointment is protected by a password and no password was submitted or the password was wrong</response>
    /// <response code="404">customerId was not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet("{customerId}/{appointmentId}")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(typeof(Participant[]), 200)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 401)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    [BasicAuthenticationOperation]
    public IActionResult Get(string customerId, string appointmentId)
    {
        Logger.LogDebug("Enter {NameofGet}, Parameter: {CustomerId}, {AppointmentId}", nameof(Get), customerId,
            appointmentId);

        if (!Guid.TryParse(customerId, out var customerIdGuid))
        {
            throw CreateBadRequestException(ErrorType.CustomerIdNotValid);
        }

        if (!Guid.TryParse(appointmentId, out var appointmentIdGuid))
        {
            throw CreateBadRequestException(ErrorType.AppointmentIdNotValid);
        }

        ValidateAppointmentRequest(customerIdGuid, appointmentIdGuid, _appointmentBusinessLayer);

        var participants =
            _appointmentBusinessLayer.GetParticipants(customerIdGuid, appointmentIdGuid);
        if (participants.IsNullOrEmpty())
        {
            throw CreateNotFoundRequestException(ErrorType.AppointmentNotFound);
        }

        return Ok(participants);
    }

    /// <summary>
    /// Create and update participants and their votings.
    /// If the appointment is protected by a password, submit the credentials of the appointment via basic authentication http header.
    /// Please use the appointment id as username and use the password of the appointment as password in the basic authentication header.
    /// </summary>
    /// <remarks></remarks>
    /// <param name="participants">all participants</param>
    /// <param name="customerId"></param>
    /// <param name="appointmentId">id of the appointment</param>
    /// <returns>ActionResult</returns>
    /// <response code="201">Votings created or updated</response>
    /// <response code="400">customerId or appointmentId was null or empty or the appointment has not be started</response>
    /// <response code="401">the appointment is protected by a password and no password was submitted or the password was wrong</response>
    /// <response code="404">customerId and/or appointmentId was not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpPut("{customerId}/{appointmentId}")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [Consumes(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(typeof(Participant[]), 201)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 401)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    [BasicAuthenticationOperation]
    public IActionResult Put([FromBody] Participant[] participants, string customerId, string appointmentId)
    {
        Logger.LogDebug("Enter {NameofPut}, Parameter: {Participants}", nameof(Put), participants);

        if (participants.IsNullOrEmpty())
        {
            throw CreateBadRequestException(ErrorType.NoInput);
        }

        if (!Guid.TryParse(customerId, out var customerIdGuid))
        {
            throw CreateBadRequestException(ErrorType.CustomerIdNotValid);
        }

        if (!Guid.TryParse(appointmentId, out var appointmentIdGuid))
        {
            throw CreateBadRequestException(ErrorType.AppointmentIdNotValid);
        }

        ValidateAppointmentRequestStatusIsStarted(customerIdGuid, appointmentIdGuid, _appointmentBusinessLayer);

        if (!_appointmentBusinessLayer.ParticipantsAreValid(participants))
        {
            throw CreateBadRequestException(ErrorType.ParticipantNotValid);
        }

        var isModelValid = TryValidateModelCreateAndUpdateParticipantModel(participants);
        if (!isModelValid)
        {
            var additionalErrorMessage = BuildAdditionalErrorMessageFromModelState();

            throw CreateBadRequestException(ErrorType.AppointmentNotValid, additionalErrorMessage);
        }

        if (!_appointmentBusinessLayer.CheckMaxTotalCountOfParticipants(customerIdGuid, appointmentIdGuid,
                participants))
        {
            throw CreateBadRequestException(ErrorType.MaximumElementsOfParticipantsAreExceeded);
        }

        _appointmentBusinessLayer.SetParticipantsForeignKeys(participants, customerIdGuid, appointmentIdGuid);
        var collectionOfParticipants =
            _appointmentBusinessLayer.AddAndUpdateParticipants(customerIdGuid, appointmentIdGuid, participants);
        return Created(CreateCreatedUri(), collectionOfParticipants);
    }

    private bool TryValidateModelCreateAndUpdateParticipantModel(ICollection<Participant> participants)
    {
        ModelState.Clear();

        // Validate manually; Nested objects won't be validated by TryValidate

        if (participants.IsNullOrEmpty())
        {
            return ModelState.IsValid;
        }

        var i = 0;
        foreach (var participantElem in participants)
        {
            TryValidateModel(participantElem, $"{nameof(participants)}[{i}]");

            var y = 0;
            foreach (var votingElem in participantElem.Votings)
            {
                TryValidateModel(votingElem, $"{nameof(participantElem.Votings)}[{y}]");
                y++;
            }

            i++;
        }

        return ModelState.IsValid;
    }

    private string CreateCreatedUri()
    {
        Logger.LogDebug("Enter {NameofCreateCreatedUri}", nameof(CreateCreatedUri));

        var uriBuilder = new UriBuilder(Request.GetEncodedUrl());
        return uriBuilder.Uri.ToString();
    }
}