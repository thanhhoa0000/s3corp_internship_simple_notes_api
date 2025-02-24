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
        [FromServices] INoteService service,
        [FromServices] ILogger<NoteEndpoints> logger,
        HttpContext httpContext,
        [FromQuery] int pageSize = 0,
        [FromQuery] int pageNumber = 1)
    {
        try
        {
            logger.LogInformation("Getting notes...");

            var notes = await service
                .GetAllNotesAsync(tracked: false, pageSize: pageSize, pageNumber: pageNumber);

            var pagination = new Pagination()
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            httpContext.Response.Headers["X-Pagination"] = JsonSerializer.Serialize(pagination);

            return TypedResults.Ok(notes);
        }
        catch (Exception ex)
        {
            logger.LogError("\n---\nError(s) occured: \n---\n{error}", ex);

            return TypedResults.BadRequest("Error(s) occured when getting the notes!");
        }
    }

    public async Task<Results<Ok<NoteDto>, NotFound<string>, BadRequest<string>>> GetNote(
        [FromServices] INoteService service,
        [FromServices] ILogger<NoteEndpoints> logger,
        Guid noteId)
    {
        try
        {
            logger.LogInformation("Getting note...");

            var note = await service.GetNoteByIdAsync(noteId, tracked: false);

            return TypedResults.Ok(note);
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
        [FromServices] INoteService service,
        [FromServices] ILogger<NoteEndpoints> logger,
        IMapper mapper)
    {
        try
        {
            logger.LogInformation("Creating note...");

            noteDto.Id = Guid.NewGuid();
            
            await service.CreateNoteAsync(noteDto);

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
        [FromServices] INoteService service,
        [FromServices] ILogger<NoteEndpoints> logger)
    {
        try
        {
            logger.LogInformation("Updating note...");
            
            noteDto.ModifiedAt = DateTime.UtcNow;
            
            await service.UpdateNoteAsync(noteDto);
            
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
            [FromServices] INoteService service,
            [FromServices] ILogger<NoteEndpoints> logger,
            Guid noteId)
    {
        try
        {
            logger.LogInformation("Deleting note...");
            
            await service.DeleteNoteAsync(noteId);
            
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