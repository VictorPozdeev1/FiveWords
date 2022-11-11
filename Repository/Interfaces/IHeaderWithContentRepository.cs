using FiveWords.DataObjects;
using System.Collections;

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
}

public interface IUserDictionariesRepository : IHeaderWithContentRepository<UserDictionary, UserDictionaryHeader, UserDictionaryHeaderWithWordsQuantity, string, WordTranslation, string>
{
    public ActionError? FindError_DictionaryWithSuchNameAlreadyExists(UserDictionaryHeader dictionaryToCheck)
        => Exists(dictionaryToCheck.Id) ? new ActionError($"Словарь \"{dictionaryToCheck.Id}\" уже существует.", dictionaryToCheck) : null;

    public ActionError? FindError_DictionaryNotFound(string dictionaryName)
        => !Exists(dictionaryName) ? new ActionError($"Словарь \"{dictionaryName}\" не найден.", dictionaryName) : null;
}