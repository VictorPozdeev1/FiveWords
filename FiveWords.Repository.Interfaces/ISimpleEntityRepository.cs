using FiveWords.CommonModels;

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