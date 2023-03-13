namespace FiveWords.CommonModels.Backend;

public record ChoosingRightOptionChallengePassedByUser(
    User User,
    ChoosingTranslationUserChallenge Challenge,
    DateTime CompletedAt,
    ChoosingRightOptionChallengeUnitUserAnswer[] UserAnswers
    )
{ }
