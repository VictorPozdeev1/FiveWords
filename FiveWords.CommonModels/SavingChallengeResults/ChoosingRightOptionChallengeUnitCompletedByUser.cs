namespace FiveWords.CommonModels.SavingChallengeResults;

public record ChoosingRightOptionChallengeUnitCompletedByUser<TQuestion, TAnswerOption>
    (
    TQuestion Question,
    ICollection<TAnswerOption> AnswerOptions,
    int RightAnswerOptionIndex,
    ChoosingRightOptionChallengeUnitUserAnswer UserAnswer
    )
{ }
