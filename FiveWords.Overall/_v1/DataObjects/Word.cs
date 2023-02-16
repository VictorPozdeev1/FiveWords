using FiveWords.DataObjects;

namespace FiveWords._v1.DataObjects;

public enum Language
{
    English,
    Russian
}

public interface ILanguageRelated
{
    Language Language { get; }
}

public record Word(int Id, string? Writing, Language Language) : BaseEntity<int>(Id)
{
    public Word() : this(default, default, default) { }
}

public record WordWithEnglishTranslationId(int Id, string? Writing, Language Language, int? DefaultEnglishTranslationId) : Word(Id, Writing, Language)
{
    public WordWithEnglishTranslationId() : this(default, default, default, default) { }
}