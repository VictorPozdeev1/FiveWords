using System.ComponentModel.DataAnnotations.Schema;

namespace FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
[Table("answer_options")]
internal class AnswerOption
{
    [Column("unit_id")]
    public int ChallengeUnitId { get; set; }

    [Column("index")]
    public int Index { get; set; }

    [Column("text")]
    public string Text { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
