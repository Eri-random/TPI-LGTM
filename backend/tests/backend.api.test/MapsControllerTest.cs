using AutoMapper;
using backend.api.Controllers;
using backend.api.Mappers;
using backend.api.Models.ResponseModels;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.api.test
{
    [TestFixture]
    public class MapsControllerTests
    {
        private Mock<IOrganizationService> _organizationServiceMock;
        private Mock<IHeadquartersService> _headquartersServiceMock;
        private Mock<ILogger<MapsController>> _loggerMock;
        private IMapper _mapper;
        private MapsController _controller;

        [SetUp]
        public void SetUp()
        {
            _organizationServiceMock = new Mock<IOrganizationService>();
            _headquartersServiceMock = new Mock<IHeadquartersService>();
            _loggerMock = new Mock<ILogger<MapsController>>();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new OrganizationProfile());
            });

            _mapper = mappingConfig.CreateMapper();
            _controller = new MapsController(_organizationServiceMock.Object, _headquartersServiceMock.Object, _mapper, _loggerMock.Object);
        }

        [Test]
        public void Constructor_WithValidArguments_Succeeds()
        {
            // Arrange
            var organizationServiceMock = new Mock<IOrganizationService>();
            var headquartersServiceMock = new Mock<IHeadquartersService>();
            var loggerMock = new Mock<ILogger<MapsController>>();

            // Act & Assert
            Assert.DoesNotThrow(() => new MapsController(organizationServiceMock.Object, headquartersServiceMock.Object, _mapper, loggerMock.Object));
        }

        [Test]
        public void Constructor_WithNullOrganizationService_ThrowsArgumentNullException()
        {
            // Arrange
            var headquartersServiceMock = new Mock<IHeadquartersService>();
            var loggerMock = new Mock<ILogger<MapsController>>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new MapsController(null, headquartersServiceMock.Object, _mapper, loggerMock.Object));
            Assert.That(ex.ParamName, Is.EqualTo("organizationService"));
        }

        [Test]
        public void Constructor_WithNullHeadquartersService_ThrowsArgumentNullException()
        {
            // Arrange
            var organizationServiceMock = new Mock<IOrganizationService>();
            var loggerMock = new Mock<ILogger<MapsController>>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new MapsController(organizationServiceMock.Object, null, _mapper, loggerMock.Object));
            Assert.That(ex.ParamName, Is.EqualTo("headquartersService"));
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var organizationServiceMock = new Mock<IOrganizationService>();
            var headquartersServiceMock = new Mock<IHeadquartersService>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new MapsController(organizationServiceMock.Object, headquartersServiceMock.Object, _mapper, null));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public void Constructor_WithNullMapper_ThrowsArgumentNullException()
        {
            // Arrange
            var organizationServiceMock = new Mock<IOrganizationService>();
            var headquartersServiceMock = new Mock<IHeadquartersService>();
            var loggerMock = new Mock<ILogger<MapsController>>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new MapsController(organizationServiceMock.Object, headquartersServiceMock.Object, null, loggerMock.Object));
            Assert.That(ex.ParamName, Is.EqualTo("mapper"));
        }

        [Test]
        public async Task GetOrganizationCoordinates_ReturnsOkResult_WithOrganizations()
        {
            // Arrange
            var organizationsList = new List<OrganizationDto>
        {
            new OrganizationDto { Id = 1, Nombre = "Org 1", Latitud = 10.0, Longitud = 20.0 },
            new OrganizationDto { Id = 2, Nombre = "Org 2", Latitud = 30.0, Longitud = 40.0 }
        };
            _organizationServiceMock.Setup(service => service.GetAllOrganizationAsync()).ReturnsAsync(organizationsList);

            // Act
            var result = await _controller.GetOrganizationCoordinates();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<OrganizationResponseModel>>());
            var returnValue = okResult.Value as List<OrganizationResponseModel>;
            Assert.That(returnValue.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetOrganizationCoordinates_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _organizationServiceMock.Setup(service => service.GetAllOrganizationAsync()).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetOrganizationCoordinates();

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetOrganizationHeadquartersCoordinates_ReturnsOkResult_WithHeadquarters()
        {
            // Arrange
            var headquartersList = new List<HeadquartersDto>
        {
            new HeadquartersDto { Id = 1, Nombre = "HQ 1", Latitud = 10.0, Longitud = 20.0, OrganizacionId = 1 },
            new HeadquartersDto { Id = 2, Nombre = "HQ 2", Latitud = 30.0, Longitud = 40.0, OrganizacionId = 1 }
        };
            _headquartersServiceMock.Setup(service => service.GetHeadquartersByOrganizationIdAsync(1)).ReturnsAsync(headquartersList);

            // Act
            var result = await _controller.GetOrganizationHeadquartersCoordinates(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<HeadquartersResponseModel>>());
            var returnValue = okResult.Value as List<HeadquartersResponseModel>;
            Assert.That(returnValue.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetOrganizationHeadquartersCoordinates_ReturnsNotFound_WhenHeadquartersNotFound()
        {
            // Arrange
            _headquartersServiceMock.Setup(service => service.GetHeadquartersByOrganizationIdAsync(1)).ReturnsAsync(new List<HeadquartersDto>());

            // Act
            var result = await _controller.GetOrganizationHeadquartersCoordinates(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("Headquarters not found"));
        }

        [Test]
        public async Task GetOrganizationHeadquartersCoordinates_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _headquartersServiceMock.Setup(service => service.GetHeadquartersByOrganizationIdAsync(1)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetOrganizationHeadquartersCoordinates(1);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
