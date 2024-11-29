using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.BusinessObject.Error;
using Dataport.Terminfinder.WebAPI.Constants;
using Dataport.Terminfinder.WebAPI.Exceptions;
using System.Net;

namespace Dataport.Terminfinder.WebAPI.ErrorHandling;

/// <summary>
/// Error handling middleware
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly IStringLocalizer _localizer;

    /// <summary>
    /// Constructor with all dependencies
    /// </summary>
    /// <param name="next"></param>
    /// <param name="loggerFactory"></param>
    /// <param name="localizer"></param>
    // ReSharper disable once UnusedMember.Global
    public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory,
        IStringLocalizer<ErrorMessageResources> localizer)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<ErrorHandlingMiddleware>();
        _localizer = localizer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "An error occurred during the request: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        try
        {
            var httpStatusCode = HttpStatusCode.InternalServerError; // 500 if unexpected
            var errorCode = ErrorType.GeneralError;
            string errorMessage = _localizer["An internal server error occurred."];

            if (exception is RestApiException restApiException)
            {
                errorCode = restApiException.ErrorCode;

                if (exception is BadRequestException badRequestException)
                {
                    httpStatusCode = HttpStatusCode.BadRequest;
                    errorMessage = $"{_localizer["The request contains semantic or syntactical errors."]} "
                                   + $"{badRequestException.LocalizedErrorMessage}";
                }
                else if (restApiException is NotFoundException)
                {
                    httpStatusCode = HttpStatusCode.NotFound;
                    // No need for additional information in error message due to limited number of parameters in request
                    errorMessage = _localizer["The requested resource does not exist."];
                }
                else if (restApiException is ConflictException)
                {
                    httpStatusCode = HttpStatusCode.Conflict;
                    errorMessage = _localizer["The status of the appointment are not allowed in this context."];
                }
                else if (restApiException is UnauthorizedException)
                {
                    httpStatusCode = HttpStatusCode.Unauthorized;
                    errorMessage =
                        _localizer["The authentication is required and has failed or has not yet been provided."];
                }
            }

            if (ShouldLogExceptionAsError(httpStatusCode))
            {
                _logger.LogError(exception, "An error occurred: {Message}", exception.Message);
            }

            var error = new ErrorResult
            {
                Language = Thread.CurrentThread.CurrentCulture.Name,
                Code = errorCode.ToFormattedErrorNumber(),
                Message = errorMessage
            };

            string result = JsonConvert.SerializeObject(error);
            context.Response.ContentType = HttpConstants.TerminfinderMediaTypeJsonV1WithUtf8CharsetSuffix;
            context.Response.StatusCode = (int)httpStatusCode;
            return context.Response.WriteAsync(result);
        }
        catch (Exception innerException)
        {
            _logger.LogError("An error occurred during handling an exception: {InnerExceptionMessage}", innerException.Message);
            _logger.LogError("The submitted exception is: {ExceptionMessage}", exception?.Message);
            throw;
        }
    }

    private static bool ShouldLogExceptionAsError(HttpStatusCode code)
    {
        return code != HttpStatusCode.BadRequest
               && code != HttpStatusCode.NotFound;
    }
}