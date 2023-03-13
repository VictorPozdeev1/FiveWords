namespace FiveWords.CommonModels.Backend;

public record ChoosingRightOptionChallengeUnitCompletedByUser<TQuestion, TAnswerOption>
    (
    TQuestion Question,
    ICollection<TAnswerOption> AnswerOptions,
    int RightAnswerOptionIndex,
    ChoosingRightOptionChallengeUnitUserAnswer UserAnswer
    )
{ }
