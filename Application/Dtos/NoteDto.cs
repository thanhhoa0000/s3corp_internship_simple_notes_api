namespace MyNotes.Application.Dtos;

public class NoteDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(50)]
    public string? Title { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ModifiedAt { get; set; }
}