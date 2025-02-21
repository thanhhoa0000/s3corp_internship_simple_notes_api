namespace MyNotes.Application.Interfaces;

public interface INoteService
{
    Task<IEnumerable<Note>> GetAllNotesAsync();
    Task<Note?> GetNoteByIdAsync(Guid id);
    Task CreateNoteAsync(Note note);
    Task UpdateNoteAsync(Note note);
    Task DeleteNoteAsync(Guid id);
}