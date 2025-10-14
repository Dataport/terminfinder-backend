using Dataport.Terminfinder.BusinessLayer;
using Dataport.Terminfinder.BusinessLayer.Security;
using Dataport.Terminfinder.Common;
using Dataport.Terminfinder.Repository;
using Dataport.Terminfinder.Repository.Setup;
using Dataport.Terminfinder.WebAPI.Constants;
using Dataport.Terminfinder.WebAPI.ErrorHandling;
using Dataport.Terminfinder.WebAPI.Localisation;
using Dataport.Terminfinder.WebAPI.RequestContext;
using Dataport.Terminfinder.WebAPI.Swagger;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Dataport.Terminfinder.WebAPI;

/// <summary>
/// Startup-Class
/// </summary>
[ExcludeFromCodeCoverage]
public class Startup
{
    private static readonly string EnUsCulture = "en-US";

    private static readonly string ContactUrl = "https://www.dataport.de";
    private static readonly string ContactEmail = "dataportdabstimmbox@dataport.de";
    private static readonly string ContactName = "dataport";
    private static readonly string LicenseText = "EUPL-1.2 Copyright © 2022-2023 Dataport AöR";
    private static readonly string LicenseUri = "https://opensource.org/licenses/EUPL-1.2";

    private static readonly string OpenApiTitle = "Terminfinder API";
    private static readonly string OpenApiDescription = "Terminfinder API";

    private readonly bool _useCors;
    private readonly bool _useHttps;

    /// <summary>
    /// Startup WebApplication
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="environment"></param>
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        WebHostingEnvironment = environment;

