namespace FiveWords.DataObjects;

public interface IContaining<TContentElement>
{
    ICollection<TContentElement> Content { get; set; }
}
