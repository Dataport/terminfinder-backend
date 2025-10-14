using Dataport.Terminfinder.BusinessLayer;
using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.BusinessObject.Error;
using Dataport.Terminfinder.WebAPI.Constants;
using Dataport.Terminfinder.WebAPI.RequestContext;
using Dataport.Terminfinder.WebAPI.Swagger;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace Dataport.Terminfinder.WebAPI.Controllers;

/// <summary>
/// Appointments Controller Class
/// </summary>
[Route("appointment")]
public class AppointmentController : ApiControllerBase
{
    private readonly IAppointmentBusinessLayer _appointmentBusinessLayer;

    /// <summary>
    /// Appointments Controller
    /// </summary>
    /// <param name="appointmentBusinessLayer"></param>
    /// <param name="requestContext"></param>
    /// <param name="logger"></param>
    /// <param name="localizer"></param>
    /// <exception cref="ArgumentNullException">appointmentBusinessLayer</exception>
    public AppointmentController(IAppointmentBusinessLayer appointmentBusinessLayer, IRequestContext requestContext,
        ILogger<AppointmentController> logger,
        IStringLocalizer<AppointmentController> localizer)
        : base(requestContext, logger, localizer)
    {
        Logger.LogDebug($"Enter {nameof(AppointmentController)}");

        _appointmentBusinessLayer = appointmentBusinessLayer ?? throw new ArgumentNullException(nameof(appointmentBusinessLayer));
    }

