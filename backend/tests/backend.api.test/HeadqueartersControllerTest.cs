using backend.api.Controllers;
using backend.api.Models;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.api.test
{
    [TestFixture]
    public  class HeadqueartersControllerTest
    {
        private Mock<IHeadquartersService> _headquartersServiceMock;
        private Mock<ILogger<HeadquartersController>> _loggerMock;
        private Mock<IMapsService> _mapsServiceMock;
        private Mock<IOrganizationService> _organizationServiceMock;
        private HeadquartersController _controller;

        [SetUp]
        public void SetUp()
        {
            _headquartersServiceMock = new Mock<IHeadquartersService>();
            _loggerMock = new Mock<ILogger<HeadquartersController>>();
            _mapsServiceMock = new Mock<IMapsService>();
            _organizationServiceMock = new Mock<IOrganizationService>();
            _controller = new HeadquartersController(_headquartersServiceMock.Object, _loggerMock.Object, _mapsServiceMock.Object, _organizationServiceMock.Object);
        }

        [Test]
        public async Task GetAllHeadquarters_ReturnsOkResult_WithHeadquarters()
        {
            // Arrange
            var headquartersList = new List<HeadquartersDto>
        {
            new HeadquartersDto { Direccion = "Direccion 1", Localidad = "Localidad 1", Nombre = "Nombre 1", Provincia = "Provincia 1", Telefono = "12345", Latitud = 0, Longitud = 0, OrganizacionId = 1 },
            new HeadquartersDto { Direccion = "Direccion 2", Localidad = "Localidad 2", Nombre = "Nombre 2", Provincia = "Provincia 2", Telefono = "67890", Latitud = 0, Longitud = 0, OrganizacionId = 2 }
        };
            _headquartersServiceMock.Setup(service => service.GetAllHeadquartersAsync()).ReturnsAsync(headquartersList);

            // Act
            var result = await _controller.GetAllHeadquarters();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<HeadquartersResponseModel>>());
            var returnValue = okResult.Value as List<HeadquartersResponseModel>;
            Assert.That(returnValue.Count, Is.EqualTo(2));
        }   

        [Test]
        public async Task CreateSede_ReturnsOkResult_WhenValidRequest()
        {
            // Arrange
            var headquartersRequestModels = new List<HeadquartersRequestModel>
        {
            new HeadquartersRequestModel { Nombre = "Nombre 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Provincia = "Provincia 1", Telefono = "12345", OrganizacionId = 1 }
        };

            // Act
            var result = await _controller.CreateSede(headquartersRequestModels);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
            var okResult = result as OkResult;
            _headquartersServiceMock.Verify(service => service.CreateHeadquartersAsync(It.IsAny<List<HeadquartersDto>>()), Times.Once);
        }

        [Test]
        public async Task GetHeadquartersByOrganizationId_ReturnsOkResult_WithHeadquarters()
        {
            // Arrange
            var organizationId = 1;
            var headquartersList = new List<HeadquartersDto>
            {
                new HeadquartersDto { Id = 1, Direccion = "Direccion 1", Localidad = "Localidad 1", Nombre = "Nombre 1", Provincia = "Provincia 1", Telefono = "12345", Latitud = 0, Longitud = 0, OrganizacionId = organizationId }
            };
            _headquartersServiceMock.Setup(service => service.GetHeadquartersByOrganizationIdAsync(organizationId)).ReturnsAsync(headquartersList);

            // Act
            var result = await _controller.GetHeadquartersByOrganizationId(organizationId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<HeadquartersResponseModel>>());
            var returnValue = okResult.Value as List<HeadquartersResponseModel>;
            Assert.That(returnValue.Count, Is.EqualTo(1));
            Assert.That(returnValue[0].OrganizacionId, Is.EqualTo(organizationId));

        }

        [Test]
        public async Task UpdateHeadquarters_ReturnsOkResult_WhenValidRequest()
        {
            // Arrange
            var headquartersRequestModel = new HeadquartersRequestModel { Id = 1, Nombre = "Nombre 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Provincia = "Provincia 1", Telefono = "12345", OrganizacionId = 1 };

            // Act
            var result = await _controller.UpdatehHeadquarters(headquartersRequestModel);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
            var okResult = result as OkResult;
            _headquartersServiceMock.Verify(service => service.UpdateHeadquartersAsync(It.IsAny<HeadquartersDto>()), Times.Once);
        }

        [Test]
        public async Task DeleteHeadquarters_ReturnsOkResult_WhenValidId()
        {
            // Arrange
            var headquartersId = 1;

            // Act
            var result = await _controller.DeleteHeadquarters(headquartersId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
            var okResult = result as OkResult;
            _headquartersServiceMock.Verify(service => service.DeleteHeadquartersAsync(headquartersId), Times.Once);
        }

        [Test]
        public async Task GetHeadquartersById_ReturnsOkResult_WithHeadquarter()
        {
            // Arrange
            var headquartersId = 1;
            var headquarters = new HeadquartersDto { Id = headquartersId, Direccion = "Direccion 1", Localidad = "Localidad 1", Nombre = "Nombre 1", Provincia = "Provincia 1", Telefono = "12345", Latitud = 0, Longitud = 0, OrganizacionId = 1 };
            _headquartersServiceMock.Setup(service => service.GetHeadquarterByIdAsync(headquartersId)).ReturnsAsync(headquarters);

            // Act
            var result = await _controller.GetHeadquartersById(headquartersId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<HeadquartersResponseModel>());
            var returnValue = okResult.Value as HeadquartersResponseModel;
            Assert.That(returnValue, Is.Not.Null);
            Assert.That(returnValue.Id, Is.EqualTo(headquartersId));
        }

        [Test]
        public async Task Evaluar_ReturnsOkResult_WithNearestHeadquarters()
        {
            {
                // Arrange
                var data = new DataRequestModel
                {
                    Usuario = new Usuario { Id = 1, Nombre = "Usuario 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Provincia = "Provincia 1", Telefono = "12345" },
                    Organizacion = new Organizacion { Id = 1, Nombre = "Organizacion 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Provincia = "Provincia 1", Telefono = "12345" },
                    Sedes = new List<Sede>
                    {
                        new Sede { Id = 1, Nombre = "Sede 1", Direccion = "Direccion Sede 1", Localidad = "Localidad Sede 1", Provincia = "Provincia Sede 1", Telefono = "12345", Latitud = 1, Longitud = 1, OrganizacionId = 1 },
                        new Sede { Id = 2, Nombre = "Sede 2", Direccion = "Direccion Sede 2", Localidad = "Localidad Sede 2", Provincia = "Provincia Sede 2", Telefono = "67890", Latitud = 2, Longitud = 2, OrganizacionId = 1 }
                    }
                };

                _mapsServiceMock.Setup(service => service.GetCoordinates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((0, 0));

                // Act
                var result = await _controller.Evaluar(data);
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.InstanceOf<OkObjectResult>());
                var okResult = result as OkObjectResult;

                Assert.That(okResult.Value, Is.InstanceOf<HeadquartersNearby>());
                var returnValue = okResult.Value as HeadquartersNearby;
                Assert.That(returnValue, Is.Not.Null);

                Assert.That(returnValue.Id, Is.EqualTo(1));
            }
        }
    }
}
