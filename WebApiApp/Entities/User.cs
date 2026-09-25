using System.ComponentModel.DataAnnotations;

namespace WebApiApp.Entities;

public enum Role
{
    User,
    Employee,
    Admin
}

public class User
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    
    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }
    
    public  Role Role { get; set; } = Role.User;
}