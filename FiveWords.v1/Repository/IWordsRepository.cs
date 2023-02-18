using FiveWords._v1.DataObjects;
using FiveWords._v1.Repository.Interfaces;

namespace FiveWords._v1.Repository;

public interface IWordsRepository : ISimpleEntityRepository<Word, int>
{
    IEnumerable<Word> GetByWritingFilter(Predicate<string?> writingFilter);
}

public interface IWordsWithEnglishTranslationRepository : ISimpleEntityRepository<WordWithEnglishTranslationId, int>
{
    IEnumerable<WordWithEnglishTranslationId> GetWordsHavingEnglishTranslationId();
}