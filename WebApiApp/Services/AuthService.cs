using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApiApp.Data;
using WebApiApp.Dto;
using WebApiApp.Entities;
using WebApiApp.IService;

namespace WebApiApp.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IValidator<CreateUserDto> _createUserDtoValidator;
    
    public AuthService(AppDbContext context, IConfiguration configuration, IValidator<CreateUserDto> createUserDtoValidator)
    {
        _context = context;
        _configuration = configuration;
        _createUserDtoValidator = createUserDtoValidator;
    }

    public async Task<Tuple<int, string>> LoginUser(UserDto dto)
    {
        try
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(user => user.Email == dto.Email);

            if (existingUser == null || string.IsNullOrEmpty(existingUser.Password))
            {
                return new Tuple<int, string>(0, "Invalid credentials");
            }

            var hasher = new PasswordHasher<User>();
            
            var result = hasher.VerifyHashedPassword(existingUser, existingUser.Password, dto.Password);
            
            if (result == PasswordVerificationResult.Failed)
            {
                return new Tuple<int, string>(0, "Invalid email or password");
            }

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                existingUser.Password = hasher.HashPassword(existingUser, dto.Password);
                await _context.SaveChangesAsync();
            }
            
            var token = CreateToken(existingUser);

            return new Tuple<int, string>(1, token);
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Tuple<int, string>> RegisterUser(CreateUserDto dto)
    {
        var validationResult = _createUserDtoValidator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return new Tuple<int, string>(0, validationResult.Errors.First().ErrorMessage);
        }
        try
        {
            var existingUser = await _context.Users.AnyAsync(user => user.Email == dto.Email);

            if (existingUser)
            {
                return new Tuple<int, string>(0, "User already exists, Register with different email");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
            };

            user.Password = new PasswordHasher<User>().HashPassword(user, dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new Tuple<int, string>(1, "User registered successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
            
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));
            
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
        var tokenDescriptor = new JwtSecurityToken(
            issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
            audience: _configuration.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
    
}