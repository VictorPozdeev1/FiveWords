namespace FiveWords.DataObjects;

public abstract record UserChallengeUnit;
public abstract record UserChallengeUnit<TQuestion>(TQuestion? Question) : UserChallengeUnit;

public abstract record HavingRightAnswer_UserChallengeUnit<TQuestion, TAnswer>(TQuestion? Question, TAnswer? RightAnswer)
    : UserChallengeUnit<TQuestion>(Question);

public record ChoosingRightOption_UserChallengeUnit<TQuestion, TAnswerOption>(TQuestion? Question, TAnswerOption[]? AnswerOptions, int RightOptionIndex)
    : HavingRightAnswer_UserChallengeUnit<TQuestion, int>(Question, RightOptionIndex);
