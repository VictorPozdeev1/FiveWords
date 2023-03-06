using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FiveWords.CommonModels;

[WordValidation]
public record WordTranslation(string Id, [property: RegularExpression(@"^[\wА-Яа-яЁё\-\.\?!\)\(,:' ]{1,30}$", ErrorMessage = "Разрешённый формат для перевода: 1-30 букв (плюс некоторые знаки препинания).")] string Translation) : BaseEntity<string>(Id)
{
    public WordTranslation() : this(default, default) { }
}

public class WordValidationAttribute : ValidationAttribute
{
    public WordValidationAttribute()
    {
        ErrorMessage = "Разрешённый формат для слова: 1-30 букв (плюс некоторые знаки препинания).";
    }

    public override bool IsValid(object? value)
    {
        WordTranslation? wordTranslation = value as WordTranslation;
        if (wordTranslation is null)
            return false;
        var word = wordTranslation.Id;
        return !string.IsNullOrWhiteSpace(word) && Regex.IsMatch(word, @"^[\wА-Яа-яЁё\-\.\?!\)\(,:' ]{1,30}$");
    }
}