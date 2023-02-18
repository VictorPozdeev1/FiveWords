using CsvHelper.Configuration;
using FiveWords.DataObjects;

namespace FiveWords.Repository.CsvRepository;

public static class DefaultChallengeDictionary
{
    static readonly WordTranslationMapping mapping = new();
    private class WordTranslationMapping : ClassMap<WordTranslation>
    {
        public WordTranslationMapping()
        {
            Map(w => w.Id).Name("Word").Index(0);
            Map(w => w.Translation).Name("Translation").Index(1);
        }
    }

    public static List<WordTranslation> GetWordTranslationSet() =>
        Utils.ReadAllFromFileToList(Path.Combine("users-data", "_default", "default-challenge-dictionary.csv"), mapping);
}
