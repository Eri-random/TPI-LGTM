using System;
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
    public class OrganizationControllerTest
    {
        private Mock<IOrganizationService> _organizationServiceMock;
        private Mock<ILogger<UserController>> _loggerMock;
        private OrganizationController _controller;

        [SetUp]
        public void SetUp()
        {
            _organizationServiceMock = new Mock<IOrganizationService>();
            _loggerMock = new Mock<ILogger<UserController>>();
            _controller = new OrganizationController(_organizationServiceMock.Object, _loggerMock.Object);
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
        public async Task GetOrganizacionByCuit_ReturnsOkResult_WithOrganization()
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
            var result = await _controller.GetOrganizacionByCuit("12345678");

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<OrganizationResponseModel>());
            var returnValue = okResult.Value as OrganizationResponseModel;
            Assert.That(returnValue.Cuit, Is.EqualTo("12345678"));
        }

        [Test]
        public async Task GetOrganizacionByCuit_ReturnsNotFound_WhenOrganizationNotFound()
        {
            // Arrange
            _organizationServiceMock.Setup(service => service.GetOrganizationByCuitAsync("12345678")).ReturnsAsync((OrganizationDto)null);

            // Act
            var result = await _controller.GetOrganizacionByCuit("12345678");

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult.Value, Is.EqualTo("Organizacion no encontrada"));
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
            Assert.That(notFoundResult.Value, Is.EqualTo("Organizacion no encontrada"));
        }

        [Test]
        public async Task GetPaginatedOrganizationsAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _organizationServiceMock.Setup(service => service.GetPaginatedOrganizationsAsync(1, 8,null,null)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetPaginatedOrganizationsAsync(1, 8);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

    }
}
