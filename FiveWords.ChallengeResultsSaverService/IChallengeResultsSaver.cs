using FiveWords.CommonModels.Backend;

namespace FiveWords.ChallengeResultsSaverService;

internal interface IChallengeResultsSaver
{
    Task AppendChallengeResultsAsync(ChoosingRightOptionChallengeCompletedByUser<string, string> challengePassedByUser);
}
