namespace FiveWords._v1.DataObjects;

public abstract record UserChallengeUnit_v1<TQuestion>(TQuestion? Question) : FiveWords.DataObjects.UserChallengeUnit;

public abstract record HavingRightAnswer_UserChallengeUnit_v1<TQuestion, TAnswer>(TQuestion? Question, TAnswer? RightAnswer)
    : UserChallengeUnit_v1<TQuestion>(Question);

public record ChoosingRightOption_UserChallengeUnit_v1<TQuestion, TAnswerVariant>(TQuestion? Question, TAnswerVariant[]? AnswerVariants, int RightAnswer)
    : HavingRightAnswer_UserChallengeUnit_v1<TQuestion, int>(Question, RightAnswer);
