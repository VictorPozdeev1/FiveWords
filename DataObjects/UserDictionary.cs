using System.Text.RegularExpressions;

namespace FiveWords.DataObjects;

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

public record UserDictionary : UserDictionaryHeader, IContaining<WordTranslation>
{
    public ICollection<WordTranslation> Content { get; set; }
}