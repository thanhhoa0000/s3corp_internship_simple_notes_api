using MyNotes.Domain.Entities;

namespace MyNotes.Presentation.Endpoints;

public class NoteEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/api/v{version:apiVersion}/notes")
            .WithApiVersionSet(apiVersionSet);

        group.MapGet("/", GetNotes);
        group.MapGet("/{noteId:guid}", GetNote);
        group.MapPost("/", CreateNote);
        group.MapPut("/", UpdateNote);
        group.MapDelete("/{noteId:guid}", DeleteNote);
    }

    public async Task<Results<Ok<IEnumerable<NoteDto>>, BadRequest<string>>> GetNotes(
        [FromServices] INoteRepository repository,
        [FromServices] ILogger<NoteEndpoints> logger,
        HttpContext httpContext,
        IMapper mapper,
        [FromQuery] int pageSize = 0,
        [FromQuery] int pageNumber = 1)
    {
        try
        {
            logger.LogInformation("Getting notes...");

            var notes = await repository
                .GetAllAsync(tracked: false, pageSize: pageSize, pageNumber: pageNumber);

            var pagination = new Pagination()
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            httpContext.Response.Headers["X-Pagination"] = JsonSerializer.Serialize(pagination);

            return TypedResults.Ok(mapper.Map<IEnumerable<NoteDto>>(notes));
        }
        catch (Exception ex)
        {
            logger.LogError("\n---\nError(s) occured: \n---\n{error}", ex);

            return TypedResults.BadRequest("Error(s) occured when getting the notes!");
        }
    }

    public async Task<Results<Ok<NoteDto>, NotFound<string>, BadRequest<string>>> GetNote(
        [FromServices] INoteRepository repository,
        [FromServices] ILogger<NoteEndpoints> logger,
        Guid noteId,
        IMapper mapper)
    {
        try
        {
            logger.LogInformation("Getting note...");

            var note = await repository.GetAsync(n => n.Id == noteId, tracked: false);

            return TypedResults.Ok(mapper.Map<NoteDto>(note));
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError($"\n---\nError(s) occured ({noteId} not found): \n---\n{ex}");

            return TypedResults.NotFound($"Note {noteId} not found!");
        }
        catch (Exception ex)
        {
            logger.LogError("\n---\nError(s) occured: \n---\n{error}", ex);
            
            return TypedResults.BadRequest("Error(s) occured when getting the note!");
        }
    }

    public async Task<Results<Created, BadRequest<string>>> CreateNote(
        [FromBody] NoteDto noteDto,
        [FromServices] INoteRepository repository,
        [FromServices] ILogger<NoteEndpoints> logger,
        IMapper mapper)
    {
        try
        {
            logger.LogInformation("Creating note...");

            noteDto.Id = Guid.NewGuid();

            var note = mapper.Map<Note>(noteDto);
            await repository.CreateAsync(note);

            return TypedResults.Created($"/api/v1/notes/{noteDto.Id}");
        }
        catch (Exception ex)
        {
            logger.LogError("\n---\nError(s) occured: \n---\n{error}", ex);
            
            return TypedResults.BadRequest("Error(s) occured when creating the note!");
        }
    }

    public async Task<Results<NoContent, NotFound<string>, BadRequest<string>>> UpdateNote(
        [FromBody] NoteDto noteDto,
        [FromServices] INoteRepository repository,
        [FromServices] ILogger<NoteEndpoints> logger,
        IMapper mapper)
    {
        try
        {
            logger.LogInformation("Updating note...");
            
            noteDto.ModifiedAt = DateTime.UtcNow;
            var note = mapper.Map<Note>(noteDto);
            
            await repository.UpdateAsync(note);
            
            return TypedResults.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError($"\n---\nError(s) occured ({noteDto.Id} not found): \n---\n{ex}");

            return TypedResults.NotFound($"Note {noteDto.Id} not found!");
        }
        catch (Exception ex)
        {
            logger.LogError("\n---\nError(s) occured: \n---\n{error}", ex);
            
            return TypedResults.BadRequest("Error(s) occured when updating the note!");
        }
    }

    public async Task<Results<NoContent, NotFound<string>, BadRequest<string>>>
        DeleteNote(
            [FromServices] INoteRepository repository,
            [FromServices] ILogger<NoteEndpoints> logger,
            Guid noteId)
    {
        try
        {
            logger.LogInformation("Deleting note...");
            
            var note = await repository.GetAsync(n => n.Id == noteId, tracked: false);
            
            await repository.RemoveAsync(note!);
            
            return TypedResults.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError($"\n---\nError(s) occured ({noteId} not found): \n---\n{ex}");

            return TypedResults.NotFound($"Note {noteId} not found!");
        }
        catch (Exception ex)
        {
            logger.LogError("\n---\nError(s) occured: \n---\n{error}", ex);
            
            return TypedResults.BadRequest("Error(s) occured when deleting the note!");
        }
    }
}