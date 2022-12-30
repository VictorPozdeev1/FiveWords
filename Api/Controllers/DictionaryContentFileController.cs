using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;
using FiveWords.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiveWords.Api.Controllers
{
    [Route("[controller]/{dictionaryNameEscaped}")]
    [ApiController]
    public class DictionaryContentFileController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public IActionResult AddWordsFromFile(string dictionaryNameEscaped, ICollection<WordTranslation> wordsToAdd, [FromServices] IUsersRepository usersRepository, [FromServices] UserDictionariesUserRepositoriesManager userDictionariesRepoManager)
        {
            // можно ещё попробовать просто через атрибут сделать, без встраивания BinderProvider: https://learn.microsoft.com/ru-ru/aspnet/core/mvc/advanced/custom-model-binding?view=aspnetcore-7.0#implementing-a-modelbinderprovider
            var currentUser = usersRepository.Get(User.Identity!.Name!);
            var userDictionariesRepo = userDictionariesRepoManager.GetRepository(currentUser!);
            var dictionaryName = Uri.UnescapeDataString(dictionaryNameEscaped);

            var maximumDictionaryLengthWillBeExceededError = userDictionariesRepo.FindError_MaximumDictionaryLengthWillBeExceeded(dictionaryName, wordsToAdd);
            if (maximumDictionaryLengthWillBeExceededError is not null)
                return Conflict(new { Error = maximumDictionaryLengthWillBeExceededError });

            userDictionariesRepo.TryAddContentElementsAndImmediatelySave(dictionaryName, wordsToAdd, out ActionError actionError);
            if (actionError is not null)
                return Conflict(new { Error = actionError });
            return Ok(new { dictionaryName, wordsToAdd });
        }
    }
}
