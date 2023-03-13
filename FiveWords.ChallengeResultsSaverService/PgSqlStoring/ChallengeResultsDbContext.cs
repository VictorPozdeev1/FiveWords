using FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models;
using Microsoft.EntityFrameworkCore;

namespace FiveWords.ChallengeResultsSaverService.PgSqlStoring;

internal class ChallengeResultsDbContext : DbContext
{
    public ChallengeResultsDbContext(DbContextOptions<ChallengeResultsDbContext> options)
        : base(options)

    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    public DbSet<Challenge> CompletedChallenges => Set<Challenge>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AnswerOption>().HasKey(answerOption => new { answerOption.ChallengeUnitId, answerOption.Index });
    }
}
