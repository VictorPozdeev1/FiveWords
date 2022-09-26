namespace FiveWords.Utils;

public class HttpFileSender
{
    IHttpContextAccessor httpContextAccessor;
    public HttpFileSender(IHttpContextAccessor httpContextAccessor) =>
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public Task SendHtmlFileAsync(string pathToFile)
    {
        var httpResponse = httpContextAccessor.HttpContext.Response;
        httpResponse.ContentType = "text/html; charset=utf-8";
        return httpResponse.SendFileAsync(pathToFile);
    }
}
