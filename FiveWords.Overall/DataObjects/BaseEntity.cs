using System.ComponentModel.DataAnnotations;

namespace FiveWords.DataObjects;

public abstract record BaseEntity<TId>([property: Required] TId Id)
//where TId : struct
where TId : IEquatable<TId>
{
    public BaseEntity() : this((TId)default) { }
}