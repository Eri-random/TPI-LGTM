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
    public class NeedControllerTest
    {
        private Mock<INeedService> _needServiceMock;
        private Mock<ILogger<NeedController>> _loggerMock;
        private IMapper _mapper;
        private NeedController _controller;

        [SetUp]
        public void SetUp()
        {
            _needServiceMock = new Mock<INeedService>();
            _loggerMock = new Mock<ILogger<NeedController>>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new NeedsProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _controller = new NeedController(_needServiceMock.Object, _loggerMock.Object, _mapper);
        }

        [Test]
        public void Constructor_WithNullNeedService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new NeedController(null, _loggerMock.Object, _mapper));
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new NeedController(_needServiceMock.Object, null, _mapper));
        }

        [Test]
        public void Constructor_WithNullMapper_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new NeedController(_needServiceMock.Object, _loggerMock.Object, null));
        }

        [Test]
        public async Task GetAllNeeds_ReturnsOkResult_WithNeeds()
        {
            // Arrange
            var needsList = new List<NeedDto>
        {
            new NeedDto { Id = 1, Nombre = "Nombre 1", Icono = "Icono 1", Subcategoria = new List<SubcategoriesDto> { new SubcategoriesDto { Id = 1, Nombre = "Nombre 1", NecesidadId = 1 } } },
            new NeedDto { Id = 2, Nombre = "Nombre 2", Icono = "Icono 2", Subcategoria = new List<SubcategoriesDto> { new SubcategoriesDto { Id = 2, Nombre = "Nombre 2", NecesidadId = 2 } } }
        };
            _needServiceMock.Setup(service => service.GetAllNeedsAsync()).ReturnsAsync(needsList);

            // Act
            var result = await _controller.GetAllNeeds();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<NeedsResponseModel>>());
            var returnValue = okResult.Value as List<NeedsResponseModel>;
            Assert.That(returnValue.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllNeeds_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _needServiceMock.Setup(service => service.GetAllNeedsAsync()).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAllNeeds();

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetAllNeeds_ReturnsEmptyList_WhenNoNeedsFound()
        {
            // Arrange
            _needServiceMock.Setup(service => service.GetAllNeedsAsync()).ReturnsAsync(new List<NeedDto>());

            // Act
            var result = await _controller.GetAllNeeds();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<NeedsResponseModel>>());
            var returnValue = okResult.Value as List<NeedsResponseModel>;
            Assert.That(returnValue.Count, Is.EqualTo(0));
        }
    }
}
