namespace FiveWords.CommonModels.SavingChallengeResults;

public record ChoosingRightOptionChallengeCompletedByUser<TQuestion, TAnswerOption>(
    Guid Id,
    ICollection<ChoosingRightOptionChallengeUnitCompletedByUser<TQuestion, TAnswerOption>> Units,
    Guid UserGuid,
    DateTime CompletedAt
    )
{ }
