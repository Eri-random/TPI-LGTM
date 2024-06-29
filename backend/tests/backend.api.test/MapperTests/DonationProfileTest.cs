using AutoMapper;
using backend.api.Mappers;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.test.MapperTests
{
    [TestFixture]
    public class DonationProfileTest
    {
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DonationProfile>();
                cfg.AddProfile<OrganizationProfile>();
                cfg.AddProfile<UserProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Test]
        public void Map_DonationDtoToDonacion_MapsCorrectly()
        {
            // Arrange
            var donationDto = new DonationDto
            {
                Id = 1,
                Cantidad = 1
            };

            // Act
            var donacion = _mapper.Map<Donacion>(donationDto);

            // Assert
            Assert.That(donacion.Id, Is.EqualTo(donationDto.Id));
            Assert.That(donacion.Cantidad, Is.EqualTo(donationDto.Cantidad));
        }

        [Test]
        public void Map_DonacionToDonationDto_MapsCorrectly()
        {
            // Arrange
            var donacion = new Donacion
            {
                Id = 1,
                Cantidad = 1
            };

            // Act
            var donationDto = _mapper.Map<DonationDto>(donacion);

            // Assert
            Assert.That(donationDto.Id, Is.EqualTo(donacion.Id));
            Assert.That(donationDto.Cantidad, Is.EqualTo(donacion.Cantidad));
        }

        [Test]
        public void Map_DonationRequestModelToDonationDto_MapsCorrectly()
        {
            // Arrange
            var donationRequestModel = new DonationRequestModel
            {
                Id = 1,
                Cantidad = 1
            };

            // Act
            var donationDto = _mapper.Map<DonationDto>(donationRequestModel);

            // Assert
            Assert.That(donationDto.Id, Is.EqualTo(donationRequestModel.Id));
            Assert.That(donationDto.Cantidad, Is.EqualTo(donationRequestModel.Cantidad));
        }

        [Test]
        public void Map_DonationDtoToDonationRequestModel_MapsCorrectly()
        {
            // Arrange
            var donationDto = new DonationDto
            {
                Id = 1,
                Cantidad = 1
            };

            // Act
            var donationRequestModel = _mapper.Map<DonationRequestModel>(donationDto);

            // Assert
            Assert.That(donationRequestModel.Id, Is.EqualTo(donationDto.Id));
            Assert.That(donationRequestModel.Cantidad, Is.EqualTo(donationDto.Cantidad));
        }

        [Test]
        public void Map_DonationDtoToDonationResponseModel_MapsCorrectly()
        {
            // Arrange
            var donationDto = new DonationDto
            {
                Id = 1,
                Cantidad = 1
            };

            // Act
            var donationResponseModel = _mapper.Map<DonationResponseModel>(donationDto);

            // Assert
            Assert.That(donationResponseModel.Id, Is.EqualTo(donationDto.Id));
            Assert.That(donationResponseModel.Cantidad, Is.EqualTo(donationDto.Cantidad));
        }

        [Test]
        public void Map_DonationRequestModelToDonationResponseModel_MapsCorrectly()
        {
            // Arrange
            var donationRequestModel = new DonationRequestModel
            {
                Id = 1,
                Cantidad = 1
            };

            // Act
            var donationDto = _mapper.Map<DonationDto>(donationRequestModel);
            var donationResponseModel = _mapper.Map<DonationResponseModel>(donationDto);

            // Assert
            Assert.That(donationResponseModel.Id, Is.EqualTo(donationRequestModel.Id));
            Assert.That(donationResponseModel.Cantidad, Is.EqualTo(donationRequestModel.Cantidad));
        }
    }
}
