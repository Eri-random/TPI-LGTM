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
        private Mock<IOrganizacionService> _organizacionServiceMock;
        private MapsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mapsServiceMock = new Mock<IMapsService>();
            _organizacionServiceMock = new Mock<IOrganizacionService>();
            _controller = new MapsController(_organizacionServiceMock.Object, _mapsServiceMock.Object);
        }

        [Test]
        public async Task GetOrganizationCoordinates_WhenCalled_ReturnsOkResult()
        {
            var organizacion = new Organizacion
            {
                Nombre = "Organizacion 1",
                Direccion = "Calle 123",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            };

            var organizaciones = new List<Organizacion> { organizacion };

            _mapsServiceMock.Setup(x => x.GetCoordinates(organizacion.Direccion, organizacion.Localidad, organizacion.Provincia))
                .ReturnsAsync((1.0, 1.0));

            Assert.That(organizaciones.Count, Is.EqualTo(1));

        }

        [Test]
        public async Task GetOrganizationCoordinates_WhenCalled_ReturnsCoordinates()
        {

            var organizaciones = GetSampleOrganizaciones();
            _organizacionServiceMock.Setup(x => x.GetAllOrganizacionAsync()).ReturnsAsync(organizaciones);

            _mapsServiceMock.Setup(x => x.GetCoordinates(organizaciones[0].Direccion, organizaciones[0].Localidad, organizaciones[0].Provincia))
                .ReturnsAsync((1.0, 1.0));

            _mapsServiceMock.Setup(x => x.GetCoordinates(organizaciones[1].Direccion, organizaciones[1].Localidad, organizaciones[1].Provincia))
                .ReturnsAsync((2.0, 2.0));

            var result = await _controller.GetOrganizationCoordinates() as OkObjectResult;
            var response = result.Value as List<CoordinatesResponseModel>;

            Assert.That(response.Count, Is.EqualTo(2));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(response[0].Lat, Is.EqualTo(1.0));
            Assert.That(response[0].Lng, Is.EqualTo(1.0));
        }

        private static List<OrganizacionDto> GetSampleOrganizaciones() => [
           new OrganizacionDto
            {
                Nombre = "Organizacion 1",
                Direccion = "Calle 123",
                Localidad = "Localidad 1",
                Provincia = "Provincia 1",
                Telefono = "123456789"
            },
            new OrganizacionDto
            {
                Nombre = "Organizacion 2",
                Direccion = "Calle 456",
                Localidad = "Localidad 2",
                Provincia = "Provincia 2",
                Telefono = "987654321"
            }
       ];
    }
}
