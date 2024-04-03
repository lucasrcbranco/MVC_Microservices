namespace Mango.Services.EmailAPI.Models;

public class EmailLogger
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
