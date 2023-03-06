using CsvHelper.Configuration;
using FiveWords.CommonModels;
using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.Csv;

public class UserDictionariesCsvRepository
    : SavingContentLength_HeadersWithContentCsvRepository<UserDictionary, UserDictionaryHeader, UserDictionaryHeaderWithWordsQuantity, string, WordTranslation, string>
    , IUserDictionariesRepository
{
    public UserDictionariesCsvRepository(string repoDirectoryPath, string fileName)
        : base(repoDirectoryPath, fileName, new UserDictionaryHeadersWithWordsQuantityMapping(), new UserDictionariesContentMapping())
    { }

    protected override string GetDefaultFileNameForContent(string id) => $"{id}.csv"; //Возможно, стоит на хэш названия перейти, чтобы не ограничивать имя словаря ограничениями на имя файла.

    private class UserDictionaryHeadersWithWordsQuantityMapping : ClassMap<UserDictionaryHeaderWithWordsQuantity>
    {
        public UserDictionaryHeadersWithWordsQuantityMapping()
        {
            Map(w => w.Id).Name("Name").Index(0);
            Map(w => w.WordsQuantity).Name("WordsQuantity").Index(1);
        }
    }

    private class UserDictionariesContentMapping : ClassMap<WordTranslation>
    {
        public UserDictionariesContentMapping()
        {
            Map(w => w.Id).Name("Word").Index(0);
            Map(w => w.Translation).Name("Translation").Index(1);
        }
    }
}