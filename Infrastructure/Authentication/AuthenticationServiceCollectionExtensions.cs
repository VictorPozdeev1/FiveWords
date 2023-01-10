using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace FiveWords.Infrastructure.Authentication;

public static class AuthenticationServiceCollectionExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration tokenValidationParameters)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            tokenValidationParameters.Bind(options.TokenValidationParameters);
            var signingKeyString = tokenValidationParameters["IssuerSigningKeyAsUTF8"];
            if (string.IsNullOrWhiteSpace(signingKeyString))
                Log.Fatal("JWT validation encoutered a problem. JWT issuer signing key is not set properly. Current value: \"{SigningKeyString}\"", signingKeyString);
            options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(signingKeyString));
        });
    }
}