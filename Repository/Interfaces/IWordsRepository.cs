using FiveWords.DataObjects;

namespace FiveWords.Repository.Interfaces;

public interface IWordsRepository : ISimpleEntityRepository<Word, int>
{
    IEnumerable<Word> GetByWritingFilter(Predicate<string?> writingFilter);
}

public interface IWordsWithEnglishTranslationRepository : ISimpleEntityRepository<WordWithEnglishTranslationId, int>
{
    IEnumerable<WordWithEnglishTranslationId> GetWordsHavingEnglishTranslationId();
}