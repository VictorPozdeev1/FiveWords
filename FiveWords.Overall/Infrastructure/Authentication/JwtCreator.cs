using FiveWords.CommonModels;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FiveWords.Infrastructure.Authentication;

public sealed class JwtCreator
{
    readonly IConfiguration tokenIssuingParameters;
    public JwtCreator(IConfiguration tokenIssuingParameters) => this.tokenIssuingParameters = tokenIssuingParameters;

    public string CreateToken(User user)
    {
        var signingKeyString = tokenIssuingParameters["IssuerSigningKeyAsUTF8"];
        if (string.IsNullOrWhiteSpace(signingKeyString))
            Log.Error("JWT issuing encoutered a problem. JWT issuer signing key is not set properly. Current value: \"{SigningKeyString}\"", signingKeyString);

        var jwtHeader = new JwtHeader(new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(signingKeyString)), SecurityAlgorithms.HmacSha256));
        var jwtPayload = new JwtPayload
        (
            tokenIssuingParameters["Issuer"],
            tokenIssuingParameters["Audience"],
            claims: new List<Claim> { new Claim(ClaimTypes.Name, user.Login) },
            notBefore: null,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(tokenIssuingParameters["LifetimeInMinutes"]))
        );
        var jwt = new JwtSecurityToken(jwtHeader, jwtPayload);
        var jwtEncoded = new JwtSecurityTokenHandler().WriteToken(jwt);
        return jwtEncoded;
    }
}