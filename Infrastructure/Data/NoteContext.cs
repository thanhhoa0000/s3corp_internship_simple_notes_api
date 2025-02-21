namespace MyNotes.Infrastructure.Data;

public class NoteContext(DbContextOptions<NoteContext> options) : DbContext(options)
{
    public DbSet<Note> Notes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Note>()
            .HasIndex(n => n.Title)
            .IsUnique();
    }
}