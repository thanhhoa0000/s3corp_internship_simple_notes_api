namespace MyNotes.Application.Mapper;

public class NoteProfile : Profile
{
    public NoteProfile()
    {
        CreateMap<Note, NoteDto>().ReverseMap(); 
    }
}