using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Dataport.Terminfinder.BusinessLayer;
using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.BusinessObject.Error;
using Dataport.Terminfinder.WebAPI.Constants;
using Dataport.Terminfinder.WebAPI.RequestContext;

namespace Dataport.Terminfinder.WebAPI.Controllers;

/// <summary>
/// App Controller Class
/// </summary>
[Route("app")]
public class AppController : ApiControllerBase
{
    private readonly IAppConfigBusinessLayer _appConfigBusinessLayer;

    /// <summary>
    /// App Controller
    /// </summary>
    /// <param name="businessLayer"></param>
    /// <param name="requestContext"></param>
    /// <param name="logger"></param>
    /// <param name="localizer"></param>
    /// <exception cref="ArgumentNullException">businessLayer</exception>
    public AppController(IAppConfigBusinessLayer businessLayer, IRequestContext requestContext,
        ILogger<AppController> logger, IStringLocalizer<AppController> localizer)
        : base(requestContext, logger, localizer)
    {
        Logger.LogDebug($"Enter {nameof(AppController)}");

        _appConfigBusinessLayer = businessLayer ?? throw new ArgumentNullException(nameof(businessLayer));
    }

    /// <summary>
    /// Get versionnumber and builddate
    /// </summary>
    /// <returns>Versionnumer and builddate</returns>
    /// <response code="200">AppInfo returned</response>
    /// <response code="500">Versionnumber was not found</response>
    [HttpGet()]
    [Produces(HttpConstants.TerminfinderMediaTypeJsonV1)]
    [ProducesResponseType(typeof(AppInfo), 200)]
    [ProducesResponseType(typeof(IErrorResult), 500)]
    public IActionResult Get()
    {
        try
        {
            Logger.LogDebug($"Enter {nameof(Get)}");

            AppInfo info = _appConfigBusinessLayer.GetAppInfo();
            if (info == null)
            {
                string errorMessage = $"ErrorCode{string.Format("{0:d4}", (int)ErrorType.VersionNumberNotFound)}";
                string language = Thread.CurrentThread.CurrentCulture.Name;

                ErrorResult errorObject = new()
                {
                    Code = ((int)ErrorType.VersionNumberNotFound).ToString(),
                    Message = Localizer[errorMessage],
                    Language = language
                };
                Logger.LogError("Error {NameofGet}: {ErrorObjectCode},{ErrorObjectMessage}", nameof(Get), errorObject.Code, errorObject.Message);

                return StatusCode(500, errorObject);
            }
            else
            {
                return Ok(info);
            }
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
            Logger.LogError(e, "Error {NameofGet}: {ErrorObjectCode},{ErrorObjectMessage}", nameof(Get), errorObject.Code, errorObject.Message);

            return StatusCode(500, errorObject);
        }
        finally
        {
            Logger.LogDebug($"Leave {nameof(Get)}");
        }
    }
}