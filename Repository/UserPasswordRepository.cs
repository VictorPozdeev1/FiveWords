using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository;

internal abstract class UsingFileSystemRepository : IBaseRepository
{
    protected UsingFileSystemRepository(string homeDirectoryPath) => this.homeDirectoryPath = homeDirectoryPath;
    protected string homeDirectoryPath;
}

internal class OneUserPasswordInFileRepository : UsingFileSystemRepository, IOnePasswordRepository
{
    protected internal OneUserPasswordInFileRepository(string homeDirectoryPath, string fileName) : base(homeDirectoryPath)
        => this.fileName = fileName;
    

    private protected string fileName;
    protected string FilePath => Path.Combine(homeDirectoryPath, fileName);

    public byte[] GetPasswordHash() => File.ReadAllBytes(FilePath);
    public void SavePasswordHash(byte[] data) => File.WriteAllBytes(FilePath, data);
}
