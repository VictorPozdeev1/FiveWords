using FiveWords.DataObjects;
using FiveWords.Repository.CsvRepository;
using FiveWords.Repository.Interfaces;
using System.Security.Cryptography;

namespace FiveWords.Repository.Tests;

internal interface ISimpleRepositoryHelper<TEntity, TId>
    where TEntity : BaseEntity<TId>
    where TId : IEquatable<TId>
{
    //UsersCsvRepository CreateRepository();
    ISimpleEntityRepository<TEntity, TId> CreateRepositoryWithOneEntity(TEntity singleEntity);
    void DeleteRepository();
    void Clean();
}

internal enum RepositoryHelperState
{
    NotInitialized,
    RepositoryNotCreated,
    RepositoryCreated,
    Cleaned
}

internal sealed class SimpleRepositoryHelper : ISimpleRepositoryHelper<User, string>
{
    public SimpleRepositoryHelper()
    {
        filesId = Guid.NewGuid().ToString(); //check - is it not initialized?
        Directory.CreateDirectory(filesId);
        state = RepositoryHelperState.RepositoryNotCreated;
    }

    RepositoryHelperState state;

    string filesId;

    /* public UsersCsvRepository CreateRepository()
     {
         if (state != RepositoryHelperState.RepositoryNotCreated)
             throw new InvalidOperationException();
         var result = new UsersCsvRepository(filesId, $"{filesId}.csv");
         state = RepositoryHelperState.RepositoryCreated;
         return result;
     }*/

    public ISimpleEntityRepository<User, string> CreateRepositoryWithOneEntity(User singleUser)
    {
        if (state != RepositoryHelperState.RepositoryNotCreated)
            throw new InvalidOperationException();
        File.WriteAllText(Path.Combine(filesId, $"{filesId}.csv"), $"Id,Guid\r\n{singleUser.Id},{singleUser.Guid.ToString()}\r\n");
        var result = new UsersCsvRepository(filesId, $"{filesId}.csv");
        state = RepositoryHelperState.RepositoryCreated;
        return result;
    }

    public void DeleteRepository()
    {
        if (state != RepositoryHelperState.RepositoryCreated)
            throw new InvalidOperationException();
        File.Delete(Path.Combine(filesId, $"{filesId}.csv"));
        state = RepositoryHelperState.RepositoryNotCreated;
    }

    public void Clean()
    {
        Directory.Delete(filesId, true);
        state = RepositoryHelperState.Cleaned;
    }

    // CreateRepoWithOneEntity() { } ??????

    //public ISimpleEntityRepository BuildRepository()
    //{
    //    return null;
    //}

    protected SimpleRepositoryHelper Initialize()
    {
        return this;
    }
}