using FiveWords.CommonModels.SavingChallengeResults;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
[Table("units")]
internal class ChallengeUnit
{
    public ChallengeUnit() { }

    public ChallengeUnit(ChoosingRightOptionChallengeUnitCompletedByUser<string, string> dataToCopy, int indexInChallenge)
    {
        Question = dataToCopy.Question;
        AnswerOptions = dataToCopy.AnswerOptions
            .Select((answerOptionText, index) => new AnswerOption()
            {
                Index = index,
                Text = answerOptionText
            })
            .ToArray();
        RightAnswerOptionIndex = dataToCopy.RightAnswerOptionIndex;
        SelectedAnswerOptionIndex = dataToCopy.UserAnswer.SelectedAnswerOptionIndex;
        AnswerTimeInMilliseconds = dataToCopy.UserAnswer.AnswerTimeInMilliseconds;
        IndexInChallenge = indexInChallenge;
    }

    [Column("id")]
    public int Id { get; set; }

    [Column("challenge_id")]
    public Guid ChallengeId { get; set; }

    [Column("index")]
    public int IndexInChallenge { get; set; }

    [Column("question")]
    public string Question { get; set; }

    public ICollection<AnswerOption> AnswerOptions { get; set; }

    [Column("right_index")]
    public int RightAnswerOptionIndex { get; set; }

    [Column("answered_index")]
    public int SelectedAnswerOptionIndex { get; set; }

    [Column("answer_time_ms")]
    public int AnswerTimeInMilliseconds { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
