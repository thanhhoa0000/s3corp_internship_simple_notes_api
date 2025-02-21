namespace MyNotes.Application.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;

    public NoteService(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public async Task<IEnumerable<Note>> GetAllNotesAsync()
    {
        return await _noteRepository.GetAllAsync();
    }

    public async Task<Note?> GetNoteByIdAsync(Guid id)
    {
        return await _noteRepository.GetAsync(n => n.Id == id);
    }

    public async Task CreateNoteAsync(Note note)
    {
        await _noteRepository.CreateAsync(note);
    }

    public async Task UpdateNoteAsync(Note note)
    {
        await _noteRepository.UpdateAsync(note);
    }

    public async Task DeleteNoteAsync(Guid id)
    {
        var existingNote = await _noteRepository.GetAsync(n => n.Id == id);
        if (existingNote is null)
            throw new KeyNotFoundException($"Note with ID {id} not found.");
        
        await _noteRepository.RemoveAsync(existingNote);
    }
}