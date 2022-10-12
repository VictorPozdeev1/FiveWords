using CsvHelper;
using CsvHelper.Configuration;
using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FiveWords.Repository.CsvRepository;

internal class OneFileCsvRepository<TEntity, TEntityId> : UsingFileSystemRepository, ISimpleEntityRepository<TEntity, TEntityId>
    where TEntity : BaseEntity<TEntityId>
    where TEntityId : IEquatable<TEntityId>
{
    public OneFileCsvRepository(string repoDirectoryPath, string fileName, ClassMap<TEntity> mapping) : base(repoDirectoryPath)
    {
        this.fileName = fileName;
        this.mapping = mapping;
        LoadFromFile();
    }

    private protected string fileName;

    Dictionary<TEntityId, TEntity> allEntities = new();

    private readonly ClassMap<TEntity> mapping;
    protected ClassMap<TEntity> Mapping => mapping;

    public bool Exists(TEntityId id) => allEntities.ContainsKey(id);

    public TEntity? Get(TEntityId id)
    {
        allEntities.TryGetValue(id, out TEntity? result);
        return result;
    }

    public IReadOnlyDictionary<TEntityId, TEntity> GetAll() => new ReadOnlyDictionary<TEntityId, TEntity>(allEntities);


    public void AddAndImmediatelySave(TEntity entity)
    {
        if (allEntities.Count == 0)
        {
            allEntities.Add(entity.Id, entity);
            SaveToFile();
        }
        else
        {
            ThrowIfExists(entity.Id);
            allEntities.Add(entity.Id, entity);
            AppendOneEntityToFile(entity);
        }
    }

    void LoadFromFile() => allEntities = Utils.ReadAllFromFileToDictionary(FilePath, Mapping, e => e.Id);

    public void UpdateAndImmediatelySave(TEntityId id, TEntity entity)
    {
        ThrowIfNotExists(id);
        if (!id.Equals(entity.Id))
        {
            ThrowIfExists(entity.Id);
            allEntities.Remove(id);
        }
        allEntities[entity.Id] = entity;
        SaveToFile();
    }

    public void DeleteAndImmediatelySave(TEntityId id)
    {
        ThrowIfNotExists(id);
        allEntities.Remove(id);
        SaveToFile();
    }

    private void SaveToFile() => Utils.WriteAllToFile(allEntities.Values, FilePath, Mapping);

    private void AppendOneEntityToFile(TEntity value) => Utils.AppendOneEntityToFile(value, FilePath, Mapping);

    string FilePath => Path.Combine(repoDirectoryPath, fileName);

    public void ThrowIfExists(TEntityId id)
    {
        if (allEntities.ContainsKey(id))
            throw new ArgumentException($"Ключ {id} уже представлен в коллекции.");
    }

    public void ThrowIfNotExists(TEntityId id)
    {
        if (!allEntities.ContainsKey(id))
            throw new ArgumentException($"Ключ {id} не представлен в коллекции.");
    }
}