    /// <summary>
    /// Get an appointment without the admin id due to security purposes.
    /// If the appointment is protected by a password, submit the credentials of the appointment via basic authentication http header.
    /// Please use the appointment id as username and use the password of the appointment as password in the basic authentication header.
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="appointmentId">id of the appointment</param>
    /// <returns>all information of the appointment</returns>
    /// <response code="200">Appointment returned</response>
    /// <response code="400">customerId or appointmentId is null or empty</response>
    /// <response code="401">the appointment is protected by a password and no password was submitted or the password was wrong</response>
    /// <response code="404">customerId or appointmentId was not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet("{customerId}/{appointmentId}")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(typeof(Appointment), 200)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 401)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    [BasicAuthenticationOperation]
    public IActionResult Get(string customerId, string appointmentId)
    {
        Logger.LogDebug("Enter {Name}, Parameter: {CustomerId}, {AppointmentId}", nameof(Get), customerId,
            appointmentId);

        if (!Guid.TryParse(customerId, out var customerIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        if (!Guid.TryParse(appointmentId, out var appointmentIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        ValidateAppointmentRequest(customerIdGuid, appointmentIdGuid, _appointmentBusinessLayer);

        var appointment = _appointmentBusinessLayer.GetAppointment(customerIdGuid, appointmentIdGuid);
        if (appointment == null)
        {
            throw CreateNotFoundRequestException(ErrorType.AppointmentNotFound);
        }

        appointment.AdminId = Guid.Empty;
        return Ok(appointment);
    }

    /// <summary>
    /// Get the information if an appointment is protected by a password.
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="appointmentId">id of the appointment</param>
    /// <returns>the information if the appointment is protected by a password</returns>
    /// <response code="200">the information if the appointment is protected by a password</response>
    /// <response code="400">customerId or appointmentId is null or empty</response>
    /// <response code="404">customerId or appointmentId was not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet("{customerId}/{appointmentId}/protection")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(typeof(AppointmentProtectionResult), 200)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    public IActionResult GetProtection(string customerId, string appointmentId)
    {
        Logger.LogDebug("Enter {NameofGetProtection}, Parameter: {CustomerId}, {AppointmentId}",
            nameof(GetProtection), customerId, appointmentId);

        if (!Guid.TryParse(customerId, out var customerIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        if (!Guid.TryParse(appointmentId, out var appointmentIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        ValidateAppointmentRequestSkipPasswordVerification(customerIdGuid, appointmentIdGuid,
            _appointmentBusinessLayer);

        var result = new AppointmentProtectionResult
        {
            AppointmentId = appointmentIdGuid,
            IsProtectedByPassword =
                _appointmentBusinessLayer.IsAppointmentPasswordProtected(customerIdGuid, appointmentIdGuid)
        };

        return Ok(result);
    }

    /// <summary>
    /// Get the information if the passwort of the protected appointment is valid.
    /// If the appointment is protected by a password, submit the credentials of the appointment via basic authentication http header.
    /// Please use the admin id as username and use the password of the appointment as password in the basic authentication header.
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="appointmentId">id of the appointment</param>
    /// <returns>the information if the password of an protected appointment is valid</returns>
    /// <response code="200">the information if the password of the protected appointment is valid</response>
    /// <response code="400">customerId or appointmentId is null or empty</response>
    /// <response code="401">the appointment is protected by a password and no password was submitted or the password was wrong</response>
    /// <response code="404">customerId or appointmentId was not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet("{customerId}/{appointmentId}/passwordverification")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(typeof(AppointmentProtectionResult), 200)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 401)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    [BasicAuthenticationOperation]
    public IActionResult GetPasswordVerification(string customerId, string appointmentId)
    {
        Logger.LogDebug("Enter {NameofGetPasswordVerification}, Parameter: {CustomerId}, {AppointmentId}",
            nameof(GetPasswordVerification), customerId, appointmentId);

        if (!Guid.TryParse(customerId, out var customerIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        if (!Guid.TryParse(appointmentId, out var appointmentIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        ValidateAppointmentRequestSkipPasswordVerification(customerIdGuid, appointmentIdGuid,
            _appointmentBusinessLayer);

        var result = new AppointmentPasswordVerificationResult
        {
            AppointmentId = appointmentIdGuid,
            IsProtectedByPassword =
                _appointmentBusinessLayer.IsAppointmentPasswordProtected(customerIdGuid, appointmentIdGuid),
            IsPasswordValid =
                IsAppointmentPasswordValid(customerIdGuid, appointmentIdGuid, _appointmentBusinessLayer)
        };

        return Ok(result);
    }

    /// <summary>
    /// Create an appointment.
    /// If the appointment is protected by a password, submit the credentials of the appointment via basic authentication http header.
    /// Please use the appointment id as username and use the password of the appointment as password in the basic authentication header.
    /// </summary>
    /// <remarks></remarks>
    /// <param name="appointmentObject">one appointment</param>
    /// <param name="customerId"></param>
    /// <returns>ActionResult</returns>
    /// <response code="201">Created appointment returned</response>
    /// <response code="400">customerId was null or empty or appointmentId/adminId are not empty</response>
    /// <response code="401">the appointment is protected by a password and no password was submitted or the password was wrong</response>
    /// <response code="404">customerId was not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpPost("{customerId}")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [Consumes(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(typeof(Appointment), 201)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 401)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    public IActionResult Post([FromBody] Appointment appointmentObject, string customerId)
    {
        Logger.LogDebug("Enter {NameofPost}, Parameter: {AppointmentObject}, {CustomerId}", nameof(Post),
            appointmentObject, customerId);

        if (appointmentObject == null
            || (appointmentObject.AdminId == Guid.Empty && appointmentObject.AppointmentId != Guid.Empty)
            || (appointmentObject.AdminId != Guid.Empty && appointmentObject.AppointmentId == Guid.Empty)
            || !Guid.TryParse(customerId, out var customerIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        ValidateCustomerRequest(customerIdGuid, _appointmentBusinessLayer);

        _appointmentBusinessLayer.SetAppointmentForeignKeys(appointmentObject, customerIdGuid);

        ValidateCreateAndUpdateAppointmentRequest(appointmentObject, customerId);

        // When the object contains both AdminId & AppointmentId the user tried to resend the request.
        // Nothing will be updated and the same object is returned
        if (appointmentObject.AdminId == Guid.Empty && appointmentObject.AppointmentId == Guid.Empty)
        {
            appointmentObject = _appointmentBusinessLayer.AddAppointment(appointmentObject);
        }

        return Created(CreateCreatedUri(appointmentObject.AppointmentId.ToString()), appointmentObject);
    }

    /// <summary>
    /// Update an appointment.
    /// If the appointment is protected by a password, submit the credentials of the appointment via basic authentication http header.
    /// Please use the appointment id as username and use the password of the appointment as password in the basic authentication header.
    /// </summary>
    /// <remarks></remarks>
    /// <param name="appointmentObject">one appointment</param>
    /// <param name="customerId"></param>
    /// <returns>ActionResult</returns>
    /// <response code="200">appointment updated</response>
    /// <response code="400">customerId or appointmentid was null or empty</response>
    /// <response code="401">the appointment is protected by a password and no password was submitted or the password was wrong</response>
    /// <response code="404">customerId and/or appointmentId was not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpPut("{customerId}")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [Consumes(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(typeof(Appointment), 200)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 401)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    [BasicAuthenticationOperation]
    public IActionResult Put([FromBody] Appointment appointmentObject, string customerId)
    {
        Logger.LogDebug("Enter {NameofPut}, Parameter: {AppointmentObject}, {CustomerId}", nameof(Put),
            appointmentObject, customerId);

        if (appointmentObject == null || appointmentObject.AdminId == Guid.Empty)
        {
            throw CreateBadRequestException(ErrorType.NoInput);
        }

        if (!Guid.TryParse(customerId, out var customerIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        ValidateAppointmentRequest(customerIdGuid, appointmentObject.AppointmentId, appointmentObject.AdminId,
            _appointmentBusinessLayer);

        _appointmentBusinessLayer.SetAppointmentForeignKeys(appointmentObject, customerIdGuid);

        ValidateCreateAndUpdateAppointmentRequest(appointmentObject, customerId);

        appointmentObject =
            _appointmentBusinessLayer.UpdateAppointment(appointmentObject, appointmentObject.AdminId);

        return Ok(appointmentObject);
    }

    [ContractAnnotation("appointmentFromRequest:null => halt; customerIdFromRequest:null => halt")]
    private void ValidateCreateAndUpdateAppointmentRequest(Appointment appointmentFromRequest,
        string customerIdFromRequest)
    {
        Logger.LogDebug(
            "Enter {NameofValidateCreateAndUpdateAppointmentRequest}, Parameter: {appointmentFromRequest}, {customerIdFromRequest}",
            nameof(ValidateCreateAndUpdateAppointmentRequest), appointmentFromRequest, customerIdFromRequest);

        if (!Guid.TryParse(customerIdFromRequest, out var customerIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        if (appointmentFromRequest == null)
        {
            throw CreateBadRequestException(ErrorType.NoInput);
        }

        if (customerIdGuid == Guid.Empty)
        {
            throw CreateNotFoundRequestException(ErrorType.CustomerIdNotFound);
        }

        if (!_appointmentBusinessLayer.ExistsCustomer(customerIdGuid))
        {
            throw CreateBadRequestException(ErrorType.CustomerNotFound);
        }

        var isModelValid = TryValidateModelCreateAndUpdateAppointmentModel(appointmentFromRequest);
        if (!isModelValid)
        {
            var additionalErrorMessage = BuildAdditionalErrorMessageFromModelState();

            throw CreateBadRequestException(ErrorType.AppointmentNotValid, additionalErrorMessage);
        }

        if (!_appointmentBusinessLayer.CheckMaxTotalCountOfParticipants(customerIdGuid,
                appointmentFromRequest.AppointmentId, appointmentFromRequest.Participants))
        {
            throw CreateBadRequestException(ErrorType.MaximumElementsOfParticipantsAreExceeded);
        }

        if (!_appointmentBusinessLayer.CheckMaxTotalCountOfSuggestedDates(customerIdGuid,
                appointmentFromRequest.AppointmentId, appointmentFromRequest.SuggestedDates))
        {
            throw CreateBadRequestException(ErrorType.MaximumElementsOfSuggestedDatesAreExceeded);
        }

        if (!_appointmentBusinessLayer.CheckMinTotalCountOfSuggestedDates(customerIdGuid,
                appointmentFromRequest.AppointmentId, appointmentFromRequest.SuggestedDates))
        {
            throw CreateBadRequestException(ErrorType.MinimumElementsOfSuggestedDatesAreNotExceeded);
        }
    }

    private bool TryValidateModelCreateAndUpdateAppointmentModel(Appointment appointmentFromRequest)
    {
        Logger.LogDebug(
            "Enter {NameofTryValidateModelCreateAndUpdateAppointmentModel}, Parameter: {AppointmentFromRequest}",
            nameof(TryValidateModelCreateAndUpdateAppointmentModel), appointmentFromRequest);

        ModelState.Clear();
        TryValidateModel(appointmentFromRequest);

        // Validate manually; Nested objects won't be validated by TryValidate

        if (appointmentFromRequest.SuggestedDates != null)
        {
            var i = 0;
            foreach (var elem in appointmentFromRequest.SuggestedDates)
            {
                TryValidateModel(elem, $"{nameof(appointmentFromRequest.SuggestedDates)}[{i}]");
                i++;
            }
        }

        return ModelState.IsValid;
    }

    private string CreateCreatedUri(string appointmentId)
    {
        Logger.LogDebug("Enter {NameofCreateCreatedUri}, Parameter: {AppointmentId}", nameof(CreateCreatedUri),
            appointmentId);

        var uriBuilder = new UriBuilder(Request.GetEncodedUrl());
        uriBuilder.Path += $"/{appointmentId}";
        return uriBuilder.Uri.ToString();
    }
}