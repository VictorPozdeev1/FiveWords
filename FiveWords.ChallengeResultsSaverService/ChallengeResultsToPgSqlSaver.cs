using FiveWords.CommonModels.Backend;

namespace FiveWords.ChallengeResultsSaverService;

internal class ChallengeResultsToPgSqlSaver : IChallengeResultsSaver
{
    public async Task AppendChallengeResultsAsync(ChoosingRightOptionChallengePassedByUser challengePassedByUser)
    {
        await Task.Yield();
    }
}
