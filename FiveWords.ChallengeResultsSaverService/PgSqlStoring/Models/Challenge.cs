using System.ComponentModel.DataAnnotations.Schema;

namespace FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models;

[Table("challenges")]
internal class Challenge
{
    public Challenge(Guid Id, Guid UserGuid, DateTime CompletedAt)
    {
        this.Id = Id;
        this.UserGuid = UserGuid;
        this.CompletedAt = CompletedAt;
    }

    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_guid")]
    public Guid UserGuid { get; set; }

    [Column("completed_at")]
    public DateTime CompletedAt { get; set; }

    public ICollection<ChallengeUnit> ChallengeUnits { get; set; }
}
