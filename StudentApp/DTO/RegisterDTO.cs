using System.ComponentModel.DataAnnotations;

namespace StudentApp.DTO;

public class RegisterStudentDTO
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(3), MaxLength(100)]
    public string Password { get; set; } = string.Empty;
    public List<string> Hobbies { get; set; } = new();
}
