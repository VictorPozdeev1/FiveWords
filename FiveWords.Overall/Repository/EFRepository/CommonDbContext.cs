using FiveWords.DataObjects;
using Microsoft.EntityFrameworkCore;

namespace FiveWords.Overall.Repository.EFRepository;

public class CommonDbContext : DbContext
{
    public CommonDbContext(DbContextOptions<CommonDbContext> options)
        : base(options)
        => Database.EnsureCreated();

    public DbSet<User> Users => Set<User>();
}
