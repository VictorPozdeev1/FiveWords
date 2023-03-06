using FiveWords.BusinessLogic;
using FiveWords.CommonModels;
using FiveWords.Repository;
using FiveWords.Repository.Interfaces;
using FiveWords.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FiveWords.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WordTranslationsChallengeController : ControllerBase
{
    [HttpGet("{unitsCount:int:range(1,15)}:{answerVariantsCount:int:range(2,8)}/{dictionaryNameEscaped?}")]
    // ?? [Authorize] ??
    // Может, разделить потом на два разных контроллера...? (Залогиненный юзер и гость)
    public IActionResult GetChallenge(byte unitsCount, byte answerVariantsCount, string? dictionaryNameEscaped, [FromServices] IUsersRepository usersRepository, [FromServices] UserDictionariesUserRepositoriesManager userDictionariesRepoManager)
    {
        var currentUserName = User.Identity!.Name;

        ICollection<WordTranslation> wordTranslationSet;
        if (currentUserName is null || dictionaryNameEscaped is null)
        {
            wordTranslationSet = Repository.Csv.DefaultChallengeDictionary.GetWordTranslationSet();
        }
        else
        {
            var currentUser = usersRepository.Get(currentUserName)!;
            var userDictionariesRepo = userDictionariesRepoManager.GetRepository(currentUser);
            wordTranslationSet = userDictionariesRepo.GetHeaderWithContent(Uri.UnescapeDataString(dictionaryNameEscaped))!.Content;
        }

        var userChallengeCreator = new ChoosingTranslationChallengeCreator();
        var userChallenge = userChallengeCreator.CreateChoosingTranslationChallenge(wordTranslationSet, unitsCount, answerVariantsCount, out ActionError? actionError);

        if (actionError is not null)
            return BadRequest(new { Error = actionError });

        var serializingOptionsProvider = HttpContext.RequestServices.GetRequiredService<IOptionsSnapshot<JsonSerializerOptions>>();
        HttpContext.Session.SetUserChallenge(userChallenge.Id, userChallenge, serializingOptionsProvider);

        return Ok(userChallenge);
    }
}