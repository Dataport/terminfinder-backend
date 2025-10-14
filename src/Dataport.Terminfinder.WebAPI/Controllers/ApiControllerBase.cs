using Dataport.Terminfinder.BusinessLayer;
using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.Common.Extension;
using Dataport.Terminfinder.WebAPI.Exceptions;
using Dataport.Terminfinder.WebAPI.RequestContext;
using Microsoft.AspNetCore.Mvc;

namespace Dataport.Terminfinder.WebAPI.Controllers;

/// <summary>
/// Base controller for all api operations
/// </summary>
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Logger
    /// </summary>
    [NotNull]
    public ILogger Logger { get; }

    /// <summary>
    /// Localizer
    /// </summary>
    [NotNull]
    public IStringLocalizer Localizer { get; }

    /// <summary>
    /// RequestContext
    /// </summary>
    [NotNull]
    private IRequestContext RequestContext { get; }

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="requestContext"></param>
    /// <param name="logger"></param>
    /// <param name="localizer"></param>
    /// <exception cref="ArgumentNullException">requestContext</exception>
    /// <exception cref="ArgumentNullException">logger</exception>
    /// <exception cref="ArgumentNullException">localizer</exception>
    protected ApiControllerBase(IRequestContext requestContext, ILogger logger, IStringLocalizer localizer)
    {
        RequestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));

        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        Localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    /// <summary>
    /// Create an UnauthorizedException
    /// </summary>
    /// <param name="errorCode"></param>
    /// <returns></returns>
    protected UnauthorizedException CreateUnauthorizedException(ErrorType errorCode)
    {
        return new UnauthorizedException(Localizer[errorCode.ToLocalisationId()], errorCode);
    }

    /// <summary>
    /// Create a NotFoundException
    /// </summary>
    /// <param name="errorCode"></param>
    /// <returns>BadRequestException</returns>
    protected NotFoundException CreateNotFoundRequestException(ErrorType errorCode)
    {
        return new NotFoundException(Localizer[errorCode.ToLocalisationId()], errorCode);
    }

    /// <summary>
    /// Create a BadRequestException
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="innerException"></param>
    /// <returns>BadRequestException</returns>
    protected BadRequestException CreateBadRequestException(ErrorType errorCode, Exception innerException = null)
    {
        // ReSharper disable once IntroduceOptionalParameters.Local
        return CreateBadRequestException(errorCode, additionalErrorMessage: null, innerException);
    }

    /// <summary>
    /// Create a BadRequestException
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="additionalErrorMessage"></param>
    /// <param name="innerException"></param>
    /// <returns>BadRequestException</returns>
    protected BadRequestException CreateBadRequestException(ErrorType errorCode,
        [CanBeNull] string additionalErrorMessage, Exception innerException = null)
    {
        return new BadRequestException(Localizer[errorCode.ToLocalisationId()]
                                       + (additionalErrorMessage == null ? string.Empty : $" {additionalErrorMessage}"),
            errorCode, innerException);
    }

    /// <summary>
    /// Create a ConflictException
    /// </summary>
    /// <param name="errorCode"></param>
    /// <returns></returns>
    protected ConflictException CreateConflictException(ErrorType errorCode)
    {
        // ReSharper disable once IntroduceOptionalParameters.Local
        return CreateConflictException(errorCode, additionalErrorMessage: null);
    }

    /// <summary>
    /// Create a ConflictException
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="additionalErrorMessage"></param>
    /// <param name="innerException"></param>
    /// <returns></returns>
    protected ConflictException CreateConflictException(ErrorType errorCode, [CanBeNull] string additionalErrorMessage,
        Exception innerException = null)
    {
        return new ConflictException(Localizer[errorCode.ToLocalisationId()]
                                     + (additionalErrorMessage == null ? string.Empty : $" {additionalErrorMessage}"),
            errorCode, innerException);
    }

    /// <summary>
    /// Get all unique error messages from the model state; If the model is valid an exception will be thrown
    /// </summary>
    /// <exception cref="InvalidOperationException">Model state contains no error messages (model state is valid)</exception>
    /// <returns>the error messages</returns>
    [NotNull]
    private IEnumerable<string> GetErrorMessagesFromModelState()
    {
        if (!ModelState.IsValid && ModelState.Keys.Any(k => ModelState[k]!.Errors.Any()))
        {
            var result = new HashSet<string>();
            foreach (var key in ModelState.Keys)
            {
                if (ModelState[key]!.Errors.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (var error in ModelState[key].Errors)
                {
                    var errorMessage = error.ErrorMessage;
                    result.Add(errorMessage);
                }
            }

            if (result.Count > 0)
            {
                return result;
            }
        }

        throw new ArgumentException($"{nameof(ModelState)} contains no errors and is valid");
    }

    /// <summary>
    /// Get all unique error messages from the model state joined to a single string; If the model is valid an exception will be thrown
    /// </summary>
    /// <exception cref="InvalidOperationException">Model state contains no error messages (model state is valid)</exception>
    /// <returns>the string with all error messages from the model state</returns>
    [NotNull]
    protected string BuildAdditionalErrorMessageFromModelState()
    {
        return string.Join(" ", GetErrorMessagesFromModelState());
    }

    #region Validate

    /// <summary>
    /// Validate customer request
    /// </summary>
    /// <param name="customerIdFromRequest"></param>
    /// <param name="appointmentBusinessLayer">appointment business layer</param>
    protected void ValidateCustomerRequest(Guid customerIdFromRequest,
        IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        if (customerIdFromRequest == Guid.Empty || appointmentBusinessLayer == null)
        {
            throw CreateNotFoundRequestException(ErrorType.CustomerIdNotFound);
        }

        if (!appointmentBusinessLayer.ExistsCustomer(customerIdFromRequest))
        {
            throw CreateNotFoundRequestException(ErrorType.CustomerIdNotFound);
        }
    }

    /// <summary>
    /// Validate application and customer request
    /// </summary>
    /// <param name="customerIdFromRequest"></param>
    /// <param name="appointmentId"></param>
    /// <param name="appointmentBusinessLayer">appointment business layer</param>
    protected void ValidateAppointmentRequest(Guid customerIdFromRequest, Guid appointmentId,
        IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        ValidateAppointmentRequestHelper(customerIdFromRequest, appointmentId, appointmentBusinessLayer);

        VerifyPasswordAuthenticationOfAppointment(customerIdFromRequest, appointmentId,
            appointmentBusinessLayer);
    }

    /// <summary>
    /// Validate application and customer request and check identical adminId
    /// </summary>
    /// <param name="customerIdFromRequest"></param>
    /// <param name="appointmentId"></param>
    /// <param name="adminId"></param>
    /// <param name="appointmentBusinessLayer">appointment business layer</param>
    protected void ValidateAppointmentRequest(Guid customerIdFromRequest, Guid appointmentId, Guid adminId,
        IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        ValidateAppointmentRequestHelper(customerIdFromRequest, appointmentId, adminId, appointmentBusinessLayer);

        VerifyPasswordAuthenticationOfAppointment(customerIdFromRequest, appointmentId,
            appointmentBusinessLayer);
    }

    /// <summary>
    /// Validate application and customer request
    /// </summary>
    /// <param name="customerIdFromRequest"></param>
    /// <param name="appointmentId"></param>
    /// <param name="appointmentBusinessLayer">appointment business layer</param>
    protected void ValidateAppointmentRequestStatusIsStarted(Guid customerIdFromRequest, Guid appointmentId,
        IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        ValidateAppointmentRequestHelperStatusHasToBeStarted(customerIdFromRequest, appointmentId,
            appointmentBusinessLayer);
    }

    /// <summary>
    /// Validate application and customer request but skip the password verification step
    /// </summary>
    /// <param name="customerIdFromRequest"></param>
    /// <param name="appointmentId"></param>
    /// <param name="appointmentBusinessLayer">appointment business layer</param>
    protected void ValidateAppointmentRequestSkipPasswordVerification(Guid customerIdFromRequest, Guid appointmentId,
        IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        ValidateAppointmentRequestHelper(customerIdFromRequest, appointmentId, appointmentBusinessLayer);
    }

    /// <summary>
    /// Validate application and customer request
    /// </summary>
    /// <param name="customerIdFromRequest"></param>
    /// <param name="appointmentId"></param>
    /// <param name="appointmentBusinessLayer">appointment business layer</param>
    private void ValidateAppointmentRequestHelper(Guid customerIdFromRequest, Guid appointmentId,
        IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        ValidateCustomerRequest(customerIdFromRequest, appointmentBusinessLayer);

        if (appointmentId == Guid.Empty)
        {
            throw CreateNotFoundRequestException(ErrorType.AppointmentIdNotFound);
        }

        if (!appointmentBusinessLayer.ExistsAppointment(customerIdFromRequest, appointmentId))
        {
            throw CreateNotFoundRequestException(ErrorType.AppointmentNotFound);
        }
    }

    /// <summary>
    /// Validate application and customer request, check adminId and appointmentId
    /// </summary>
    /// <param name="customerIdFromRequest"></param>
    /// <param name="appointmentId"></param>
    /// <param name="adminId"></param>
    /// <param name="appointmentBusinessLayer">appointment business layer</param>
    private void ValidateAppointmentRequestHelper(Guid customerIdFromRequest, Guid appointmentId, Guid adminId,
        IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        ValidateCustomerRequest(customerIdFromRequest, appointmentBusinessLayer);

        if (appointmentId == Guid.Empty || adminId == Guid.Empty)
        {
            throw CreateNotFoundRequestException(ErrorType.AppointmentIdNotFound);
        }

        if (!appointmentBusinessLayer.ExistsAppointment(customerIdFromRequest, appointmentId, adminId))
        {
            throw CreateNotFoundRequestException(ErrorType.AppointmentNotFound);
        }
    }

    /// <summary>
    /// Validate application and customer request and check status has be started
    /// </summary>
    /// <param name="customerIdFromRequest"></param>
    /// <param name="appointmentId"></param>
    /// <param name="appointmentBusinessLayer">appointment business layer</param>
    private void ValidateAppointmentRequestHelperStatusHasToBeStarted(Guid customerIdFromRequest, Guid appointmentId,
        IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        ValidateAppointmentRequestHelper(customerIdFromRequest, appointmentId, appointmentBusinessLayer);

        if (!appointmentBusinessLayer.ExistsAppointmentIsStarted(customerIdFromRequest, appointmentId))
        {
            throw CreateBadRequestException(ErrorType.AppointmentHasNotBeStarted);
        }

        VerifyPasswordAuthenticationOfAppointment(customerIdFromRequest, appointmentId,
            appointmentBusinessLayer);
    }

    /// <summary>
    /// Execute the verification process. The appointment will be identified by customer id and appointment id.
    /// </summary>
    private void VerifyPasswordAuthenticationOfAppointment(Guid customerIdFromRequest, Guid appointmentId,
        IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        if (appointmentBusinessLayer.IsAppointmentPasswordProtected(customerIdFromRequest, appointmentId))
        {
            var password = GetDecodedPasswordFromRequest();
            if (password == null)
            {
                throw CreateUnauthorizedException(ErrorType.PasswordRequired);
            }

            if (!appointmentBusinessLayer.VerifyAppointmentPassword(customerIdFromRequest, appointmentId, password))
            {
                throw CreateUnauthorizedException(ErrorType.AuthorizationFailed);
            }
        }
    }

    /// <summary>
    /// Execute the verification process. The appointment will be identified by customer id and admin id.
    /// </summary>
    protected void VerifyPasswordAuthenticationOfAppointmentByAdminId(Guid customerIdFromRequest, Guid adminId,
        IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        if (appointmentBusinessLayer.IsAppointmentPasswordProtectedByAdminId(customerIdFromRequest, adminId))
        {
            var password = GetDecodedPasswordFromRequest();
            if (password == null)
            {
                throw CreateUnauthorizedException(ErrorType.PasswordRequired);
            }

            if (!appointmentBusinessLayer.VerifyAppointmentPasswordByAdminId(customerIdFromRequest, adminId, password))
            {
                throw CreateUnauthorizedException(ErrorType.AuthorizationFailed);
            }
        }
    }

    /// <summary>
    /// Verification the password of the protected appointment identified by customer id and appointment id
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="appointmentId">Id of the appointment</param>
    /// <param name="appointmentBusinessLayer"></param>
    /// <returns>true if the password is valid and the appointment is protected by a password; otherwise false</returns>
    /// <exception cref="InvalidOperationException">will be thrown if the appointment does not exist</exception>
    protected bool IsAppointmentPasswordValid(Guid customerId, Guid appointmentId,
        IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        if (appointmentBusinessLayer.IsAppointmentPasswordProtected(customerId, appointmentId))
        {
            var password = GetDecodedPasswordFromRequest();
            if (password == null)
            {
                Logger.LogDebug($"The password is null");
                return false;
            }

            if (!appointmentBusinessLayer.VerifyAppointmentPassword(customerId, appointmentId, password))
            {
                Logger.LogDebug("The passwort is not equal");
                return false;
            }

            return true;
        }

        Logger.LogDebug("The application {AppointmentId} is not protected", appointmentId);
        return false;
    }

    /// <summary>
    /// Verification the password of the protected appointment identified by customer id and adminid
    /// </summary>
    /// <param name="customerId">Id of the customer</param>
    /// <param name="adminId">Id of the appointment</param>
    /// <param name="appointmentBusinessLayer"></param>
    /// <returns>true if the password is valid and the appointment is protected by a password; otherwise false</returns>
    /// <exception cref="InvalidOperationException">will be thrown if the appointment does not exist</exception>
    protected bool IsAppointmentPasswordValidByAdminId(Guid customerId, Guid adminId,
        IAppointmentBusinessLayer appointmentBusinessLayer)
    {
        if (appointmentBusinessLayer.IsAppointmentPasswordProtectedByAdminId(customerId, adminId))
        {
            var password = GetDecodedPasswordFromRequest();
            if (password == null)
            {
                Logger.LogDebug($"The password is null");
                return false;
            }

            if (appointmentBusinessLayer.VerifyAppointmentPasswordByAdminId(customerId, adminId, password))
            {
                return true;
            }

            Logger.LogDebug("The passwort is not equal");
            return false;
        }

        Logger.LogDebug("The application {AdminId} is not protected", adminId);
        return false;
    }

    /// <summary>
    /// Get the password from the request
    /// </summary>
    /// <returns>the decoded password or null (e.g. if the password does not exist in the request)</returns>
    /// <exception cref="BadRequestException">an error occurred when decoding the password from the request</exception>
    [CanBeNull]
    protected string GetDecodedPasswordFromRequest()
    {
        try
        {
            return RequestContext.GetDecodedBasicAuthCredentials()?.Password;
        }
        catch (DecodingBasicAuthenticationValueFailedException dfe)
        {
            Logger.LogDebug(dfe,
                "An error occurred while decoding the password from the basic authentication header of the request: {Message}", dfe.Message);
            throw CreateBadRequestException(ErrorType.DecodingPasswordFailed, dfe);
        }
    }

    #endregion Validate
}