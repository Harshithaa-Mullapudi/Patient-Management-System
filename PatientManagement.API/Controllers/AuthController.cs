using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Infrastructure.Data;

namespace PatientManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthController(
        PatientDbContext context,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    // POST: api/Auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequestDto registerDto)
    {
        var email = registerDto.Email.Trim().ToLowerInvariant();

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == email);

        if (emailExists)
        {
            return Conflict("An account with this email already exists.");
        }

        var user = new User
        {
            FirstName = registerDto.FirstName.Trim(),
            LastName = registerDto.LastName.Trim(),
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            registerDto.Password);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return Ok(CreateAuthResponse(user));
    }

    // POST: api/Auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequestDto loginDto)
    {
        var email = loginDto.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            loginDto.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Invalid email or password.");
        }

        // Update the password hash if the hasher recommends a newer format.
        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                loginDto.Password);

            await _context.SaveChangesAsync();
        }

        return Ok(CreateAuthResponse(user));
    }

    private AuthResponseDto CreateAuthResponse(User user)
    {
        var expiryMinutes =
            _configuration.GetValue<int>("Jwt:ExpiryMinutes");

        return new AuthResponseDto
        {
            AccessToken = _tokenService.GenerateToken(user),
            TokenType = "Bearer",
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes),
            UserId = user.Id,
            Email = user.Email
        };
    }
}