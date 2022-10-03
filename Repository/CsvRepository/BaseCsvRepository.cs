using CsvHelper;
using CsvHelper.Configuration;
using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FiveWords.Repository.CsvRepository;

abstract internal class OneFileCsvRepository<TEntity, TEntityId, TMapping> : UsingFileSystemRepository, IBaseRepository<TEntity, TEntityId>
    where TEntity : BaseEntity<TEntityId>
    where TEntityId : IEquatable<TEntityId>
    where TMapping : ClassMap<TEntity>
{
    protected OneFileCsvRepository(string homeDirectoryPath, string fileName) : base(homeDirectoryPath)
    {
        this.fileName = fileName;
        LoadFromFile();
    }

    private protected string fileName;

    Dictionary<TEntityId, TEntity> allEntities = new();
    abstract protected TMapping Mapping { get; }

    public TEntity? Get(TEntityId id)
    {
        allEntities.TryGetValue(id, out TEntity? result);
        return result;
    }

    public IReadOnlyDictionary<TEntityId, TEntity> GetAll() => new ReadOnlyDictionary<TEntityId, TEntity>(allEntities);

    public void AddAndImmediatelySave(IEnumerable<TEntity> entities)
    {
        throw new NotImplementedException();
    }

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
            using var stream = File.Open(FilePath, FileMode.Append);
            using var writer = new StreamWriter(stream);
            using var csvWriter = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false });
            csvWriter.Context.RegisterClassMap(Mapping);
            csvWriter.WriteRecords(new TEntity[1] { entity });
        }
    }

    void LoadFromFile()
    {
        using var reader = new StreamReader(FilePath, new FileStreamOptions { Mode = FileMode.OpenOrCreate });
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        csvReader.Context.RegisterClassMap(Mapping);
        allEntities = csvReader.GetRecords<TEntity>().ToDictionary(e => e.Id);
    }

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

    private void SaveToFile()
    {
        using var writer = new StreamWriter(FilePath);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csvWriter.Context.RegisterClassMap(Mapping);
        csvWriter.WriteRecords(allEntities.Values);
    }

    string FilePath => Path.Combine(homeDirectoryPath, fileName);
}