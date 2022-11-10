using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using FiveWords.Repository;
using FiveWords.Repository.Interfaces;
using FiveWords.Model;
using FiveWords.View;
using FiveWords.Repository.CsvRepository;
using FiveWords.Utils;
using FiveWords.Api;
using FiveWords.Api.v1;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder();

builder.Configuration.AddJsonFile("appsettings_my.json");
builder.Services.Configure<JsonSerializerOptions>("Internal", builder.Configuration.GetSection("JsonSerializerOptions:Internal"));
builder.Services.AddOptions<JsonSerializerOptions>("Web")
    .Bind(builder.Configuration.GetSection("JsonSerializerOptions:Web"))
    .Configure(options => {
        if (bool.TryParse(builder.Configuration["JsonSerializerOptions:Web:UseCamelCaseForPropertyNaming"], out bool useCamelCase))
            if (useCamelCase)
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddAuthorization();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromSeconds(3600));
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<FiveWords.Utils.HttpFileSender>();

builder.Services.AddSingleton<IUsersRepository, UsersCsvRepository>((services) => new UsersCsvRepository("users-list", "users-list.csv"));
builder.Services.AddSingleton<UserPasswordRepositoriesManager>();
builder.Services.AddSingleton<UserDictionariesUserRepositoriesManager>();

//todo �����, ������, � ������ ���������� ��������� � ����� �������� ��� ����� IEnumerable, � ����� ������ .First(repo => repo.Language = Language.English/Russian) ?
//builder.Services.AddSingleton<UserRepositoriesManager<IWordsRepository>, EnglishWordsUserRepositoriesManager>();
builder.Services.AddSingleton<UserRepositoriesManager<IWordsRepository>, EnglishWordsUserRepositoriesManager>();
builder.Services.AddSingleton<RussianWordsUserRepositoriesManager>();
builder.Services.AddSingleton<TranslationUserChallengeCreator>();
builder.Services.AddSingleton<GuessRightVariantChallengeResult_HtmlViewCreator>();
builder.Services.AddSingleton<GuessRightVariant_UserAnswerAssessor>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //todo ������� ������ � ������������ (� ����� ���� � Options?),
        //� ��������� ���� ������ ����� ������� � �������� ����, � ����� File.ReadAllBytes(FilePath)
        ValidIssuer = "FiveWords",
        ValidAudience = "FiveWords",
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("uig284hnj&*#^%\\\"34.h567&y"))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddControllers();

var services = builder.Services;
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Press Esc to clear console before the next request processing.");
    app.UseDeveloperExceptionPage();
    app.Use((context, next) => {
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

app.UseRewriter(new Microsoft.AspNetCore.Rewrite.RewriteOptions()
    .AddRewrite("^home$", "home.html", true)
    .AddRewrite(@"^dictionary-page/(.+)$", "html/dictionary.html", true));

app.Use((context, next) =>
{
    Console.WriteLine($"not-v1 logger. PathBase: \"{context.Request.PathBase}\". Path: \"{context.Request.Path}\". MThreadId: {Thread.CurrentThread.ManagedThreadId}.");
    return next(context);
});


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
app.MapGet("/services", (HttpContext context)=> ServiceInfo.PrintDIServices(services)).WithTags("INFO");
app.UseEndpoints(_ => { }); //(routeBuilder => FiveWords.EndpointMapper.MapEndpoints(routeBuilder));

app.Run();

//async Task DefaultRequestHandler(HttpContext context)
//{
//    var path = context.Request.Path;
//    switch (path)
//    {
//        case "/":
//            context.Response.Redirect("welcome");
//            break;
//        case "/welcome":
//            context.Response.ContentType = "text/html; charset=utf-8";
//            await context.Response.SendFileAsync("html/welcome.html");
//            break;
//        case "/default-challenge-page":
//            context.Response.ContentType = "text/html; charset=utf-8";
//            await context.Response.SendFileAsync("html/five-words-challenge.html");
//            break;
//        case "/challenge/guess-translate":
//            var query = context.Request.Query;
//            if (byte.TryParse(query["UnitsCount"], out byte unitsCount))
//                if (byte.TryParse(query["AnswerVariantsCount"], out byte answerVariantsCount))
//                {
//                    var currentUser = User.Default;
//                    var userChallenge = UserChallengeCreator.CreateGuessTranslateChallenge(currentUser, unitsCount, answerVariantsCount);
//                    //using (var tempFile = new FileStream("temp.json", FileMode.OpenOrCreate))
//                    //{
//                    //    var options = new JsonSerializerOptions();
//                    //    options.WriteIndented = true;
//                    //    await JsonSerializer.SerializeAsync(tempFile, userChallenge, userChallenge.GetType(), options);
//                    //}
//                    string userChallengeJson = JsonSerializer.Serialize(userChallenge);
//                    //GuessTranslate_UserChallenge? userChallengeSaved =
//                    //    JsonSerializer.Deserialize<GuessTranslate_UserChallenge>(userChallengeJson);
//                    //JsonElement? test = JsonSerializer.Deserialize<JsonElement>(userChallengeJson);
//                    context.Session.SetString("CurrentUserChallenge", userChallengeJson);
//                    await context.Response.WriteAsJsonAsync(userChallenge);
//                }
//            break;
//        case "/challenge-result/guess-translate":
//            switch (context.Request.Method)
//            {
//                case "POST":
//                    string? userChallengeSavedJson = context.Session.GetString("CurrentUserChallenge");
//                    GuessTranslate_UserChallenge? userChallengeSaved = JsonSerializer.Deserialize<GuessTranslate_UserChallenge>(userChallengeSavedJson);
//                    byte rightAnswersCounter = 0;
//                    try
//                    {
//                        int[] userAnswers = await context.Request.ReadFromJsonAsync<int[]>();
//                        for (byte i = 0; i < userAnswers.Length; i++)
//                            if (userAnswers[i] == userChallengeSaved.Units[i].RightAnswerIndex)
//                                rightAnswersCounter++;
//                    }
//                    catch (Exception ex)
//                    {
//                        throw;
//                    }

//                    context.Response.ContentType = "text/html; charset=utf-8";
//                    //await context.Response.SendFileAsync("html\\challenge-result.html");

//                    HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
//                    try
//                    {
//                        using (var fileStream = File.Open("html/challenge-result.html", FileMode.Open))
//                        {
//                            htmlDocument.Load(fileStream);
//                        }
//                        var mainLabel = htmlDocument.GetElementbyId("result-text");//htmlDocument.DocumentNode.SelectSingleNode("//*[@id='result-text']");
//                        mainLabel.InnerHtml = $"���������� �������: {rightAnswersCounter}.";
//                        var whatsMoreLabel = htmlDocument.GetElementbyId("whats-more");
//                        whatsMoreLabel.InnerHtml = "(��� �����-������ ����� ����������� ������������������, ����� ��������� ����������� ������� � ��������� ����������, ����� ����������, � �. �.)";
//                        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
//                        using (StringWriter writer = new StringWriter(stringBuilder))
//                        {
//                            htmlDocument.Save(writer);
//                            writer.Flush();
//                        }
//                        await context.Response.WriteAsync(stringBuilder.ToString());
//                    }
//                    catch (Exception ex) { }
//                    //await context.Response.()

//                    break;
//            }
//            break;
//        default:
//            if (File.Exists(path.Value?.TrimStart('/')))
//                await context.Response.SendFileAsync(path.Value.TrimStart('/'));
//            else
//                context.Response.StatusCode = 404;
//            break;
//    }
//}