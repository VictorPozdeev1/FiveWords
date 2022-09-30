using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace FiveWords.DataObjects;

public enum Language
{
    English,
    Russian
}

public interface ILanguageRelated
{
    Language Language { get; }
}

public abstract record BaseEntity<TId>(TId Id)
//where TId : struct
where TId: IEquatable<TId>
{
    public BaseEntity() : this((TId)default) { }
}

public record Word(int Id, string? Writing, Language Language) : BaseEntity<int>(Id)
{
    public Word() : this(default, default, default) { }
}

public record WordWithEnglishTranslationId(int Id, string? Writing, Language Language, int? DefaultEnglishTranslationId) : Word(Id, Writing, Language)
{
    public WordWithEnglishTranslationId() : this(default, default, default, default) { }
}


//public record User(Guid Id, string? Login) : BaseEntity<Guid>(Id)
//{
//    public User() : this(default, default) { }

//    private static User? _default;
//    public static User Default => _default ??= new User();
//}

public record User(string Id, Guid Guid) : BaseEntity<string>(Id)
{
    public User() : this(default, default) { }

    private static User? _default;
    public static User Default => _default ??= new User();

    public string Login { get { return Id; } }
}

public record WordTranslation(string Id, string Translation) : BaseEntity<string>(Id)
{
    public WordTranslation() : this(default, default) { }
}

public record UserDictionaryHeader(string Id, int WordsQuantity) : BaseEntity<string>(Id)
{
    public UserDictionaryHeader() : this(default, default) { }

    public Dictionary<string, string[]> GetValidationProblems()
    {
        Dictionary<string, string[]> result = new();
        if (!Regex.IsMatch(Id, @"^[\wа-яA-ЯЁё ]{4,20}$"))
            result["Id"] = new string[] { "Название словаря должно состоять из 4-20 букв, цифр, пробелов или подчёркиваний." };
        return result;
    }
}