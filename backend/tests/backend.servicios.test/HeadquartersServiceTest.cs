﻿using AutoMapper;
using backend.api.Mappers;
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
    public class HeadquartersServiceTest
    {
        private Mock<ILogger<HeadquartersService>> _loggerMock;
        private Mock<IMapsService> _mapsMock;
        private ApplicationDbContext _context;
        private IRepository<Sede> _repository;
        private HeadquartersService _headquartersService;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<HeadquartersService>>();
            _mapsMock = new Mock<IMapsService>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new OrganizationProfile());
            });
            var mapper = mappingConfig.CreateMapper();

            _context = new ApplicationDbContext(options);
            _repository = new Repository<Sede>(_context);
            _headquartersService = new HeadquartersService(_repository, _loggerMock.Object, _mapsMock.Object, mapper);
        }

        [Test]
        public async Task CreateHeadquartersAsync_WhenHeadquartersDtoIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            List<HeadquartersDto> headquartersDtos = null;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _headquartersService.CreateHeadquartersAsync(headquartersDtos));
            Assert.That(ex.ParamName, Is.EqualTo("headquartersDto"));
        }

        [Test]
        public async Task CreateHeadquartersAsync_WhenHeadquartersDtoIsValid_SavesSuccessfully()
        {
            // Arrange
            var headquartersDtos = new List<HeadquartersDto>
        {
            new HeadquartersDto
            {
                Nombre = "Sede 1",
                Direccion = "Direccion 1",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789",
                OrganizacionId = 1
            }
        };

            _mapsMock.Setup(m => m.GetCoordinates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                     .ReturnsAsync((1.0, 1.0));

            // Act
            await _headquartersService.CreateHeadquartersAsync(headquartersDtos);

            // Assert
            var headquarters = await _context.Sedes.ToListAsync();
            Assert.AreEqual(1, headquarters.Count);
            Assert.AreEqual("Sede 1", headquarters[0].Nombre);
        }

        [Test]
        public async Task GetAllHeadquartersAsync_WhenCalled_ReturnsAllHeadquarters()
        {
            // Arrange
            var sede1 = new Sede { Nombre = "Sede 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Telefono = "112232", Provincia = "Provincia 1", Latitud = 1.0, Longitud = 1.0, OrganizacionId = 1 };
            var sede2 = new Sede { Nombre = "Sede 2", Direccion = "Direccion 2", Localidad = "Localidad 2", Telefono = "123231", Provincia = "Provincia 2", Latitud = 2.0, Longitud = 2.0, OrganizacionId = 2 };
            _context.Sedes.AddRange(sede1, sede2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _headquartersService.GetAllHeadquartersAsync();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetHeadquartersByOrganizationIdAsync_WhenCalled_ReturnsHeadquarters()
        {
            // Arrange
            var sede = new Sede { Nombre = "Sede 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Telefono = "1221321", Provincia = "Provincia 1", Latitud = 1.0, Longitud = 1.0, OrganizacionId = 1 };
            _context.Sedes.Add(sede);
            await _context.SaveChangesAsync();

            // Act
            var result = await _headquartersService.GetHeadquartersByOrganizationIdAsync(1);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Sede 1", result.First().Nombre);
        }

        [Test]
        public async Task UpdateHeadquartersAsync_WhenHeadquartersDtoIsValid_UpdatesSuccessfully()
        {
            // Arrange
            var sede = new Sede { Id = 1, Nombre = "Sede 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Telefono = "12321312", Provincia = "Provincia 1", Latitud = 1.0, Longitud = 1.0, OrganizacionId = 1 };
            _context.Sedes.Add(sede);
            await _context.SaveChangesAsync();

            var headquartersDto = new HeadquartersDto { Id = 1, Nombre = "Sede Actualizada", Direccion = "Direccion Actualizada", Localidad = "Localidad Actualizada", Provincia = "Provincia Actualizada", Telefono = "987654321", OrganizacionId = 1 };

            _mapsMock.Setup(m => m.GetCoordinates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                     .ReturnsAsync((2.0, 2.0));

            // Act
            await _headquartersService.UpdateHeadquartersAsync(headquartersDto);

            // Assert
            var updatedSede = await _context.Sedes.FirstOrDefaultAsync(s => s.Id == 1);
            Assert.AreEqual("Sede Actualizada", updatedSede.Nombre);
            Assert.AreEqual(2.0, updatedSede.Latitud);
            Assert.AreEqual(2.0, updatedSede.Longitud);
        }

        [Test]
        public async Task DeleteHeadquartersAsync_WhenHeadquartersExists_DeletesSuccessfully()
        {
            // Arrange
            var sede = new Sede { Id = 1, Nombre = "Sede 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Telefono = "11123332", Provincia = "Provincia 1", Latitud = 1.0, Longitud = 1.0, OrganizacionId = 1 };
            _context.Sedes.Add(sede);
            await _context.SaveChangesAsync();

            // Act
            await _headquartersService.DeleteHeadquartersAsync(1);

            // Assert
            var deletedSede = await _context.Sedes.FirstOrDefaultAsync(s => s.Id == 1);
            Assert.Null(deletedSede);
        }

        [Test]
        public async Task GetHeadquarterByIdAsync_WhenCalled_ReturnsHeadquarter()
        {
            // Arrange
            var sede = new Sede { Id = 1, Nombre = "Sede 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Telefono = "11123332", Provincia = "Provincia 1", Latitud = 1.0, Longitud = 1.0, OrganizacionId = 1 };
            _context.Sedes.Add(sede);
            await _context.SaveChangesAsync();

            // Act
            var result = await _headquartersService.GetHeadquarterByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Sede 1", result.Nombre);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }

}
