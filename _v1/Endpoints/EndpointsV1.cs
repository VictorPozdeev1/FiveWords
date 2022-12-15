using Microsoft.Extensions.Options;
using System.Text.Json;
using FiveWords.DataObjects;
using FiveWords._v1.DataObjects;
using FiveWords._v1.View;
using FiveWords._v1.BusinessLogic;

namespace FiveWords._v1.Endpoints;

static class EndpointsV1
{
    public static void MapEndpointsV1(IEndpointRouteBuilder routeBuilder, PathString v1PathBase)
    {
        //routeBuilder.MapGet("/routes", ServiceInfo.GetRoutes);
        routeBuilder.MapGet("/default-challenge-page", () => Results.File(
                    Path.Combine(Directory.GetCurrentDirectory(), "static-files/v1/five-words-challenge.html"), contentType: "text/html; charset=utf-8"))
            .WithTags(v1PathBase);
        //(FiveWords.Utils.HttpFileSender fileSender) => fileSender.SendHtmlFileAsync("static-files/five-words-challenge.html"));

        routeBuilder.MapGet("/challenge/guess-translate/{unitsCount:int:range(1,15)}:{answerVariantsCount:int:range(2,8)}", (HttpContext httpContext, TranslationUserChallengeCreator userChallengeCreator, byte unitsCount, byte answerVariantsCount, IOptionsSnapshot<JsonSerializerOptions> serializeOptions) =>
        {
            var currentUser = User.Default;
            var userChallenge = userChallengeCreator.CreateGuessTranslateChallenge(currentUser, unitsCount, answerVariantsCount);
            httpContext.Session.SetString("CurrentUserChallenge", JsonSerializer.Serialize(userChallenge, serializeOptions.Get("Internal")));
            return Results.Content(JsonSerializer.Serialize(userChallenge, serializeOptions.Get("Web")), contentType: "application/json; charset=utf-8");
        })
            .WithTags(v1PathBase); ;

        routeBuilder.MapPost("/challenge-result/guess-right-variant", async (HttpContext httpContext, IOptionsSnapshot<JsonSerializerOptions> serializingOptions, GuessRightVariant_UserAnswerAssessor userAnswerAssessor, GuessRightVariantChallengeResult_HtmlViewCreator htmlViewCreator) =>
        {
            //todo Если по этому эндпойнту могут проверяться и челленджи другого типа, то, мб, сохранять в Session название сохранённого типа, и потом Type.GetType(typeName)?
            UserChallenge<ChoosingRightOption_UserChallengeUnit_v1<WordWithEnglishTranslationId, Word>>? userChallengeSaved = null;
            try
            {
                string? userChallengeSavedJson = httpContext.Session.GetString("CurrentUserChallenge");
                userChallengeSaved = JsonSerializer.Deserialize<UserChallenge<ChoosingRightOption_UserChallengeUnit_v1<WordWithEnglishTranslationId, Word>>>(userChallengeSavedJson!, serializingOptions.Get("Internal"));
            }
            catch { }
            if (userChallengeSaved is null)
                return Results.StatusCode(StatusCodes.Status500InternalServerError);

            int[]? userAnswers = null;
            try
            {
                userAnswers = await httpContext.Request.ReadFromJsonAsync<int[]>(serializingOptions.Get("Web"));
            }
            catch { }
            if (userAnswers is null)
                return Results.ValidationProblem(new Dictionary<string, string[]>() { { "userAnswers", new string[] { "Список задан некорректно или пуст." } } });

            string assessment = userAnswerAssessor.GetAssessment(userChallengeSaved, userAnswers);
            var htmlDocument = htmlViewCreator.CreateView(assessment);
            // will be disposed by Results.File()
            Stream stream = new MemoryStream();
            htmlDocument.Save(stream);
            stream.Position = 0;
            return Results.File(stream, contentType: "text/html; charset=utf-8");

            //var stringBuilder = new System.Text.StringBuilder();
            //using (StringWriter writer = new StringWriter(stringBuilder))
            //{
            //    htmlDocument.Save(writer);
            //    writer.Flush();
            //}
            //return Results.Content(stringBuilder.ToString(), "text/html", System.Text.Encoding.UTF8);
        })
            .WithTags(v1PathBase); ;
    }
}
