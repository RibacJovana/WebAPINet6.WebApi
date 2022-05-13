using Microsoft.OpenApi.Models;
using WebAPINet6.WebApi.Middleware;
using WebAPINet6.BusinessLogic.Enumerations;
using WebAPINet6.BusinessLogic.Services;
using WebAPINet6.BusinessLogic.Services.Background;
using WebAPINet6.BusinessLogic.Model;

WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

// Set configuration
var environment = webApplicationBuilder.Environment;
webApplicationBuilder.Configuration.AddJsonFile("appsettings.json", optional: true)
                  .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);

// Add services to the container.
ConfigureServices(webApplicationBuilder.Services, webApplicationBuilder.Configuration);

WebApplication webApplication = webApplicationBuilder.Build();

ConfigureMiddleware(webApplication, webApplication.Services, webApplication.Environment);
ConfigureEndpoints(webApplication);

webApplication.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{

    services.AddControllers().AddXmlSerializerFormatters();

    services.Configure<TTWSConfiguration>(configuration.GetSection(nameof(TTWSConfiguration)));

    services.AddHttpClient<WebAPINet6.BusinessLogic.Repository.IClient, WebAPINet6.BusinessLogic.Repository.Client>();

    // dodaje singleton klasu IMemoryCache
    services.AddMemoryCache();

    services.AddSingleton<Keys>();
    services.AddScoped<IClient, Client>();
    services.AddScoped<IXmlParser, XmlParser>();
    services.AddScoped<MyMiddleware>();
    services.AddScoped<ICacheTaker, CacheTaker>();
    services.AddScoped<IClientTaker, ClientTaker>();

    // dodavanje background task
    services.AddHostedService<CacheData>();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();

    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
    });
}

static void ConfigureMiddleware(IApplicationBuilder builder, IServiceProvider provider, IWebHostEnvironment environment)
{
    builder.UseSwagger(); 
   
    builder.UseSwaggerUI();

    

    builder.UseHttpsRedirection();

    builder.UseMiddleware<MyMiddleware>();

    builder.UseAuthorization();
}

static void ConfigureEndpoints(IEndpointRouteBuilder app)
{
    app.MapControllers();
}