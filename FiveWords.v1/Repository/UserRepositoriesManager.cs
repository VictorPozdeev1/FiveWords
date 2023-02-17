using FiveWords._v1.DataObjects;
using FiveWords.Repository;
using FiveWords._v1.Repository.Interfaces;

namespace FiveWords._v1.Repository;

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

public abstract class UserRepositoriesManager_v1<TRepository> : UserRepositoriesManager<TRepository>
    where TRepository : IBaseRepository
{
    protected override string GetRepoDirectoryPath(User owner)
    {
        var ownerSubfolderName = owner == User.Default ? "_default" : owner.Guid.ToString();
        return Path.Combine(STORAGE_PATH, ownerSubfolderName, "v1", RepoSubfolderName);
    }
}

public class EnglishWordsUserRepositoriesManager : UserRepositoriesManager_v1<IWordsRepository>
{
    protected override string RepoSubfolderName => @"words-repository";

    protected override IWordsRepository InstantiateRepository(string homeDirectoryPath) => new WordsCsvRepository(homeDirectoryPath, "english-words.scv");
}

public class RussianWordsUserRepositoriesManager : UserRepositoriesManager_v1<IWordsWithEnglishTranslationRepository>
{
    protected override string RepoSubfolderName => @"words-repository";

    protected override IWordsWithEnglishTranslationRepository InstantiateRepository(string repoDirectoryPath) => new WordsWithEnglishTranslationId_CsvRepository(repoDirectoryPath, "russian-words.scv");
}