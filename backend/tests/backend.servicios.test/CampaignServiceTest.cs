using AutoMapper;
using backend.api.Mappers;
using backend.data.Models;
using backend.repositories.interfaces;
using backend.servicios.DTOs;
using backend.servicios.Servicios;
using Microsoft.Extensions.Logging;
using Moq;

namespace backend.servicios.test
{
    [TestFixture]
    public class CampaignServiceTest
    {
        private Mock<IRepository<Campaign>> _campaignRepositoryMock;
        private Mock<ILogger<NeedService>> _loggerMock;
        private IMapper _mapper;
        private CampaignService _campaignService;

        [SetUp]
        public void SetUp()
        {
            _campaignRepositoryMock = new Mock<IRepository<Campaign>>();
            _loggerMock = new Mock<ILogger<NeedService>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NeedsProfile>();
            });

            _mapper = config.CreateMapper();
            _campaignService = new CampaignService(_campaignRepositoryMock.Object, _loggerMock.Object, _mapper);
        }

        [Test]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<NeedService>>();
            var mapper = new Mock<IMapper>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new CampaignService(null, loggerMock.Object, mapper.Object));
            Assert.That(ex.ParamName, Is.EqualTo("repository"));
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<Campaign>>();
            var mapper = new Mock<IMapper>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new CampaignService(repositoryMock.Object, null, mapper.Object));
            Assert.That(ex.ParamName, Is.EqualTo("fail"));
        }

        [Test]
        public void Constructor_WithNullMapper_ThrowsArgumentNullException()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<Campaign>>();
            var loggerMock = new Mock<ILogger<NeedService>>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new CampaignService(repositoryMock.Object, loggerMock.Object, null));
            Assert.That(ex.ParamName, Is.EqualTo("mapper"));
        }

        [Test]
        public async Task CreateCampaign_WithValidCampaign_CreatesCampaign()
        {
            // Arrange
            var campaignDto = new CampaignDto { Id = 1, Title = "Campaign1" };

            // Act
            await _campaignService.CreateCampaign(campaignDto);

            // Assert
            _campaignRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Campaign>()), Times.Once);
        }

        [Test]
        public void CreateCampaign_WithNullCampaign_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _campaignService.CreateCampaign(null));
            Assert.That(ex.ParamName, Is.EqualTo("campaign"));
        }

        [Test]
        public async Task DeleteCampaign_WithValidCampaignId_DeletesCampaign()
        {
            // Arrange
            var campaignId = 1;

            // Act
            await _campaignService.DeleteCampaign(campaignId);

            // Assert
            _campaignRepositoryMock.Verify(x => x.DeleteAsync(campaignId), Times.Once);
        }

        [Test]
        public async Task GetCampaignById_WithValidCampaignId_ReturnsCampaign()
        {
            // Arrange
            var campaign = new Campaign { Id = 1, Title = "Campaign1" };
            _campaignRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(campaign);

            // Act
            var result = await _campaignService.GetCampaignById(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("Campaign1"));
        }

        [Test]
        public async Task GetCampaignById_WithInvalidCampaignId_ReturnsNull()
        {
            // Arrange
            _campaignRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Campaign)null);

            // Act
            var result = await _campaignService.GetCampaignById(1);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCampaigns_WithValidOrganizationId_ReturnsCampaigns()
        {
            // Arrange
            var campaigns = new List<Campaign>
        {
            new Campaign { Id = 1, Title = "Campaign1", OrganizacionId = 1 },
            new Campaign { Id = 2, Title = "Campaign2", OrganizacionId = 1 }
        };
            _campaignRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(campaigns);

            // Act
            var result = await _campaignService.GetCampaigns(1);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task UpdateCampaign_WithValidCampaign_UpdatesCampaign()
        {
            // Arrange
            var campaignDto = new CampaignDto { Id = 1, Title = "UpdatedCampaign" };
            var campaign = new Campaign { Id = 1, Title = "Campaign1" };
            _campaignRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(campaign);

            // Act
            await _campaignService.UpdateCampaign(campaignDto);

            // Assert
            _campaignRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Campaign>()), Times.Once);
        }

        [Test]
        public void UpdateCampaign_WithNullCampaign_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _campaignService.UpdateCampaign(null));
            Assert.That(ex.ParamName, Is.EqualTo("campaign"));
        }

        [Test]
        public async Task UpdateCampaign_WithInvalidCampaignId_LogsWarning()
        {
            // Arrange
            var campaignDto = new CampaignDto { Id = 1, Title = "UpdatedCampaign" };
            _campaignRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Campaign)null);

            // Act
            await _campaignService.UpdateCampaign(campaignDto);

            // Assert
            _campaignRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Campaign>()), Times.Never);
        }
    }
}
