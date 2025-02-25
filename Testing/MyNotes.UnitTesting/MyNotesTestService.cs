using System.Linq.Expressions;
using AutoMapper;
using MyNotes.Application.Dtos;
using MyNotes.Application.Interfaces;
using MyNotes.Application.Services;
using MyNotes.Domain.Entities;
using MyNotes.Domain.Interfaces;

namespace MyNotes.UnitTesting;

[TestFixture]
public class MyNotesTestService
{
    private Mock<INoteRepository> _mockRepository;
    private Mock<IMapper> _mockMapper;
    private INoteService _noteService;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<INoteRepository>();
        _mockMapper = new Mock<IMapper>();
        _noteService = new NoteService(_mockRepository.Object, _mockMapper.Object);
    }

    /// <summary>
    /// Test case for getting all notes
    /// It should return an empty list if no notes to be found
    /// </summary>
    [Test]
    [TestCase(null, true, 0, 1)]
    public async Task GetAllNotesTest(
        Expression<Func<Note,bool>>? filter, 
        bool tracked, 
        int pageSize, 
        int pageNumber)
    {
        _mockRepository.Setup(repo => repo.GetAllAsync(filter, tracked, pageSize, pageNumber))
            .ReturnsAsync(new List<Note>());
        
        var result = await _noteService.GetAllNotesAsync();
        
        Assert.That(result, Is.Not.Null, "No notes returned");
        Assert.That(result, Is.Empty, "No notes returned");
    }

    /// <summary>
    /// Test case for creating a note
    /// The repository should be called for the method Create
    /// The Id and CreatedAt should be created by default
    /// </summary>
    [Test]
    public async Task CreateNoteTest()
    {
        var note = new Note { Title = "Test", Content = "Test Content" };
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Note>()))
            .Returns(Task.CompletedTask);
        
        await _noteService.CreateNoteAsync(_mockMapper.Object.Map<NoteDto>(note));
        
        _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<Note>()), Times.Once);
        
        Assert.That(note.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(note.CreatedAt, Is.Not.EqualTo(DateTime.MinValue));
    }
}