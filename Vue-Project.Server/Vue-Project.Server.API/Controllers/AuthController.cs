using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Vue_Project.Server.API.Context;
using Vue_Project.Server.Models.DTOs;
using Vue_Project.Server.Models.Tables;

namespace Test_Project.Server.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(EfContext context, IConfiguration configuration) : ControllerBase
{
    private readonly EfContext _context = context;
    private readonly string _secretKey = configuration["TokenSecretKey"] 
        ?? throw new ArgumentNullException("TokenSecretKey", "Token secret key cannot be null.");

    # region Endpoints
    [HttpGet("user")]
    public async Task<IActionResult> GetUser()
    {
        var accessToken = Request.Cookies["Access"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Access token not provided.");
        }

        var principal = ValidateToken(accessToken, validateLifetime: true);
        if (principal is null)
        {
            return Unauthorized("Invalid access token.");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid token.");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user is null)
        {
            return Unauthorized("User not found.");
        }

        return Ok(new UserDTO
        {
            Username = user.Username ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            ProfilePicture = user.ProfilePicture ?? string.Empty
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return BadRequest("Invalid username or password.");
        }

        SetAuthCookies(user);
        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO request)
    {
        var exists = await _context.Users
            .AnyAsync(u => u.Username == request.Username || u.Email == request.Email);

        if (exists)
        {
            return BadRequest("User already exists with this username or email.");
        }

        var newUser = new User
        {
            Username = request.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("refresh")]
    public IActionResult Refresh()
    {
        var refreshToken = Request.Cookies["Refresh"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("Refresh token not provided.");
        }

        var principal = ValidateToken(refreshToken, validateLifetime: false);
        if (principal is null)
        {
            return Unauthorized("Invalid refresh token.");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid token.");
        }

        var user = _context.Users.Find(userId);
        if (user is null)
        {
            return Unauthorized("User not found.");
        }

        SetAuthCookies(user);
        return Ok();
    }
    # endregion

    # region Helper Methods
    private void SetAuthCookies(User user)
    {
        var (accessToken, accessExp) = CreateToken(user, isRefresh: false);
        var (refreshToken, refreshExp) = CreateToken(user, isRefresh: true);

        AppendTokenCookie("Access", accessToken, accessExp);
        AppendTokenCookie("Refresh", refreshToken, refreshExp);
    }

    private (string token, DateTime expiration) CreateToken(User user, bool isRefresh)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var expiration = isRefresh ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddMinutes(15);

        var token = new JwtSecurityTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            SigningCredentials = creds
        });

        return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
    }

    private ClaimsPrincipal? ValidateToken(string token, bool validateLifetime)
    {
        try
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = validateLifetime
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }

    private void AppendTokenCookie(string name, string token, DateTime expiration)
    {
        Response.Cookies.Append(name, token, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = expiration
        });
    }
    # endregion
}