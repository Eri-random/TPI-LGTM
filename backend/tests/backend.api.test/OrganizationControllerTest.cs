using AutoMapper;
using backend.api.Controllers;
using backend.api.Mappers;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.api.test
{
    [TestFixture]
    public class OrganizationControllerTest
    {
        private Mock<IOrganizationService> _organizationServiceMock;
        private Mock<ILogger<OrganizationController>> _loggerMock;
        private IMapper _mapper;
        private OrganizationController _controller;

        [SetUp]
        public void SetUp()
        {
            _organizationServiceMock = new Mock<IOrganizationService>();
            _loggerMock = new Mock<ILogger<OrganizationController>>();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new OrganizationProfile());
            });

            _mapper = mappingConfig.CreateMapper();
            _controller = new OrganizationController(_organizationServiceMock.Object, _loggerMock.Object, _mapper);
        }

        [Test]
        public void Constructor_WithNullOrganizationService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new OrganizationController(null, _loggerMock.Object, _mapper));
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new OrganizationController(_organizationServiceMock.Object, null, _mapper));
        }

        [Test]
        public void Constructor_WithNullMapper_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new OrganizationController(_organizationServiceMock.Object, _loggerMock.Object, null));
        }

        [Test]
        public async Task GetAllOrganizations_ReturnsOkResult_WithOrganizations()
        {
            // Arrange
            var organizationsList = new List<OrganizationDto>
        {
            new OrganizationDto { Id = 1, Nombre = "Org 1", Cuit = "12345678", Direccion = "Direccion 1", Localidad = "Localidad 1", Provincia = "Provincia 1", Telefono = "123456789" },
            new OrganizationDto { Id = 2, Nombre = "Org 2", Cuit = "87654321", Direccion = "Direccion 2", Localidad = "Localidad 2", Provincia = "Provincia 2", Telefono = "987654321" }
        };
            _organizationServiceMock.Setup(service => service.GetAllOrganizationAsync()).ReturnsAsync(organizationsList);

            // Act
            var result = await _controller.GetAllOrganizations();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<OrganizationResponseModel>>());
            var returnValue = okResult.Value as List<OrganizationResponseModel>;
            Assert.That(returnValue.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllOrganizations_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _organizationServiceMock.Setup(service => service.GetAllOrganizationAsync()).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAllOrganizations();

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetOrganizationByCuit_ReturnsOkResult_WithOrganization()
        {
            // Arrange
            var organizationDto = new OrganizationDto
            {
                Id = 1,
                Nombre = "Org 1",
                Cuit = "12345678",
                Direccion = "Direccion 1",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };
            _organizationServiceMock.Setup(service => service.GetOrganizationByCuitAsync("12345678")).ReturnsAsync(organizationDto);

            // Act
            var result = await _controller.GetOrganizationByCuit("12345678");

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<OrganizationResponseModel>());
            var returnValue = okResult.Value as OrganizationResponseModel;
            Assert.That(returnValue.Cuit, Is.EqualTo("12345678"));
        }

        [Test]
        public async Task GetOrganizationByCuit_ReturnsNotFound_WhenOrganizationNotFound()
        {
            // Arrange
            _organizationServiceMock.Setup(service => service.GetOrganizationByCuitAsync("12345678")).ReturnsAsync((OrganizationDto)null);

            // Act
            var result = await _controller.GetOrganizationByCuit("12345678");

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("Organization not found"));
        }

        [Test]
        public async Task GetOrganizationById_ReturnsOkResult_WithOrganization()
        {
            // Arrange
            var organizationDto = new OrganizationDto
            {
                Id = 1,
                Nombre = "Org 1",
                Cuit = "12345678",
                Direccion = "Direccion 1",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };
            _organizationServiceMock.Setup(service => service.GetOrganizationByIdAsync(1)).ReturnsAsync(organizationDto);

            // Act
            var result = await _controller.GetOrganizationById(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<OrganizationResponseModel>());
            var returnValue = okResult.Value as OrganizationResponseModel;
            Assert.That(returnValue.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task GetOrganizationById_ReturnsNotFound_WhenOrganizationNotFound()
        {
            // Arrange
            _organizationServiceMock.Setup(service => service.GetOrganizationByIdAsync(1)).ReturnsAsync((OrganizationDto)null);

            // Act
            var result = await _controller.GetOrganizationById(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("Organization not found"));
        }

        [Test]
        public async Task GetPaginatedOrganizationsAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _organizationServiceMock.Setup(service => service.GetPaginatedOrganizationsAsync(1, 8, null, null)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetPaginatedOrganizationsAsync(1, 8);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task AssignSubcategoriesAsync_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var subcategories = new List<SubcategoriesDto>
        {
            new SubcategoriesDto { Id = 1, Nombre = "Subcategoria 1" },
            new SubcategoriesDto { Id = 2, Nombre = "Subcategoria 2" }
        };

            _organizationServiceMock.Setup(service => service.AssignSubcategoriesAsync(1, subcategories)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AssignSubcategoriesAsync(1, subcategories);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<object>());
        }

        [Test]
        public async Task AssignSubcategoriesAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var subcategories = new List<SubcategoriesDto>
        {
            new SubcategoriesDto { Id = 1, Nombre = "Subcategoria 1" },
            new SubcategoriesDto { Id = 2, Nombre = "Subcategoria 2" }
        };

            _organizationServiceMock.Setup(service => service.AssignSubcategoriesAsync(1, subcategories)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.AssignSubcategoriesAsync(1, subcategories);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetAssignedSubcategories_ReturnsOkResult_WithSubcategories()
        {
            // Arrange
            var subcategoriesList = new List<SubcategoriesDto>
        {
            new SubcategoriesDto { Id = 1, Nombre = "Subcategoria 1" },
            new SubcategoriesDto { Id = 2, Nombre = "Subcategoria 2" }
        };
            _organizationServiceMock.Setup(service => service.GetAssignedSubcategoriesAsync(1)).ReturnsAsync(subcategoriesList);

            // Act
            var result = await _controller.GetAssignedSubcategories(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<SubcategoriesDto>>());
            var returnValue = okResult.Value as List<SubcategoriesDto>;
            Assert.That(returnValue.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAssignedSubcategories_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _organizationServiceMock.Setup(service => service.GetAssignedSubcategoriesAsync(1)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAssignedSubcategories(1);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetGroupedSubcategories_ReturnsOkResult_WithGroupedSubcategories()
        {
            // Arrange
            var groupedSubcategories = new List<NeedDto>
        {
            new NeedDto { Id = 1, Nombre = "Need 1", Subcategoria = new List<SubcategoriesDto> { new SubcategoriesDto { Id = 1, Nombre = "Subcategoria 1" } } },
            new NeedDto { Id = 2, Nombre = "Need 2", Subcategoria = new List<SubcategoriesDto> { new SubcategoriesDto { Id = 2, Nombre = "Subcategoria 2" } } }
        };
            _organizationServiceMock.Setup(service => service.GetAssignedSubcategoriesGroupedAsync(1)).ReturnsAsync(groupedSubcategories);

            // Act
            var result = await _controller.GetGroupedSubcategories(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<NeedDto>>());
            var returnValue = okResult.Value as List<NeedDto>;
            Assert.That(returnValue.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetGroupedSubcategories_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _organizationServiceMock.Setup(service => service.GetAssignedSubcategoriesGroupedAsync(1)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetGroupedSubcategories(1);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task UpdateOrganization_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var organizationRequest = new OrganizationRequestModel
            {
                Id = 1,
                Nombre = "Org 1",
                Cuit = "12345678",
                Direccion = "Direccion 1",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            _organizationServiceMock.Setup(service => service.UpdateOrganizationAsync(It.IsAny<OrganizationDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateOrganization(organizationRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<object>());
        }

        [Test]
        public async Task UpdateOrganization_ReturnsBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = await _controller.UpdateOrganization(null);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("Organization cannot be null"));
        }

        [Test]
        public async Task UpdateOrganization_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var organizationRequest = new OrganizationRequestModel
            {
                Id = 1,
                Nombre = "Org 1",
                Cuit = "12345678",
                Direccion = "Direccion 1",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            _organizationServiceMock.Setup(service => service.UpdateOrganizationAsync(It.IsAny<OrganizationDto>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.UpdateOrganization(organizationRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
            Assert.That(objectResult.Value, Is.EqualTo("Internal server error"));
        }
    }
}
