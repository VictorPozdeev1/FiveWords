using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.Csv;

public abstract class UsingFileSystemRepository : IBaseRepository
{
    protected UsingFileSystemRepository(string repoDirectoryPath) => this.repoDirectoryPath = repoDirectoryPath;
    protected string repoDirectoryPath;
}