using FiveWords.Api;
using FiveWords.DataObjects;
using System.Collections;

namespace FiveWords.Repository.Interfaces;

public interface IHeaderWithContentRepository<THeaderWithContent, THeader, THeaderWithContentLength, THeaderId, TContentElement> : IBaseRepository
    where THeaderWithContent : IHeaderWithContent<THeader, THeaderId, TContentElement>
    where THeader : BaseEntity<THeaderId>
    where THeaderWithContentLength : BaseEntity<THeaderId>
    where THeaderId : IEquatable<THeaderId>
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
}

public interface IUserDictionariesRepository : IHeaderWithContentRepository<UserDictionary, UserDictionaryHeader, UserDictionaryHeaderWithWordsQuantity, string, WordTranslation>
{
    public RequestError? FindError_DictionaryWithSuchNameAlreadyExists(UserDictionaryHeader dictionaryToCheck)
        => Exists(dictionaryToCheck.Id) ? new RequestError($"Словарь \"{dictionaryToCheck.Id}\" уже существует.", dictionaryToCheck) : null;

    public RequestError? FindError_DictionaryNotFound(string dictionaryName)
        => !Exists(dictionaryName) ? new RequestError($"Словарь \"{dictionaryName}\" не найден.", dictionaryName) : null;
}