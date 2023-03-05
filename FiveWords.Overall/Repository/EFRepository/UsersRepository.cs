using FiveWords.Repository.Interfaces;

namespace FiveWords.Overall.Repository.EFRepository;

public class UsersRepository : IBaseRepository
{
    public UsersRepository(CommonDbContext dbContext)
        => this.dbContext = dbContext;
    private readonly CommonDbContext dbContext;
    public int Test => dbContext.Users.Count();
}
