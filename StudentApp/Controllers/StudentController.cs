using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using StudentApp.DTO;
using StudentApp.Models;
using StudentApp.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace StudentApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly StudentService _services;
    private readonly AuthService _auth;

    public StudentController(StudentService studentService, AuthService authService)
    {
        _services = studentService;
        _auth = authService;
    }

    // Register 
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterStudentDTO dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var existing = await _services.GetByEmailAsync(email);
        if (existing != null)
            return Conflict(new { message = "Email already exists." });

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var student = new Student
        {
            Name = dto.Name.Trim(),
            Email = email,
            PasswordHash = hashedPassword,
            Hobbies = dto.Hobbies ?? new List<string>()
        };

        await _services.CreateAsync(student);
        return StatusCode(201, new { message = "Student register successfully." });
    }

    // Login
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginStudentDTO dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var student = await _services.GetByEmailAsync(email);

        if (student == null || !BCrypt.Net.BCrypt.Verify(dto.Password, student.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var token = _auth.GenerateToken(student.Id!, student.Email);
        return Ok(new
        {
            message = "Login Successfully",
            token,
            student = new StudentDTO
            {
                Id = student.Id!,
                Name = student.Name,
                Email = student.Email,
                Hobbies = student.Hobbies
            }
        });
    }

    // Get all student
    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var students = await _services.GetAllAsync();
        var data = students.Select(s => new StudentDTO
        {
            Id = s.Id!,
            Name = s.Name,
            Email = s.Email,
            Hobbies = s.Hobbies
        });
        return Ok(data);
    }

    // Get student by id
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(id, out _))
            return BadRequest(new { message = "Invalid student ID" });
        
        var student = await _services.GetByIdAsync(id);
        if (student == null)
            return NotFound(new { message = "Student not found" });

        return Ok(new StudentDTO
        {
            Id = student.Id!,
            Name = student.Name,
            Email = student.Email,
            Hobbies = student.Hobbies
        });
    }

    // Delete student
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(string id)
    {
        if (!MongoDB.Bson.ObjectId.TryParse(id, out _))
            return BadRequest(new { message = "Invalid student ID" });
        
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        // Users can delete only their own account
        if (currentUserId != id) return Forbid();

        var success = await _services.DeleteByIdAsync(id);
        if (!success)
            return NotFound(new { message = "Student not found" });

        return Ok(new { message = "Student deleted successfully" });
    }
}