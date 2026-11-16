using MongoDB.Driver;
using StudentApp.Models;

namespace StudentApp.Services;

public class StudentService
{
    private readonly IMongoCollection<Student> _students;

    public StudentService(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _students = db.GetCollection<Student>("Students");
    }

    // Get All
    public async Task<List<Student>> GetAllAsync() =>
        await _students.Find(_ => true).ToListAsync();

    // Create Student
    public async Task CreateAsync(Student student) =>
        await _students.InsertOneAsync(student);

    // Update Student
    public async Task<bool> UpdateAsync(string id, Student student)
    {
        student.Id = id;
        var result = await _students.ReplaceOneAsync(s => s.Id == id, student);
        return result.MatchedCount > 0;
    }

    // Get by email
    public async Task<Student?> GetByEmailAsync(string email) =>
        await _students.Find(s => s.Email == email).FirstOrDefaultAsync();
    
    // Check email..
    public async Task<bool> EmailExistAsync(string email) =>
        await _students.Find(s=> s.Email == email).AnyAsync();

    // Get by id
    public async Task<Student?> GetByIdAsync(string id) => 
        await _students.Find(s => s.Id == id).FirstOrDefaultAsync();

    // Delete by id
    public async Task<bool> DeleteByIdAsync(string id)
    {
        var result = await _students.DeleteOneAsync(s => s.Id == id);
        return result.DeletedCount > 0;
    }
}