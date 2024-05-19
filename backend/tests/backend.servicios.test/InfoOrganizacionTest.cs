using backend.data.DataContext;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;

using backend.servicios.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.servicios.test
{
    [TestFixture]
    public class InfoOrganizacionTest
    {
        private Mock<ILogger<OrganizacionService>> _loggerMock;
        private ApplicationDbContext _context;
        private InfoOrganizacionService _infoOrganizacionService;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<OrganizacionService>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;

            _context = new ApplicationDbContext(options);

            _context.SaveChanges();

            _infoOrganizacionService = new InfoOrganizacionService(_context, _loggerMock.Object);


        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task SaveDataInfoOrganizacion_Returns_CorrectInfoOrganizacion()
        {
            // Arrange
            var infoOrganizacionDto = new InfoOrganizacionDto
            {
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
                Img = "Img",
                OrganizacionId = 1
            };

            await _infoOrganizacionService.SaveDataInfoOrganizacion(infoOrganizacionDto);

            var organizacionCreate = await _context.InfoOrganizacions.FirstOrDefaultAsync(u => u.Organizacion == "Organizacion");

            // Assert
            Assert.That(organizacionCreate, Is.Not.Null);
            Assert.That(organizacionCreate.Organizacion, Is.EqualTo("Organizacion"));
        }

        [Test]
        public void SaveDataInfoOrganizacionc_NullInfoOrganizacionDto_ArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _infoOrganizacionService.SaveDataInfoOrganizacion(null));
            Assert.Multiple(() =>
            {
                Assert.That(ex.ParamName, Is.EqualTo("infoOrganizacionDto"));
                Assert.That(ex.Message, Contains.Substring("La informacion de la organizacion proporcionada no puede ser nula."));
            });
        }

        [Test]
        public async Task UpdateInfoOrganizacionAsync_Returns_CorrectInfoOrganizacion()
        {
            var testOrganizacion = "Amigos";

            var infoOrganizacionDto = new InfoOrganizacionDto
            {
                Organizacion = "organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
                Img = "Img",
                OrganizacionId = 1
            };

            await _context.SaveChangesAsync();

            infoOrganizacionDto.Organizacion = testOrganizacion;

            await _infoOrganizacionService.UpdateInfoOrganizacionAsync(infoOrganizacionDto);

            var organizacionUpdate = await _context.InfoOrganizacions.FirstOrDefaultAsync(u => u.Organizacion == testOrganizacion);

            Assert.That(organizacionUpdate, Is.Not.Null);
            Assert.That(organizacionUpdate.Organizacion, Is.EqualTo(testOrganizacion));

        }

        [Test]
        public async Task UpdateInfoOrganizacionAsync_InfoDoesNotExist_InvalidOperationException()
        {
            var infoOrganizacionDto = new InfoOrganizacionDto
            {
                Organizacion = "organizacion"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _infoOrganizacionService.UpdateInfoOrganizacionAsync(infoOrganizacionDto));
            Assert.That(ex.Message, Is.EqualTo("La informacion de la organizacion no existe."));
        }
    }
}
