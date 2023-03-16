namespace FiveWords.CommonModels;

public record ChallengeResult<TChallengeUnitUserAnswer>(Guid ChallengeId, TChallengeUnitUserAnswer[] UserAnswers)
    where TChallengeUnitUserAnswer : ChallengeUnitUserAnswer
{ }
