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
    public class NeedServiceTest
    {
        private Mock<ILogger<NeedService>> _loggerMock;
        private ApplicationDbContext _context;
        private IRepository<Necesidad> _repository;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<NeedService>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new Repository<Necesidad>(_context);
        }

        [Test]
        public async Task GetAllNeedAsync_WhenThereAreNoNeeds_ReturnsEmptyList()
        {
            // Arrange
            var needService = new NeedService(_repository, _loggerMock.Object);

            // Act
            var result = await needService.GetAllNeedsAsync();

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetAllNeedAsync_WhenThereAreNeeds_ReturnsListOfNeeds()
        {
            // Arrange
            var need = new Necesidad
            {
                Id = 1,
                Nombre = "Necesidad 1",
                Icono = "Icono 1",
                Subcategoria = new List<Subcategorium>
                {
                    new Subcategorium
                    {
                        Id = 1,
                        Nombre = "Subcategoria 1",
                        NecesidadId = 1
                    }
                }
            };
            _context.Necesidads.Add(need);
            _context.SaveChanges();

            var needService = new NeedService(_repository, _loggerMock.Object);

            // Act
            var result = await needService.GetAllNeedsAsync();

            // Assert
            Assert.IsNotEmpty(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().Id);
            Assert.AreEqual("Necesidad 1", result.First().Nombre);
            Assert.AreEqual("Icono 1", result.First().Icono);
            Assert.IsNotEmpty(result.First().Subcategoria);
            Assert.AreEqual(1, result.First().Subcategoria.Count());
            Assert.AreEqual(1, result.First().Subcategoria.First().Id);
            Assert.AreEqual("Subcategoria 1", result.First().Subcategoria.First().Nombre);
            Assert.AreEqual(1, result.First().Subcategoria.First().NecesidadId);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
