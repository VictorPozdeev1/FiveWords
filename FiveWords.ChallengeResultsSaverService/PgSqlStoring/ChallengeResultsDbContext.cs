using FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models;
using Microsoft.EntityFrameworkCore;

namespace FiveWords.ChallengeResultsSaverService.PgSqlStoring;

internal class ChallengeResultsDbContext : DbContext
{
    public ChallengeResultsDbContext(DbContextOptions<ChallengeResultsDbContext> options)
        : base(options)

    {
        Database.Migrate();
    }

    public DbSet<Challenge> CompletedChallenges => Set<Challenge>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnswerOption>().HasKey(answerOption => new { answerOption.ChallengeUnitId, answerOption.Index });
    }
}
