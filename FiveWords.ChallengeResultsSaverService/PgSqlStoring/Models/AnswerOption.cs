using System.ComponentModel.DataAnnotations.Schema;

namespace FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models;

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
