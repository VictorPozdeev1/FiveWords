using FiveWords.Model;
using FiveWords.View;
using FiveWords.Repository;
using Microsoft.Extensions.Options;
using System.Text.Json;
using FiveWords.Repository.Interfaces;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using FiveWords.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FiveWords.DataObjects;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace FiveWords.ApiV1;

static class Endpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/exception", void () => throw new Exception("This is a test exception")).WithTags("INFO");

        routeBuilder.MapGet("/routes", ServiceInfo.PrintRoutes).WithTags("INFO");

        routeBuilder.MapPost("/registration", async (HttpContext httpContext, IOptionsSnapshot<JsonSerializerOptions> serializeOptions, IUsersRepository usersRepository, UserPasswordRepositoriesManager passwordRepositoriesManager) =>
        {
            UserPasswordPair? userPasswordPair = null;

            try { userPasswordPair = await httpContext.Request.ReadFromJsonAsync<UserPasswordPair>(serializeOptions.Get("Web")); }
            catch { }

            Dictionary<string, string[]> validationProblems = UserPasswordPair.GetValidationProblems(userPasswordPair, out byte[]? passwordHash);
            if (validationProblems.Count > 0)
                return Results.ValidationProblem(validationProblems);

            var existingUser = usersRepository.Get(userPasswordPair!.Login);
            if (existingUser != null)
                return Results.Conflict(new
                {
                    Error = new
                    {
                        Message = $"Пользователь с логином {userPasswordPair!.Login} уже существует.",
                        User = userPasswordPair!.Login
                    }
                });

            var registeringUser = new User(userPasswordPair.Login, Guid.NewGuid());
            usersRepository.AddAndImmediatelySave(registeringUser);

            passwordRepositoriesManager.GetRepository(registeringUser).SavePasswordHash(passwordHash!);
            //return Results.RedirectToRoute()
            return Results.NoContent();
        });

        routeBuilder.MapPost("/login", async (HttpContext httpContext, IOptionsSnapshot<JsonSerializerOptions> serializingOptions, IUsersRepository usersRepository, UserPasswordRepositoriesManager passwordRepositoriesManager) =>
        {
            //Dictionary<string, string[]> validationProblems = new();
            UserPasswordPair? userPasswordPair = null;
            try { userPasswordPair = await httpContext.Request.ReadFromJsonAsync<UserPasswordPair>(serializingOptions.Get("Web")); }
            catch { }

            Dictionary<string, string[]> validationProblems = UserPasswordPair.GetValidationProblems(userPasswordPair, out byte[]? passwordHash);
            if (validationProblems.Count > 0)
                return Results.ValidationProblem(validationProblems);

            User? user = usersRepository.Get(userPasswordPair!.Login);
            if (user is null || !passwordHash!.SequenceEqual(passwordRepositoriesManager.GetRepository(user).GetPasswordHash()))
                return Results.Conflict(new
                {
                    Error = new
                    {
                        Message = $"Неверный логин и/или пароль.",
                        User = userPasswordPair!.Login
                    }
                });

            //todo Ключ надо бы вынести, например, в файл или в конфигурацию. А ещё можно в utils вынести SigningCredentials, а также issuer (или вообще генерацию токена).

            var jwtHeader = new JwtHeader(new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("uig284hnj&*#^%\\\"34.h567&y")), SecurityAlgorithms.HmacSha256));
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login) };
            var jwtPayload = new JwtPayload("FiveWords", "FiveWords", claims, null, expires: DateTime.UtcNow.AddHours(24));
            var jwt = new JwtSecurityToken(jwtHeader, jwtPayload);
            var jwtEncoded = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Results.Text(jwtEncoded);

            //context.SignInAsync() - проверить,как работает.. а точнее, Results.Signin
        });

        routeBuilder.MapGet("/dictionaryHeaders", [Authorize] (ClaimsPrincipal claimsPrincipal, IUsersRepository usersRepository, UserDictionariesUserRepositoriesManager userDictionariesRepoManager) =>
        {
            var currentUser = usersRepository.Get(claimsPrincipal!.Identity!.Name!);
            var userDictionariesRepo = userDictionariesRepoManager.GetRepository(currentUser!);
            var dictionaryHeaders = userDictionariesRepo.GetAllHeadersWithContentLength();
            return Results.Json(new { User = currentUser!.Login, DictionaryHeaders = dictionaryHeaders.Values });
        });

        routeBuilder.MapDelete("/dictionaries/{dictionaryName}", [Authorize] (string dictionaryName, ClaimsPrincipal claimsPrincipal, IUsersRepository usersRepository, UserDictionariesUserRepositoriesManager userDictionariesRepoManager) =>
        {
            var currentUser = usersRepository.Get(claimsPrincipal!.Identity!.Name!);
            var userDictionariesRepo = userDictionariesRepoManager.GetRepository(currentUser!);
            if (!userDictionariesRepo.Exists(dictionaryName))
                return Results.NotFound($"Не найден словарь \"{dictionaryName}\".");
            userDictionariesRepo.DeleteAndImmediatelySave(dictionaryName);
            return Results.NoContent();
        });

        routeBuilder.MapPut("/dictionaryHeaders/{dictionaryName}", [Authorize] ([FromBody] UserDictionaryHeader updatedDictionaryHeader, string dictionaryName, ClaimsPrincipal claimsPrincipal, IUsersRepository usersRepository, UserDictionariesUserRepositoriesManager userDictionariesRepoManager) =>
        {
            var validationProblems = updatedDictionaryHeader.GetValidationProblems();
            if (validationProblems.Count > 0)
                return Results.ValidationProblem(validationProblems);

            var currentUser = usersRepository.Get(claimsPrincipal!.Identity!.Name!);
            var userDictionariesRepo = userDictionariesRepoManager.GetRepository(currentUser!);

            if (updatedDictionaryHeader.Id != dictionaryName)
            {
                var nameAlreadyExistsConflict = userDictionariesRepo.FindConflict_DictionaryWithSuchNameAlreadyExists(updatedDictionaryHeader);
                if (nameAlreadyExistsConflict is not null)
                    return Results.Conflict(nameAlreadyExistsConflict);
            }

            userDictionariesRepo.UpdateHeaderAndImmediatelySave(dictionaryName, updatedDictionaryHeader);
            return Results.Json(userDictionariesRepo.GetHeaderWithContentLength(updatedDictionaryHeader.Id));
        });

        routeBuilder.MapPut("/dictionaries/{dictionaryName}", [Authorize] ([FromBody] UserDictionary updatedDictionary, string dictionaryName, ClaimsPrincipal claimsPrincipal, IUsersRepository usersRepository, UserDictionariesUserRepositoriesManager userDictionariesRepoManager) =>
        {
            var validationProblems = updatedDictionary.GetValidationProblems();
            if (validationProblems.Count > 0)
                return Results.ValidationProblem(validationProblems);

            var currentUser = usersRepository.Get(claimsPrincipal!.Identity!.Name!);
            var userDictionariesRepo = userDictionariesRepoManager.GetRepository(currentUser!);

            if (updatedDictionary.Header.Id != dictionaryName)
            {
                var nameAlreadyExistsConflict = userDictionariesRepo.FindConflict_DictionaryWithSuchNameAlreadyExists(updatedDictionary.Header);
                if (nameAlreadyExistsConflict is not null)
                    return Results.Conflict(nameAlreadyExistsConflict);
            }
            userDictionariesRepo.UpdateAndImmediatelySave(dictionaryName, updatedDictionary);
            return Results.Json(userDictionariesRepo.GetHeaderWithContent(updatedDictionary.Header.Id));
        });

        routeBuilder.MapPost("/dictionaries", [Authorize] (HttpContext httpContext, [FromBody] UserDictionary dictionaryToCreate, ClaimsPrincipal claimsPrincipal, IUsersRepository usersRepository, UserDictionariesUserRepositoriesManager userDictionariesRepoManager) =>
        {
            var validationProblems = dictionaryToCreate.GetValidationProblems();
            if (validationProblems.Count > 0)
                return Results.ValidationProblem(validationProblems);

            var currentUser = usersRepository.Get(claimsPrincipal!.Identity!.Name!);
            var userDictionariesRepo = userDictionariesRepoManager.GetRepository(currentUser!);
            var nameAlreadyExistsConflict = userDictionariesRepo.FindConflict_DictionaryWithSuchNameAlreadyExists(dictionaryToCreate.Header);
            if (nameAlreadyExistsConflict is not null)
                return Results.Conflict(nameAlreadyExistsConflict);
            userDictionariesRepo.AddAndImmediatelySave(dictionaryToCreate);
            return Results.Json(userDictionariesRepo.GetHeaderWithContent(dictionaryToCreate.Header.Id));
        });

        routeBuilder.MapGet("/dictionaries/{dictionaryName}", [Authorize] (string dictionaryName, ClaimsPrincipal claimsPrincipal, IUsersRepository usersRepository, UserDictionariesUserRepositoriesManager userDictionariesRepoManager) =>
        {
            var currentUser = usersRepository.Get(claimsPrincipal!.Identity!.Name!);
            var userDictionariesRepo = userDictionariesRepoManager.GetRepository(currentUser!);
            var dictionary = userDictionariesRepo.GetHeaderWithContent(dictionaryName);
            return dictionary is null ? Results.NotFound($"Не найден словарь \"{dictionaryName}\".") : Results.Json(dictionary);
        });

        routeBuilder.MapGet("/data", [Authorize] (ClaimsPrincipal user) =>
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<p>");
            sb.Append($"{user!.Identity!.Name},");
            sb.Append($" auth: {user.Identity.IsAuthenticated}, type: {user.Identity.AuthenticationType}");
            sb.Append("</p>");
            sb.Append("<br>");
            foreach (var claim in user.Claims)
            {
                sb.Append($"<p>{claim.ToString()} </p>");
            }
            return Results.Text(sb.ToString());
        }).WithTags("INFO", "AUTHORIZE");
    }
}
