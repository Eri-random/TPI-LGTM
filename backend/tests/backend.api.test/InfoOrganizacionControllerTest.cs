using AutoMapper;
using backend.api.Controllers;
using backend.api.Mappers;
using backend.api.Models.RequestModels;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.api.test
{
    [TestFixture]
    public class InfoOrganizacionControllerTest
    {
        private Mock<IOrganizationService> _organizationServiceMock;
        private Mock<IOrganizationInfoService> _organizationInfoServiceMock;
        private Mock<ILogger<InfoOrganizationController>> _loggerMock;
        private IMapper _mapper;
        private InfoOrganizationController _controller;

        [SetUp]
        public void SetUp()
        {
            _organizationServiceMock = new Mock<IOrganizationService>();
            _organizationInfoServiceMock = new Mock<IOrganizationInfoService>();
            _loggerMock = new Mock<ILogger<InfoOrganizationController>>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new OrganizationProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _controller = new InfoOrganizationController(_organizationServiceMock.Object, _organizationInfoServiceMock.Object, _loggerMock.Object, _mapper);
        }

        [Test]
        public void Constructor_WithNullOrganizationService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new InfoOrganizationController(null, _organizationInfoServiceMock.Object, _loggerMock.Object, _mapper));
        }

        [Test]
        public void Constructor_WithNullOrganizationInfoService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new InfoOrganizationController(_organizationServiceMock.Object, null, _loggerMock.Object, _mapper));
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new InfoOrganizationController(_organizationServiceMock.Object, _organizationInfoServiceMock.Object, null, _mapper));
        }

        [Test]
        public void Constructor_WithNullMapper_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new InfoOrganizationController(_organizationServiceMock.Object, _organizationInfoServiceMock.Object, _loggerMock.Object, null));
        }

        [Test]
        public async Task Details_ReturnsBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Invalid organization data", badRequestResult.Value);
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenOrganizationNotFound()
        {
            // Arrange
            var infoOrganizationRequest = new InfoOrganizationRequestModel
            {
                OrganizacionId = 1
            };

            _organizationServiceMock.Setup(x => x.GetOrganizationByIdAsync(infoOrganizationRequest.OrganizacionId))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.Details(infoOrganizationRequest);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Organization not found", notFoundResult.Value);
        }

        [Test]
        public async Task Details_ReturnsCreatedAtActionResult_WithValidInput()
        {
            // Arrange
            var infoOrganizationRequest = new InfoOrganizationRequestModel
            {
                OrganizacionId = 1,
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
            };

            var organization = new OrganizationDto
            {
                Nombre = "Organizacion",
                Direccion = "Calle 123",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            _organizationServiceMock.Setup(x => x.GetOrganizationByIdAsync(infoOrganizationRequest.OrganizacionId))
                .ReturnsAsync(organization);

            _organizationInfoServiceMock.Setup(x => x.SaveInfoOrganizationDataAsync(It.IsAny<InfoOrganizationDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Details(infoOrganizationRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());

            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("Details", createdResult.ActionName);
            Assert.AreEqual(infoOrganizationRequest.OrganizacionId, createdResult.RouteValues["id"]);
            Assert.AreEqual(infoOrganizationRequest.OrganizacionId, (createdResult.Value as InfoOrganizationDto).OrganizacionId);
            Assert.AreEqual(infoOrganizationRequest.Organizacion, (createdResult.Value as InfoOrganizationDto).Organizacion);
        }

        [Test]
        public async Task Details_ReturnsBadRequest_OnInvalidOperationException()
        {
            // Arrange
            var infoOrganizationRequest = new InfoOrganizationRequestModel
            {
                OrganizacionId = 1,
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
            };

            _organizationServiceMock.Setup(x => x.GetOrganizationByIdAsync(infoOrganizationRequest.OrganizacionId))
                .ReturnsAsync(new OrganizationDto());

            _organizationInfoServiceMock.Setup(x => x.SaveInfoOrganizationDataAsync(It.IsAny<InfoOrganizationDto>()))
                .ThrowsAsync(new InvalidOperationException("Invalid operation"));

            // Act
            var result = await _controller.Details(infoOrganizationRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Invalid operation", badRequestResult.Value);
        }

        [Test]
        public async Task Details_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var infoOrganizationRequest = new InfoOrganizationRequestModel
            {
                OrganizacionId = 1,
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
            };

            _organizationServiceMock.Setup(x => x.GetOrganizationByIdAsync(infoOrganizationRequest.OrganizacionId))
                .ReturnsAsync(new OrganizationDto());

            _organizationInfoServiceMock.Setup(x => x.SaveInfoOrganizationDataAsync(It.IsAny<InfoOrganizationDto>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Details(infoOrganizationRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("Internal server error", objectResult.Value);
        }

        [Test]
        public async Task Update_ReturnsBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = await _controller.Update(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Invalid organization data", badRequestResult.Value);
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenOrganizationNotFound()
        {
            // Arrange
            var infoOrganizationRequest = new InfoOrganizationRequestModel
            {
                OrganizacionId = 1,
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
            };

            _organizationServiceMock.Setup(x => x.GetOrganizationByIdAsync(infoOrganizationRequest.OrganizacionId))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.Update(infoOrganizationRequest);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Organization not found", notFoundResult.Value);
        }

        [Test]
        public async Task Update_ReturnsCreatedAtActionResult_WithValidInput()
        {
            // Arrange
            var infoOrganizationRequest = new InfoOrganizationRequestModel
            {
                OrganizacionId = 1,
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
                ImageUrl = "http://localhost:5203/images/test.jpg"
            };

            var organization = new OrganizationDto
            {
                Nombre = "Organizacion",
                Direccion = "Calle 123",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789",
                InfoOrganizacion = new InfoOrganizationDto
                {
                    Img = "http://localhost:5203/images/old_image.jpg"
                }
            };

            _organizationServiceMock.Setup(x => x.GetOrganizationByIdAsync(infoOrganizationRequest.OrganizacionId))
                .ReturnsAsync(organization);

            _organizationInfoServiceMock.Setup(x => x.UpdateInfoOrganizationAsync(It.IsAny<InfoOrganizationDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(infoOrganizationRequest);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);

            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("Update", createdResult.ActionName);
            Assert.AreEqual(infoOrganizationRequest.OrganizacionId, createdResult.RouteValues["id"]);

            var returnedValue = createdResult.Value as InfoOrganizationDto;
            Assert.IsNotNull(returnedValue);
            Assert.AreEqual(infoOrganizationRequest.OrganizacionId, returnedValue.OrganizacionId);
            Assert.AreEqual(infoOrganizationRequest.Organizacion, returnedValue.Organizacion);
            Assert.AreEqual(infoOrganizationRequest.DescripcionBreve, returnedValue.DescripcionBreve);
            Assert.AreEqual(infoOrganizationRequest.DescripcionCompleta, returnedValue.DescripcionCompleta);
            Assert.AreEqual("http://localhost:5203/images/test.jpg", returnedValue.Img);
        }

        [Test]
        public async Task Update_ReturnsBadRequest_OnInvalidOperationException()
        {
            // Arrange
            var infoOrganizationRequest = new InfoOrganizationRequestModel
            {
                OrganizacionId = 1,
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
                ImageUrl = "http://localhost:5203/images/test.jpg"
            };

            var organization = new OrganizationDto
            {
                Nombre = "Organizacion",
                Direccion = "Calle 123",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789",
                InfoOrganizacion = new InfoOrganizationDto
                {
                    Img = "http://localhost:5203/images/old_image.jpg"
                }
            };

            _organizationServiceMock.Setup(x => x.GetOrganizationByIdAsync(infoOrganizationRequest.OrganizacionId))
                .ReturnsAsync(organization);

            _organizationInfoServiceMock.Setup(x => x.UpdateInfoOrganizationAsync(It.IsAny<InfoOrganizationDto>()))
                .ThrowsAsync(new InvalidOperationException("Invalid operation"));

            // Act
            var result = await _controller.Update(infoOrganizationRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Invalid operation", badRequestResult.Value);
        }

        [Test]
        public async Task Update_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var infoOrganizationRequest = new InfoOrganizationRequestModel
            {
                OrganizacionId = 1,
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
                ImageUrl = "http://localhost:5203/images/test.jpg"
            };

            var organization = new OrganizationDto
            {
                Nombre = "Organizacion",
                Direccion = "Calle 123",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789",
                InfoOrganizacion = new InfoOrganizationDto
                {
                    Img = "http://localhost:5203/images/old_image.jpg"
                }
            };

            _organizationServiceMock.Setup(x => x.GetOrganizationByIdAsync(infoOrganizationRequest.OrganizacionId))
                .ReturnsAsync(organization);

            _organizationInfoServiceMock.Setup(x => x.UpdateInfoOrganizationAsync(It.IsAny<InfoOrganizationDto>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Update(infoOrganizationRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("Internal server error", objectResult.Value);
        }
    }
}
