namespace MyNotes.Infrastructure.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly NoteContext _context;
    private readonly DbSet<Note> _dbSet;
    private bool _disposed = false;

    public NoteRepository(NoteContext context)
    {
        _context = context;
        _dbSet = _context.Set<Note>();
    }

    public async Task<IEnumerable<Note>> GetAllAsync(
        Expression<Func<Note, bool>>? filter = null, 
        bool tracked = true,
        int pageSize = 0, 
        int pageNumber = 1)
    {
        IQueryable<Note> query = _dbSet;
        
        if (!tracked)
            query = query.AsNoTracking();
        
        if (filter is not null)
            query = query.Where(filter);
        
        if (pageSize > 0)
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        
        IEnumerable<Note> notes = await query.ToListAsync();
        
        return notes;
    }

    public async Task<Note?> GetAsync(Expression<Func<Note, bool>>? filter = null, bool tracked = true)
    {
        IQueryable<Note> query = _dbSet;
        
        if (!tracked)
            query = query.AsNoTracking();
        
        if (filter is not null)
            query = query.Where(filter);
        
        return await query.FirstOrDefaultAsync();
    }
    
    public async Task CreateAsync(Note note)
    {
        await _dbSet.AddAsync(note);
        await SaveAsync();
    }
    
    public async Task UpdateAsync(Note note)
    {
        var existingNote = await _dbSet.FindAsync(note.Id);

        if (existingNote != null)
        {
            _context.Entry(existingNote).CurrentValues.SetValues(note);
        }
        else
        {
            _dbSet.Attach(note);
            _context.Entry(note).State = EntityState.Modified;
        }

        await SaveAsync();
    }
    
    public async Task RemoveAsync(Note note)
    {
        var existingNote = _dbSet.Local.FirstOrDefault(n => n.Id == note.Id);
        if (existingNote != null)
        {
            _dbSet.Remove(existingNote);
        }
        else
        {
            _dbSet.Attach(note);
            _dbSet.Remove(note);
        }
        
        await SaveAsync();
    }
    
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }
}
