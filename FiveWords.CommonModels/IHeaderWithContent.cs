namespace FiveWords.CommonModels;

public interface IHeaderWithContent<THeader, THeaderId, TContentElement>
    //where THeader : BaseEntity<THeaderId>
    where THeaderId : IEquatable<THeaderId>
{
    THeader Header { get; init; }
    ICollection<TContentElement> Content { get; init; }
}

public interface IHeaderDetachingContentLength<THeaderWithoutContentLength>
{
    THeaderWithoutContentLength GetHeaderWithoutContentLength();
    int ContentLength { get; }
}

public interface IHeaderAttachingContentLength<THeaderWithContentLength>
{
    THeaderWithContentLength GetHeaderWithContentLength(int contentLength);
}

public interface IHeaderAttachingContent<THeaderWithContent, TContentElement>
{
    THeaderWithContent GetHeaderWithContent(ICollection<TContentElement> content);
}