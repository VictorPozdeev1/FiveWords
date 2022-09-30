using FiveWords.DataObjects;

namespace FiveWords.Repository.Interfaces;

public interface IBaseRepository { }

public interface IBaseRepository<TEntity, TEntityId> : IBaseRepository
    where TEntity : BaseEntity<TEntityId>
    where TEntityId : IEquatable<TEntityId>
{
    TEntity? Get(TEntityId id);
    IReadOnlyDictionary<TEntityId, TEntity> GetAll();
    void AddAndImmediatelySave(TEntity entity);
    void UpdateAndImmediatelySave(TEntityId id, TEntity entity);
    void DeleteAndImmediatelySave(TEntityId id);
}

public interface IOnePasswordRepository : IBaseRepository
{
    byte[] GetPasswordHash();
    void SavePasswordHash(byte[] data);
}


public interface IUsersRepository : IBaseRepository<User, string>
{
    //User? FindByLogin(string login);
}

public interface IUserDictionariesRepository : IBaseRepository<UserDictionaryHeader, string>
{
    public object? FindConflict_DictionaryWithSuchNameAlreadyExists(UserDictionaryHeader dictionaryToCheck)
    {
        var existingDictionary = Get(dictionaryToCheck.Id);
        if (existingDictionary is not null)
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