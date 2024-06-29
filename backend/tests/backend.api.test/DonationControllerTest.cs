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
    public class DonationControllerTest
    {
        private Mock<ILogger<DonationController>> _loggerMock;
        private Mock<IDonationService> _donationServiceMock;
        private Mock<IUserService> _userServiceMock;    
        private DonationController _controller;
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<DonationController>>();
            _donationServiceMock = new Mock<IDonationService>();
            _userServiceMock = new Mock<IUserService>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DonationProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _controller = new DonationController(_loggerMock.Object, _userServiceMock.Object, _donationServiceMock.Object, _mapper);
        }

        [Test]
        public void Constructor_WithValidParameters_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new DonationController(_loggerMock.Object, _userServiceMock.Object, _donationServiceMock.Object, _mapper));
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new DonationController(null, _userServiceMock.Object, _donationServiceMock.Object, _mapper));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public void Constructor_WithNullUserService_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new DonationController(_loggerMock.Object, null, _donationServiceMock.Object, _mapper));
            Assert.That(ex.ParamName, Is.EqualTo("userService"));
        }

        [Test]
        public void Constructor_WithNullDonationService_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new DonationController(_loggerMock.Object, _userServiceMock.Object, null, _mapper));
            Assert.That(ex.ParamName, Is.EqualTo("donationService"));
        }

        [Test]
        public void Constructor_WithNullMapper_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new DonationController(_loggerMock.Object, _userServiceMock.Object, _donationServiceMock.Object, null));
            Assert.That(ex.ParamName, Is.EqualTo("mapper"));
        }

        [Test]
        public async Task UpdateDonationsState_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var request = new UpdateDonationsStateRequest
            {
                DonationIds = new List<int> { 1, 2, 3 },
                State = "Received"
            };

            _donationServiceMock.Setup(service => service.UpdateDonationsStateAsync(request.DonationIds, request.State)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateDonationsState(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public async Task UpdateDonationsState_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new UpdateDonationsStateRequest
            {
                DonationIds = new List<int> { 1, 2, 3 },
                State = "Received"
            };

            _donationServiceMock
                .Setup(service => service.UpdateDonationsStateAsync(It.IsAny<List<int>>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateDonationsState(request);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetDonationsByUserId_ReturnsOkResult_WithEmptyList_WhenNoDonations()
        {
            // Arrange
            var userId = 1;
            var donationsList = new List<DonationDto>();
            _donationServiceMock.Setup(service => service.GetDonationsByUserIdAsync(userId)).ReturnsAsync(donationsList);

            // Act
            var result = await _controller.GetDonationsByUserId(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<DonationResponseModel>>());
            var returnValue = okResult.Value as List<DonationResponseModel>;
            Assert.That(returnValue.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetDonationsByOrganizationId_ReturnsOkResult_WithEmptyList_WhenNoDonations()
        {
            // Arrange
            var organizationId = 1;
            var donationsList = new List<DonationDto>();
            _donationServiceMock.Setup(service => service.GetDonationsByOrganizationIdAsync(organizationId)).ReturnsAsync(donationsList);

            // Act
            var result = await _controller.GetDonationsByOrganizationId(organizationId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<DonationResponseModel>>());
            var returnValue = okResult.Value as List<DonationResponseModel>;
            Assert.That(returnValue.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task SaveDonacion_ReturnsBadRequest_WhenInvalidRequest()
        {
            // Arrange
            DonationRequestModel donationRequest = null;

            // Act
            var result = await _controller.SaveDonacion(donationRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task SaveDonacion_ReturnsCreatedResult_WhenValidRequest()
        {
            // Arrange
            var donationRequest = new DonationRequestModel
            {
                Id = 1,
                Producto = "Producto 1",
                Cantidad = 1,
                UsuarioId = 1,
                OrganizacionId = 1,
                Cuit = "123456789"
            };

            var newDonation = new DonationDto
            {
                Id = donationRequest.Id,
                Producto = donationRequest.Producto,
                Cantidad = donationRequest.Cantidad,
                UsuarioId = donationRequest.UsuarioId,
                OrganizacionId = donationRequest.OrganizacionId,
                Cuit = donationRequest.Cuit
            };

            _donationServiceMock.Setup(service => service.SaveDonationAsync(newDonation)).Returns(Task.CompletedTask);
            _userServiceMock.Setup(service => service.GetUserByIdAsync(donationRequest.UsuarioId)).ReturnsAsync(new UserDto());

            // Act
            var result = await _controller.SaveDonacion(donationRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
        }

        [Test]
        public async Task SaveDonacion_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var donationRequest = new DonationRequestModel
            {
                Producto = "Producto 1",
                Cantidad = 1,
                UsuarioId = 1,
                OrganizacionId = 1,
            };

            var newDonation = new DonationDto
            {
                Id = donationRequest.Id,
                Producto = donationRequest.Producto,
                Cantidad = donationRequest.Cantidad,
                UsuarioId = donationRequest.UsuarioId,
                OrganizacionId = donationRequest.OrganizacionId,
                Cuit = donationRequest.Cuit
            };

            _donationServiceMock
            .Setup(service => service.SaveDonationAsync(It.IsAny<DonationDto>()))
            .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.SaveDonacion(donationRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetDonationsByUserId_ReturnsOkResult_WithDonations()
        {
            // Arrange
            var userId = 1;
            var donationsList = new List<DonationDto>
                {
                    new DonationDto { Id = 1, Producto = "Producto 1", Cantidad = 1, UsuarioId = userId, OrganizacionId = 1, Cuit = "123456789" }
                };
            _donationServiceMock.Setup(service => service.GetDonationsByUserIdAsync(userId)).ReturnsAsync(donationsList);

            // Act
            var result = await _controller.GetDonationsByUserId(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<DonationResponseModel>>());
            var returnValue = okResult.Value as List<DonationResponseModel>;
            Assert.That(returnValue.Count, Is.EqualTo(1));
            Assert.That(returnValue[0].UsuarioId, Is.EqualTo(userId));
        }

        [Test]
        public async Task GetDonationsByUserId_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var userId = 1;
            _donationServiceMock
                .Setup(service => service.GetDonationsByUserIdAsync(userId))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetDonationsByUserId(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetDonationsByOrganizationId_ReturnsOkResult_WithDonations()
        {
            // Arrange
            var organizationId = 1;
            var donationsList = new List<DonationDto>
            {
                    new DonationDto { Id = 1, Producto = "Producto 1", Cantidad = 1, UsuarioId = 1, OrganizacionId = organizationId, Cuit = "123456789" }
                };
            _donationServiceMock.Setup(service => service.GetDonationsByOrganizationIdAsync(organizationId)).ReturnsAsync(donationsList);

            // Act
            var result = await _controller.GetDonationsByOrganizationId(organizationId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<DonationResponseModel>>());
            var returnValue = okResult.Value as List<DonationResponseModel>;
            Assert.That(returnValue.Count, Is.EqualTo(1));
            Assert.That(returnValue[0].OrganizacionId, Is.EqualTo(organizationId));
        }
    }
}
