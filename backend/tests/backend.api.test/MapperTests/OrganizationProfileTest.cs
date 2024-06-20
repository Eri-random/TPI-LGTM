using AutoMapper;
using backend.api.Mappers;
using backend.api.Models;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.test.MapperTests
{
    [TestFixture]
    public class OrganizationProfileTest
    {
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OrganizationProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Test]
        public void Map_OrganizacionToOrganizationDto_MapsCorrectly()
        {
            // Arrange
            var organizacion = new Organizacion
            {
                Id = 1,
                Nombre = "Organization1"
            };

            // Act
            var organizationDto = _mapper.Map<OrganizationDto>(organizacion);

            // Assert
            Assert.That(organizationDto.Id, Is.EqualTo(organizacion.Id));
            Assert.That(organizationDto.Nombre, Is.EqualTo(organizacion.Nombre));
        }

        [Test]
        public void Map_InfoOrganizacionToInfoOrganizationDto_MapsCorrectly()
        {
            // Arrange
            var infoOrganizacion = new InfoOrganizacion
            {
                Id = 1,
                DescripcionBreve = "Info1"
            };

            // Act
            var infoOrganizationDto = _mapper.Map<InfoOrganizationDto>(infoOrganizacion);

            // Assert
            Assert.That(infoOrganizationDto.OrganizacionId, Is.EqualTo(infoOrganizacion.Id));
            Assert.That(infoOrganizationDto.DescripcionBreve, Is.EqualTo(infoOrganizacion.DescripcionBreve));
        }

        [Test]
        public void Map_InfoOrganizationDtoToInfoOrganizacion_MapsCorrectly()
        {
            // Arrange
            var infoOrganizationDto = new InfoOrganizationDto
            {
                OrganizacionId = 1,
                DescripcionBreve = "Info1"
            };

            // Act
            var infoOrganizacion = _mapper.Map<InfoOrganizacion>(infoOrganizationDto);

            // Assert
            Assert.That(infoOrganizacion.Id, Is.EqualTo(infoOrganizationDto.OrganizacionId));
            Assert.That(infoOrganizacion.DescripcionBreve, Is.EqualTo(infoOrganizationDto.DescripcionBreve));
        }

        [Test]
        public void Map_OrganizationDtoToOrganizacion_MapsCorrectly()
        {
            // Arrange
            var organizationDto = new OrganizationDto
            {
                Id = 1,
                Nombre = "Organization1"
            };

            // Act
            var organizacion = _mapper.Map<Organizacion>(organizationDto);

            // Assert
            Assert.That(organizacion.Id, Is.EqualTo(organizationDto.Id));
            Assert.That(organizacion.Nombre, Is.EqualTo(organizationDto.Nombre));
        }

        [Test]
        public void Map_OrganizationDtoToOrganizationResponseModel_MapsCorrectly()
        {
            // Arrange
            var organizationDto = new OrganizationDto
            {
                Id = 1,
                Nombre = "Organization1"
            };

            // Act
            var organizationResponseModel = _mapper.Map<OrganizationResponseModel>(organizationDto);

            // Assert
            Assert.That(organizationResponseModel.Id, Is.EqualTo(organizationDto.Id));
            Assert.That(organizationResponseModel.Nombre, Is.EqualTo(organizationDto.Nombre));
        }

        [Test]
        public void Map_OrganizationRequestModelToOrganizationDto_MapsCorrectly()
        {
            // Arrange
            var organizationRequestModel = new OrganizationRequestModel
            {
                Id = 1,
                Nombre = "Organization1"
            };

            // Act
            var organizationDto = _mapper.Map<OrganizationDto>(organizationRequestModel);

            // Assert
            Assert.That(organizationDto.Id, Is.EqualTo(organizationRequestModel.Id));
            Assert.That(organizationDto.Nombre, Is.EqualTo(organizationRequestModel.Nombre));
        }

        [Test]
        public void Map_InfoOrganizationRequestToInfoOrganizationDto_MapsCorrectly()
        {
            // Arrange
            var infoOrganizationRequest = new InfoOrganizationRequestModel
            {
                OrganizacionId = 1,
                DescripcionBreve = "Info1"
            };

            // Act
            var infoOrganizationDto = _mapper.Map<InfoOrganizationDto>(infoOrganizationRequest);

            // Assert
            Assert.That(infoOrganizationDto.OrganizacionId, Is.EqualTo(infoOrganizationRequest.OrganizacionId));
            Assert.That(infoOrganizationDto.DescripcionBreve, Is.EqualTo(infoOrganizationRequest.DescripcionBreve));
        }

        [Test]
        public void Map_HeadquartersDtoToSede_MapsCorrectly()
        {
            // Arrange
            var headquartersDto = new HeadquartersDto
            {
                Id = 1,
                Nombre = "Headquarters1"
            };

            // Act
            var sede = _mapper.Map<Sede>(headquartersDto);

            // Assert
            Assert.That(sede.Id, Is.EqualTo(headquartersDto.Id));
            Assert.That(sede.Nombre, Is.EqualTo(headquartersDto.Nombre));
        }

        [Test]
        public void Map_SedeToHeadquartersDto_MapsCorrectly()
        {
            // Arrange
            var sede = new Sede
            {
                Id = 1,
                Nombre = "Headquarters1"
            };

            // Act
            var headquartersDto = _mapper.Map<HeadquartersDto>(sede);

            // Assert
            Assert.That(headquartersDto.Id, Is.EqualTo(sede.Id));
            Assert.That(headquartersDto.Nombre, Is.EqualTo(sede.Nombre));
        }

        [Test]
        public void Map_HeadquartersResponseModelToHeadquartersDto_MapsCorrectly()
        {
            // Arrange
            var headquartersResponseModel = new HeadquartersResponseModel
            {
                Id = 1,
                Nombre = "Headquarters1"
            };

            // Act
            var headquartersDto = _mapper.Map<HeadquartersDto>(headquartersResponseModel);

            // Assert
            Assert.That(headquartersDto.Id, Is.EqualTo(headquartersResponseModel.Id));
            Assert.That(headquartersDto.Nombre, Is.EqualTo(headquartersResponseModel.Nombre));
        }

        [Test]
        public void Map_HeadquartersDtoToHeadquartersRequestModel_MapsCorrectly()
        {
            // Arrange
            var headquartersDto = new HeadquartersDto
            {
                Id = 1,
                Nombre = "Headquarters1"
            };

            // Act
            var headquartersRequestModel = _mapper.Map<HeadquartersRequestModel>(headquartersDto);

            // Assert
            Assert.That(headquartersRequestModel.Id, Is.EqualTo(headquartersDto.Id));
            Assert.That(headquartersRequestModel.Nombre, Is.EqualTo(headquartersDto.Nombre));
        }

        [Test]
        public void Map_HeadquartersRequestModelToHeadquartersDto_MapsCorrectly()
        {
            // Arrange
            var headquartersRequestModel = new HeadquartersRequestModel
            {
                Id = 1,
                Nombre = "Headquarters1"
            };

            // Act
            var headquartersDto = _mapper.Map<HeadquartersDto>(headquartersRequestModel);

            // Assert
            Assert.That(headquartersDto.Id, Is.EqualTo(headquartersRequestModel.Id));
            Assert.That(headquartersDto.Nombre, Is.EqualTo(headquartersRequestModel.Nombre));
        }

        [Test]
        public void Map_HeadquartersDtoToHeadquartersResponseModel_MapsCorrectly()
        {
            // Arrange
            var headquartersDto = new HeadquartersDto
            {
                Id = 1,
                Nombre = "Headquarters1"
            };

            // Act
            var headquartersResponseModel = _mapper.Map<HeadquartersResponseModel>(headquartersDto);

            // Assert
            Assert.That(headquartersResponseModel.Id, Is.EqualTo(headquartersDto.Id));
            Assert.That(headquartersResponseModel.Nombre, Is.EqualTo(headquartersDto.Nombre));
        }

        [Test]
        public void Map_OrganizacionToHeadquartersNearby_MapsCorrectly()
        {
            // Arrange
            var organizacion = new Organizacion
            {
                Id = 1,
                Nombre = "Organization1"
            };

            // Act
            var headquartersNearby = _mapper.Map<HeadquartersNearbyDto>(organizacion);

            // Assert
            Assert.That(headquartersNearby.Id, Is.EqualTo(organizacion.Id));
            Assert.That(headquartersNearby.Nombre, Is.EqualTo(organizacion.Nombre));
        }
    }
}
