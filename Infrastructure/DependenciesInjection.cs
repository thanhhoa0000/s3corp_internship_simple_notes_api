namespace MyNotes.Infrastructure;

public static class DependenciesInjection
{
    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NoteContext>(options
            => options.UseSqlServer(
                configuration.GetConnectionString("NotesDB"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(15),
                    errorNumbersToAdd: null)
            ));
        
        services.AddScoped<INoteRepository, NoteRepository>();
        
        return services;
    }
}