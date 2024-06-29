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
    public class InfoOrganizationServiceTest
    {
        private Mock<ILogger<OrganizationService>> _loggerMock;
        private ApplicationDbContext _context;
        private IRepository<InfoOrganizacion> _repository;
        private InfoOrganizationService _infoOrganizationService;
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<OrganizationService>>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new OrganizationProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.SaveChanges();
            _repository = new Repository<InfoOrganizacion>(_context);
            _infoOrganizationService = new InfoOrganizationService(_repository, _loggerMock.Object, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task SaveInfoOrganizationDataAsync_ValidInfoOrganizationDto_SavesSuccessfully()
        {
            // Arrange
            var infoOrganizationDto = new InfoOrganizationDto
            {
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
                Img = "Img",
                OrganizacionId = 1
            };

            await _infoOrganizationService.SaveInfoOrganizationDataAsync(infoOrganizationDto);

            var organizacionCreated = await _context.InfoOrganizacions.FirstOrDefaultAsync(u => u.Organizacion == "Organizacion");

            // Assert
            Assert.That(organizacionCreated, Is.Not.Null);
            Assert.That(organizacionCreated.Organizacion, Is.EqualTo("Organizacion"));
        }

        [Test]
        public void SaveInfoOrganizationDataAsync_NullInfoOrganizationDto_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _infoOrganizationService.SaveInfoOrganizationDataAsync(null));
            Assert.Multiple(() =>
            {
                Assert.That(ex.ParamName, Is.EqualTo("infoOrganizationDto"));
                Assert.That(ex.Message, Contains.Substring("La informacion de la organizacion proporcionada no puede ser nula."));
            });
        }

        [Test]
        public async Task UpdateInfoOrganizationAsync_ValidInfoOrganizationDto_UpdatesSuccessfully()
        {
            var initialInfoOrganization = new InfoOrganizacion
            {
                Organizacion = "Initial Organization",
                DescripcionBreve = "Initial Description",
                DescripcionCompleta = "Initial Complete Description",
                Img = "Initial Image",
                Id = 4
            };
            _context.InfoOrganizacions.Add(initialInfoOrganization);
            await _context.SaveChangesAsync();

            var infoOrganizationDto = new InfoOrganizationDto
            {
                Organizacion = "Updated Organization",
                DescripcionBreve = "Updated Description",
                DescripcionCompleta = "Updated Complete Description",
                Img = "Updated Image",
                OrganizacionId = 4
            };

            await _infoOrganizationService.UpdateInfoOrganizationAsync(infoOrganizationDto);

            var organizacionUpdated = await _context.InfoOrganizacions.FirstOrDefaultAsync(u => u.Organizacion == "Updated Organization");

            Assert.That(organizacionUpdated, Is.Not.Null);
            Assert.That(organizacionUpdated.Organizacion, Is.EqualTo("Updated Organization"));
        }

        [Test]
        public async Task UpdateInfoOrganizationAsync_InfoDoesNotExist_ThrowsInvalidOperationException()
        {
            var infoOrganizationDto = new InfoOrganizationDto
            {
                Organizacion = "Non-existent Organization",
                DescripcionBreve = "Description",
                DescripcionCompleta = "Complete Description",
                Img = "Image",
                OrganizacionId = 999 // Non-existent ID
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _infoOrganizationService.UpdateInfoOrganizationAsync(infoOrganizationDto));
            Assert.That(ex.Message, Is.EqualTo("La informacion de la organizacion no existe."));
        }

        [Test]
        public void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new InfoOrganizationService(null, _loggerMock.Object, _mapper));
            Assert.That(ex.ParamName, Is.EqualTo("repository"));
        }

        [Test]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new InfoOrganizationService(_repository, null, _mapper));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public void Constructor_NullMapper_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new InfoOrganizationService(_repository, _loggerMock.Object, null));
            Assert.That(ex.ParamName, Is.EqualTo("mapper"));
        }
    }
}
