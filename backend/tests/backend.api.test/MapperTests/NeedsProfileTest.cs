using AutoMapper;
using backend.api.Mappers;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.test.MapperTests
{
    [TestFixture]
    public class NeedsProfileTest
    {
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NeedsProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Test]
        public void Map_SubcategoriumToSubcategoriesDto_MapsCorrectly()
        {
            // Arrange
            var subcategorium = new Subcategorium
            {
                Id = 1,
                Nombre = "Subcategory1"
            };

            // Act
            var subcategoriesDto = _mapper.Map<SubcategoriesDto>(subcategorium);

            // Assert
            Assert.That(subcategoriesDto.Id, Is.EqualTo(subcategorium.Id));
            Assert.That(subcategoriesDto.Nombre, Is.EqualTo(subcategorium.Nombre));
        }

        [Test]
        public void Map_NecesidadToNeedDto_MapsCorrectly()
        {
            // Arrange
            var necesidad = new Necesidad
            {
                Id = 1,
                Nombre = "Need1"
            };

            // Act
            var needDto = _mapper.Map<NeedDto>(necesidad);

            // Assert
            Assert.That(needDto.Id, Is.EqualTo(necesidad.Id));
            Assert.That(needDto.Nombre, Is.EqualTo(necesidad.Nombre));
        }

        [Test]
        public void Map_NeedDtoToNecesidad_MapsCorrectly()
        {
            // Arrange
            var needDto = new NeedDto
            {
                Id = 1,
                Nombre = "Need1"
            };

            // Act
            var necesidad = _mapper.Map<Necesidad>(needDto);

            // Assert
            Assert.That(necesidad.Id, Is.EqualTo(needDto.Id));
            Assert.That(necesidad.Nombre, Is.EqualTo(needDto.Nombre));
        }

        [Test]
        public void Map_NeedDtoToNeedsResponseModel_MapsCorrectly()
        {
            // Arrange
            var needDto = new NeedDto
            {
                Id = 1,
                Nombre = "Need1"
            };

            // Act
            var needsResponseModel = _mapper.Map<NeedsResponseModel>(needDto);

            // Assert
            Assert.That(needsResponseModel.Id, Is.EqualTo(needDto.Id));
            Assert.That(needsResponseModel.Nombre, Is.EqualTo(needDto.Nombre));
        }

        [Test]
        public void Map_CampaignDtoToCampaign_MapsCorrectly()
        {
            // Arrange
            var campaignDto = new CampaignDto
            {
                Id = 1,
                Title = "Campaign1"
            };

            // Act
            var campaign = _mapper.Map<Campaign>(campaignDto);

            // Assert
            Assert.That(campaign.Id, Is.EqualTo(campaignDto.Id));
            Assert.That(campaign.Title, Is.EqualTo(campaignDto.Title));
        }

        [Test]
        public void Map_CampaignToCampaignDto_MapsCorrectly()
        {
            // Arrange
            var campaign = new Campaign
            {
                Id = 1,
                Title = "Campaign1"
            };

            // Act
            var campaignDto = _mapper.Map<CampaignDto>(campaign);

            // Assert
            Assert.That(campaignDto.Id, Is.EqualTo(campaign.Id));
            Assert.That(campaignDto.Title, Is.EqualTo(campaign.Title));
        }

        [Test]
        public void Map_CampaignDtoToCampaignResponseModel_MapsCorrectly()
        {
            // Arrange
            var campaignDto = new CampaignDto
            {
                Id = 1,
                Title = "Campaign1"
            };

            // Act
            var campaignResponseModel = _mapper.Map<CampaignResponseModel>(campaignDto);

            // Assert
            Assert.That(campaignResponseModel.Id, Is.EqualTo(campaignDto.Id));
            Assert.That(campaignResponseModel.Title, Is.EqualTo(campaignDto.Title));
        }

        [Test]
        public void Map_CampaignRequestModelToCampaignDto_MapsCorrectly()
        {
            // Arrange
            var campaignRequestModel = new CampaignRequestModel
            {
                Id = 1,
                Title = "Campaign1"
            };

            // Act
            var campaignDto = _mapper.Map<CampaignDto>(campaignRequestModel);

            // Assert
            Assert.That(campaignDto.Id, Is.EqualTo(campaignRequestModel.Id));
            Assert.That(campaignDto.Title, Is.EqualTo(campaignRequestModel.Title));
        }
    }
}
