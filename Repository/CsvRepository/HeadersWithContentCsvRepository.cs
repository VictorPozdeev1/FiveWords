using CsvHelper;
using CsvHelper.Configuration;
using FiveWords.DataObjects;

namespace FiveWords.Repository.CsvRepository;



internal abstract class SavingContentLength_HeadersWithContentCsvRepository<THeaderWithContent, THeader, THeaderWithContentLength, THeaderId, TContentElement>
where THeaderWithContent : IHeaderWithContent<THeader, THeaderId, TContentElement>
where THeader : BaseEntity<THeaderId>, IHeaderAttachingContentLength<THeaderWithContentLength>
where THeaderWithContentLength : BaseEntity<THeaderId>, IHeaderDetachingContentLength<THeader>, IHeaderAttachingContent<THeaderWithContent, TContentElement>
where THeaderId : IEquatable<THeaderId>
{
    protected SavingContentLength_HeadersWithContentCsvRepository(string repoDirectoryPath, string headersFileName, ClassMap<THeaderWithContentLength> headersWithContentLengthMapping, ClassMap<TContentElement> contentMapping)
    {
        headersRepository = new OneFileCsvRepository<THeaderWithContentLength, THeaderId>(repoDirectoryPath, headersFileName, headersWithContentLengthMapping);
        contentDirectoryPath = Path.Combine(repoDirectoryPath, "Content");
        if (!Directory.Exists(contentDirectoryPath))
            Directory.CreateDirectory(contentDirectoryPath);
        this.contentMapping = contentMapping;
    }

    private readonly OneFileCsvRepository<THeaderWithContentLength, THeaderId> headersRepository;

    protected readonly string contentDirectoryPath;
    protected readonly ClassMap<TContentElement> contentMapping;

    protected abstract string GetDefaultFileNameForContent(THeaderId id);
    private string GetDefaultFilePathForContent(THeaderId id) => Path.Combine(contentDirectoryPath, GetDefaultFileNameForContent(id));

    public bool Exists(THeaderId id) => headersRepository.Exists(id);

    public THeaderWithContentLength? GetHeaderWithContentLength(THeaderId id) => headersRepository.Get(id);
    public IReadOnlyDictionary<THeaderId, THeaderWithContentLength> GetAllHeadersWithContentLength() => headersRepository.GetAll();

    public THeader? GetHeader(THeaderId id) => GetHeaderWithContentLength(id)?.GetHeaderWithoutContentLength();

    public THeaderWithContent? GetHeaderWithContent(THeaderId id)
    {
        var headerWithContentLength = headersRepository.Get(id);
        if (headerWithContentLength is null)
            return default;
        return headerWithContentLength.GetHeaderWithContent(ReadContentFromFile(id));
    }

    private List<TContentElement> ReadContentFromFile(THeaderId id)
        => Utils.ReadAllFromFileToList(GetDefaultFilePathForContent(id), contentMapping);


    public void AddAndImmediatelySave(THeaderWithContent headerWithContent)
    {
        headersRepository.AddAndImmediatelySave(headerWithContent.Header.GetHeaderWithContentLength(headerWithContent.Content.Count));
        SaveContent(headerWithContent);
    }

    public void DeleteAndImmediatelySave(THeaderId id)
    {
        DeleteContent(id);
        headersRepository.DeleteAndImmediatelySave(id);
    }

    private void SaveContent(THeaderWithContent headerWithContent)
    {
        Utils.WriteAllToFile(headerWithContent.Content, GetDefaultFilePathForContent(headerWithContent.Header.Id), contentMapping);
    }

    private void DeleteContent(THeaderId id)
    {
        File.Delete(GetDefaultFilePathForContent(id));
    }

    private void RenameContent(THeaderId oldHeaderId, THeaderId newHeaderId)
    {
        File.Move(GetDefaultFilePathForContent(oldHeaderId), GetDefaultFilePathForContent(newHeaderId));
    }

    public void UpdateHeaderAndImmediatelySave(THeaderId id, THeader header)
    {
        headersRepository.ThrowIfNotExists(id);
        if (!id.Equals(header.Id))
            headersRepository.ThrowIfExists(header.Id);

        var oldHeaderWithContentLength = GetHeaderWithContentLength(id);
        var currentContentLength = oldHeaderWithContentLength!.ContentLength;
        headersRepository.UpdateAndImmediatelySave(id, header.GetHeaderWithContentLength(currentContentLength));
        if (!id.Equals(header.Id))
            RenameContent(id, header.Id);
    }

    public void UpdateAndImmediatelySave(THeaderId id, THeaderWithContent headerWithContent)
    {
        throw new NotImplementedException();
        //headersRepository.AddAndImmediatelySave(headerWithContent.Header.GetHeaderWithContentLength(headerWithContent.Content.Count));
        //SaveContent(headerWithContent);
    }
}