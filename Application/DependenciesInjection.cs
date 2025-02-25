using MyNotes.Infrastructure;

namespace MyNotes.Application;

public static class DependenciesInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INoteService, NoteService>();
        services.AddAutoMapper(typeof(NoteProfile));

        services.AddInfrastructure(configuration);
        
        return services;
    }
}