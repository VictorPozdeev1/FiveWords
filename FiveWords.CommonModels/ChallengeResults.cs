namespace FiveWords.CommonModels;

public record ChallengeResults<TChallengeUnitUserAnswer>(Guid ChallengeId, TChallengeUnitUserAnswer[] Answers)
    where TChallengeUnitUserAnswer : ChallengeUnitUserAnswer
{ }
