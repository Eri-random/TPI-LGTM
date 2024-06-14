using backend.data.DataContext;
using backend.data.Models;
using backend.repositories.implementations;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
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

            _context = new ApplicationDbContext(options);
            _repository = new Repository<Donacion>(_context);
            _donationService = new DonationService(_repository, _loggerMock.Object);
        }

        [Test]
        public async Task SaveDonationAsync_WhenDonationDtoIsNull_ThrowsArgumentNullException()
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
            var organizacion = new Organizacion { Id = 1, Nombre = "Org 1", Cuit = "cuit 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Provincia = "Provincia 1" , Telefono = "Telefono 1" };
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
            Assert.AreEqual(1, donations.Count());
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

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
