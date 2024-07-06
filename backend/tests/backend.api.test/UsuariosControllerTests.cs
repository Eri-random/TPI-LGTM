using AutoMapper;
using backend.api.Controllers;
using backend.api.Mappers;
using backend.api.Models;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.api.test
{
    [TestFixture]
    public class UsuariosControllerTests
    {
        private Mock<IUserService> _usuarioServiceMock;
        private Mock<ILogger<UserController>> _loggerMock;
        private IMapper _mapper;
        private UserController _controller;

        [SetUp]
        public void SetUp()
        {
            _usuarioServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<UserController>>();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UserProfile());
            });
            _mapper = mappingConfig.CreateMapper();
            _controller = new UserController(_usuarioServiceMock.Object, _loggerMock.Object, _mapper);
        }

        [Test]
        public void Constructor_WithValidArguments_Succeeds()
        {
            // Arrange
            var usuarioServiceMock = new Mock<IUserService>();
            var loggerMock = new Mock<ILogger<UserController>>();

            // Act & Assert
            Assert.DoesNotThrow(() => new UserController(usuarioServiceMock.Object, loggerMock.Object, _mapper));
        }

        [Test]
        public void Constructor_WithNullUsuarioService_ThrowsArgumentNullException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<UserController>>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new UserController(null, loggerMock.Object, _mapper));
            Assert.That(ex.ParamName, Is.EqualTo("userService"));
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var usuarioServiceMock = new Mock<IUserService>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new UserController(usuarioServiceMock.Object, null, _mapper));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public async Task GetAllUsuarios_ReturnsAllUsuarios_WhenUsersExist()
        {
            // Arrange
            var usuarios = GetSampleUsuarios();
            _usuarioServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(usuarios);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var returnedUsuarios = okResult.Value as List<UserResponseModel>;
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
            _usuarioServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync([]);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var returnedUsuarios = okResult.Value as List<UserResponseModel>;
            Assert.That(returnedUsuarios, Is.Empty);
        }

        [Test]
        public async Task GetAllUsuarios_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var exception = new Exception("Database error");
            _usuarioServiceMock.Setup(s => s.GetAllUsersAsync()).ThrowsAsync(exception);

            // Act
            var result = await _controller.GetAllUsers();

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
            var mockUsuario = new UserDto { Id = 1, Nombre = "Test", Apellido = "User", Email = email, Telefono = "1234567890", RolId = 1, Provincia = "SomeProvince", Localidad = "SomeCity", Direccion = "123 Test St" };
            _usuarioServiceMock.Setup(x => x.GetUserByEmailAsync(email)).ReturnsAsync(mockUsuario);

            // Act
            var result = await _controller.GetUserByEmail(email);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            var usuarioResponse = okResult.Value as UserResponseModel;
            Assert.IsNotNull(usuarioResponse);
            Assert.That(usuarioResponse.Email, Is.EqualTo(email));
            Assert.That(usuarioResponse.Nombre, Is.EqualTo("Test"));
        }

        [Test]
        public async Task GetUsuarioByEmail_UserNotExists_ReturnsNotFound()
        {
            // Arrange
            string email = "nonexistent@example.com";
            _usuarioServiceMock.Setup(x => x.GetUserByEmailAsync(email)).ReturnsAsync((UserDto)null);

            // Act
            var result = await _controller.GetUserByEmail(email);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetUsuarioByEmail_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            string email = "error@example.com";
            _usuarioServiceMock.Setup(x => x.GetUserByEmailAsync(email)).ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.GetUserByEmail(email);

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
        public async Task Authenticate_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var usuarioLogIn = new UserLogInRequestModel
            {
                Email = "john@example.com",
                Password = "securePassword123"
            };

            var hashedPassword = PasswordHasher.HashPassword(usuarioLogIn.Password);

            var mockUsuario = new UserDto { Id = 1, Nombre = "Test", Apellido = "User", Email = usuarioLogIn.Email, Password = hashedPassword, Telefono = "1234567890", RolId = 1,RolNombre = "usuario", Provincia = "SomeProvince", Localidad = "SomeCity", Direccion = "123 Test St" };
            _usuarioServiceMock.Setup(x => x.GetUserByEmailAsync(usuarioLogIn.Email)).ReturnsAsync(mockUsuario);

            // Act
            var result = await _controller.Authenticate(usuarioLogIn);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result); // Verifica que el resultado sea un OkObjectResult
            var okResult = result as OkObjectResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.AreEqual("Login exitoso", okResult.Value?.GetType().GetProperty("Message").GetValue(okResult.Value));
        }

        [Test]
        public async Task Authenticate_InvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var usuarioLogIn = new UserLogInRequestModel
            {
                Email = "john@example.com",
                Password = "wrongPassword"
            };

            var mockUsuario = (UserDto)null;
            _usuarioServiceMock.Setup(x => x.GetUserByEmailAsync(usuarioLogIn.Email)).ReturnsAsync(mockUsuario);

            // Act
            var result = await _controller.Authenticate(usuarioLogIn);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result); // Verifica que el resultado sea un BadRequestObjectResult
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("usuario y/o contraseña invalido")); // Verifica el mensaje de error
        }

        [Test]
        public async Task CreateUsuario_NullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.CreateUser(null);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid user data"));
        }

        [Test]
        public async Task CreateUsuario_ValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var usuarioRequest = new UserRequestModel
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

            _usuarioServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<UserDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateUser(usuarioRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.Multiple(() =>
            {
                Assert.That(createdResult?.ActionName, Is.EqualTo(nameof(_controller.CreateUser)));
                Assert.That(createdResult?.RouteValues?["email"], Is.EqualTo("john@example.com"));
            });
        }

        [Test]
        public async Task CreateUsuario_ThrowsInvalidOperationException_ReturnsBadRequest()
        {
            // Arrange
            var usuarioRequest = new UserRequestModel();
            _usuarioServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<UserDto>()))
                               .ThrowsAsync(new InvalidOperationException("Duplicate email"));

            // Act
            var result = await _controller.CreateUser(usuarioRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Duplicate email"));
        }

        [Test]
        public async Task CreateUsuario_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var usuarioRequest = new UserRequestModel();
            _usuarioServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<UserDto>()))
                               .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.CreateUser(usuarioRequest);

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
            var result = await _controller.UpdateUser(null);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("Invalid user data"));
        }

        [Test]
        public async Task UpdateUser_ReturnsOk_WhenUserIsUpdatedSuccessfully()
        {
            // Arrange
            var userRequest = new UserRequestModel
            {
                Nombre = "John",
                Apellido = "Doe",
                Telefono = "123456789",
                Direccion = "123 Street",
                Localidad = "City",
                Provincia = "Province",
                Email = "john.doe@example.com",
                Password = "password",
                RolId = 1
            };

            _usuarioServiceMock.Setup(service => service.UpdateUserAsync(It.IsAny<UserDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateUser(userRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task UpdateUsuario_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var usuarioRequest = new UserRequestModel
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
            _usuarioServiceMock.Setup(x => x.UpdateUserAsync(It.IsAny<UserDto>()))
                               .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.UpdateUser(usuarioRequest);

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
            _usuarioServiceMock.Setup(x => x.GetUserByEmailAsync(email)).ReturnsAsync(() => null);

            // Act
            var result = await _controller.DeleteUser(email);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.Value, Is.EqualTo("User to delete not found"));
        }

        [Test]
        public async Task DeleteUsuario_ValidUser_ReturnsNoContent()
        {
            // Arrange
            var email = "existing@example.com";
            var usuario = new UserDto { Email = email };
            _usuarioServiceMock.Setup(x => x.GetUserByEmailAsync(email)).ReturnsAsync(usuario);
            _usuarioServiceMock.Setup(x => x.DeleteUserAsync(email)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteUser(email);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteUsuario_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var email = "error@example.com";
            var usuario = new UserDto { Email = email };
            _usuarioServiceMock.Setup(x => x.GetUserByEmailAsync(email)).ReturnsAsync(usuario);
            _usuarioServiceMock.Setup(x => x.DeleteUserAsync(email)).ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await _controller.DeleteUser(email);

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

        private static List<UserDto> GetSampleUsuarios() => [
                    new UserDto { Id = 1, Nombre = "Alice", Apellido = "Johnson", Email = "alice@example.com", Telefono = "1234567890" },
                    new UserDto { Id = 2, Nombre = "Bob", Apellido = "Smith", Email = "bob@example.com", Telefono = "0987654321" }
                ];

    }
}
