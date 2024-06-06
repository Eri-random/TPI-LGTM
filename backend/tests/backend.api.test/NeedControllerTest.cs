using backend.api.Controllers;
using backend.api.Models;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using backend.servicios.Models;
using backend.servicios.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace backend.api.test
{
    public class NeedControllerTest
    {
        private Mock<INeedService> _needServiceMock;
        private Mock<ILogger<NeedController>> _loggerMock;
        private NeedController _controller;

        [SetUp]
        public void SetUp()
        {
            _needServiceMock = new Mock<INeedService>();
            _loggerMock = new Mock<ILogger<NeedController>>();
            _controller = new NeedController(_needServiceMock.Object, _loggerMock.Object);
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
            _needServiceMock.Setup(service => service.GetAllNeedAsync()).ReturnsAsync(needsList);

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
            _needServiceMock.Setup(service => service.GetAllNeedAsync()).ThrowsAsync(new Exception());

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
            _needServiceMock.Setup(service => service.GetAllNeedAsync()).ReturnsAsync(new List<NeedDto>());

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
