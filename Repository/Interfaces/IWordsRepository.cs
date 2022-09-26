using FiveWords.DataObjects;

namespace FiveWords.Repository.Interfaces;

public interface IWordsRepository : IBaseRepository<Word, int>
{
    IEnumerable<Word> GetByWritingFilter(Predicate<string?> writingFilter);
}

public interface IWordsWithEnglishTranslationRepository : IBaseRepository<WordWithEnglishTranslationId, int>
{
    IEnumerable<WordWithEnglishTranslationId> GetWordsHavingEnglishTranslationId();
}