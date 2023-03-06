using FiveWords.CommonModels;
using FiveWords.Repository;
using FiveWords.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiveWords.Api.Controllers;

[ApiController]
[Route("[controller]/{dictionaryNameEscaped}")]
//[Produces("application/json")]
public class DictionaryContentElementsController : ControllerBase
{
    [HttpPut("{id}")]
    [Authorize]
    public IActionResult UpdateWordTranslation(string dictionaryNameEscaped, string id, [FromBody] WordTranslation newValue, [FromServices] IUsersRepository usersRepository, [FromServices] UserDictionariesUserRepositoriesManager userDictionariesRepoManager)
    {
        // todo Надо бы попробовать получать модель (dictionaryName, userDictionariesRepo) в нужном виде в фильтрах, а не тут.
        var currentUser = usersRepository.Get(User.Identity!.Name!);
        var userDictionariesRepo = userDictionariesRepoManager.GetRepository(currentUser!);
        var dictionaryName = Uri.UnescapeDataString(dictionaryNameEscaped);
        userDictionariesRepo.TryUpdateContentElementAndImmediatelySave(dictionaryName, id, newValue, out ActionError actionError);
        if (actionError is not null)
            return UnprocessableEntity(new { Error = actionError });
        return Ok(new { dictionaryName, id, newValue });
    }

    [HttpPost]
    [Authorize]
    public IActionResult AddWordTranslation(string dictionaryNameEscaped, [FromBody] WordTranslation valueToAdd, [FromServices] IUsersRepository usersRepository, [FromServices] UserDictionariesUserRepositoriesManager userDictionariesRepoManager)
    {
        var currentUser = usersRepository.Get(User.Identity!.Name!);
        var userDictionariesRepo = userDictionariesRepoManager.GetRepository(currentUser!);
        var dictionaryName = Uri.UnescapeDataString(dictionaryNameEscaped);

        var maximumDictionaryLengthWillBeExceededError = userDictionariesRepo.FindError_MaximumDictionaryLengthWillBeExceeded(dictionaryName, new WordTranslation[1] { valueToAdd });
        if (maximumDictionaryLengthWillBeExceededError is not null)
            return Conflict(new { Error = maximumDictionaryLengthWillBeExceededError });

        userDictionariesRepo.TryAddContentElementAndImmediatelySave(dictionaryName, valueToAdd, out ActionError actionError);
        if (actionError is not null)
            return Conflict(new { Error = actionError });
        return Ok(new { dictionaryName, valueToAdd });
    }

    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult DeleteWordTranslation(string dictionaryNameEscaped, string id, [FromServices] IUsersRepository usersRepository, [FromServices] UserDictionariesUserRepositoriesManager userDictionariesRepoManager)
    {
        var currentUser = usersRepository.Get(User.Identity!.Name!);
        var userDictionariesRepo = userDictionariesRepoManager.GetRepository(currentUser!);
        var dictionaryName = Uri.UnescapeDataString(dictionaryNameEscaped);
        userDictionariesRepo.TryDeleteContentElementAndImmediatelySave(dictionaryName, id, out ActionError actionError);
        if (actionError is not null)
            return UnprocessableEntity(new { Error = actionError });
        return NoContent();
    }
}
