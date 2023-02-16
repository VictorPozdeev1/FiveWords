using FiveWords.DataObjects;
using FiveWords.Repository;

namespace FiveWords._v1.Repository;

public abstract class UserRepositoriesManager_v1<TRepository> : UserRepositoriesManager<TRepository>
    where TRepository : FiveWords.Repository.Interfaces.IBaseRepository
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