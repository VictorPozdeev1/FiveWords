using CsvHelper.Configuration;
using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.CsvRepository;

internal class UserDictionariesCsvRepository: OneFileCsvRepository<UserDictionaryHeader, string, ClassMap<UserDictionaryHeader>>, IUserDictionariesRepository
{
    protected internal UserDictionariesCsvRepository(string homeDirectoryPath, string fileName) : base(homeDirectoryPath, fileName) { }

    protected override ClassMap<UserDictionaryHeader> Mapping => new UserDictionariesMapping();

    private class UserDictionariesMapping : ClassMap<UserDictionaryHeader>
    {
        public UserDictionariesMapping()
        {
            Map(w => w.Id).Name("Name").Index(0);
            Map(w => w.WordsQuantity).Name("WordsQuantity").Index(1);
        }
    }
}