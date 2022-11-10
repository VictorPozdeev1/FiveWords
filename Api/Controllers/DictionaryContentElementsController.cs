using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;
using FiveWords.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FiveWords.Api.Controllers;

[ApiController]
[Route("[controller]/{dictionaryName}")]
//[Produces("application/json")]
public class DictionaryContentElementsController : ControllerBase
{
    [HttpPut("{id}")]
    //[Route("{id}")]
    [Authorize]
    public IActionResult UpdateWordTranslation(string dictionaryName, string id, [FromBody] WordTranslation newValue, [FromServices] IUsersRepository usersRepository, [FromServices] UserDictionariesUserRepositoriesManager userDictionariesRepoManager)
    {
        if (!ModelState.IsValid)
            return ValidationProblem();

        var currentUser = usersRepository.Get(User.Identity!.Name!);
        var userDictionariesRepo = userDictionariesRepoManager.GetRepository(currentUser!);
        userDictionariesRepo.TryUpdateContentElementAndImmediatelySave(dictionaryName, id, newValue, out ActionError actionError);
        if (actionError is not null)
            return Conflict(actionError);
        return Ok(new { dictionaryName, id, newValue });
    }
}
