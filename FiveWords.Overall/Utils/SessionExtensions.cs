using FiveWords.DataObjects;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FiveWords.Utils;

public static class SessionExtensions
{
    public static void SetUserChallenge<TUserChallenge>(this ISession session, Guid id, TUserChallenge userChallenge, IOptionsSnapshot<JsonSerializerOptions>? serializingOptionsProvider)
        where TUserChallenge : UserChallenge
        => session.SetString(id.ToString(), JsonSerializer.Serialize(userChallenge, serializingOptionsProvider?.Get("Internal")));

    public static TUserChallenge? GetUserChallenge<TUserChallenge>(this ISession session, Guid id, IOptionsSnapshot<JsonSerializerOptions>? serializingOptionsProvider)
        where TUserChallenge : UserChallenge
    {
        var savedJson = session.GetString(id.ToString());
        if (string.IsNullOrWhiteSpace(savedJson))
            return default;
        return JsonSerializer.Deserialize<TUserChallenge>(savedJson, serializingOptionsProvider?.Get("Internal"));
    }
}