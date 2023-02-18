namespace FiveWords._v1.DataObjects;

public record User(string Id, Guid Guid) : BaseEntity<string>(Id)
{
    public User() : this(default, default) { }

    private static User? _default;
    public static User Default => _default ??= new User();

    public string Login { get { return Id; } }
}