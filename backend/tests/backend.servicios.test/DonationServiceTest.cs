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
    public class DonationServiceTest
    {
        private Mock<ILogger<DonationService>> _loggerMock;

        private ApplicationDbContext _context;
        private IRepository<Donacion> _repository;
        private DonationService _donationService;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<DonationService>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "DonationServiceTest")
                .Options;

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DonationProfile());
                mc.AddProfile(new OrganizationProfile());
                mc.AddProfile(new UserProfile());
            });
            var mapper = mappingConfig.CreateMapper();

            _context = new ApplicationDbContext(options);
            _repository = new Repository<Donacion>(_context);
            _donationService = new DonationService(_repository, _loggerMock.Object, mapper);
        }

        [Test]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<DonationService>>();
            var mapper = new Mock<IMapper>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new DonationService(null, loggerMock.Object, mapper.Object));
            Assert.That(ex.ParamName, Is.EqualTo("repository"));
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<Donacion>>();
            var mapper = new Mock<IMapper>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new DonationService(repositoryMock.Object, null, mapper.Object));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public void Constructor_WithNullMapper_ThrowsArgumentNullException()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<Donacion>>();
            var loggerMock = new Mock<ILogger<DonationService>>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new DonationService(repositoryMock.Object, loggerMock.Object, null));
            Assert.That(ex.ParamName, Is.EqualTo("mapper"));
        }

        [Test]
        public void SaveDonationAsync_WhenDonationDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            DonationDto donationDto = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _donationService.SaveDonationAsync(donationDto));
        }

        [Test]
        public async Task SaveDonationAsync_WhenValidDonationDto_SavesSuccessfully()
        {
            // Arrange
            var donationDto = new DonationDto
            {
                Producto = "Producto 1",
                Cantidad = 1,
                UsuarioId = 1,
                OrganizacionId = 1
            };

            // Act
            await _donationService.SaveDonationAsync(donationDto);

            // Assert
            var donation = await _context.Donacions.FirstOrDefaultAsync();
            Assert.NotNull(donation);
            Assert.AreEqual(donationDto.Producto, donation.Producto);
        }

        [Test]
        public async Task GetDonationsByOrganizationIdAsync_WhenCalled_ReturnsDonations()
        {
            // Arrange
            var usuario = new Usuario { Id = 1, Nombre = "Usuario 1", Telefono = "123456", Email = "user1@example.com" };
            var organizacion = new Organizacion { Id = 1, Nombre = "Org 1", Cuit = "cuit 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Provincia = "Provincia 1", Telefono = "Telefono 1" };
            _context.Usuarios.Add(usuario);
            _context.Organizacions.Add(organizacion);
            _context.Donacions.Add(new Donacion
            {
                Producto = "Producto 1",
                Cantidad = 1,
                UsuarioId = 1,
                OrganizacionId = 1,
                Usuario = usuario,
                Organizacion = organizacion
            });
            await _context.SaveChangesAsync();

            // Act
            var donations = await _donationService.GetDonationsByOrganizationIdAsync(1);

            // Assert
            Assert.NotNull(donations);
            Assert.AreEqual("Producto 1", donations.First().Producto);
        }

        [Test]
        public async Task GetDonationsByUserIdAsync_WhenCalled_ReturnsDonations()
        {
            // Arrange
            _context.Usuarios.RemoveRange(_context.Usuarios);
            _context.Organizacions.RemoveRange(_context.Organizacions);
            _context.Donacions.RemoveRange(_context.Donacions);
            await _context.SaveChangesAsync();

            var usuario = new Usuario { Id = 1, Nombre = "Usuario 1", Telefono = "123456", Email = "user1@example.com" };
            var organizacion = new Organizacion { Id = 1, Nombre = "Org 1", Cuit = "cuit 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Provincia = "Provincia 1", Telefono = "Telefono 1" };
            _context.Usuarios.Add(usuario);
            _context.Organizacions.Add(organizacion);
            _context.Donacions.Add(new Donacion
            {
                Producto = "Producto 1",
                Cantidad = 1,
                UsuarioId = 1,
                OrganizacionId = 1,
                Usuario = usuario,
                Organizacion = organizacion
            });
            await _context.SaveChangesAsync();

            // Act
            var donations = await _donationService.GetDonationsByUserIdAsync(1);

            // Assert
            Assert.NotNull(donations);
            Assert.AreEqual(1, donations.Count());
            Assert.AreEqual("Producto 1", donations.First().Producto);
        }

        [Test]
        public async Task UpdateDonationsStateAsync_WithValidDonations_UpdatesStateSuccessfully()
        {
            // Arrange
            var donationIds = new List<int> { 1 };
            _context.Donacions.Add(new Donacion
            {
                Id = 1,
                Producto = "Producto 1",
                Cantidad = 1,
                UsuarioId = 1,
                OrganizacionId = 1,
                Estado = "OldState"
            });
            await _context.SaveChangesAsync();

            // Act
            await _donationService.UpdateDonationsStateAsync(donationIds, "NewState");

            // Assert
            var donation = await _context.Donacions.FirstOrDefaultAsync(d => d.Id == 1);
            Assert.NotNull(donation);
            Assert.AreEqual("NewState", donation.Estado);
        }

        [Test]
        public void UpdateDonationsStateAsync_WithNonExistingDonations_ThrowsInvalidOperationException()
        {
            // Arrange
            var donationIds = new List<int> { 999 };

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _donationService.UpdateDonationsStateAsync(donationIds, "NewState"));
            Assert.That(ex.Message, Is.EqualTo("Las donaciones no existen."));
        }

        [Test]
        public async Task GetDonationIdAsync_WhenDonationExists_ReturnsDonationId()
        {
            // Arrange
            var donation = new Donacion
            {
                Producto = "Producto 1",
                Cantidad = 1,
                UsuarioId = 1,
                OrganizacionId = 1,
                Estado = "Estado"
            };
            _context.Donacions.Add(donation);
            await _context.SaveChangesAsync();

            var donationDto = new DonationDto
            {
                Producto = "Producto 1",
                Cantidad = 1,
                UsuarioId = 1,
                OrganizacionId = 1,
                Estado = "Estado"
            };

            // Act
            var result = await _donationService.GetDonationIdAsync(donationDto);

            // Assert
            Assert.AreEqual(donation.Id, result);
        }

        [Test]
        public async Task GetDonationIdAsync_WhenDonationDoesNotExist_ReturnsZero()
        {
            // Arrange
            var donationDto = new DonationDto
            {
                Producto = "Producto 1",
                Cantidad = 1,
                UsuarioId = 1,
                OrganizacionId = 1,
                Estado = "Estado"
            };

            // Act
            var result = await _donationService.GetDonationIdAsync(donationDto);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
