using FiveWords.DataObjects;
using FiveWords.Repository.CsvRepository;
using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository;

public abstract class UserRepositoriesManager<TRepository>
    where TRepository : IBaseRepository
{
    protected UserRepositoriesManager() { }

    private Dictionary<Guid, TRepository> repositoriesByOwners = new();
    public TRepository GetRepository(User owner)
    {
        TRepository result;
        if (repositoriesByOwners.TryGetValue(owner.Guid, out result))
            return result;
        result = CreateRepository(owner);
        repositoriesByOwners.Add(owner.Guid, result);
        return result;
    }

    protected const string STORAGE_PATH = @"users-data";
    protected abstract string RepoSubfolderName { get; }

    protected virtual TRepository CreateRepository(User owner)
    {
        string repoDirectoryPath = GetRepoDirectoryPath(owner);
        if (!Directory.Exists(repoDirectoryPath))
            Directory.CreateDirectory(repoDirectoryPath);
        return InstantiateRepository(repoDirectoryPath);
    }

    protected virtual string GetRepoDirectoryPath(User owner)
    {
        var ownerSubfolderName = owner == User.Default ? "_default" : owner.Guid.ToString();
        return Path.Combine(STORAGE_PATH, ownerSubfolderName, RepoSubfolderName);
    }

    protected abstract TRepository InstantiateRepository(string repoDirectoryPath);
}

public class UserPasswordRepositoriesManager: UserRepositoriesManager<IOnePasswordRepository>
{
    protected override string RepoSubfolderName => "password-hash";
    protected override IOnePasswordRepository InstantiateRepository(string repoDirectoryPath) => new OneUserPasswordInFileRepository(repoDirectoryPath, "password-hash");
}

public class UserDictionariesUserRepositoriesManager : UserRepositoriesManager<IUserDictionariesRepository>
{
    protected override string RepoSubfolderName => @"dictionaries";
    protected override IUserDictionariesRepository InstantiateRepository(string repoDirectoryPath) => new UserDictionariesCsvRepository(repoDirectoryPath, "dictionaries-list.csv");
}