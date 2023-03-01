using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.Tests.Helpers.EF;

internal sealed class UsersRepositoryHelper : ISimpleEntityRepositoryHelper<User, string>
{
    public void Clean()
    {
        throw new NotImplementedException();
    }

    public ISimpleEntityRepository<User, string> CreateRepositoryWithOneEntity(User singleEntity)
    {
        throw new NotImplementedException();
    }

    public ISimpleEntityRepository<User, string> CreateRepositoryWithSomeEntities(IEnumerable<User> entitiesToAdd)
    {
        throw new NotImplementedException();
    }

    public void DeleteRepository()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<User> GetAllEntitiesFromRepository()
    {
        throw new NotImplementedException();
    }

    public string GetSimilarButNotExistingId(string exampleId)
    {
        throw new NotImplementedException();
    }
}
