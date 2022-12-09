using FiveWords.Repository;

namespace FiveWords._v1.Repository;

public class EnglishWordsUserRepositoriesManager : UserRepositoriesManager<IWordsRepository>
{
    protected override string RepoSubfolderName => @"words-repository";

    protected override IWordsRepository InstantiateRepository(string homeDirectoryPath) => new WordsCsvRepository(homeDirectoryPath, "english-words.scv");
}

public class RussianWordsUserRepositoriesManager : UserRepositoriesManager<IWordsWithEnglishTranslationRepository>
{
    protected override string RepoSubfolderName => @"words-repository";

    protected override IWordsWithEnglishTranslationRepository InstantiateRepository(string repoDirectoryPath) => new WordsWithEnglishTranslationId_CsvRepository(repoDirectoryPath, "russian-words.scv");
}