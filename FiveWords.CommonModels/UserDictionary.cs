using System.Text.RegularExpressions;

namespace FiveWords.CommonModels;

public record UserDictionaryHeader(string Id) : BaseEntity<string>(Id), IHeaderAttachingContentLength<UserDictionaryHeaderWithWordsQuantity>
{
    public UserDictionaryHeader() : this((string)default) { }

    public UserDictionaryHeaderWithWordsQuantity GetHeaderWithContentLength(int wordsQuantity) => new UserDictionaryHeaderWithWordsQuantity(Id, wordsQuantity);

    public Dictionary<string, string[]> GetValidationProblems()
    {
        Dictionary<string, string[]> result = new();
        if (Id is null)
            result["Id"] = new string[] { "Не указано название словаря." };
        else
        {
            if (!Regex.IsMatch(Id, @"^[\wа-яA-ЯЁё ]{4,20}$"))
                result["Id"] = new string[] { "Название словаря должно состоять из 4-20 букв, цифр, пробелов или подчёркиваний." };
        }
        return result;
    }
}

public record UserDictionaryHeaderWithWordsQuantity(string Id, int WordsQuantity) : BaseEntity<string>(Id), IHeaderDetachingContentLength<UserDictionaryHeader>, IHeaderAttachingContent<UserDictionary, WordTranslation>
{
    public UserDictionaryHeaderWithWordsQuantity() : this(default, default) { }

    public int ContentLength => WordsQuantity;

    public UserDictionary GetHeaderWithContent(ICollection<WordTranslation> content) => new UserDictionary() { Header = GetHeaderWithoutContentLength(), Content = content };

    public UserDictionaryHeader GetHeaderWithoutContentLength() => new UserDictionaryHeader(this.Id);
}

public record UserDictionary(UserDictionaryHeader Header, ICollection<WordTranslation> Content) : IHeaderWithContent<UserDictionaryHeader, string, WordTranslation>
{
    public UserDictionary() : this(default, default) { }

    public Dictionary<string, string[]> GetValidationProblems()
    {
        Dictionary<string, string[]> result = new();
        if (Header is null)
            result["Header"] = new string[] { "Не указан заголовок словаря." };
        else
        {
            foreach (var kvp in Header.GetValidationProblems())
                result[$"Header.{kvp.Key}"] = kvp.Value;
        }
        if (Content is null)
            result["Content"] = new string[] { "Не указан список слов в словаре. Если в словаре не должно быть слов, нужно указать пустой список." };
        return result;
    }
}