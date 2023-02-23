using FiveWords.DataObjects;
using FiveWords.Repository.CsvRepository;
using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.Tests.Helpers.Csv;

internal sealed class UsersRepositoryHelper : ISimpleEntityRepositoryHelper<User, string>
{
    public UsersRepositoryHelper()
    {
        filesId = Guid.NewGuid().ToString();
        Directory.CreateDirectory(filesId);
        state = RepositoryHelperState.RepositoryIsDown;
    }

    RepositoryHelperState state;

    string filesId;

    public ISimpleEntityRepository<User, string> CreateRepositoryWithOneEntity(User singleUser)
    {
        if (state != RepositoryHelperState.RepositoryIsDown)
            throw new InvalidOperationException();
        File.WriteAllText(Path.Combine(filesId, $"{filesId}.csv"), $"Id,Guid\r\n{singleUser.Id},{singleUser.Guid.ToString()}\r\n");
        var result = new UsersCsvRepository(filesId, $"{filesId}.csv");
        state = RepositoryHelperState.RepositoryIsUp;
        return result;
    }

    public void DeleteRepository()
    {
        if (state != RepositoryHelperState.RepositoryIsUp)
            throw new InvalidOperationException();
        File.Delete(Path.Combine(filesId, $"{filesId}.csv"));
        state = RepositoryHelperState.RepositoryIsDown;
    }

    public void Clean()
    {
        Directory.Delete(filesId, true);
        state = RepositoryHelperState.Cleaned;
    }

    public string GetSomeSimilarId(string exampleId) => exampleId + ' ';
}
