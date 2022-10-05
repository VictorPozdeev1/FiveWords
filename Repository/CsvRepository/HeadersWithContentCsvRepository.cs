using CsvHelper;
using CsvHelper.Configuration;
using FiveWords.DataObjects;
using System.Globalization;

namespace FiveWords.Repository.CsvRepository;

internal abstract class HeadersWithContentCsvRepository<TEntity, TEntityId, THeaderMapping, TContentElement> : OneFileCsvRepository<TEntity, TEntityId, THeaderMapping>
    where TEntity : BaseEntity<TEntityId>, IContaining<TContentElement>
    where TEntityId : IEquatable<TEntityId>
    where THeaderMapping : ClassMap<TEntity>
{
    protected HeadersWithContentCsvRepository(string homeDirectoryPath, string headersFileName) : base(homeDirectoryPath, headersFileName)
    {
    }

    protected abstract ClassMap<TContentElement> ContentMapping { get; }

    protected abstract string GetDefaultFileNameForContent(TEntity entity);
    private string GetDefaultFilePathForContent(TEntity entity) => Path.Combine(repoDirectoryPath, "Content", GetDefaultFileNameForContent(entity));

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
}