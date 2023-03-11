using FiveWords.BusinessLogic;
using FiveWords.CommonModels;
using FiveWords.Repository;
using FiveWords.Repository.Interfaces;
using FiveWords.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FiveWords.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WordTranslationsChallengeResultsController : ControllerBase
{
    [HttpPost]
    [Authorize]
    public IActionResult PostChallengeResults([FromBody] ChoosingRightOptionChallengeResults challengeResults, [FromServices] IUsersRepository usersRepository)
    {
        var serializingOptionsProvider = HttpContext.RequestServices.GetRequiredService<IOptionsSnapshot<JsonSerializerOptions>>();
        var userChallenge = HttpContext.Session.GetUserChallenge<ChoosingTranslationUserChallenge>(challengeResults.ChallengeId, serializingOptionsProvider);

        if (userChallenge is null)
            return BadRequest(new { Error = new ActionError($"Не найден тест с Id {challengeResults.ChallengeId}", challengeResults.ChallengeId) });

        var currentUserName = User.Identity!.Name!;
        var currentUser = usersRepository.Get(currentUserName)!;

        return Ok(userChallenge);
    }
}