using backend.api.Controllers;
using backend.api.Models;
using backend.servicios.DTOs;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.api.test
{
    [TestFixture]
    public class UsuariosControllerTests
    {
        private Mock<IUsuarioService> _usuarioServiceMock;
        private Mock<ILogger<UsuariosController>> _loggerMock;
        private UsuariosController _controller;

        [SetUp]
        public void SetUp()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _loggerMock = new Mock<ILogger<UsuariosController>>();
            _controller = new UsuariosController(_usuarioServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public void Constructor_WithValidArguments_Succeeds()
        {
            // Arrange
            var usuarioServiceMock = new Mock<IUsuarioService>();
            var loggerMock = new Mock<ILogger<UsuariosController>>();

            // Act & Assert
            Assert.DoesNotThrow(() => new UsuariosController(usuarioServiceMock.Object, loggerMock.Object));
        }

        [Test]
        public void Constructor_WithNullUsuarioService_ThrowsArgumentNullException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<UsuariosController>>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new UsuariosController(null, loggerMock.Object));
            Assert.That(ex.ParamName, Is.EqualTo("usuarioService"));
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var usuarioServiceMock = new Mock<IUsuarioService>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new UsuariosController(usuarioServiceMock.Object, null));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public async Task GetAllUsuarios_ReturnsAllUsuarios_WhenUsersExist()
        {
            // Arrange
            var usuarios = GetSampleUsuarios();
            _usuarioServiceMock.Setup(s => s.GetAllUsuariosAsync()).ReturnsAsync(usuarios);

            // Act
            var result = await _controller.GetAllUsuarios();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var returnedUsuarios = okResult.Value as List<UsuarioResponseModel>;
            Assert.That(returnedUsuarios, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(returnedUsuarios[0].Nombre, Is.EqualTo(usuarios[0].Nombre));
                Assert.That(returnedUsuarios[1].Nombre, Is.EqualTo(usuarios[1].Nombre));
            });
        }

        [Test]
        public async Task GetAllUsuarios_ReturnsEmptyList_WhenNoUsersExist()
        {
            // Arrange
            _usuarioServiceMock.Setup(s => s.GetAllUsuariosAsync()).ReturnsAsync([]);

            // Act
            var result = await _controller.GetAllUsuarios();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var returnedUsuarios = okResult.Value as List<UsuarioResponseModel>;
            Assert.That(returnedUsuarios, Is.Empty);
        }

        [Test]
        public async Task GetAllUsuarios_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var exception = new Exception("Database error");
            _usuarioServiceMock.Setup(s => s.GetAllUsuariosAsync()).ThrowsAsync(exception);

            // Act
            var result = await _controller.GetAllUsuarios();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500));


            _loggerMock.Verify(
                    x => x.Log(
                        It.Is<LogLevel>(l => l == LogLevel.Error),
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => true),
                        It.IsAny<Exception>(),
                        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Test]
        public async Task GetUsuarioByEmail_UserExists_ReturnsUser()
        {
            // Arrange
            string email = "test@example.com";
            var mockUsuario = new UsuarioDto { Id = 1, Nombre = "Test", Apellido = "User", Email = email, Telefono = "1234567890", Rol = 1, Provincia = "SomeProvince", Localidad = "SomeCity", Direccion = "123 Test St" };
            _usuarioServiceMock.Setup(x => x.GetUsuarioByEmailAsync(email)).ReturnsAsync(mockUsuario);

            // Act
            var result = await _controller.GetUsuarioByEmail(email);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var usuarioResponse = okResult.Value as UsuarioResponseModel;
            Assert.IsNotNull(usuarioResponse);
            Assert.That(usuarioResponse.Email, Is.EqualTo(email));
            Assert.That(usuarioResponse.Nombre, Is.EqualTo("Test"));
        }

        [Test]
        public async Task GetUsuarioByEmail_UserNotExists_ReturnsNotFound()
        {
            // Arrange
            string email = "nonexistent@example.com";
            _usuarioServiceMock.Setup(x => x.GetUsuarioByEmailAsync(email)).ReturnsAsync((UsuarioDto)null);

            // Act
            var result = await _controller.GetUsuarioByEmail(email);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetUsuarioByEmail_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            string email = "error@example.com";
            _usuarioServiceMock.Setup(x => x.GetUsuarioByEmailAsync(email)).ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetUsuarioByEmail(email);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500));
            _loggerMock.Verify(
                    x => x.Log(
                        It.Is<LogLevel>(l => l == LogLevel.Error),
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => true),
                        It.IsAny<Exception>(),
                        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Test]
        public async Task CreateUsuario_NullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.CreateUsuario(null);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Datos de usuario inválidos"));
        }

        [Test]
        public async Task CreateUsuario_ValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var usuarioRequest = new UsuarioRequestModel
            {
                Nombre = "John",
                Apellido = "Doe",
                Email = "john@example.com",
                Telefono = "1234567890",
                Direccion = "123 Main St",
                Localidad = "Townsville",
                Provincia = "State",
                Password = "securePassword123"
            };
            _usuarioServiceMock.Setup(x => x.CreateUsuarioAsync(It.IsAny<UsuarioDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateUsuario(usuarioRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.Multiple(() =>
            {
                Assert.That(createdResult?.ActionName, Is.EqualTo(nameof(_controller.CreateUsuario)));
                Assert.That(createdResult?.RouteValues?["email"], Is.EqualTo("john@example.com"));
                Assert.That(createdResult?.Value, Is.EqualTo(usuarioRequest));
            });
        }

        [Test]
        public async Task CreateUsuario_ThrowsInvalidOperationException_ReturnsBadRequest()
        {
            // Arrange
            var usuarioRequest = new UsuarioRequestModel();
            _usuarioServiceMock.Setup(x => x.CreateUsuarioAsync(It.IsAny<UsuarioDto>()))
                               .ThrowsAsync(new InvalidOperationException("Duplicate email"));

            // Act
            var result = await _controller.CreateUsuario(usuarioRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Duplicate email"));
        }

        [Test]
        public async Task CreateUsuario_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var usuarioRequest = new UsuarioRequestModel();
            _usuarioServiceMock.Setup(x => x.CreateUsuarioAsync(It.IsAny<UsuarioDto>()))
                               .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.CreateUsuario(usuarioRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.Multiple(() =>
            {
                Assert.That(objectResult?.StatusCode, Is.EqualTo(500));
                Assert.That(objectResult?.Value, Is.EqualTo("Internal server error"));
            });
        }

        [Test]
        public async Task UpdateUsuario_NullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UpdateUsuario(null);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Datos de usuario inválidos"));
        }

        [Test]
        public async Task UpdateUsuario_ValidRequest_ReturnsNoContent()
        {
            // Arrange
            var usuarioRequest = new UsuarioRequestModel
            {
                Nombre = "John",
                Apellido = "Doe",
                Email = "john@example.com",
                Telefono = "1234567890",
                Direccion = "123 Main St",
                Localidad = "Townsville",
                Provincia = "State",
                Password = "securePassword123",
                RolId = 1
            };
            _usuarioServiceMock.Setup(x => x.UpdateUsuarioAsync(It.IsAny<UsuarioDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateUsuario(usuarioRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task UpdateUsuario_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var usuarioRequest = new UsuarioRequestModel
            {
                Nombre = "John",
                Apellido = "Doe",
                Email = "john@example.com",
                Telefono = "1234567890",
                Direccion = "123 Main St",
                Localidad = "Townsville",
                Provincia = "State",
                Password = "securePassword123",
                RolId = 1
            };
            _usuarioServiceMock.Setup(x => x.UpdateUsuarioAsync(It.IsAny<UsuarioDto>()))
                               .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.UpdateUsuario(usuarioRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.Multiple(() =>
            {
                Assert.That(objectResult?.StatusCode, Is.EqualTo(500));
                Assert.That(objectResult?.Value, Is.EqualTo("Internal server error"));
            });
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Test]
        public async Task DeleteUsuario_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _usuarioServiceMock.Setup(x => x.GetUsuarioByEmailAsync(email)).ReturnsAsync(() => null);

            // Act
            var result = await _controller.DeleteUsuario(email);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("Usuario a eliminar no encontrado"));
        }

        [Test]
        public async Task DeleteUsuario_ValidUser_ReturnsNoContent()
        {
            // Arrange
            var email = "existing@example.com";
            var usuario = new UsuarioDto { Email = email };
            _usuarioServiceMock.Setup(x => x.GetUsuarioByEmailAsync(email)).ReturnsAsync(usuario);
            _usuarioServiceMock.Setup(x => x.DeleteUsuarioAsync(email)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteUsuario(email);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteUsuario_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var email = "error@example.com";
            var usuario = new UsuarioDto { Email = email };
            _usuarioServiceMock.Setup(x => x.GetUsuarioByEmailAsync(email)).ReturnsAsync(usuario);
            _usuarioServiceMock.Setup(x => x.DeleteUsuarioAsync(email)).ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.DeleteUsuario(email);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.Multiple(() =>
            {
                Assert.That(objectResult?.StatusCode, Is.EqualTo(500));
                Assert.That(objectResult?.Value, Is.EqualTo("Internal server error"));
            });
            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }


        private static List<UsuarioDto> GetSampleUsuarios() => [
                    new UsuarioDto { Id = 1, Nombre = "Alice", Apellido = "Johnson", Email = "alice@example.com", Telefono = "1234567890" },
                    new UsuarioDto { Id = 2, Nombre = "Bob", Apellido = "Smith", Email = "bob@example.com", Telefono = "0987654321" }
                ];

    }
}