        var cors = Configuration["Terminfinder:UseCors"] ?? "false";
        _useCors = cors.ToLower() == "true";
        var https = Configuration["Terminfinder:UseHttps"] ?? "false";
        _useHttps = https.ToLower() == "true";
    }

    /// <summary>
    /// Configuration
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Environment
    /// </summary>
    public IWebHostEnvironment WebHostingEnvironment { get; }

    /// <summary>
    /// ConfigureServices
    /// </summary>
    /// <remarks>This method gets called by the runtime. Use this method to add services to the container.</remarks>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IAppointmentRepository, AppointmentRepository>();
        services.AddTransient<IAppConfigRepository, AppConfigRepository>();
        services.AddTransient<ICustomerRepository, CustomerRepository>();
        services.AddTransient<IAppConfigRepository, AppConfigRepository>();
        services.AddTransient<IMigrationManager, MigrationManager>();
        services.AddTransient<IAppointmentBusinessLayer, AppointmentBusinessLayer>();
        services.AddTransient<IAppConfigBusinessLayer, AppConfigBusinessLayer>();
        services.AddTransient<ICustomerBusinessLayer, CustomerBusinessLayer>();
        services.AddTransient<ISaltGenerator, SaltGenerator>();
        services.AddTransient<IBcryptWrapper, BcryptWrapper>();
        services.AddSingleton<IRequestContext, RequestContextAdapter>();
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        services.AddLogging();

        if (WebHostingEnvironment.IsDevelopment())
        {
            Console.WriteLine(@"warning: application is running in development mode!");
        }

        // In the production deployment we configure that via the ingress controller.
        // to override the cors setting, set configuration to true
        if (WebHostingEnvironment.IsDevelopment() || _useCors)
        {
            services.AddCors();
        }

        services.AddRouting();
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Version = AssemblyUtils.GetProductVersion(GetType().Assembly),
                    Title = OpenApiTitle,
                    Description = OpenApiDescription,
                    Contact = new OpenApiContact()
                    {
                        Name = ContactName,
                        Email = ContactEmail,
                        Url = new Uri(ContactUrl)
                    },
                    License = new OpenApiLicense()
                    {
                        Name = LicenseText,
                        Url = new Uri(LicenseUri)
                    }
                });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            s.IncludeXmlComments(xmlPath);
            s.SchemaFilter<EnumTypesSchemaFilter>(xmlPath);

            var xmlPathBusinessObjects = "Dataport.Terminfinder.BusinessObject.xml";
            var projectBusinessObjectXmlPath = Path.Combine(AppContext.BaseDirectory, xmlPathBusinessObjects);
            s.IncludeXmlComments(projectBusinessObjectXmlPath);
            s.SchemaFilter<EnumTypesSchemaFilter>(projectBusinessObjectXmlPath);

            // Add basic authentication
            s.AddSecurityDefinition("basic", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                In = ParameterLocation.Header,
                Description = "Basic Authorization header using the Bearer scheme."
            });

            s.OperationFilter<BasicAuthOperationFilter>();
        }).AddSwaggerGenNewtonsoftSupport();
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        // supported language
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en-GB"),
                new CultureInfo(EnUsCulture),
                new CultureInfo("de-DE")
            };

            // Formatting numbers, dates, etc.
            options.SupportedCultures = supportedCultures;
            // UI strings that we have localized.
            options.SupportedUICultures = supportedCultures;
            options.DefaultRequestCulture = new RequestCulture(culture: EnUsCulture, uiCulture: EnUsCulture);
        });

        services.AddControllers(config =>
            {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
                config.MaxModelValidationErrors = 20;

                // Manipulate the default InputFormatter for JSON to support a custom media type. It will probably not changed.
                // This was done because of swashbuckle does not evaluate the meda type from the consume attribute.
                // s. https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/213
                if (config.InputFormatters.FirstOrDefault(s => s is SystemTextJsonInputFormatter) is
                    SystemTextJsonInputFormatter formatter)
                {
                    formatter.SupportedMediaTypes.Add(
                        MediaTypeHeaderValue.Parse(HttpConstants.TerminfinderMediaTypeJsonV1));
                }
            })
            .AddNewtonsoftJson(options => { options.SerializerSettings.Converters.Add(new StringEnumConverter()); })
            .AddDataAnnotationsLocalization();

        services.AddSingleton<IValidationAttributeAdapterProvider, CustomValidationAttributeAdapterProvider>();
        var connectionString = Configuration["ConnectionStrings:TerminfinderConnection"];
        services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(connectionString);
            if (WebHostingEnvironment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });
    }

    /// <summary>
    /// Configure
    /// </summary>
    /// <remarks>This method gets called by the runtime. Use this method to configure the HTTP request pipeline.</remarks>
    /// <param name="app"></param>
    /// <param name="logger"></param>
    public void Configure(IApplicationBuilder app, ILoggerFactory logger)
    {
        var log4NetFilename = Configuration["Terminfinder:Log4NetConfigFilename"];
        if (!string.IsNullOrEmpty(log4NetFilename))
        {
            logger.AddLog4Net(log4NetFilename);
        }
        else
        {
            logger.AddLog4Net();
        }

        if (WebHostingEnvironment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"./swagger/v1/swagger.json", "Terminfinder API V1");
                c.RoutePrefix = string.Empty;
            });
        }

        // In the production deployment we configure that via the ingress controller.
        // to override this setting, set configuration to true
        if (WebHostingEnvironment.IsProduction() && _useHttps)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        var requestLocalizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
        if (requestLocalizationOptions != null)
        {
            app.UseRequestLocalization(requestLocalizationOptions.Value);
        }

        app.UseStaticFiles();

        app.UseRouting();

        // Shows UseCors with CorsPolicyBuilder.
        // In the production deployment we configure that via the ingress controller.
        // to override this setting, set configuration to true
        if (WebHostingEnvironment.IsDevelopment() || _useCors)
        {
            app.UseCors(builder =>
                builder.WithOrigins()
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            app.UseCors();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        // custom error handler
        app.UseMiddleware(typeof(ErrorHandlingMiddleware));

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
            context.Response.Headers.Append("X-Xss-Protection", "1; mode=block");
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
            await next();
        });

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
