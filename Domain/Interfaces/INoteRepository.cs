namespace MyNotes.Domain.Interfaces;

public interface INoteRepository : IDisposable
{
    Task<IEnumerable<Note>> GetAllAsync(Expression<Func<Note, bool>>? filter = null, bool tracked = true, int pageSize = 0, int pageNumber = 1);
    Task<Note?> GetAsync(Expression<Func<Note, bool>>? filter = null, bool tracked = true);
    Task CreateAsync(Note entity);
    Task RemoveAsync(Note entity);
    Task UpdateAsync(Note entity);
    Task SaveAsync();
}