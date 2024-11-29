using Microsoft.AspNetCore.Mvc;
using Dataport.Terminfinder.BusinessLayer;
using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.BusinessObject.Error;
using Dataport.Terminfinder.Common.Extension;
using Dataport.Terminfinder.WebAPI.Constants;
using Dataport.Terminfinder.WebAPI.RequestContext;
using Dataport.Terminfinder.WebAPI.Swagger;
// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace Dataport.Terminfinder.WebAPI.Controllers;

/// <summary>
/// Admin Controller Class
/// </summary>
[Route("admin")]
public class AdminController : ApiControllerBase
{
    private readonly IAppointmentBusinessLayer _appointmentBusinessLayer;

    /// <summary>
    /// Admin Controller
    /// </summary>
    /// <param name="appointmentBusinessLayer"></param>
    /// <param name="requestContext"></param>
    /// <param name="logger"></param>
    /// <param name="localizer"></param>
    /// <exception cref="ArgumentNullException">logger</exception>
    /// <exception cref="ArgumentNullException">appointmentBusinessLayer</exception>
    public AdminController(IAppointmentBusinessLayer appointmentBusinessLayer,
        IRequestContext requestContext,
        ILogger<AdminController> logger,
        IStringLocalizer<AdminController> localizer)
        : base(requestContext, logger, localizer)
    {
        Logger.LogDebug($"Enter {nameof(AdminController)}");

        _appointmentBusinessLayer = appointmentBusinessLayer ??
                                    throw new ArgumentNullException(nameof(appointmentBusinessLayer));
    }

