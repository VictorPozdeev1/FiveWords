namespace FiveWords.CommonModels;

public record ChoosingRightOptionChallengeUnitUserAnswer(int AnswerTimeInMilliseconds, int SelectedAnswerOptionIndex)
    : ChallengeUnitUserAnswer(AnswerTimeInMilliseconds)
{ }
