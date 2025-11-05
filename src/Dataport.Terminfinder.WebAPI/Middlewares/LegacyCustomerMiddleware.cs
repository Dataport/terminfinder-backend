using Dataport.Terminfinder.Repository;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

namespace Dataport.Terminfinder.WebAPI.Middlewares;

/// <summary>
/// Handle Requests to legacy customers
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class LegacyCustomerMiddleware(
    RequestDelegate next,
    IHttpClientFactory clientFactory,
    ILogger<LegacyCustomerMiddleware> logger)
{
    private const string HeaderKeyForwardedFor = "X-Forwarded-For";

    /// <summary>
    /// Handle requests for customer or legacy customer
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="customerRepository"></param>
    /// <param name="legacyCustomerRepository"></param>
    public async Task Invoke(
        HttpContext ctx,
        ICustomerRepository customerRepository,
        ILegacyCustomerRepository legacyCustomerRepository)
    {
        var customerId = ExtractCustomerId(ctx.Request.Path);

        if (customerId == null
            || customerRepository.ExistsCustomer(customerId.Value)
            || !legacyCustomerRepository.ExistsLegacyCustomer(customerId.Value))
        {
            // normal handling
            await next(ctx);
            return;
        }

        var legacyCustomer = legacyCustomerRepository.GetLegacyCustomer(customerId.Value);
        if (legacyCustomer == null || legacyCustomer.CustomerId == Guid.Empty)
        {
            logger.LogWarning("Legacy customer with id '{CustomerId}' exists but couldn't be loaded", customerId.Value);
            await next(ctx);
            return;
        }

        var client = clientFactory.CreateClient("legacy");
        client.Timeout = TimeSpan.FromSeconds(30);

        var destination = $"{legacyCustomer!.HostAddress.TrimEnd('/')}{ctx.Request.Path}{ctx.Request.QueryString}";
        var legacyRequest = new HttpRequestMessage(new HttpMethod(ctx.Request.Method), destination);

        foreach (var header in ctx.Request.Headers)
        {
            legacyRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }

        // set header 'forwarded-for' to indicate where the request got forwarded from
        var remoteIpAddress = ctx.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(remoteIpAddress))
        {
            legacyRequest.Headers.TryAddWithoutValidation(HeaderKeyForwardedFor, remoteIpAddress);
        }

        if (ctx.Request.ContentLength > 0)
        {
            legacyRequest.Content = new StreamContent(ctx.Request.Body);
            if (!string.IsNullOrEmpty(ctx.Request.ContentType))
            {
                legacyRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(ctx.Request.ContentType);
            }
        }

        try
        {
            logger.LogInformation(
                "Request for a legacy customer with id '{CustomerId}' is being forwarded to '{Method}' '{Destination}'",
                customerId.Value, ctx.Request.Method, destination);

            using var legacyResponse =
                await client.SendAsync(legacyRequest, HttpCompletionOption.ResponseHeadersRead, ctx.RequestAborted);

            ctx.Response.StatusCode = (int)legacyResponse.StatusCode;

            foreach (var header in legacyResponse.Headers)
            {
                ctx.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in legacyResponse.Content.Headers)
            {
                ctx.Response.Headers[header.Key] = header.Value.ToArray();
            }

            await legacyResponse.Content.CopyToAsync(ctx.Response.Body, ctx.RequestAborted);

            logger.LogInformation(
                "Request to legacy customer with id '{CustomerId}' was successfully forwarded. The server responded with status code '{StatusCode}'.",
                customerId.Value, legacyResponse.StatusCode);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Connection to legacy customer id '{CustomerId}' couldn't be established.",
                customerId.Value);
            await next(ctx);
        }
    }

    private static Guid? ExtractCustomerId(PathString path)
    {
        var segments = path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments is not { Length: > 1 })
        {
            return null;
        }

        if (Guid.TryParse(segments[1], out var customerId))
        {
            return customerId;
        }

        return null;
    }
}