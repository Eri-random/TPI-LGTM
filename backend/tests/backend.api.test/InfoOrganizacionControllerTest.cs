using AutoMapper;
using backend.api.Controllers;
using backend.api.Mappers;
using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace backend.api.test
{
    [TestFixture]
    public class InfoOrganizacionControllerTest
    {
        private Mock<IOrganizationService> _organizacionService;
        private Mock<IOrganizationInfoService> _organizacionInfoService;
        private Mock<ILogger<UserController>> _logger;
        private IMapper _mapper;
        private InfoOrganizationController _controller;

        [SetUp]
        public void SetUp()
        {
            _organizacionService = new Mock<IOrganizationService>();
            _organizacionInfoService = new Mock<IOrganizationInfoService>();
            _logger = new Mock<ILogger<UserController>>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new OrganizationProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            _controller = new InfoOrganizationController(_organizacionService.Object, _organizacionInfoService.Object, _logger.Object, _mapper);
        }

        [Test]
        public async Task Details_WhenCalled_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Datos de organización inválidos", badRequestResult.Value);
        }

        [Test]
        public async Task Details_WhenCalled_ReturnsNotFound()
        {
            // Arrange
            var infoOrganizacionRequest = new InfoOrganizationRequest
            {
                OrganizacionId = 1
            };

            _organizacionService.Setup(x => x.GetOrganizationByIdAsync(infoOrganizacionRequest.OrganizacionId))
                .ReturnsAsync(() => null);

            // Act
            var result = await _controller.Details(infoOrganizacionRequest);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Organización no encontrada", notFoundResult.Value);
        }

        [Test]
        public async Task Details_WithValidInput_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var infoOrganizacionRequest = new InfoOrganizationRequest
            {
                OrganizacionId = 1,
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
            };

            var organizacion = new OrganizationDto
            {
                Nombre = "Organizacion",
                Direccion = "Calle 123",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            _organizacionService.Setup(x => x.GetOrganizationByIdAsync(infoOrganizacionRequest.OrganizacionId))
                .ReturnsAsync(organizacion);

            _organizacionInfoService.Setup(x => x.SaveInfoOrganizationDataAsync(It.IsAny<InfoOrganizationDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Details(infoOrganizacionRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());

            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("Details", createdResult.ActionName);
            Assert.AreEqual(1, createdResult.RouteValues["id"]);
            Assert.AreEqual(infoOrganizacionRequest.OrganizacionId, createdResult.RouteValues["id"]);
            Assert.AreEqual(infoOrganizacionRequest.OrganizacionId, (createdResult.Value as InfoOrganizationDto).OrganizacionId);
            Assert.AreEqual(infoOrganizacionRequest.Organizacion, (createdResult.Value as InfoOrganizationDto).Organizacion);
        }

        [Test]
        public async Task Update_WhenCalled_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var infoOrganizacionRequest = new InfoOrganizationRequest
            {
                OrganizacionId = 1,
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
            };

            var organizacion = new OrganizationDto
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

            _organizacionService.Setup(x => x.GetOrganizationByIdAsync(infoOrganizacionRequest.OrganizacionId))
                .ReturnsAsync(organizacion);

            _organizacionInfoService.Setup(x => x.UpdateInfoOrganizationAsync(It.IsAny<InfoOrganizationDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(infoOrganizacionRequest);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);

            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("Update", createdResult.ActionName);
            Assert.AreEqual(infoOrganizacionRequest.OrganizacionId, createdResult.RouteValues["id"]);

            var returnedValue = createdResult.Value as InfoOrganizationDto;
            Assert.IsNotNull(returnedValue);
            Assert.AreEqual(infoOrganizacionRequest.OrganizacionId, returnedValue.OrganizacionId);
            Assert.AreEqual(infoOrganizacionRequest.Organizacion, returnedValue.Organizacion);
            Assert.AreEqual(infoOrganizacionRequest.DescripcionBreve, returnedValue.DescripcionBreve);
            Assert.AreEqual(infoOrganizacionRequest.DescripcionCompleta, returnedValue.DescripcionCompleta);
            Assert.AreEqual("http://localhost:5203/images/test.jpg", returnedValue.Img);
        }

        [Test]
        public async Task Update_WhenCalled_ReturnsBadRequest()
        {
            // Arrange
            var infoOrganizacionRequest = new InfoOrganizationRequest
            {
                OrganizacionId = 1,
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
            };

            var organizacion = new OrganizationDto
            {
                Nombre = "Organizacion",
                Direccion = "Calle 123",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            _organizacionService.Setup(x => x.GetOrganizationByIdAsync(infoOrganizacionRequest.OrganizacionId))
                .ReturnsAsync(() => null);

            var infoOrganizacionDto = new InfoOrganizationDto
            {
                Organizacion = infoOrganizacionRequest.Organizacion,
                DescripcionBreve = infoOrganizacionRequest.DescripcionBreve,
                DescripcionCompleta = infoOrganizacionRequest.DescripcionCompleta,
                Img = infoOrganizacionRequest.ImageUrl,
                OrganizacionId = infoOrganizacionRequest.OrganizacionId
            };

            _organizacionInfoService.Setup(x => x.UpdateInfoOrganizationAsync(infoOrganizacionDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(infoOrganizacionRequest);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);

            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("Organización no encontrada", notFoundResult.Value);
        }
    }
}
