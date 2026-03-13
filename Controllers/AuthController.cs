using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EvoStockAPI.Data;
using EvoStockAPI.Dtos;
using EvoStockAPI.Models;
using System.Security.Cryptography;
using System.Text;

namespace EvoStockAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { message = "Toate câmpurile sunt obligatorii." });
        }

        var email = dto.Email.Trim().ToLower();

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Există deja un cont cu acest email." });
        }

        var user = new User
        {
            Name = dto.Name.Trim(),
            Email = email,
            PasswordHash = HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Cont creat cu succes.",
            user = new
            {
                user.Id,
                user.Name,
                user.Email
            }
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { message = "Emailul și parola sunt obligatorii." });
        }

        var email = dto.Email.Trim().ToLower();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return Unauthorized(new { message = "Email sau parolă greșită." });
        }

        var hashedPassword = HashPassword(dto.Password);
        if (user.PasswordHash != hashedPassword)
        {
            return Unauthorized(new { message = "Email sau parolă greșită." });
        }

        return Ok(new
        {
            message = "Autentificare reușită.",
            user = new
            {
                user.Id,
                user.Name,
                user.Email
            }
        });
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}