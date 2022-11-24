using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;
using FiveWords.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        //if (!ModelState.IsValid)
        //    return ValidationProblem();

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
        userDictionariesRepo.TryAddContentElementAndImmediatelySave(dictionaryName, valueToAdd, out ActionError actionError);
        if (actionError is not null)
            return Conflict(new { Error = actionError });
        return Ok(new { dictionaryName, valueToAdd});
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
