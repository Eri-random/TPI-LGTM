using backend.api.Controllers;
using backend.api.Models;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using Newtonsoft.Json.Linq;


namespace backend.api.test
{
    [TestFixture]
    public class MapsControllerTest
    {
        private Mock<IMapsService> _mapsServiceMock;
        private Mock<IUsuarioService> _usuarioServiceMock;
        private MapsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mapsServiceMock = new Mock<IMapsService>();
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _controller = new MapsController(_usuarioServiceMock.Object, _mapsServiceMock.Object);
        }

        [Test]
        public async Task GetOrganizationCoordinates_WhenCalled_ReturnsOkResult()
        {
            var usuario = new Usuario
            {
                Id = 1,
                Nombre = "Organizacion 1",
                Direccion = "Calle 123",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789",
                Rol = new Rol { Nombre = "organizacion" }
            };

            var usuarios = new List<Usuario> { usuario };

            _mapsServiceMock.Setup(x => x.GetCoordinates(usuario.Direccion, usuario.Localidad, usuario.Provincia))
                .ReturnsAsync((1.0, 1.0));

            Assert.That(await _controller.GetOrganizationCoordinates(), Is.TypeOf<OkObjectResult>());

        }

        [Test]
        public async Task GetOrganizationCoordinates_WhenCalled_ReturnsCoordinates()
        {
            var usuarios = GetSampleUsuarios();
            _usuarioServiceMock.Setup(x => x.GetAllUsuariosAsync()).ReturnsAsync(usuarios);

            _mapsServiceMock.Setup(x => x.GetCoordinates(usuarios[0].Direccion, usuarios[0].Localidad, usuarios[0].Provincia))
                .ReturnsAsync((1.0, 1.0));

            _mapsServiceMock.Setup(x => x.GetCoordinates(usuarios[1].Direccion, usuarios[1].Localidad, usuarios[1].Provincia))
                .ReturnsAsync((2.0, 2.0));


            var result = await _controller.GetOrganizationCoordinates() as OkObjectResult;
            var response = result.Value as List<CoordinatesResponseModel>;

            Assert.That(response.Count, Is.EqualTo(2));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(response[0].Lat, Is.EqualTo(1.0));
            Assert.That(response[0].Lng, Is.EqualTo(1.0));
        }

        private static List<UsuarioDto> GetSampleUsuarios() => [
            new UsuarioDto
            {
                Id = 1,
                Nombre = "Organizacion 1",
                Direccion = "Calle 123",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789",
                RolNombre = "organizacion"
            },
            new UsuarioDto
            {
                Id = 2,
                Nombre = "Organizacion 2",
                Direccion = "Calle 456",
                Localidad = "Localidad 2",
                Provincia = "Provincia 2",
                Telefono = "987654321",
                RolNombre = "organizacion"
            }
        ];
    }
}
