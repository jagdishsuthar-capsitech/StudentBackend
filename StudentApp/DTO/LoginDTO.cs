using System.ComponentModel.DataAnnotations;

namespace StudentApp.DTO;

public class LoginStudentDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}