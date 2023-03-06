using FiveWords.CommonModels;

namespace FiveWords.Repository.Interfaces;

public interface IUsersRepository : ISimpleEntityRepository<User, string>
{
    public ActionError? FindError_UserWithSuchLoginAlreadyExists(string login)
        => Exists(login) ? new ActionError($"Пользователь с логином \"{login}\" уже существует.", login) : null;
}