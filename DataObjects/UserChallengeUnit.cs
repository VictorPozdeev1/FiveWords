namespace FiveWords.DataObjects;

public abstract record UserChallengeUnit : BaseEntity<int>;
public abstract record UserChallengeUnit<TQuestion>(TQuestion? Question) : UserChallengeUnit;

public abstract record UserChallengeUnitHavingRightAnswer<TQuestion, TAnswer>(TQuestion? Question, TAnswer? RightAnswer)
    : UserChallengeUnit<TQuestion>(Question);

public record GuessRightVariant_UserChallengeUnit<TQuestion, TAnswerVariant>(TQuestion? Question, TAnswerVariant[]? AnswerVariants, int RightAnswer)
    : UserChallengeUnitHavingRightAnswer<TQuestion, int>(Question, RightAnswer);

