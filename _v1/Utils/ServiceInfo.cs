namespace FiveWords._v1.Utils;

public static class ServiceInfo
{
    public static IResult PrintRoutes(IEnumerable<EndpointDataSource> endpoints) => Results.Content(
            string.Join('\n', endpoints.SelectMany(endpointsData => endpointsData.Endpoints, (endpointData, endpoint) =>
                $"[{string.Join(", ", endpoint.Metadata.OfType<TagsAttribute>().SelectMany(data => data.Tags))}] {(endpoint as RouteEndpoint)?.RoutePattern.RawText}: {string.Join(", ", endpoint.Metadata.OfType<HttpMethodMetadata>().SelectMany(metadata => metadata.HttpMethods))}")));

    public static IResult PrintDIServices(IServiceCollection services)
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("<h1>Все сервисы</h1>");
        sb.Append("<table>");
        sb.Append("<tr><th>Тип</th><th>Lifetime</th><th>Реализация</th></tr>");
        foreach (var svc in services)
        {
            sb.Append("<tr>");
            sb.Append($"<td>{svc.ServiceType.FullName}</td>");
            sb.Append($"<td>{svc.Lifetime}</td>");
            sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
            sb.Append("</tr>");
        }
        sb.Append("</table>");
        return Results.Text(sb.ToString(), "text/html;charset=utf-8");
    }
}
