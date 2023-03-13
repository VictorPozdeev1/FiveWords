using FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models;
using FiveWords.CommonModels.Backend;

namespace FiveWords.ChallengeResultsSaverService.PgSqlStoring;

internal class ChallengeResultsToPgSqlSaver : IChallengeResultsSaver
{
    readonly IServiceProvider serviceProvider;
    public ChallengeResultsToPgSqlSaver(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider;

    public async Task AppendChallengeResultsAsync(ChoosingRightOptionChallengeCompletedByUser<string, string> challengeCompletedByUser)
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ChallengeResultsDbContext>();

        Challenge challenge = new(challengeCompletedByUser);

        dbContext.CompletedChallenges.Add(challenge);
        await dbContext.SaveChangesAsync();
    }
}