    /// <summary>
    /// Get an appointment with the admin id due to security purposes.
    /// If the appointment is protected by a password, submit the credentials of the appointment via basic authentication http header.
    /// Please use the admin id as username and use the password of the appointment as password in the basic authentication header.
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="adminId">adminid of the appointment</param>
    /// <returns>all information of the appointment</returns>
    /// <response code="200">Appointment returned</response>
    /// <response code="400">customerId or appointmentId is null or empty</response>
    /// <response code="401">the appointment is protected by a password and no password was submitted or the password was wrong</response>
    /// <response code="404">customerId or appointmentId was not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet("{customerId}/{adminId}")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(typeof(Appointment), 200)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 401)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    [BasicAuthenticationOperation]
    public IActionResult Get(string customerId, string adminId)
    {
        Logger.LogDebug("Enter {NameofGet}, Parameter: {CustomerId}, {AdminId}", nameof(Get), customerId, adminId);

        if (!Guid.TryParse(customerId, out Guid customerIdGuid)
            || !Guid.TryParse(adminId, out Guid adminIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        ValidateReadAppointmentByAdminIdRequest(customerIdGuid, adminIdGuid, _appointmentBusinessLayer);

        Appointment appointment = _appointmentBusinessLayer.GetAppointmentByAdminId(customerIdGuid, adminIdGuid);

        return Ok(appointment);
    }

    /// <summary>
    /// Get the information if an appointment is protected by a password.
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="adminId">adminid of the appointment</param>
    /// <returns>the information if the appointment is protected by a password</returns>
    /// <response code="200">the information if the appointment is protected by a password</response>
    /// <response code="400">customerId or appointmentId is null or empty</response>
    /// <response code="404">customerId or appointmentId was not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet("{customerId}/{adminId}/protection")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(typeof(AppointmentProtectionResult), 200)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    public IActionResult GetProtection(string customerId, string adminId)
    {
        Logger.LogDebug("Enter {NameofGetProtection}, Parameter: {CustomerId}, {AdminId}", nameof(GetProtection),
            customerId, adminId);

        if (!Guid.TryParse(customerId, out Guid customerIdGuid)
            || !Guid.TryParse(adminId, out Guid adminIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        ValidateReadAppointmentByAdminIdRequestSkipPasswordVerfication(customerIdGuid, adminIdGuid,
            _appointmentBusinessLayer);

        var result = new AppointmentProtectionResult
        {
            AppointmentId =
                _appointmentBusinessLayer.GetAppointmentByAdminId(customerIdGuid, adminIdGuid)?.AppointmentId ??
                Guid.Empty,
            IsProtectedByPassword =
                _appointmentBusinessLayer.IsAppointmentPasswordProtectedByAdminId(customerIdGuid, adminIdGuid)
        };

        return Ok(result);
    }

    /// <summary>
    /// Get the information if the passwort of the protected appointment is valid.
    /// If the appointment is protected by a password, submit the credentials of the appointment via basic authentication http header.
    /// Please use the admin id as username and use the password of the appointment as password in the basic authentication header.
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="adminId">adminid of the appointment</param>
    /// <returns>the information if the password of an protected appointment is valid</returns>
    /// <response code="200">the information if the password of the protected appointment is valid</response>
    /// <response code="400">customerId or adminId is null or empty</response>
    /// <response code="401">the appointment is protected by a password and no password was submitted or the password was wrong</response>
    /// <response code="404">customerId or adminId was not found</response>
    /// <response code="500">Unexpected error</response>
    [HttpGet("{customerId}/{adminId}/passwordverification")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(typeof(AppointmentPasswordVerificationResult), 200)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 401)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    [BasicAuthenticationOperation]
    public IActionResult GetPasswordVerification(string customerId, string adminId)
    {
        Logger.LogDebug("Enter {NameofGetPasswordVerification}, Parameter: {CustomerId}, {AdminId}",
            nameof(GetPasswordVerification), customerId, adminId);

        if (!Guid.TryParse(customerId, out Guid customerIdGuid)
            || !Guid.TryParse(adminId, out Guid adminIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        ValidateReadAppointmentByAdminIdRequestSkipPasswordVerfication(customerIdGuid, adminIdGuid,
            _appointmentBusinessLayer);

        var result = new AppointmentPasswordVerificationResult
        {
            AppointmentId =
                _appointmentBusinessLayer.GetAppointmentByAdminId(customerIdGuid, adminIdGuid)?.AppointmentId ??
                Guid.Empty,
            IsProtectedByPassword =
                _appointmentBusinessLayer.IsAppointmentPasswordProtectedByAdminId(customerIdGuid, adminIdGuid),
            IsPasswordValid =
                IsAppointmentPasswordValidByAdminId(customerIdGuid, adminIdGuid, _appointmentBusinessLayer)
        };

        return Ok(result);
    }

    /// <summary>
    /// Set the new status of an appointment
    /// If the appointment is protected by a password, submit the credentials of the appointment via basic authentication http header.
    /// Please use the admin id as username and use the password of the appointment as password in the basic authentication header.
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <param name="adminId">admin id of the appointment</param>
    /// <param name="statusType">new status of the appointment</param>
    /// <response code="200">appointment updated</response>
    /// <response code="400">customerId or appointmentid was null or empty</response>
    /// <response code="401">the appointment is protected by a password and no password was submitted or the password was wrong</response>
    /// <response code="404">customerId and/or appointmentId was not found</response>
    /// <response code="409">the status of the appointment can not changed to the new status</response>
    /// <response code="500">Unexpected error</response>
    [HttpPut("{customerId}/{adminId}/{statusType}/status")]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [Consumes(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(typeof(Appointment), 200)]
    [ProducesResponseType(typeof(IErrorResult), 400)]
    [ProducesResponseType(typeof(IErrorResult), 401)]
    [ProducesResponseType(typeof(IErrorResult), 404)]
    [ProducesResponseType(typeof(IErrorResult), 409)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    [BasicAuthenticationOperation]
    public IActionResult SetStatus(string customerId, string adminId, string statusType)
    {
        Logger.LogDebug("Enter {NameofSetStatus}, Parameter: {CustomerId}, {AdminId}, {StatusType}", nameof(SetStatus),
            customerId, adminId, statusType);

        if (!Guid.TryParse(customerId, out Guid customerIdGuid)
            || !Guid.TryParse(adminId, out Guid adminIdGuid))
        {
            throw CreateBadRequestException(ErrorType.WrongInputOrNotAllowed);
        }

        ValidateReadAppointmentByAdminIdRequest(customerIdGuid, adminIdGuid, _appointmentBusinessLayer);

        Appointment appointmentObject = _appointmentBusinessLayer.SetAppointmentStatusType(customerIdGuid,
            adminIdGuid, statusType.ToEnum<AppointmentStatusType>());
        if (appointmentObject == null)
        {
            throw CreateConflictException(ErrorType.AppointmentStatusTypeNotAllowed);
        }

        return Ok(appointmentObject);
    }

    /// <summary>
    /// Validate application and customer request
    /// </summary>
    /// <param name="customerIdFromRequest"></param>
    /// <param name="adminId"></param>
    /// <param name="appointmentBusinessLayer">appointment business layer</param>
    private void ValidateReadAppointmentByAdminIdRequest(Guid customerIdFromRequest, Guid adminId,
        IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        ValidateReadAppointmentByAdminIdRequestHelper(customerIdFromRequest, adminId, appointmentBusinessLayer,
            skipPasswordVerification: false);
    }

    /// <summary>
    /// Validate application and customer request but skip the password verfication step
    /// </summary>
    /// <param name="customerIdFromRequest"></param>
    /// <param name="adminId"></param>
    /// <param name="appointmentBusinessLayer">appointment business layer</param>
    private void ValidateReadAppointmentByAdminIdRequestSkipPasswordVerfication(Guid customerIdFromRequest,
        Guid adminId, IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        ValidateReadAppointmentByAdminIdRequestHelper(customerIdFromRequest, adminId, appointmentBusinessLayer,
            skipPasswordVerification: true);
    }

    /// <summary>
    /// Validate application and customer request
    /// </summary>
    /// <param name="customerIdFromRequest"></param>
    /// <param name="adminId"></param>
    /// <param name="appointmentBusinessLayer">appointment business layer</param>
    /// <param name="skipPasswordVerification"></param>
    private void ValidateReadAppointmentByAdminIdRequestHelper(Guid customerIdFromRequest, Guid adminId,
        IAppointmentBusinessLayer appointmentBusinessLayer, bool skipPasswordVerification)
    {
        ValidateCustomerRequest(customerIdFromRequest, appointmentBusinessLayer);

        if (adminId == Guid.Empty)
        {
            throw CreateNotFoundRequestException(ErrorType.AdminIdNotFound);
        }

        if (!appointmentBusinessLayer.ExistsAppointmentByAdminId(customerIdFromRequest, adminId))
        {
            throw CreateNotFoundRequestException(ErrorType.AppointmentNotFound);
        }

        if (!skipPasswordVerification)
        {
            VerifyPasswordAuthenticationOfAppointmentByAdminId(customerIdFromRequest, adminId,
                appointmentBusinessLayer);
        }
    }
}