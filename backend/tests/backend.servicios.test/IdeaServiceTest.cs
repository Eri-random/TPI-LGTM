using AutoMapper;
using backend.api.Mappers;
using backend.data.DataContext;
using backend.data.Models;
using backend.repositories.implementations;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.servicios.test
{
    [TestFixture]
    public class IdeaServiceTest
    {
        private Mock<ILogger<IdeaService>> _loggerMock;
        private ApplicationDbContext _context;
        private IRepository<Idea> _ideaRepository;
        private IRepository<Paso> _pasoRepository;
        private IMapper _mapper;
        private IdeaService _ideaService;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<IdeaService>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new IdeaProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _context = new ApplicationDbContext(options);
            _ideaRepository = new Repository<Idea>(_context);
            _pasoRepository = new Repository<Paso>(_context);
            _ideaService = new IdeaService(_ideaRepository, _pasoRepository, _loggerMock.Object, _mapper);
        }

        [Test]
        public async Task SaveIdeaAsync_WhenIdeaDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IdeaDto ideaDto = null;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _ideaService.SaveIdeaAsync(ideaDto));
            Assert.That(ex.ParamName, Is.EqualTo("ideaDto"));
        }

        [Test]
        public async Task SaveIdeaAsync_WhenValidIdeaDto_SavesSuccessfully()
        {
            // Arrange
            var ideaDto = new IdeaDto
            {
                Titulo = "Idea 1",
                UsuarioId = 1,
                Pasos = new List<StepDto>
            {
                new StepDto { Descripcion = "Paso 1", ImagenUrl = "ImagenUrl" },
                new StepDto { Descripcion = "Paso 2", ImagenUrl = "ImagenUrl" }
            }
            };

            // Act
            await _ideaService.SaveIdeaAsync(ideaDto);

            // Assert
            var ideas = await _context.Ideas.Include(i => i.Pasos).ToListAsync();
            Assert.AreEqual(1, ideas.Count);
            Assert.AreEqual("Idea 1", ideas[0].Titulo);
            Assert.AreEqual(2, ideas[0].Pasos.Count);
        }

        [Test]
        public async Task GetIdeasByUserIdAsync_WhenCalled_ReturnsIdeas()
        {
            // Arrange
            var idea1 = new Idea { Titulo = "Idea 1", UsuarioId = 1, Pasos = new List<Paso> { new Paso { Descripcion = "Paso 1", ImagenUrl = "ImagenUrl" } } };
            var idea2 = new Idea { Titulo = "Idea 2", UsuarioId = 2, Pasos = new List<Paso> { new Paso { Descripcion = "Paso 2", ImagenUrl = "ImagenUrl" } } };
            _context.Ideas.AddRange(idea1, idea2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _ideaService.GetIdeasByUserIdAsync(1);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Idea 1", result.First().Titulo);
        }

        [Test]
        public async Task GetIdeaByIdAsync_WhenCalled_ReturnsIdea()
        {
            // Arrange
            var idea = new Idea { Titulo = "Idea 1", UsuarioId = 1, Pasos = new List<Paso> { new Paso { Descripcion = "Paso 1", ImagenUrl = "ImagenUrl" } } };
            _context.Ideas.Add(idea);
            await _context.SaveChangesAsync();

            // Act
            var result = await _ideaService.GetIdeaByIdAsync(idea.Id);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Idea 1", result.Titulo);
        }

        [Test]
        public async Task GetIdeaByIdAsync_WhenIdeaNotFound_ReturnsNull()
        {
            // Act
            var result = await _ideaService.GetIdeaByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task DeleteIdeaByIdAsync_WhenIdeaExists_DeletesSuccessfully()
        {
            // Arrange
            var idea = new Idea { Titulo = "Idea 1", UsuarioId = 1, Pasos = new List<Paso> { new Paso { Descripcion = "Paso 1", ImagenUrl = "ImagenUrl" } } };
            _context.Ideas.Add(idea);
            await _context.SaveChangesAsync();

            // Act
            await _ideaService.DeleteIdeaByIdAsync(idea.Id);

            // Assert
            var deletedIdea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == idea.Id);
            var deletedSteps = await _context.Pasos.Where(p => p.IdeaId == idea.Id).ToListAsync();
            Assert.Null(deletedIdea);
            Assert.IsEmpty(deletedSteps);
        }

        [Test]
        public async Task DeleteIdeaByIdAsync_WhenIdeaNotFound_ThrowsInvalidOperationException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _ideaService.DeleteIdeaByIdAsync(1));
            Assert.That(ex.Message, Is.EqualTo("La idea no existe."));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
