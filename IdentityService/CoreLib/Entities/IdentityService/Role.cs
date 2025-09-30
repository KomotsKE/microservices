namespace CoreLib.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<User> Users { get; set; } = new();

}