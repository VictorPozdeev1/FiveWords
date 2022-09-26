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
}