namespace FiveWords.Repository.Tests.Helpers;

internal enum RepositoryHelperState
{
    NotInitialized = 0, // to be default value
    RepositoryIsDown,
    RepositoryIsUp,
    Cleaned
}