using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.EF;

public class UsersRepository : ISimpleEntityRepository<User, string>
{
    public UsersRepository(CommonDbContext dbContext)
        => this.dbContext = dbContext;
    private readonly CommonDbContext dbContext;

    public bool Exists(string id)
    {
        throw new NotImplementedException();
    }

    public User? Get(string id)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyDictionary<string, User> GetAll()
    {
        throw new NotImplementedException();
    }

    public void AddAndImmediatelySave(User entity)
    {
        throw new NotImplementedException();
    }

    public void UpdateAndImmediatelySave(string id, User entity)
    {
        throw new NotImplementedException();
    }

    public void DeleteAndImmediatelySave(string id)
    {
        throw new NotImplementedException();
    }
}
