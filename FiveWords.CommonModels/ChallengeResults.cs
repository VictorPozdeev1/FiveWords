namespace FiveWords.CommonModels;

public record ChallengeResults<TChallengeUnitUserAnswer>(Guid ChallengeId, TChallengeUnitUserAnswer[] UserAnswers)
    where TChallengeUnitUserAnswer : ChallengeUnitUserAnswer
{ }
