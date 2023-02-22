using FiveWords.DataObjects;
using FiveWords.Repository.CsvRepository;
using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.Tests;

internal interface ISimpleRepositoryHelper
{
    UsersCsvRepository CreateRepository();
    UsersCsvRepository CreateRepositoryWithOneEntity(string id, string guidString);
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

internal sealed class SimpleRepositoryHelper : ISimpleRepositoryHelper
{
    public SimpleRepositoryHelper()
    {
        filesId = Guid.NewGuid().ToString(); //check - is it not initialized?
        Directory.CreateDirectory(filesId);
        state = RepositoryHelperState.RepositoryNotCreated;
    }

    RepositoryHelperState state;

    string filesId;

    public UsersCsvRepository CreateRepository()
    {
        if (state != RepositoryHelperState.RepositoryNotCreated)
            throw new InvalidOperationException();
        var result = new UsersCsvRepository(filesId, $"{filesId}.csv");
        state = RepositoryHelperState.RepositoryCreated;
        return result;
    }

    public UsersCsvRepository CreateRepositoryWithOneEntity(string id, string guidString)
    {
        if (state != RepositoryHelperState.RepositoryNotCreated)
            throw new InvalidOperationException();
        File.WriteAllText(Path.Combine(filesId, $"{filesId}.csv"), $"Id,Guid\r\n{id},{guidString}\r\n");
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