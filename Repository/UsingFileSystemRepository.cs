using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository;

internal abstract class UsingFileSystemRepository : IBaseRepository
{
    protected UsingFileSystemRepository(string repoDirectoryPath) => this.repoDirectoryPath = repoDirectoryPath;
    protected string repoDirectoryPath;
}