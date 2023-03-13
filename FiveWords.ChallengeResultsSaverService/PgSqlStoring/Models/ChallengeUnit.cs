using System.ComponentModel.DataAnnotations.Schema;

namespace FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models;

[Table("units")]
internal class ChallengeUnit
{
    [Column("id")]
    public int Id { get; set; }
    [Column("challenge_id")]
    public Guid ChallengeId { get; set; }
    [Column("question")]
    public string Question { get; set; }
    public ICollection<AnswerOption> AnswerOptions { get; set; }
    [Column("right_index")]
    public int RightAnswerOptionIndex { get; set; }
    [Column("answered_index")]
    public int SelectedAnswerOptionIndex { get; set; }
}
