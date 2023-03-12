using FiveWords.CommonModels.Backend;

namespace FiveWords.ChallengeResultsSaverService;

internal interface IChallengeResultsSaver
{
    Task AppendChallengeResultsAsync(ChoosingRightOptionChallengePassedByUser challengePassedByUser);
}
