namespace FiveWords.DataObjects
{
    public abstract class UserChallenge { }

    public class UserChallenge<TUnit> : UserChallenge
        where TUnit : UserChallengeUnit
    {
        public TUnit[] Units { get; init; } = Array.Empty<TUnit>();
    }

    //public class GuessTranslation_UserChallenge : UserChallenge<GuessRightVariant_UserChallengeUnit<WordWithEnglishTranslation, Word>>
    //{ }
}