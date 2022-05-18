using Microsoft.OpenApi.Models;
using WebAPINet6.WebApi.Middleware;
using WebAPINet6.BusinessLogic.Model;
using WebAPINet6.BusinessLogic.Services;
using WebAPINet6.BusinessLogic.Enumerations;
using WebAPINet6.BusinessLogic.Services.Background;
using WebAPINet6.BusinessLogic.Services.Interfaces;

WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

// Set configuration
var environment = webApplicationBuilder.Environment;
webApplicationBuilder.Configuration.AddJsonFile("appsettings.json", optional: true)
                                   .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);

ConfigureLogging(webApplicationBuilder.Logging, webApplicationBuilder.Configuration, webApplicationBuilder.Environment);

// Add services to the container.
ConfigureServices(webApplicationBuilder.Services, webApplicationBuilder.Configuration);

WebApplication webApplication = webApplicationBuilder.Build();

ConfigureMiddleware(webApplication, webApplication.Services, webApplication.Environment);
ConfigureEndpoints(webApplication);

webApplication.Run();


// Methods

static void ConfigureLogging(ILoggingBuilder logging, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
{
    logging.ClearProviders();
    logging.AddConfiguration(configuration.GetSection("Logging"));

    if (hostEnvironment.EnvironmentName != Environments.Development) return;

    logging.AddFile();
}

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{

    services.AddControllers().AddXmlSerializerFormatters();

    services.Configure<TTWSConfiguration>(configuration.GetSection(nameof(TTWSConfiguration)));

    services.AddHttpClient("Client");
    services.AddSingleton<Keys>();
    services.AddScoped<IClient, Client>();
    services.AddScoped<IXmlParser, XmlParser>();
    services.AddScoped<IDataTaker, DataTaker>();
    services.AddScoped<WebAPINet6.BusinessLogic.Repository.IClient, WebAPINet6.BusinessLogic.Repository.Client>();

    // dodaje singleton IMemoryCache
    services.AddMemoryCache();

    // dodaje singleton Logging MW
    services.AddSingleton<LoggingMiddleware>();

    // dodaje singleton Background task
    services.AddHostedService<CacheUpdater>();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
    });
}

static void ConfigureMiddleware(IApplicationBuilder builder, IServiceProvider provider, IWebHostEnvironment environment)
{
    if (!environment.IsProduction())
        builder.UseDeveloperExceptionPage();
    
    builder.UseSwagger(); 
    builder.UseSwaggerUI();

    builder.UseMiddleware<LoggingMiddleware>();

    builder.UseHttpsRedirection();

    builder.UseAuthorization();
}

static void ConfigureEndpoints(IEndpointRouteBuilder app)
{
    app.MapControllers();
}