using FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models;
using FiveWords.CommonModels.Backend;

namespace FiveWords.ChallengeResultsSaverService.PgSqlStoring;

internal class ChallengeResultsToPgSqlSaver : IChallengeResultsSaver
{
    readonly IServiceProvider serviceProvider;
    public ChallengeResultsToPgSqlSaver(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider;
    public async Task AppendChallengeResultsAsync(ChoosingRightOptionChallengePassedByUser challengePassedByUser)
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ChallengeResultsDbContext>();

        Challenge challenge = new(challengePassedByUser.Challenge.Id, challengePassedByUser.User.Guid, challengePassedByUser.CompletedAt);

        dbContext.CompletedChallenges.Add(challenge);
        await dbContext.SaveChangesAsync();
    }
}
