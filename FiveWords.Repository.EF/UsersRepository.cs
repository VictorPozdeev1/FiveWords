using FiveWords.Repository.Interfaces;

namespace FiveWords.Repository.EF;

public class UsersRepository : IBaseRepository
{
    public UsersRepository(CommonDbContext dbContext)
        => this.dbContext = dbContext;
    private readonly CommonDbContext dbContext;
    public int Test => dbContext.Users.Count();
}
