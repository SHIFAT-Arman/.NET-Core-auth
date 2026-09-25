using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApiApp.Dto;
using WebApiApp.Entities;
using WebApiApp.GenericResponse;
using WebApiApp.IService;
using WebApiApp.Services;

namespace WebApiApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;


        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserDto dto)
        {
            try
            {
                var result = await _authService.LoginUser(dto);
                if (result.Item1 == 0)
                {
                    return Unauthorized(ResponseResult<string>.Failure(null, "Unauthorized"));
                }
                
                return Ok(ResponseResult<string>.Success(result.Item2, "Success"));
            }
            catch (Exception ex)
            {
                throw;  
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto dto)
        {
            try
            {
                var result = await _authService.RegisterUser(dto);
                if (result.Item1 == 0)
                {
                    return BadRequest(ResponseResult<string>.Failure(result.Item2, ""));
                }

                return Ok(ResponseResult<string>.Success(null, result.Item2));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authenticated");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("Admin authenticated");
        }
        
    }
}
