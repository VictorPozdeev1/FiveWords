using FiveWords._v1.BusinessLogic;
using FiveWords._v1.Endpoints;
using FiveWords._v1.Repository;
using FiveWords._v1.Utils;
using FiveWords._v1.View;
using Microsoft.AspNetCore.Rewrite;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings_my.json");
builder.Services.Configure<JsonSerializerOptions>("Internal", builder.Configuration.GetSection("JsonSerializerOptions:Internal"));
builder.Services.AddOptions<JsonSerializerOptions>("Web")
    .Bind(builder.Configuration.GetSection("JsonSerializerOptions:Web"))
    .Configure(options =>
    {
        if (bool.TryParse(builder.Configuration["JsonSerializerOptions:Web:UseCamelCaseForPropertyNaming"], out bool useCamelCase))
            if (useCamelCase)
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpFileSender>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromSeconds(3600));

//�����, ������, � ������ ���������� ��������� � ����� �������� ��� ����� IEnumerable, � ����� ������ .First(repo => repo.Language = Language.English/Russian) ?
builder.Services.AddSingleton<UserRepositoriesManager<IWordsRepository>, EnglishWordsUserRepositoriesManager>();
builder.Services.AddSingleton<RussianWordsUserRepositoriesManager>();
builder.Services.AddSingleton<TranslationUserChallengeCreator>();
builder.Services.AddSingleton<GuessRightVariantChallengeResult_HtmlViewCreator>();
builder.Services.AddSingleton<GuessRightVariant_UserAnswerAssessor>();

var app = builder.Build();

app.UseSession();

app.Use((context, next) =>
{
    Console.WriteLine($"Outer logger. PathBase: \"{context.Request.PathBase}\". Path: \"{context.Request.Path}\". MThreadId: {Thread.CurrentThread.ManagedThreadId}.");
    return next(context);
});

PathString v1PathBase = new PathString("/v1");
app.Map(v1PathBase, false, app =>
{
    app.Use((context, next) =>
    {
        Console.WriteLine($"v1 logger. PathBase: \"{context.Request.PathBase}\". Path: \"{context.Request.Path}\". MThreadId: {Thread.CurrentThread.ManagedThreadId}.");
        return next(context);
    });
    var staticFilesProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "static-files", "v1"));
    app.UseDefaultFiles(new DefaultFilesOptions()
    {
        FileProvider = staticFilesProvider,
        DefaultFileNames = new string[1] { "default-page/welcome.html" }
    });
    app.UseStaticFiles(new StaticFileOptions() { FileProvider = staticFilesProvider });

    app.UseRouting();
    app.UseEndpoints(routeBuilder => EndpointsV1.MapEndpointsV1(routeBuilder, v1PathBase));
});

// ����� �� ��� ������� ����� ���?
// ��������, ��� ������ ������� - ������, ��� ��� ��� ������������ � v2
app.UseRewriter(new RewriteOptions()
    .AddRewrite("^home$", "home.html", true)
    .AddRewrite(@"^dictionary-page/(.+)$", "html/dictionary.html", true)
    .AddRewrite(@"^guest-challenge-page$", "html/challenge.html", true)
    .AddRewrite(@"^challenge-page/(.+)$", "html/challenge.html", true));

app.Use((context, next) =>
{
    Console.WriteLine($"not-v1 logger. PathBase: \"{context.Request.PathBase}\". Path: \"{context.Request.Path}\". MThreadId: {Thread.CurrentThread.ManagedThreadId}.");
    return next(context);
});

app.Run();
