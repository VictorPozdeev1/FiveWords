namespace FiveWords.CommonModels;

public record ChoosingRightOptionChallengeResult(Guid ChallengeId, ChoosingRightOptionChallengeUnitUserAnswer[] UserAnswers) : ChallengeResult<ChoosingRightOptionChallengeUnitUserAnswer>(ChallengeId, UserAnswers)
{ }
