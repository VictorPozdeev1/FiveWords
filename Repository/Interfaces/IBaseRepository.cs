using FiveWords.DataObjects;

namespace FiveWords.Repository.Interfaces;

public interface IBaseRepository { }

public interface ISimpleEntityRepository<TEntity, TEntityId> : IBaseRepository
    where TEntity : BaseEntity<TEntityId>
    where TEntityId : IEquatable<TEntityId>
{
    public bool Exists(TEntityId id);
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


public interface IUsersRepository : ISimpleEntityRepository<User, string>
{
    public ActionError? FindError_UserWithSuchLoginAlreadyExists(string login)
        => Exists(login) ? new ActionError($"Пользователь с логином \"{login}\" уже существует.", login) : null;
}