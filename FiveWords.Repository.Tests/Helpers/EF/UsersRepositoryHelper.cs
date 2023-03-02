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

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(configuration.GetConnectionString("Testing_PgSqlCommon"));
        //connectionStringBuilder.Database = Guid.NewGuid().ToString();
        connectionStringBuilder.Password = configuration["DbPasswords:Testing_PgSqlCommon"];
        connectionString = connectionStringBuilder.ConnectionString; // is used for both global (for this class) dbContext and local (in some methods) dbContexts

        state = RepositoryHelperState.RepositoryIsDown;
    }

    RepositoryHelperState state;
    string connectionString;
    CommonDbContext dbContext;

    OneTableDbContext<User> CreateTempDbContext()
    {
        DbContextOptionsBuilder<OneTableDbContext<User>> optionsBuilder = new();
        optionsBuilder.UseNpgsql(connectionString);
        return new OneTableDbContext<User>(optionsBuilder.Options);
    }

    public ISimpleEntityRepository<User, string> CreateRepositoryWithOneEntity(User singleEntity)
    {
        if (state != RepositoryHelperState.RepositoryIsDown)
            throw new InvalidOperationException();


        using (var tempDbContext = CreateTempDbContext())
        {
            tempDbContext.Set<User>().Add(singleEntity);
            tempDbContext.SaveChanges();
        }

        var optionsBuilder = new DbContextOptionsBuilder<CommonDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        dbContext = new CommonDbContext(optionsBuilder.Options);

        var result = new UsersRepository(dbContext);
        state = RepositoryHelperState.RepositoryIsUp;
        return result;
    }

    public ISimpleEntityRepository<User, string> CreateRepositoryWithSomeEntities(IEnumerable<User> entitiesToAdd)
    {
        if (state != RepositoryHelperState.RepositoryIsDown)
            throw new InvalidOperationException();

        using (var tempDbContext = CreateTempDbContext())
        {
            tempDbContext.Set<User>().AddRange(entitiesToAdd);
            tempDbContext.SaveChanges();
        }

        var optionsBuilder = new DbContextOptionsBuilder<CommonDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        dbContext = new CommonDbContext(optionsBuilder.Options);

        var result = new UsersRepository(dbContext);
        state = RepositoryHelperState.RepositoryIsUp;
        return result;
    }

    public void DeleteRepository()
    {
        if (state != RepositoryHelperState.RepositoryIsUp)
            throw new InvalidOperationException();
        dbContext.Database.ExecuteSqlRaw("Truncate table \"Users\"");
        dbContext.Dispose();
        state = RepositoryHelperState.RepositoryIsDown;
    }

    public void Clean()
    {
        using (var tempDbContext = CreateTempDbContext())
        {
            tempDbContext.Database.EnsureDeleted();
        }
        state = RepositoryHelperState.Cleaned;
    }

    public IEnumerable<User> GetAllEntitiesFromRepository()
    {
        List<User> result = new();
        using (var tempDbContext = CreateTempDbContext())
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
