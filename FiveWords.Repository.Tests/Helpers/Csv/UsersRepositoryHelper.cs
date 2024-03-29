﻿using FiveWords.CommonModels;
using FiveWords.Repository.Csv;
using FiveWords.Repository.Interfaces;
using System.Text;

namespace FiveWords.Repository.Tests.Helpers.Csv;

internal sealed class UsersRepositoryHelper : ISimpleEntityRepositoryHelper<User, string>
{
    public UsersRepositoryHelper()
    {
        filesId = Guid.NewGuid().ToString();
        Directory.CreateDirectory(filesId);
        state = RepositoryHelperState.RepositoryIsDown;
    }

    RepositoryHelperState state;

    string filesId;
    string PathToFile => Path.Combine(filesId, $"{filesId}.csv");

    public ISimpleEntityRepository<User, string> CreateRepositoryWithOneEntity(User singleUser)
    {
        if (state != RepositoryHelperState.RepositoryIsDown)
            throw new InvalidOperationException();
        File.WriteAllText(PathToFile, $"Id,Guid\r\n{singleUser.Id},{singleUser.Guid.ToString()}\r\n");
        var result = new UsersCsvRepository(filesId, $"{filesId}.csv");
        state = RepositoryHelperState.RepositoryIsUp;
        return result;
    }

    public ISimpleEntityRepository<User, string> CreateRepositoryWithSomeEntities(IEnumerable<User> usersToAdd)
    {
        if (state != RepositoryHelperState.RepositoryIsDown)
            throw new InvalidOperationException();
        StringBuilder stringBuilder = new StringBuilder("Id,Guid\r\n");
        foreach (var userToAdd in usersToAdd)
            stringBuilder.Append($"{userToAdd.Id},{userToAdd.Guid.ToString()}\r\n");
        File.WriteAllText(PathToFile, stringBuilder.ToString());
        var result = new UsersCsvRepository(filesId, $"{filesId}.csv");
        state = RepositoryHelperState.RepositoryIsUp;
        return result;
    }

    public IEnumerable<User> GetAllEntitiesFromRepository()
    {
        // Тут хорошо бы не использовать методы самого репозитория, а самому получить данные из файла. Но пока что сделаю так..
        return new UsersCsvRepository(filesId, $"{filesId}.csv").GetAll().Select(it => it.Value);
    }

    public void DeleteRepository()
    {
        if (state != RepositoryHelperState.RepositoryIsUp)
            throw new InvalidOperationException();
        File.Delete(PathToFile);
        state = RepositoryHelperState.RepositoryIsDown;
    }

    public void Clean()
    {
        Directory.Delete(filesId, true);
        state = RepositoryHelperState.Cleaned;
    }

    // Для простоты предполагается, что наборы тестовых данных составлены так, что требование "not existing" выполняется при простом добавлении пробела.
    // Если в какой-то момент это станет не так, то придётся дописать проверку и другую генерацию.
    public string GetSimilarButNotExistingId(string exampleId) => exampleId + ' ';
}