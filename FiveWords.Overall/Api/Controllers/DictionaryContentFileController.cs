using FiveWords.Api.ModelBinding;
using FiveWords.DataObjects;
using FiveWords.Repository;
using FiveWords.Repository.Interfaces;
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
        public IActionResult AddWordsFromFile(string dictionaryNameEscaped, [ModelBinder(typeof(WordTranslationsFromFile_ModelBinder))] ICollection<WordTranslation> wordsToAdd, [FromServices] IUsersRepository usersRepository, [FromServices] UserDictionariesUserRepositoriesManager userDictionariesRepoManager)
        {
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
