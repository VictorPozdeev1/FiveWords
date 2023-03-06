using Microsoft.EntityFrameworkCore;

namespace FiveWords.Repository.Tests.Helpers.EF;

internal class OneTableDbContext<TEntity> : DbContext
    where TEntity : class // По-хорошему, тут должно быть : BaseEntity<TId>, но т.к. я собираюсь добавить в PgSql суррогатные Id, то хз, как это всё склеится потом
{
    public OneTableDbContext(DbContextOptions<OneTableDbContext<TEntity>> options)
        : base(options)
        => Database.EnsureCreated();

    public DbSet<TEntity> Entities => Set<TEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // hmmmm...
        modelBuilder.Entity<TEntity>().ToTable(typeof(TEntity).Name + 's');
    }
}