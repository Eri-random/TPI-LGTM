using AutoMapper;
using backend.api.Controllers;
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
    public class CampaignControllerTest
    {
        private Mock<ICampaignService> _mockCampaignService;
        private Mock<ILogger<CampaignController>> _mockLogger;
        private Mock<IMapper> _mockMapper;
        private Mock<INeedService> _mockNeedService;
        private CampaignController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockCampaignService = new Mock<ICampaignService>();
            _mockLogger = new Mock<ILogger<CampaignController>>();
            _mockMapper = new Mock<IMapper>();
            _mockNeedService = new Mock<INeedService>();
            _controller = new CampaignController(_mockCampaignService.Object, _mockLogger.Object, _mockMapper.Object, _mockNeedService.Object);
        }

        [Test]
        public void Constructor_WithValidParameters_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new CampaignController(_mockCampaignService.Object, _mockLogger.Object, _mockMapper.Object, _mockNeedService.Object));
        }

        [Test]
        public void Constructor_WithNullCampaignService_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new CampaignController(null, _mockLogger.Object, _mockMapper.Object, _mockNeedService.Object));
            Assert.That(ex.ParamName, Is.EqualTo("campaignService"));
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new CampaignController(_mockCampaignService.Object, null, _mockMapper.Object, _mockNeedService.Object));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public void Constructor_WithNullMapper_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new CampaignController(_mockCampaignService.Object, _mockLogger.Object, null, _mockNeedService.Object));
            Assert.That(ex.ParamName, Is.EqualTo("mapper"));
        }

        [Test]
        public void Constructor_WithNullNeedService_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new CampaignController(_mockCampaignService.Object, _mockLogger.Object, _mockMapper.Object, null));
            Assert.That(ex.ParamName, Is.EqualTo("needService"));
        }

        [Test]
        public async Task GetCampaignsByOrganizationId_ValidId_ReturnsOk()
        {
            // Arrange
            var organizationId = 1;
            var campaigns = new List<CampaignDto> { new CampaignDto { Id = 1, Title = "Campaign 1", Subcategorias = "11" } };
            var sub = new List<SubcategoriesDto>
            {
                new SubcategoriesDto { Id = 1, NecesidadId = 1 }
            };
            var needs = new List<NeedDto> { new NeedDto { Id = 1, Nombre = "Need 1", Subcategoria = sub } };

            _mockCampaignService.Setup(s => s.GetCampaigns(It.IsAny<int>())).ReturnsAsync(campaigns);
            _mockNeedService.Setup(s => s.GetAllNeedsAsync()).ReturnsAsync(needs);
            _mockMapper.Setup(m => m.Map<CampaignResponseModel>(It.IsAny<CampaignDto>())).Returns(new CampaignResponseModel());

            // Act
            var result = await _controller.GetCampaignsByOrganizationId(organizationId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<List<CampaignResponseModel>>(okResult.Value);
        }

        [Test]
        public async Task GetCampaignsByOrganizationId_NoCampaigns_ReturnsNotFound()
        {
            // Arrange
            var organizationId = 1;

            _mockCampaignService.Setup(s => s.GetCampaigns(It.IsAny<int>())).ReturnsAsync((List<CampaignDto>)null);

            // Act
            var result = await _controller.GetCampaignsByOrganizationId(organizationId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetCampaignsByOrganizationId_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var organizationId = 1;

            _mockCampaignService.Setup(s => s.GetCampaigns(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetCampaignsByOrganizationId(organizationId);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task CreateCampaign_ValidRequest_ReturnsCreated()
        {
            // Arrange
            var campaignRequest = new CampaignRequestModel { Id = 1, Title = "Campaign 1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };
            var campaignDto = new CampaignDto { Id = 1, Title = "Campaign 1" };

            _mockMapper.Setup(m => m.Map<CampaignDto>(It.IsAny<CampaignRequestModel>())).Returns(campaignDto);

            // Act
            var result = await _controller.CreateCampaign(campaignRequest);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
        }

        [Test]
        public async Task CreateCampaign_NullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.CreateCampaign(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task CreateCampaign_InvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var campaignRequest = new CampaignRequestModel { Id = 1, Title = "Campaign 1", StartDate = DateTime.Now.AddDays(1), EndDate = DateTime.Now };

            // Act
            var result = await _controller.CreateCampaign(campaignRequest);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task CreateCampaign_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var campaignRequest = new CampaignRequestModel { Id = 1, Title = "Campaign 1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };

            _mockMapper.Setup(m => m.Map<CampaignDto>(It.IsAny<CampaignRequestModel>())).Throws(new Exception());

            // Act
            var result = await _controller.CreateCampaign(campaignRequest);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task DeleteCampaign_ValidId_ReturnsNoContent()
        {
            // Arrange
            var campaignId = 1;
            var campaignDto = new CampaignDto { Id = 1, Title = "Campaign 1" };

            _mockCampaignService.Setup(s => s.GetCampaignById(It.IsAny<int>())).ReturnsAsync(campaignDto);

            // Act
            var result = await _controller.DeleteCampaign(campaignId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteCampaign_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var campaignId = 1;

            _mockCampaignService.Setup(s => s.GetCampaignById(It.IsAny<int>())).ReturnsAsync((CampaignDto)null);

            // Act
            var result = await _controller.DeleteCampaign(campaignId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task DeleteCampaign_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var campaignId = 1;

            _mockCampaignService.Setup(s => s.GetCampaignById(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.DeleteCampaign(campaignId);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task UpdateCampaignStatus_ValidRequest_ReturnsNoContent()
        {
            // Arrange
            var campaignRequest = new CampaignRequestModel { Id = 1, Title = "Campaign 1" };
            var campaignDto = new CampaignDto { Id = 1, Title = "Campaign 1" };

            _mockCampaignService.Setup(s => s.GetCampaignById(It.IsAny<int>())).ReturnsAsync(campaignDto);
            _mockMapper.Setup(m => m.Map<CampaignDto>(It.IsAny<CampaignRequestModel>())).Returns(campaignDto);

            // Act
            var result = await _controller.UpdateCampaignStatus(campaignRequest);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task UpdateCampaignStatus_NullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UpdateCampaignStatus(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UpdateCampaignStatus_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var campaignRequest = new CampaignRequestModel { Id = 1, Title = "Campaign 1" };

            _mockCampaignService.Setup(s => s.GetCampaignById(It.IsAny<int>())).ReturnsAsync((CampaignDto)null);

            // Act
            var result = await _controller.UpdateCampaignStatus(campaignRequest);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task UpdateCampaignStatus_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var campaignRequest = new CampaignRequestModel { Id = 1, Title = "Campaign 1" };

            _mockCampaignService.Setup(s => s.GetCampaignById(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateCampaignStatus(campaignRequest);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [Test]
        public async Task GetCampaignById_ValidId_ReturnsOk()
        {
            // Arrange
            var campaignId = 1;
            var campaignDto = new CampaignDto { Id = 1, Title = "Campaign 1", Subcategorias = "11" };
            var sub = new List<SubcategoriesDto>
            {
                new SubcategoriesDto { Id = 1, NecesidadId = 1 }
            };
            var needs = new List<NeedDto> { new NeedDto { Id = 1, Nombre = "Need 1", Subcategoria = sub } };

            _mockCampaignService.Setup(s => s.GetCampaignById(It.IsAny<int>())).ReturnsAsync(campaignDto);
            _mockNeedService.Setup(s => s.GetAllNeedsAsync()).ReturnsAsync(needs);
            _mockMapper.Setup(m => m.Map<CampaignResponseModel>(It.IsAny<CampaignDto>())).Returns(new CampaignResponseModel());

            // Act
            var result = await _controller.GetCampaignById(campaignId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<CampaignResponseModel>(okResult.Value);
        }

        [Test]
        public async Task GetCampaignById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var campaignId = 1;

            _mockCampaignService.Setup(s => s.GetCampaignById(It.IsAny<int>())).ReturnsAsync((CampaignDto)null);

            // Act
            var result = await _controller.GetCampaignById(campaignId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetCampaignById_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var campaignId = 1;

            _mockCampaignService.Setup(s => s.GetCampaignById(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetCampaignById(campaignId);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
        }
    }
}
