using backend.data.DataContext;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.servicios.test
{
    [TestFixture]
    public class UsuarioServiceTests
    {
        private Mock<ILogger<UsuarioService>> _loggerMock;

        private ApplicationDbContext _context;
        private UsuarioService _usuarioService;


        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<UsuarioService>>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _context.Usuarios.AddRange(
                new Usuario { Nombre = "test", Email = "test@test.com", RolId = 1 },
                new Usuario { /* ... properties ... */ }
            );

            _context.Roles.AddRange(
                new Rol { RolId = 1, Nombre = "admin" },
                new Rol { RolId = 2, Nombre = "usuario" }
            );

            _context.SaveChanges();
            _usuarioService = new UsuarioService(_context, _loggerMock.Object);
        }

        [Test]
        public void Constructor_WithAllDependencies_ShouldNotThrowException()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new UsuarioService(_context, _loggerMock.Object));
        }

        [Test]
        public void Constructor_WithNullDbContext_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new UsuarioService(null, _loggerMock.Object));
            Assert.That(ex.ParamName, Is.EqualTo("context"));
        }

        [Test]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new UsuarioService(_context, null));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public async Task GetAllUsuariosAsync_WhenUsersExist_ReturnsTransformedUsers()
        {
            // Act
            var result = await _usuarioService.GetAllUsuariosAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Count(), Is.EqualTo(2));
                Assert.That(result.All(u => u is not null));
            });
        }

        [Test]
        public async Task GetAllUsuariosAsync_ReturnsCorrectData()
        {
            // Act
            var result = await _usuarioService.GetAllUsuariosAsync();

            // Assert
            var user = result.FirstOrDefault();
            Assert.That(user, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(user.Nombre, Is.EqualTo("test")); 
                Assert.That(user.Email, Is.EqualTo("test@test.com"));
            });
        }

        [Test]
        public async Task GetAllUsuariosAsync_WhenDatabaseIsEmpty_ReturnsEmpty()
        {
            // Arrange
            _context.Usuarios.RemoveRange(_context.Usuarios);
            _context.SaveChanges();

            // Act
            var result = await _usuarioService.GetAllUsuariosAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetUsuarioByEmailAsync_NullOrWhitespaceEmail_ThrowsArgumentNullException()
        {
            var nullEmail = Assert.ThrowsAsync<ArgumentNullException>(() => _usuarioService.GetUsuarioByEmailAsync(null));
            Assert.Multiple(() =>
            {
                Assert.That(nullEmail.ParamName, Is.EqualTo("email"));
                Assert.That(nullEmail.Message, Does.Contain("El email no puede ser nulo o estar vacío."));
            });

            var whitespaceEmail = Assert.ThrowsAsync<ArgumentNullException>(() => _usuarioService.GetUsuarioByEmailAsync("  "));
            Assert.Multiple(() =>
            {
                Assert.That(whitespaceEmail.ParamName, Is.EqualTo("email"));
                Assert.That(whitespaceEmail.Message, Does.Contain("El email no puede ser nulo o estar vacío."));
            });
        }

        [Test]
        public async Task GetUsuarioByEmailAsync_UserExists_ReturnsUsuarioDto()
        {
            // Arrange
            var testEmail = "test@test.com";

            // Act
            var result = await _usuarioService.GetUsuarioByEmailAsync(testEmail);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Nombre, Is.EqualTo("test"));
                Assert.That(result.Email, Is.EqualTo(testEmail));
            });
        }

        [Test]
        public async Task GetUsuarioByEmailAsync_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var nonExistentEmail = "nonexistent@test.com";

            // Act
            var result = await _usuarioService.GetUsuarioByEmailAsync(nonExistentEmail);

            // Assert
            Assert.That(result, Is.Null);
        }


        [Test]
        public void CreateUsuarioAsync_NullUsuarioDto_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _usuarioService.CreateUsuarioAsync(null));
            Assert.Multiple(() =>
            {
                Assert.That(ex.ParamName, Is.EqualTo("usuarioDto"));
                Assert.That(ex.Message, Contains.Substring("El usuario proporcionado no puede ser nulo."));
            });
        }

        [Test]
        public async Task CreateUsuarioAsync_UserExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var usuarioDto = new UsuarioDto { Email = "test@test.com", Nombre = "New Name" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _usuarioService.CreateUsuarioAsync(usuarioDto));
            Assert.That(ex.Message, Contains.Substring("Ya existe un usuario con el email proporcionado."));
        }

        [Test]
        public async Task CreateUsuarioAsync_ValidUser_AddsUserSuccessfully()
        {
            // Arrange
            var usuarioDto = new UsuarioDto
            {
                Nombre = "John",
                Apellido = "Doe",
                Email = "john.doe@example.com",
                Password = "password123",
                Telefono = "1234567890",
                Direccion = "123 Main St",
                Localidad = "Town",
                Provincia = "State"
            };

            // Act
            await _usuarioService.CreateUsuarioAsync(usuarioDto);
            var createdUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == "john.doe@example.com");

            // Assert
            Assert.That(createdUser, Is.Not.Null);
            Assert.That(createdUser.Nombre, Is.EqualTo("John"));
        }

        [Test]
        public void UpdateUsuarioAsync_NullUsuarioDto_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _usuarioService.UpdateUsuarioAsync(null));
            Assert.Multiple(() =>
            {
                Assert.That(ex.ParamName, Is.EqualTo("usuarioDto"));
                Assert.That(ex.Message, Contains.Substring("El usuario proporcionado no puede ser nulo."));
            });
        }

        [Test]
        public void UpdateUsuarioAsync_UserDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var usuarioDto = new UsuarioDto { Email = "nonexistent@example.com" };

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _usuarioService.UpdateUsuarioAsync(usuarioDto));
            Assert.That(ex.Message, Contains.Substring("Usuario no encontrado para actualizar."));
        }

        [Test]
        public async Task UpdateUsuarioAsync_UserExists_UpdatesUserSuccessfully()
        {
            // Arrange
            var testEmail = "existing@example.com";
            _context.Usuarios.Add(new Usuario
            {
                Email = testEmail,
                Nombre = "Old Name",
                Apellido = "Old Surname",
                Telefono = "0000000000",
                Direccion = "Old Address",
                Localidad = "Old City",
                Provincia = "Old State",
                Contrasena = "OldPassword"
            });
            await _context.SaveChangesAsync();

            var usuarioDto = new UsuarioDto
            {
                Email = testEmail,
                Nombre = "New Name",
                Apellido = "New Surname",
                Telefono = "1234567890",
                Direccion = "New Address",
                Localidad = "New City",
                Provincia = "New State",
                Password = "NewPassword"
            };

            // Act
            await _usuarioService.UpdateUsuarioAsync(usuarioDto);
            var updatedUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == testEmail);

            // Assert
            Assert.That(updatedUser, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(updatedUser.Nombre, Is.EqualTo("New Name"));
                Assert.That(updatedUser.Apellido, Is.EqualTo("New Surname"));
                Assert.That(updatedUser.Telefono, Is.EqualTo("1234567890"));
                Assert.That(updatedUser.Direccion, Is.EqualTo("New Address"));
                Assert.That(updatedUser.Localidad, Is.EqualTo("New City"));
                Assert.That(updatedUser.Provincia, Is.EqualTo("New State"));
                Assert.That(updatedUser.Contrasena, Is.EqualTo("NewPassword"));
            });
        }

        [Test]
        public void DeleteUsuarioAsync_NullOrEmptyEmail_ThrowsArgumentNullException()
        {
            // Null email test
            var exNull = Assert.ThrowsAsync<ArgumentNullException>(() => _usuarioService.DeleteUsuarioAsync(null));
            Assert.That(exNull.ParamName, Is.EqualTo("email"));
            Assert.That(exNull.Message, Contains.Substring("El email proporcionado no puede ser nulo o estar vacío."));

            // Empty email test
            var exEmpty = Assert.ThrowsAsync<ArgumentNullException>(() => _usuarioService.DeleteUsuarioAsync(""));
            Assert.That(exEmpty.ParamName, Is.EqualTo("email"));
            Assert.That(exEmpty.Message, Contains.Substring("El email proporcionado no puede ser nulo o estar vacío."));
        }

        [Test]
        public void DeleteUsuarioAsync_UserDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var nonExistentEmail = "nonexistent@example.com";

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _usuarioService.DeleteUsuarioAsync(nonExistentEmail));
            Assert.That(ex.Message, Contains.Substring("Usuario no encontrado para eliminar."));
        }

        [Test]
        public async Task DeleteUsuarioAsync_UserExists_DeletesUserSuccessfully()
        {
            // Arrange
            var emailToDelete = "delete@example.com";
            _context.Usuarios.Add(new Usuario { Email = emailToDelete });
            await _context.SaveChangesAsync();

            // Act
            await _usuarioService.DeleteUsuarioAsync(emailToDelete);
            var deletedUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == emailToDelete);

            // Assert
            Assert.IsNull(deletedUser);
        }


        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
