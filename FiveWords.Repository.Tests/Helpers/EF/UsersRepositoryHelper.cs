using FiveWords.DataObjects;
using FiveWords.Repository.EF;
using FiveWords.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace FiveWords.Repository.Tests.Helpers.EF;

internal sealed class UsersRepositoryHelper : ISimpleEntityRepositoryHelper<User, string>
{
    public UsersRepositoryHelper()
    {
        ConfigurationManager configuration = new ConfigurationManager();
        configuration.AddJsonFile("testingappsettings.json");
        configuration.AddUserSecrets("8563013a-6b16-431b-871b-250f9ffea08d");

        var optionsBuilder = new DbContextOptionsBuilder<CommonDbContext>();
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(configuration.GetConnectionString("Testing_PgSqlCommon"));
        //connectionStringBuilder.Database = Guid.NewGuid().ToString();
        connectionStringBuilder.Password = configuration["DbPasswords:Testing_PgSqlCommon"];
        connectionString = connectionStringBuilder.ConnectionString; // will be used for other local dbContexts also
        optionsBuilder.UseNpgsql(connectionString);

        dbContext = new CommonDbContext(optionsBuilder.Options);
        dbContext.Database.EnsureCreated();

        state = RepositoryHelperState.RepositoryIsDown;
    }

    RepositoryHelperState state;
    string connectionString;
    CommonDbContext dbContext;

    public ISimpleEntityRepository<User, string> CreateRepositoryWithOneEntity(User singleEntity)
    {
        if (state != RepositoryHelperState.RepositoryIsDown)
            throw new InvalidOperationException();
        dbContext.Set<User>().Add(singleEntity);
        dbContext.SaveChanges();
        var result = new UsersRepository(dbContext);
        state = RepositoryHelperState.RepositoryIsUp;
        return result;
    }

    public ISimpleEntityRepository<User, string> CreateRepositoryWithSomeEntities(IEnumerable<User> entitiesToAdd)
    {
        if (state != RepositoryHelperState.RepositoryIsDown)
            throw new InvalidOperationException();
        dbContext.Set<User>().AddRange(entitiesToAdd);
        dbContext.SaveChanges();
        var result = new UsersRepository(dbContext);
        state = RepositoryHelperState.RepositoryIsUp;
        return result;
    }

    public void DeleteRepository()
    {
        if (state != RepositoryHelperState.RepositoryIsUp)
            throw new InvalidOperationException();
        dbContext.Database.ExecuteSqlRaw("Truncate table \"Users\"");
        state = RepositoryHelperState.RepositoryIsDown;
    }

    public void Clean()
    {
        dbContext.Database.EnsureDeleted();
        state = RepositoryHelperState.Cleaned;
    }

    public IEnumerable<User> GetAllEntitiesFromRepository()
    {
        List<User> result = new();
        DbContextOptionsBuilder<OneTableDbContext<User>> optionsBuilder = new DbContextOptionsBuilder<OneTableDbContext<User>>();
        optionsBuilder.UseNpgsql(connectionString);
        using (OneTableDbContext<User> tempDbContext = new OneTableDbContext<User>(optionsBuilder.Options))
        {
            tempDbContext.Entities.FromSqlRaw("Select * from \"Users\"");
            result = tempDbContext.Entities.ToList();
        }
        return result;
    }

    public string GetSimilarButNotExistingId(string exampleId)
    {
        // 1. Possibly, later there will be surrogate Id from PgSql instead of string Id
        // 2. Для простоты предполагается, что наборы тестовых данных составлены так, что требование "not existing" выполняется при простом добавлении пробела.
        // Если в какой-то момент это станет не так, то придётся дописать проверку и другую генерацию.
        return exampleId + ' ';
    }
}
