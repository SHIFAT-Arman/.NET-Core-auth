using System.ComponentModel.DataAnnotations;

namespace WebApiApp.Entities;

public class Employee
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
    public DateTime DOB { get; set; }
    public string? Position { get; set; }
    public string? Department { get; set; }
    public string? Email { get; set; }
}