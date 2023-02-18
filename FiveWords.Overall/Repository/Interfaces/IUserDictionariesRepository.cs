using FiveWords.DataObjects;

namespace FiveWords.Repository.Interfaces;

public interface IUserDictionariesRepository : IHeaderWithContentRepository<UserDictionary, UserDictionaryHeader, UserDictionaryHeaderWithWordsQuantity, string, WordTranslation, string>
{
    const int MAXIMUM_DICTIONARY_LENGTH = 200;

    public ActionError? FindError_DictionaryWithSuchNameAlreadyExists(UserDictionaryHeader dictionaryToCheck)
        => Exists(dictionaryToCheck.Id) ? new ActionError($"Словарь \"{dictionaryToCheck.Id}\" уже существует.", dictionaryToCheck) : null;

    public ActionError? FindError_DictionaryNotFound(string dictionaryName)
        => !Exists(dictionaryName) ? new ActionError($"Словарь \"{dictionaryName}\" не найден.", dictionaryName) : null;

    public ActionError? FindError_MaximumDictionaryLengthWillBeExceeded(string dictionaryName, ICollection<WordTranslation> wordsToAdd)
    {
        int existingLength = GetHeaderWithContentLength(dictionaryName)?.ContentLength ?? 0;
        int exceeding = existingLength + wordsToAdd.Count - MAXIMUM_DICTIONARY_LENGTH;
        if (exceeding > 0)
            return new ActionError($"Превышен (на {exceeding}) максимально возможный размер словаря ({MAXIMUM_DICTIONARY_LENGTH} слов).", new { ExistingLength = existingLength, AddingLength = wordsToAdd.Count, MaximumLength = MAXIMUM_DICTIONARY_LENGTH });
        return null;
    }
}