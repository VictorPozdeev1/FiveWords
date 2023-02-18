namespace FiveWords.DataObjects;

public interface IHeaderWithContent<THeader, THeaderId, TContentElement>
    //where THeader : BaseEntity<THeaderId>
    where THeaderId : IEquatable<THeaderId>
{
    THeader Header { get; init; }
    ICollection<TContentElement> Content { get; init; }
}

internal interface IHeaderDetachingContentLength<THeaderWithoutContentLength>
{
    THeaderWithoutContentLength GetHeaderWithoutContentLength();
    int ContentLength { get; }
}

internal interface IHeaderAttachingContentLength<THeaderWithContentLength>
{
    THeaderWithContentLength GetHeaderWithContentLength(int contentLength);
}

internal interface IHeaderAttachingContent<THeaderWithContent, TContentElement>
{
    THeaderWithContent GetHeaderWithContent(ICollection<TContentElement> content);
}