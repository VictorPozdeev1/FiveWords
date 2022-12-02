using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;
using FiveWords.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace FiveWords.Api.Controllers
{
    [Route("[controller]/{dictionaryNameEscaped}")]
    [ApiController]
    public class DictionaryContentFileController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public IActionResult AddWordsFromFile(string dictionaryNameEscaped, IFormFile uploadingFile, [FromServices] IUsersRepository usersRepository, [FromServices] UserDictionariesUserRepositoriesManager userDictionariesRepoManager)
        {
            var currentUser = usersRepository.Get(User.Identity!.Name!);
            var userDictionariesRepo = userDictionariesRepoManager.GetRepository(currentUser!);
            var dictionaryName = Uri.UnescapeDataString(dictionaryNameEscaped);

            IEnumerable<WordTranslation> wordsToAdd = null;
            try
            {
                using var stream = uploadingFile.OpenReadStream();
                using var reader = new StreamReader(stream);
                using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                csvReader.Context.RegisterClassMap(new TempMapping());
                wordsToAdd = csvReader.GetRecords<WordTranslation>().ToList();
            }
            catch (Exception exc)
            {
                return BadRequest(new { Error = new ActionError("Не удалось прочитать данные из файла.", exc.Message) });
            }
            userDictionariesRepo.TryAddContentElementsAndImmediatelySave(dictionaryName, wordsToAdd, out ActionError actionError);
            if (actionError is not null)
                return Conflict(new { Error = actionError });
            return Ok(new { dictionaryName, wordsToAdd });
        }

        private class TempMapping : ClassMap<WordTranslation>
        {
            public TempMapping()
            {
                Map(w => w.Id).Name("Word").Index(0);
                Map(w => w.Translation).Name("Translation").Index(1);
            }
        }
    }
}
