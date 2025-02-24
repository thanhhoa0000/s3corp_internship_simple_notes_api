namespace MyNotes.Application.Interfaces;

public interface INoteService
{
    Task<IEnumerable<NoteDto>> GetAllNotesAsync(Expression<Func<Note, bool>>? filter = null, bool tracked = true, int pageSize = 0, int pageNumber = 1);
    Task<NoteDto?> GetNoteByIdAsync(Guid id, bool tracked = true);
    Task CreateNoteAsync(NoteDto noteDto);
    Task UpdateNoteAsync(NoteDto noteDto);
    Task DeleteNoteAsync(Guid id);
}