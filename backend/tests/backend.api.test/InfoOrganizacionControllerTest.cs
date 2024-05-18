using backend.api.Controllers;
using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using Newtonsoft.Json.Linq;

namespace backend.api.test
{
    [TestFixture]

    public class InfoOrganizacionControllerTest
    {
        private Mock<IOrganizacionService> _organizacionService;
        private Mock<IOrganizacionInfoService> _organizacionInfoService;
        private Mock<ILogger<UsuariosController>> _logger;
        private InfoOrganizacionController _controller;

        [SetUp]

        public void SetUp()
        {
            _organizacionService = new Mock<IOrganizacionService>();
            _organizacionInfoService = new Mock<IOrganizacionInfoService>();
            _logger = new Mock<ILogger<UsuariosController>>();
            _controller = new InfoOrganizacionController(_organizacionService.Object, _organizacionInfoService.Object, _logger.Object);
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
            var infoOrganizacionRequest = new InfoOrganizacionRequest
            {
                OrganizacionId = 1
            };


            _organizacionService.Setup(x => x.GetOrganizacionByIdAsync(infoOrganizacionRequest.OrganizacionId))
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
        public async Task Details_WhenCalled_ReturnsCreated()
        {
            // Arrange
            var infoOrganizacionRequest = new InfoOrganizacionRequest
            {
                OrganizacionId = 1,
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
                Img = "Img"
            };

            var organizacion = new OrganizacionDto
            {
                Nombre = "Organizacion",
                Direccion = "Calle 123",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            _organizacionService.Setup(x => x.GetOrganizacionByIdAsync(infoOrganizacionRequest.OrganizacionId))
                .ReturnsAsync(organizacion);

            var infoOrganizacionDto = new InfoOrganizacionDto
            {
                Organizacion = infoOrganizacionRequest.Organizacion,
                DescripcionBreve = infoOrganizacionRequest.DescripcionBreve,
                DescripcionCompleta = infoOrganizacionRequest.DescripcionCompleta,
                Img = infoOrganizacionRequest.Img,
                OrganizacionId = infoOrganizacionRequest.OrganizacionId
            };

            _organizacionInfoService.Setup(x => x.SaveDataInfoOrganizacion(infoOrganizacionDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Details(infoOrganizacionRequest);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);

            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("Details", createdResult.ActionName);
            Assert.AreEqual(1, createdResult.RouteValues["id"]);
            Assert.AreEqual(infoOrganizacionRequest.OrganizacionId, createdResult.RouteValues["id"]);
            Assert.AreEqual(infoOrganizacionRequest.OrganizacionId, (createdResult.Value as InfoOrganizacionDto).OrganizacionId);

            Assert.AreEqual(infoOrganizacionRequest.Organizacion, (createdResult.Value as InfoOrganizacionDto).Organizacion);
        }

        [Test]
        public async Task Update_WhenCalled_ReturnsOk() 
        {
            // Arrange
            var infoOrganizacionRequest = new InfoOrganizacionRequest
            {
                OrganizacionId = 1,
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
                Img = "Img"
            };

            var organizacion = new OrganizacionDto
            {
                Nombre = "Organizacion",
                Direccion = "Calle 123",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            _organizacionService.Setup(x => x.GetOrganizacionByIdAsync(infoOrganizacionRequest.OrganizacionId))
                .ReturnsAsync(organizacion);

            var infoOrganizacionDto = new InfoOrganizacionDto
            {
                Organizacion = infoOrganizacionRequest.Organizacion,
                DescripcionBreve = infoOrganizacionRequest.DescripcionBreve,
                DescripcionCompleta = infoOrganizacionRequest.DescripcionCompleta,
                Img = infoOrganizacionRequest.Img,
                OrganizacionId = infoOrganizacionRequest.OrganizacionId
            };

            _organizacionInfoService.Setup(x => x.UpdateInfoOrganizacionAsync(infoOrganizacionDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(infoOrganizacionRequest);

            // Assert

            Assert.IsInstanceOf<CreatedAtActionResult>(result);

            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);

            Assert.AreEqual("Update", createdResult.ActionName);
            Assert.AreEqual(1, createdResult.RouteValues["id"]);
            Assert.AreEqual(infoOrganizacionRequest.OrganizacionId, createdResult.RouteValues["id"]);

        }

        [Test]
        public async Task Update_WhenCalled_ReturnsBadRequest()
        {
            // Arrange
            var infoOrganizacionRequest = new InfoOrganizacionRequest
            {
                OrganizacionId = 1,
                Organizacion = "Organizacion",
                DescripcionBreve = "DescripcionBreve",
                DescripcionCompleta = "DescripcionCompleta",
                Img = "Img"
            };

            var organizacion = new OrganizacionDto
            {
                Nombre = "Organizacion",
                Direccion = "Calle 123",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            _organizacionService.Setup(x => x.GetOrganizacionByIdAsync(infoOrganizacionRequest.OrganizacionId))
                .ReturnsAsync(() => null);

            var infoOrganizacionDto = new InfoOrganizacionDto
            {
                Organizacion = infoOrganizacionRequest.Organizacion,
                DescripcionBreve = infoOrganizacionRequest.DescripcionBreve,
                DescripcionCompleta = infoOrganizacionRequest.DescripcionCompleta,
                Img = infoOrganizacionRequest.Img,
                OrganizacionId = infoOrganizacionRequest.OrganizacionId
            };

            _organizacionInfoService.Setup(x => x.UpdateInfoOrganizacionAsync(infoOrganizacionDto)).Returns(Task.CompletedTask);

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
