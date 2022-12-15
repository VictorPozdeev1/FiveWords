namespace FiveWords.DataObjects
{
    public abstract record UserChallenge : BaseEntity<Guid> { }

    public record UserChallenge<TUnit> : UserChallenge
        where TUnit : UserChallengeUnit
    {
        public TUnit[] Units { get; init; } = Array.Empty<TUnit>();
    }

    public record ChoosingTranslationUserChallenge : UserChallenge<ChoosingRightOption_UserChallengeUnit<string, string>> { }
}