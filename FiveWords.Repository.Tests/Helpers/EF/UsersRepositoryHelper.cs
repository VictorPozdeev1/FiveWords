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
        connectionStringBuilder.Password = configuration["DbPasswords:Testing_PgSqlCommon"];
        optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString);

        dbContext = new CommonDbContext(optionsBuilder.Options);
        dbContext.Database.EnsureCreated();
    }

    RepositoryHelperState state;
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
        throw new NotImplementedException();
    }

    public string GetSimilarButNotExistingId(string exampleId)
    {
        throw new NotImplementedException();
    }
}
