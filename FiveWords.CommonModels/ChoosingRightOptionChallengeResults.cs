namespace FiveWords.CommonModels;

public record ChoosingRightOptionChallengeResults(Guid ChallengeId, ChoosingRightOptionChallengeUnitUserAnswer[] UserAnswers) : ChallengeResults<ChoosingRightOptionChallengeUnitUserAnswer>(ChallengeId, UserAnswers)
{ }
