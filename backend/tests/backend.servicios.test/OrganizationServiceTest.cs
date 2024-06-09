using backend.data.DataContext;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using backend.servicios.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.servicios.test
{
    [TestFixture]
    public class OrganizationServiceTest
    {
        private Mock<ILogger<OrganizationService>> _loggerMock;
        private Mock<IMapsService> _mapsMock;
        private ApplicationDbContext _context;
        private OrganizationService _organizationService;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<OrganizationService>>();
            _mapsMock = new Mock<IMapsService>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _organizationService = new OrganizationService(_context, _loggerMock.Object, _mapsMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }


        [Test]
        public async Task SaveOrganizationAsync_WhenOrganizationDtoIsValid_SavesSuccessfully()
        {
            // Arrange
            var organizationDto = new OrganizationDto
            {
                Nombre = "Org 1",
                Cuit = "12345678",
                Direccion = "Direccion 1",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            // Act
            await _organizationService.SaveOrganizationAsync(organizationDto);

            // Assert
            var organization = await _context.Organizacions.FirstOrDefaultAsync();
            Assert.NotNull(organization);
            Assert.AreEqual("Org 1", organization.Nombre);
        }

        [Test]
        public async Task GetAllOrganizationAsync_WhenThereAreNoOrganizations_ReturnsEmptyList()
        {
            // Act
            var result = await _organizationService.GetAllOrganizationAsync();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetAllOrganizationAsync_WhenThereAreOrganizations_ReturnsListOfOrganizations()
        {
            // Arrange
            var organization = new Organizacion
            {
                Id = 1,
                Nombre = "Org 1",
                Cuit = "12345678",
                Direccion = "Direccion 1",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            _context.Organizacions.Add(organization);
            await _context.SaveChangesAsync();

            // Act
            var result = await _organizationService.GetAllOrganizationAsync();

            // Assert
            Assert.IsNotEmpty(result);
            var orgResult = result.First();
            Assert.AreEqual(1, orgResult.Id);
            Assert.AreEqual("Org 1", orgResult.Nombre);
        }

        [Test]
        public async Task GetOrganizationByIdAsync_WhenOrganizationExists_ReturnsOrganization()
        {
            // Arrange
            var organization = new Organizacion
            {
                Id = 1,
                Nombre = "Org 1",
                Cuit = "12345678",
                Direccion = "Direccion 1",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            _context.Organizacions.Add(organization);
            await _context.SaveChangesAsync();

            // Act
            var result = await _organizationService.GetOrganizationByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Org 1", result.Nombre);
        }

        [Test]
        public async Task GetOrganizationByIdAsync_WhenOrganizationDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _organizationService.GetOrganizationByIdAsync(999);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOrganizationByCuitAsync_WhenOrganizationExists_ReturnsOrganization()
        {
            // Arrange
            var organization = new Organizacion
            {
                Id = 1,
                Nombre = "Org 1",
                Cuit = "12345678",
                Direccion = "Direccion 1",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            _context.Organizacions.Add(organization);
            await _context.SaveChangesAsync();

            // Act
            var result = await _organizationService.GetOrganizationByCuitAsync("12345678");

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Org 1", result.Nombre);
        }

        [Test]
        public async Task GetOrganizationByCuitAsync_WhenOrganizationDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _organizationService.GetOrganizationByCuitAsync("99999999");

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task UpdateOrganizationAsync_WhenOrganizationExists_UpdatesSuccessfully()
        {
            // Arrange
            var organization = new Organizacion
            {
                Id = 1,
                Nombre = "Org 1",
                Cuit = "12345678",
                Direccion = "Direccion 1",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            _context.Organizacions.Add(organization);
            await _context.SaveChangesAsync();

            var organizationDto = new OrganizationDto
            {
                Id = 1,
                Nombre = "Org Actualizada",
                Cuit = "12345678",
                Direccion = "Direccion Actualizada",
                Localidad = "Localidad Actualizada",
                Provincia = "Provincia Actualizada",
                Telefono = "987654321"
            };

            _mapsMock.Setup(m => m.GetCoordinates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                     .ReturnsAsync((1.0, 1.0));

            // Act
            await _organizationService.UpdateOrganizationAsync(organizationDto);

            // Assert
            var updatedOrg = await _context.Organizacions.FirstOrDefaultAsync(o => o.Id == 1);
            Assert.AreEqual("Org Actualizada", updatedOrg.Nombre);
            Assert.AreEqual("Direccion Actualizada", updatedOrg.Direccion);
        }


        [Test]
        public async Task AssignSubcategoriesAsync_WhenOrganizationExists_AssignsSubcategoriesSuccessfully()
        {
            // Arrange
            var organization = new Organizacion
            {
                Id = 1,
                Nombre = "Org 1",
                Cuit = "12345678",
                Direccion = "Direccion 1",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789",
                Subcategoria = new List<Subcategorium>()
            };

            _context.Organizacions.Add(organization);
            await _context.SaveChangesAsync();

            var subcategories = new List<Subcategorium>
            {
                new Subcategorium { Id = 1, Nombre = "Sub 1", NecesidadId = 1 },
                new Subcategorium { Id = 2, Nombre = "Sub 2", NecesidadId = 1 }
            };

            _context.Subcategoria.AddRange(subcategories);
            await _context.SaveChangesAsync();

            var subcategoriesDto = subcategories.Select(s => new SubcategoriesDto { Id = s.Id, Nombre = s.Nombre }).ToList();

            // Act
            await _organizationService.AssignSubcategoriesAsync(1, subcategoriesDto);

            // Assert
            var org = await _context.Organizacions.Include(o => o.Subcategoria).FirstOrDefaultAsync(o => o.Id == 1);
            Assert.AreEqual(2, org.Subcategoria.Count);
        }

        [Test]
        public async Task GetAssignedSubcategoriesAsync_WhenOrganizationExists_ReturnsSubcategories()
        {
            // Arrange
            var organization = new Organizacion
            {
                Id = 1,
                Nombre = "Org 1",
                Cuit = "12345678",
                Direccion = "Direccion 1",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789",
                Subcategoria = new List<Subcategorium>()
            };

            _context.Organizacions.Add(organization);
            await _context.SaveChangesAsync();

            var subcategories = new List<Subcategorium>
            {
                new Subcategorium { Id = 1, Nombre = "Sub 1", NecesidadId = 1, Organizacions = new List<Organizacion> { organization } },
                new Subcategorium { Id = 2, Nombre = "Sub 2", NecesidadId = 1, Organizacions = new List<Organizacion> { organization } }
            };

            _context.Subcategoria.AddRange(subcategories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _organizationService.GetAssignedSubcategoriesAsync(1);

            // Assert
            Assert.AreEqual(2, result.Count);
        }
    }
}