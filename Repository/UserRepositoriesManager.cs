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
        result = CreateOneFileRepository(owner);
        repositoriesByOwners.Add(owner.Guid, result);
        return result;
    }

    protected const string STORAGE_PATH = @"users-data";
    protected abstract string RepoSubfolderName { get; }

    protected virtual TRepository CreateOneFileRepository(User owner)
    {
        string homeDirectoryPath = GetRepoHomeDirectoryPath(owner);
        if (!Directory.Exists(homeDirectoryPath))
            Directory.CreateDirectory(homeDirectoryPath);
        return InstantiateRepository(homeDirectoryPath);
    }

    protected string GetRepoHomeDirectoryPath(User owner)
    {;
        var ownerSubfolderName = owner == User.Default ? "_default" : owner.Guid.ToString();
        return Path.Combine(STORAGE_PATH, ownerSubfolderName, RepoSubfolderName);
    }

    protected abstract TRepository InstantiateRepository(string homeDirectoryPath);
}

public class EnglishWordsUserRepositoriesManager : UserRepositoriesManager<IWordsRepository>
{
    protected override string RepoSubfolderName => @"words-repository";

    protected override IWordsRepository InstantiateRepository(string homeDirectoryPath) => new WordsCsvRepository(homeDirectoryPath, "english-words.scv");
}

public class RussianWordsUserRepositoriesManager : UserRepositoriesManager<IWordsWithEnglishTranslationRepository>
{
    protected override string RepoSubfolderName => @"words-repository";

    protected override IWordsWithEnglishTranslationRepository InstantiateRepository(string homeDirectoryPath) => new WordsWithEnglishTranslationId_CsvRepository(homeDirectoryPath, "russian-words.scv");
}

public class UserPasswordRepositoriesManager: UserRepositoriesManager<IOnePasswordRepository>
{
    protected override string RepoSubfolderName => "password-hash";
    protected override IOnePasswordRepository InstantiateRepository(string homeDirectoryPath) => new OneUserPasswordInFileRepository(homeDirectoryPath, "password-hash");
}

public class UserDictionariesUserRepositoriesManager : UserRepositoriesManager<IUserDictionariesRepository>
{
    protected override string RepoSubfolderName => @"dictionaries-list";
    protected override IUserDictionariesRepository InstantiateRepository(string homeDirectoryPath) => new UserDictionariesCsvRepository(homeDirectoryPath, "dictionaries-list.csv");
}