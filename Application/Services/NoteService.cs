namespace MyNotes.Application.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;

    public NoteService(INoteRepository noteRepository, IMapper mapper)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NoteDto>> GetAllNotesAsync(Expression<Func<Note, bool>>? filter = null, bool tracked = true, int pageSize = 0, int pageNumber = 1)
    {
        var note = await _noteRepository.GetAllAsync(tracked: tracked, filter: filter, pageSize: pageSize, pageNumber: pageNumber);
        
        return _mapper.Map<IEnumerable<NoteDto>>(note);
    }

    public async Task<NoteDto?> GetNoteByIdAsync(Guid id, bool tracked = true)
    {
        var note = await _noteRepository.GetAsync(n => n.Id == id, tracked: tracked);
        
        return _mapper.Map<NoteDto>(note);
    }

    public async Task CreateNoteAsync(NoteDto noteDto)
    {
        var note = _mapper.Map<Note>(noteDto);
        
        await _noteRepository.CreateAsync(note);
    }

    public async Task UpdateNoteAsync(NoteDto noteDto)
    {
        var note = _mapper.Map<Note>(noteDto);
        
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