using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace FiveWords.Repository.CsvRepository;

internal static class Utils
{
    public static List<TEntity> ReadAllFromFileToList<TEntity>(string filePath, ClassMap<TEntity> mapping )
    {
        using var reader = new StreamReader(filePath, new FileStreamOptions { Mode = FileMode.OpenOrCreate });
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        csvReader.Context.RegisterClassMap(mapping);
        return csvReader.GetRecords<TEntity>().ToList();
    }

    public static Dictionary<TEntityId, TEntity> ReadAllFromFileToDictionary<TEntityId, TEntity>(string filePath, ClassMap<TEntity> mapping, Func<TEntity, TEntityId> keySelector)
    {
        using var reader = new StreamReader(filePath, new FileStreamOptions { Mode = FileMode.OpenOrCreate });
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        csvReader.Context.RegisterClassMap(mapping);
        return csvReader.GetRecords<TEntity>().ToDictionary(keySelector);
    }

    public static void WriteAllToFile<TEntity>(IEnumerable<TEntity> values, string filePath, ClassMap<TEntity> mapping)
    {
        using var writer = new StreamWriter(filePath);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csvWriter.Context.RegisterClassMap(mapping);
        csvWriter.WriteRecords(values);
    }

    public static void AppendOneEntityToFile<TEntity>(TEntity value, string filePath, ClassMap<TEntity> mapping)
    {
        using var stream = File.Open(filePath, FileMode.Append);
        using var writer = new StreamWriter(stream);
        using var csvWriter = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false });
        csvWriter.Context.RegisterClassMap(mapping);
        csvWriter.WriteRecords(new TEntity[1] { value });
    }
}
