namespace WebApiApp.Dto;

public record CreateUserDto(
    string Name,
    string Email,
    string Password
    );