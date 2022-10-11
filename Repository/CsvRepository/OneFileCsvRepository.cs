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
            if (allEntities.ContainsKey(entity.Id))
                throw new ArgumentException($"Ключ {entity.Id} уже представлен в коллекции.");

            allEntities.Add(entity.Id, entity);
            AppendOneEntityToFile(entity);
        }
    }

    void LoadFromFile() => allEntities = Utils.ReadAllFromFileToDictionary(FilePath, Mapping, e => e.Id);

    public void UpdateAndImmediatelySave(TEntityId id, TEntity entity)
    {
        if (!allEntities.ContainsKey(id))
            throw new ArgumentException($"Ключ {id} не представлен в коллекции.");
        if (!id.Equals(entity.Id))
        {
            if (allEntities.ContainsKey(entity.Id))
                throw new ArgumentException($"Ключ {entity.Id} уже представлен в коллекции.");
            allEntities.Remove(id);
        }
        allEntities[entity.Id] = entity;
        SaveToFile();
    }

    public void DeleteAndImmediatelySave(TEntityId id)
    {
        if (!allEntities.ContainsKey(id))
            throw new ArgumentException($"Ключ {id} не представлен в коллекции.");
        allEntities.Remove(id);
        SaveToFile();
    }

    private void SaveToFile() => Utils.WriteAllToFile(allEntities.Values, FilePath, Mapping);

    private void AppendOneEntityToFile(TEntity value) => Utils.AppendOneEntityToFile(value, FilePath, Mapping);

    string FilePath => Path.Combine(repoDirectoryPath, fileName);
}