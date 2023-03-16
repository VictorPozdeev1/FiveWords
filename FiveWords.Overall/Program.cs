using FiveWords.Api;
using FiveWords.Infrastructure.Authentication;
using FiveWords.Infrastructure.TelegramAlerting;
using FiveWords.Overall.Infrastructure.RabbitMQ;
using FiveWords.Overall.Utils;
using FiveWords.Repository;
using FiveWords.Repository.Csv;
using FiveWords.Repository.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder();

Log.Logger = new LoggerConfiguration()
    .WriteTo.File(Path.Combine("log", "default-log.txt"))
    .CreateLogger();

builder.Configuration.AddJsonFile("appsettings_my.json");

builder.Services.Configure<ForwardedHeadersOptions>(options =>
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);

builder.Services.Configure<TelegramNotifierOptions>(builder.Configuration.GetSection("TelegramAlerts"));
builder.Services.AddScoped<TelegramNotifierProvider>();

builder.Services.Configure<JsonSerializerOptions>("Internal", builder.Configuration.GetSection("JsonSerializerOptions:Internal"));
builder.Services.AddOptions<JsonSerializerOptions>("Web")
    .Bind(builder.Configuration.GetSection("JsonSerializerOptions:Web"))
    .Configure(options =>
    {
        if (bool.TryParse(builder.Configuration["JsonSerializerOptions:Web:UseCamelCaseForPropertyNaming"], out bool useCamelCase))
            if (useCamelCase)
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.Configure<RabbitQueuesOptions>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddAuthorization();

//builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IUsersRepository, UsersCsvRepository>((services) => new UsersCsvRepository("users-list", "users-list.csv"));
builder.Services.AddSingleton<UserPasswordRepositoriesManager>();
builder.Services.AddSingleton<UserDictionariesUserRepositoriesManager>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromSeconds(3600));

builder.Services.AddJwtAuthentication(builder.Configuration.GetSection("JwtAuthentication:TokenValidationParameters"));
builder.Services.AddAuthorization();

builder.Services.AddScoped(serviceProvider =>
    new JwtCreator(serviceProvider.GetRequiredService<IConfiguration>().GetSection("JwtAuthentication:TokenIssuingParameters")));

builder.Services.AddControllers(options => { /*options.ModelBinderProviders.Insert(0, new WordTranslationsFromFile_ModelBinderProvider());*/ });

var services = builder.Services;
var app = builder.Build();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Press Esc to clear console before the next request processing.");
    app.UseDeveloperExceptionPage();
    app.Use((context, next) =>
    {
        if (Console.KeyAvailable)
        {
            var consoleKey = Console.ReadKey(false).Key;
            if (consoleKey == ConsoleKey.Escape)
                Console.Clear();
        }
        return next(context);
    });

}
//else
//    app.UseExceptionHandler();

app.UseSession();

// Сделать по-хорошему, как здесь? https://github.com/serilog/serilog-aspnetcore/blob/dev/samples/Sample/Program.cs
DateTime currentLogFileDate = DateTime.MinValue;
Serilog.Core.Logger? plainRequestLogger = null;
app.Use(async (context, next) =>
{
    try
    {
        var currentDate = DateTime.UtcNow.Date;
        if (currentLogFileDate != currentDate)
        {
            if (plainRequestLogger != null)
                _ = plainRequestLogger.DisposeAsync();
            plainRequestLogger = new LoggerConfiguration()
            .WriteTo.File(Path.Combine("log", "plain-request-log", $"{currentDate:yyyy-MM-dd}.txt"))
            .CreateLogger();
            currentLogFileDate = currentDate;
        }
        plainRequestLogger!.Information
        (
            "Processing request {RequestId} {HttpVerb} {RequestPath} from {RemoteIP} auth by {Token}",
            context.TraceIdentifier,
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress,
            context.Request.Headers["Authorization"]
        );
    }
    catch (Exception exc)
    {
        Log.Logger.Error(exc, "Error occured during logging a request.");
    }

    await next(context);

    try
    {
        plainRequestLogger!.Information
        (
            "Processed request {RequestId} {HttpVerb} {RequestPath} from {RemoteIP} authed as {UserName} responded {HttpResponseCode}",
            context.TraceIdentifier,
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress,
            context.User.Identity!.Name,
            context.Response.StatusCode
        );
    }
    catch (Exception exc)
    {
        Log.Logger.Error(exc, "Error occured during logging a request.");
    }
});

app.UseRewriter(new RewriteOptions()
    .AddRewrite("^home$", "home.html", true)
    .AddRewrite(@"^dictionary-page/(.+)$", "html/dictionary.html", true)
    .AddRewrite(@"^guest-challenge-page$", "html/challenge.html", true)
    .AddRewrite(@"^challenge-page/(.+)$", "html/challenge.html", true));

var staticFilesProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "static-files", "v2"));
//var staticFilesProvider = new MyFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "static-files", "v2"));
app.UseDefaultFiles(new DefaultFilesOptions()
{
    FileProvider = staticFilesProvider,
    //DefaultFileNames = new string[1] { "default-page/welcome.html" }
});
app.UseStaticFiles(new StaticFileOptions() { FileProvider = staticFilesProvider });

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Endpoints.MapEndpoints(app);
//app.MapGet("/services", (HttpContext context) => ServiceInfo.PrintDIServices(services)).WithTags("INFO");
app.UseEndpoints(_ => { }); //(routeBuilder => FiveWords.EndpointMapper.MapEndpoints(routeBuilder));

app.Run();