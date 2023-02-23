using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.Tests.Helpers;

internal interface ISimpleEntityRepositoryHelper<TEntity, TId>
    where TEntity : BaseEntity<TId>
    where TId : IEquatable<TId>
{
    ISimpleEntityRepository<TEntity, TId> CreateRepositoryWithOneEntity(TEntity singleEntity);
    void DeleteRepository();
    void Clean();
    TId GetSomeSimilarId(TId exampleId);
}