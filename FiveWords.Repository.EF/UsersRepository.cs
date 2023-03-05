using FiveWords.DataObjects;
using FiveWords.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FiveWords.Repository.EF;

public class UsersRepository : GenericSimpleEntityRepository<User, string>
{
    public UsersRepository(CommonDbContext dbContext) : base(dbContext) { }
}
