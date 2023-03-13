using System.ComponentModel.DataAnnotations.Schema;

namespace FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models;

[Table("challenges")]
internal class Challenge
{
    [Column("id")]
    public Guid Id { get; set; }
    [Column("user_guid")]
    public Guid UserGuid { get; set; }
    [Column("completion_time")]
    public DateTime Completion_Time { get; set; }
    public ICollection<ChallengeUnit> ChallengeUnits { get; set; }
}
