using System.Text.Json.Serialization;

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

record UserPasswordPair(string Login, ushort[] PasswordHash)
{
    public string PasswordHashAsString => checked(BitConverter.ToString(PasswordHash.Select(us => (byte)us).ToArray()));

    //    [JsonPropertyName("Login")]
    //    public string? IdPropertyForJsonReading { get => Id; init => Id = value; }
    //    public string PasswordHashAsString => checked(BitConverter.ToString(PasswordHash.Select(us => (byte)us).ToArray()));

    public static Dictionary<string, string[]> GetValidationProblems(UserPasswordPair? userPasswordPair, out byte[]? passwordHash)
    {
        Dictionary<string, string[]> result = new();
        passwordHash = null;

        if (userPasswordPair is null)
        {
            string correctJsonExample = "{\"login\":\"Login example\",\"passwordHash\":[244,83,75,54,55,175,56,159,100,173,122,2,46,49,210,45,4,228,176,242,62,67,56,230,226,31,248,154,53,233,251,168]}";
            result["Json Format"] = new string[] { $"Incorrect Json format.  Expected {correctJsonExample}." };
            return result;
        }

        if (string.IsNullOrWhiteSpace(userPasswordPair.Login))
            result["Login"] = new string[] { "Не указан логин." };

        if (userPasswordPair.PasswordHash is null)
            result["PasswordHash"] = new string[] { "Не указан пароль." };
        else
        {
            const int SHA256_LENGTH = 32;
            if (userPasswordPair.PasswordHash.Length != SHA256_LENGTH)
                result["PasswordHash"] = new string[] { $"Неверная длина хэша пароля. Ожидалось {SHA256_LENGTH} байт, получено {userPasswordPair.PasswordHash.Length}" };
            else
            {
                try { passwordHash = checked(userPasswordPair.PasswordHash!.Select(@ushort => (byte)@ushort).ToArray()); }
                catch { }
                if (passwordHash is null)
                    result["PasswordHash"] = new string[] { $"Хэш пароля должен содержать числа типа 'byte'." };
            }
        }
        return result;
    }
}

public record WordTranslation(string Id, string Translation) : BaseEntity<string>(Id)
{
    public WordTranslation() : this(default, default) { }
}

public record UserDictionaryHeader(string Id, int WordsQuantity) : BaseEntity<string>(Id)
{
    public UserDictionaryHeader() : this(default, default) { }
}