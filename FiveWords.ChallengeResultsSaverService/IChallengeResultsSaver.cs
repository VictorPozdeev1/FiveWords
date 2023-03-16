using FiveWords.CommonModels.SavingChallengeResults;

namespace FiveWords.ChallengeResultsSaverService;

internal interface IChallengeResultsSaver
{
    Task AppendChallengeResultsAsync(ChoosingRightOptionChallengeCompletedByUser<string, string> challengePassedByUser);
}
