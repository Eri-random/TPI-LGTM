using AutoMapper;
using backend.api.Controllers;
using backend.api.Mappers;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
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
        private IMapper _mapper;
        private HeadquartersController _controller;

        [SetUp]
        public void SetUp()
        {
            _headquartersServiceMock = new Mock<IHeadquartersService>();
            _loggerMock = new Mock<ILogger<HeadquartersController>>();
            _mapsServiceMock = new Mock<IMapsService>();
            _organizationServiceMock = new Mock<IOrganizationService>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new OrganizationProfile());
            });

            _mapper = mappingConfig.CreateMapper();
            _controller = new HeadquartersController(_headquartersServiceMock.Object, _loggerMock.Object, _mapsServiceMock.Object, _organizationServiceMock.Object, _mapper);
        }

        [Test]
        public void Constructor_WithValidParameters_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new HeadquartersController(_headquartersServiceMock.Object, _loggerMock.Object, _mapsServiceMock.Object, _organizationServiceMock.Object, _mapper));
        }

        [Test]
        public void Constructor_WithNullHeadquartersService_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new HeadquartersController(null, _loggerMock.Object, _mapsServiceMock.Object, _organizationServiceMock.Object, _mapper));
            Assert.That(ex.ParamName, Is.EqualTo("headquartersService"));
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new HeadquartersController(_headquartersServiceMock.Object, null, _mapsServiceMock.Object, _organizationServiceMock.Object, _mapper));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public void Constructor_WithNullMapsService_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new HeadquartersController(_headquartersServiceMock.Object, _loggerMock.Object, null, _organizationServiceMock.Object, _mapper));
            Assert.That(ex.ParamName, Is.EqualTo("mapsService"));
        }

        [Test]
        public void Constructor_WithNullOrganizationService_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new HeadquartersController(_headquartersServiceMock.Object, _loggerMock.Object, _mapsServiceMock.Object, null, _mapper));
            Assert.That(ex.ParamName, Is.EqualTo("organizationService"));
        }

        [Test]
        public void Constructor_WithNullMapper_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new HeadquartersController(_headquartersServiceMock.Object, _loggerMock.Object, _mapsServiceMock.Object, _organizationServiceMock.Object, null));
            Assert.That(ex.ParamName, Is.EqualTo("mapper"));
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
        public async Task GetAllHeadquarters_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _headquartersServiceMock.Setup(service => service.GetAllHeadquartersAsync()).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAllHeadquarters();

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task CreateSede_ReturnsBadRequest_WhenInvalidRequest()
        {
            // Arrange
            List<HeadquartersRequestModel> headquartersRequestModels = null;

            // Act
            var result = await _controller.CreateHeadquarter(headquartersRequestModels);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
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
            var result = await _controller.CreateHeadquarter(headquartersRequestModels);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
            var okResult = result as OkResult;
            _headquartersServiceMock.Verify(service => service.CreateHeadquartersAsync(It.IsAny<List<HeadquartersDto>>()), Times.Once);
        }

        [Test]
        public async Task CreateSede_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var headquartersRequestModels = new List<HeadquartersRequestModel>
        {
            new HeadquartersRequestModel { Nombre = "Nombre 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Provincia = "Provincia 1", Telefono = "12345", OrganizacionId = 1 }
        };
            _headquartersServiceMock.Setup(service => service.CreateHeadquartersAsync(It.IsAny<List<HeadquartersDto>>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.CreateHeadquarter(headquartersRequestModels);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
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
        public async Task GetHeadquartersByOrganizationId_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var organizationId = 1;
            _headquartersServiceMock.Setup(service => service.GetHeadquartersByOrganizationIdAsync(organizationId)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetHeadquartersByOrganizationId(organizationId);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task UpdateHeadquarters_ReturnsBadRequest_WhenInvalidRequest()
        {
            // Arrange
            HeadquartersRequestModel headquartersRequestModel = null;

            // Act
            var result = await _controller.UpdateHeadquarters(headquartersRequestModel);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task UpdateHeadquarters_ReturnsOkResult_WhenValidRequest()
        {
            // Arrange
            var headquartersRequestModel = new HeadquartersRequestModel { Id = 1, Nombre = "Nombre 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Provincia = "Provincia 1", Telefono = "12345", OrganizacionId = 1 };

            // Act
            var result = await _controller.UpdateHeadquarters(headquartersRequestModel);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
            var okResult = result as OkResult;
            _headquartersServiceMock.Verify(service => service.UpdateHeadquartersAsync(It.IsAny<HeadquartersDto>()), Times.Once);
        }

        [Test]
        public async Task UpdateHeadquarters_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var headquartersRequestModel = new HeadquartersRequestModel { Id = 1, Nombre = "Nombre 1", Direccion = "Direccion 1", Localidad = "Localidad 1", Provincia = "Provincia 1", Telefono = "12345", OrganizacionId = 1 };
            _headquartersServiceMock.Setup(service => service.UpdateHeadquartersAsync(It.IsAny<HeadquartersDto>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateHeadquarters(headquartersRequestModel);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
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
        public async Task DeleteHeadquarters_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var headquartersId = 1;
            _headquartersServiceMock.Setup(service => service.DeleteHeadquartersAsync(headquartersId)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.DeleteHeadquarters(headquartersId);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
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
        public async Task GetHeadquartersById_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var headquartersId = 1;
            _headquartersServiceMock.Setup(service => service.GetHeadquarterByIdAsync(headquartersId)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetHeadquartersById(headquartersId);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task Evaluate_ReturnsBadRequest_WhenInvalidRequest()
        {
            // Arrange
            DataRequestModel data = null;

            // Act
            var result = await _controller.Evaluate(data);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Evaluate_ReturnsOkResult_WithNearestHeadquarters()
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
            var result = await _controller.Evaluate(data);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<HeadquartersNearbyDto>());
            var returnValue = okResult.Value as HeadquartersNearbyDto;
            Assert.That(returnValue, Is.Not.Null);

            Assert.That(returnValue.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task Evaluate_ReturnsInternalServerError_WhenExceptionIsThrown()
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

            _mapsServiceMock.Setup(service => service.GetCoordinates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.Evaluate(data);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
