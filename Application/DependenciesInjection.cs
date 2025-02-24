using MyNotes.Infrastructure;

namespace MyNotes.Application;

public static class DependenciesInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseContext(configuration);
        
        services.AddScoped<INoteService, NoteService>();
        
        services.AddAutoMapper(typeof(NoteProfile));
        
        return services;
    }
}