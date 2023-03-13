using FiveWords.CommonModels.SavingChallengeResults;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiveWords.ChallengeResultsSaverService.PgSqlStoring.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
[Table("challenges")]
internal class Challenge
{
    public Challenge() { }

    public Challenge(ChoosingRightOptionChallengeCompletedByUser<string, string> dataToCopy)
    {
        Id = dataToCopy.Id;
        UserGuid = dataToCopy.UserGuid;
        CompletedAt = dataToCopy.CompletedAt;
        ChallengeUnits = dataToCopy.Units
                .Select((unit, unitIndex) => new ChallengeUnit(unit, unitIndex)).ToArray();
    }

    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_guid")]
    public Guid UserGuid { get; set; }

    [Column("completed_at")]
    public DateTime CompletedAt { get; set; }

    public ICollection<ChallengeUnit> ChallengeUnits { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
