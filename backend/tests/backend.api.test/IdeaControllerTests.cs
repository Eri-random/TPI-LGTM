using AutoMapper;
using backend.api.Controllers;
using backend.api.Mappers;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.servicios.Interfaces;
using backend.servicios.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace backend.api.test
{
    [TestFixture]
    public class IdeaControllerTests
    {
        private Mock<IGenerateIdeaApiService> _groqApiServiceMock;
        private Mock<IIdeaService> _ideaServiceMock;
        private Mock<ILogger<IdeaController>> _loggerMock;
        private Mock<IImageService> _imageServiceMock;
        private IMapper _mapper;
        private IdeaController _controller;

        [SetUp]
        public void SetUp()
        {
            _groqApiServiceMock = new Mock<IGenerateIdeaApiService>();
            _ideaServiceMock = new Mock<IIdeaService>();
            _loggerMock = new Mock<ILogger<IdeaController>>();
            _imageServiceMock = new Mock<IImageService>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new IdeaProfile());
            });

            _mapper = mappingConfig.CreateMapper();
            _controller = new IdeaController(_groqApiServiceMock.Object, _loggerMock.Object, _ideaServiceMock.Object, _imageServiceMock.Object, _mapper);
        }

        [Test]
        public async Task GenerateIdea_ReturnsBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = await _controller.GenerateIdea(null);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("Invalid request payload"));
        }

        [Test]
        public async Task GenerateIdea_ReturnsBadRequest_WhenMessageIsEmpty()
        {
            // Arrange
            var request = new GenerateIdeaRequestModel { Message = string.Empty };

            // Act
            var result = await _controller.GenerateIdea(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("Invalid request payload"));
        }

        [Test]
        public async Task GenerateIdea_ReturnsBadRequest_WhenNoIdeaGenerated()
        {
            // Arrange
            var request = new GenerateIdeaRequestModel { Message = "Test message" };
            var response = new ChatResponseModel
            {
                Choices = [new Choice { Message = new Message { Content = string.Empty } }]
            };
            var responseContent = JsonConvert.SerializeObject(response);

            _groqApiServiceMock.Setup(service => service.GenerateIdea(It.IsAny<string>()))
                .ReturnsAsync(responseContent);

            // Act
            var result = await _controller.GenerateIdea(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("No idea generated."));
        }

        [Test]
        public async Task GenerateIdea_ReturnsOk_WhenIdeaIsGenerated()
        {
            // Arrange
            var request = new GenerateIdeaRequestModel { Message = "Test message" };
            var response = new ChatResponseModel
            {
                Choices = new List<Choice> { new Choice { Message = new Message { Content = "{\"Idea\":\"Generated idea\",\"Pasos\":[\"Step 1\",\"Step 2\"]}" } } },
                Usage = new Usage { TotalTokens = 100, TotalTime = 1.2 }
            };
            var responseContent = JsonConvert.SerializeObject(response);

            _groqApiServiceMock.Setup(service => service.GenerateIdea(It.IsAny<string>()))
                .ReturnsAsync(responseContent);

            // Act
            var result = await _controller.GenerateIdea(request);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var ideaResponse = okResult.Value as GenerateIdeaResponseModel;

            Assert.That(ideaResponse, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(ideaResponse.Idea, Is.EqualTo("Generated idea"));
                Assert.That(ideaResponse.Steps, Has.Length.EqualTo(2));
            });
            Assert.Multiple(() =>
            {
                Assert.That(ideaResponse.Steps[0], Is.EqualTo("Step 1"));
                Assert.That(ideaResponse.Steps[1], Is.EqualTo("Step 2"));
            });
        }

        [Test]
        public async Task GenerateIdea_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new GenerateIdeaRequestModel { Message = "Test message" };
            _groqApiServiceMock.Setup(service => service.GenerateIdea(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GenerateIdea(request);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.Multiple(() =>
            {
                Assert.That(objectResult.StatusCode, Is.EqualTo(500));
                Assert.That(objectResult.Value, Is.EqualTo("Error when generating idea"));
            });
        }
    }
}
