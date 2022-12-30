using FiveWords.DataObjects;

namespace FiveWords.Repository.Interfaces;

public interface IHeaderWithContentRepository<THeaderWithContent, THeader, THeaderWithContentLength, THeaderId, TContentElement, TContentElementId> : IBaseRepository
    where THeaderWithContent : IHeaderWithContent<THeader, THeaderId, TContentElement>
    where THeader : BaseEntity<THeaderId>
    where THeaderWithContentLength : BaseEntity<THeaderId>
    where THeaderId : IEquatable<THeaderId>
    where TContentElement : BaseEntity<TContentElementId>
    where TContentElementId : IEquatable<TContentElementId>
{
    bool Exists(THeaderId headerId);

    THeaderWithContentLength? GetHeaderWithContentLength(THeaderId id);
    IReadOnlyDictionary<THeaderId, THeaderWithContentLength> GetAllHeadersWithContentLength();

    THeaderWithContent? GetHeaderWithContent(THeaderId id);

    void AddAndImmediatelySave(THeaderWithContent headerWithContent);

    void UpdateHeaderAndImmediatelySave(THeaderId id, THeader updatedHeader);
    void UpdateAndImmediatelySave(THeaderId id, THeaderWithContent headerWithContent);

    void DeleteAndImmediatelySave(THeaderId id);

    //THeader? GetHeaderOnly(TId id);
    //IReadOnlyDictionary<TId, THeader> GetAllHeadersOnly();
    //IReadOnlyDictionary<TId, THeaderWithContent> GetAllHeadersWithContent();
    //void AddAndImmediatelySaveHeaderOnly(THeader header);

    void TryUpdateContentElementAndImmediatelySave(THeaderId headerId, TContentElementId contentElementId, TContentElement newValue, out ActionError error);
    void TryDeleteContentElementAndImmediatelySave(THeaderId headerId, TContentElementId contentElementId, out ActionError error);
    void TryAddContentElementAndImmediatelySave(THeaderId headerId, TContentElement valueToAdd, out ActionError error);
    void TryAddContentElementsAndImmediatelySave(THeaderId headerId, IEnumerable<TContentElement> valuesToAdd, out ActionError error);
}

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