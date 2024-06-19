using AutoMapper;
using backend.api.Mappers;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.test.MapperTests
{
    [TestFixture]
    public class UserProfileTest
    {
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Test]
        public void Map_UsuarioToUserDto_MapsCorrectly()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Nombre = "User1",
                Rol = new Rol { Nombre = "Admin" }
            };

            // Act
            var userDto = _mapper.Map<UserDto>(usuario);

            // Assert
            Assert.That(userDto.Id, Is.EqualTo(usuario.Id));
            Assert.That(userDto.Nombre, Is.EqualTo(usuario.Nombre));
            Assert.That(userDto.RolNombre, Is.EqualTo(usuario.Rol.Nombre));
        }

        [Test]
        public void Map_UserDtoToUsuario_MapsCorrectly()
        {
            // Arrange
            var userDto = new UserDto
            {
                Id = 1,
                Nombre = "User1",
                RolNombre = "Admin"
            };

            // Act
            var usuario = _mapper.Map<Usuario>(userDto);

            // Assert
            Assert.That(usuario.Id, Is.EqualTo(userDto.Id));
            Assert.That(usuario.Nombre, Is.EqualTo(userDto.Nombre));
        }

        [Test]
        public void Map_UserDtoToUserResponseModel_MapsCorrectly()
        {
            // Arrange
            var userDto = new UserDto
            {
                Id = 1,
                Nombre = "User1"
            };

            // Act
            var userResponseModel = _mapper.Map<UserResponseModel>(userDto);

            // Assert
            Assert.That(userResponseModel.Id, Is.EqualTo(userDto.Id));
            Assert.That(userResponseModel.Nombre, Is.EqualTo(userDto.Nombre));
        }

        [Test]
        public void Map_UserRequestModelToUserDto_MapsCorrectly()
        {
            // Arrange
            var userRequestModel = new UserRequestModel
            {
                Direccion = "Test",
                Nombre = "User1"
            };

            // Act
            var userDto = _mapper.Map<UserDto>(userRequestModel);

            // Assert
            Assert.That(userDto.Direccion, Is.EqualTo(userRequestModel.Direccion));
            Assert.That(userDto.Nombre, Is.EqualTo(userRequestModel.Nombre));
        }
    }
}
