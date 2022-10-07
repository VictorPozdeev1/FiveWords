using CsvHelper.Configuration;
using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.CsvRepository;

internal class UserDictionariesCsvRepository: OneFileCsvRepository<UserDictionaryHeader, string>, IUserDictionariesRepository
{
    protected internal UserDictionariesCsvRepository(string repoDirectoryPath, string fileName) : base(repoDirectoryPath, fileName) { }

    protected override ClassMap<UserDictionaryHeader> InitialisingMapping => new UserDictionariesMapping();

    private class UserDictionariesMapping : ClassMap<UserDictionaryHeader>
    {
        public UserDictionariesMapping()
        {
            Map(w => w.Id).Name("Name").Index(0);
            Map(w => w.WordsQuantity).Name("WordsQuantity").Index(1);
        }
    }
}