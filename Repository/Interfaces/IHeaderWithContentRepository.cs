using FiveWords.DataObjects;

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
    public object? FindConflict_DictionaryWithSuchNameAlreadyExists(UserDictionaryHeader dictionaryToCheck)
    {
        if (Exists(dictionaryToCheck.Id))
            return new
            {
                Error = new
                {
                    Message = $"Словарь \"{dictionaryToCheck.Id}\" уже существует.",
                    Dictionary = dictionaryToCheck
                }
            };
        return null;
    }
}