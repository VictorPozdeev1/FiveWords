using CsvHelper;
using CsvHelper.Configuration;
using FiveWords.DataObjects;
using System.Globalization;

namespace FiveWords.Repository.CsvRepository;

internal abstract class HeadersWithContentCsvRepository<TEntity, TEntityId, TContentElement> : OneFileCsvRepository<TEntity, TEntityId>
    where TEntity : BaseEntity<TEntityId>, IContaining<TContentElement>
    where TEntityId : IEquatable<TEntityId>
{
    protected string contentDirectoryPath;

    protected HeadersWithContentCsvRepository(string repoDirectoryPath, string headersFileName) : base(repoDirectoryPath, headersFileName)
    {
        contentDirectoryPath = Path.Combine(repoDirectoryPath, "Content");
        if (!Directory.Exists(contentDirectoryPath))
            Directory.CreateDirectory(contentDirectoryPath);
    }

    protected abstract ClassMap<TContentElement> ContentMapping { get; }

    protected abstract string GetDefaultFileNameForContent(TEntity entity);
    private string GetDefaultFilePathForContent(TEntity entity) => Path.Combine(contentDirectoryPath, GetDefaultFileNameForContent(entity));

    public override TEntity? Get(TEntityId id)
    {
        TEntity? result = base.Get(id);
        if (result is not null)
            result.Content = ReadContentFromFile(result);
        return result;
    }

    public override IReadOnlyDictionary<TEntityId, TEntity> GetAll()
    {
        var result = base.GetAll();
        foreach (var kvp in result)
            kvp.Value.Content = ReadContentFromFile(kvp.Value);
        return result;
    }

    private List<TContentElement> ReadContentFromFile(TEntity entity)
        => Utils.ReadAllFromFileToList(GetDefaultFilePathForContent(entity), ContentMapping);

    public override void AddAndImmediatelySave(TEntity entity)
    {
        base.AddAndImmediatelySave(entity);
        SaveContent(entity);
    }

    private void SaveContent(TEntity entity)
    {
        Utils.WriteAllToFile(entity.Content, GetDefaultFilePathForContent(entity), ContentMapping);
    }
}