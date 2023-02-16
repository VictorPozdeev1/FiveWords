namespace FiveWords._v1.View;
using HtmlAgilityPack;

public abstract class HtmlViewCreator<TData>
{
    protected virtual string HtmlTemplatePath => Path.Combine("_v1", "html-templates", HtmlTemplateFilename);
    protected abstract string HtmlTemplateFilename { get; }

    public virtual HtmlDocument CreateView(TData data)
    {
        HtmlDocument htmlDocument = new();
        using (var fileStream = File.Open(HtmlTemplatePath, FileMode.Open))
        {
            htmlDocument.Load(fileStream);
        }
        WriteData(htmlDocument, data);
        return htmlDocument;
    }

    protected abstract void WriteData(HtmlDocument htmlDocument, TData data);
}