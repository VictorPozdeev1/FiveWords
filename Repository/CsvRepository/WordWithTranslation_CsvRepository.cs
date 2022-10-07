using CsvHelper.Configuration;
using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.CsvRepository;

//entrypoint: Начать с того, что новый BaseObject сделать.
internal class WordWithTranslation_CsvRepository : OneFileCsvRepository<Word, int>, IWordsRepository
{
    protected internal WordWithTranslation_CsvRepository(string homeDirectoryPath, string fileName) : base(homeDirectoryPath, fileName) { }

    public IEnumerable<Word> GetByWritingFilter(Predicate<string?> writingFilter) => GetAll()
        .Where(kvp => writingFilter(kvp.Value.Writing))
        .Select(kvp => kvp.Value);

    protected override ClassMap<Word> InitialisingMapping => new WordMapping();

    private class WordMapping : ClassMap<Word>
    {
        public WordMapping()
        {
            Map(w => w.Id).Name("Id").Index(0);
            Map(w => w.Writing).Name("Writing").Index(1);
            Map(w => w.Language).Name("Language").Index(2);
            //Map(w => w.DefaultTranslate.Writing).Name("DefaultTranslate").Index(3);
        }
    }
}