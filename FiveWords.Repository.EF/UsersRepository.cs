using FiveWords.CommonModels;

namespace FiveWords.Repository.EF;

public class UsersRepository : GenericSimpleEntityRepository<User, string>
{
    public UsersRepository(CommonDbContext dbContext) : base(dbContext) { }
}
