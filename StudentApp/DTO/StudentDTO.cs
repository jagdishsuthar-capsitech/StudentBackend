namespace StudentApp.DTO;

public class StudentDTO
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Hobbies { get; set; } = new();
}
