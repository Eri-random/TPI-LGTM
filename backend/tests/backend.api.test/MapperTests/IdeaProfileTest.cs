using AutoMapper;
using backend.api.Mappers;
using backend.api.Models;
using backend.data.Models;
using backend.servicios.DTOs;

namespace backend.api.test.MapperTests
{
    [TestFixture]
    public class IdeaProfileTest
    {
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<IdeaProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Test]
        public void Map_IdeaToIdeaDto_MapsCorrectly()
        {
            // Arrange
            var idea = new Idea
            {
                Id = 1,
                Titulo = "Idea1"
            };

            // Act
            var ideaDto = _mapper.Map<IdeaDto>(idea);

            // Assert
            Assert.That(ideaDto.Id, Is.EqualTo(idea.Id));
            Assert.That(ideaDto.Titulo, Is.EqualTo(idea.Titulo));
        }

        [Test]
        public void Map_IdeaDtoToIdea_MapsCorrectly()
        {
            // Arrange
            var ideaDto = new IdeaDto
            {
                Id = 1,
                Titulo = "Idea1"
            };

            // Act
            var idea = _mapper.Map<Idea>(ideaDto);

            // Assert
            Assert.That(idea.Id, Is.EqualTo(ideaDto.Id));
            Assert.That(idea.Titulo, Is.EqualTo(ideaDto.Titulo));
        }

        [Test]
        public void Map_PasoToStepDto_MapsCorrectly()
        {
            // Arrange
            var paso = new Paso
            {
                Id = 1,
                Descripcion = "Step1"
            };

            // Act
            var stepDto = _mapper.Map<StepDto>(paso);

            // Assert
            Assert.That(stepDto.Id, Is.EqualTo(paso.Id));
            Assert.That(stepDto.Descripcion, Is.EqualTo(paso.Descripcion));
        }

        [Test]
        public void Map_StepDtoToPaso_MapsCorrectly()
        {
            // Arrange
            var stepDto = new StepDto
            {
                Id = 1,
                Descripcion = "Step1"
                // Initialize other properties as needed
            };

            // Act
            var paso = _mapper.Map<Paso>(stepDto);

            // Assert
            Assert.That(paso.Id, Is.EqualTo(stepDto.Id));
            Assert.That(paso.Descripcion, Is.EqualTo(stepDto.Descripcion));
        }

        [Test]
        public void Map_IdeaDtoToIdeaResponseModel_MapsCorrectly()
        {
            // Arrange
            var ideaDto = new IdeaDto
            {
                Id = 1,
                Titulo = "Idea1"
            };

            // Act
            var ideaResponseModel = _mapper.Map<IdeaResponseModel>(ideaDto);

            // Assert
            Assert.That(ideaResponseModel.Id, Is.EqualTo(ideaDto.Id));
            Assert.That(ideaResponseModel.Titulo, Is.EqualTo(ideaDto.Titulo));
        }
    }
}
