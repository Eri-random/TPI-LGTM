using AutoMapper;
using backend.api.Controllers;
using backend.api.Mappers;
using backend.api.Models.RequestModels;
using backend.api.Models.ResponseModels;
using backend.servicios.Config;
using backend.servicios.DTOs;
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
            _controller = new IdeaController(_groqApiServiceMock.Object, _loggerMock.Object, _ideaServiceMock.Object, _imageServiceMock.Object, _mapper, new servicios.Config.OpenAiApiConfig());
        }

        [Test]
        public void Constructor_WithValidParameters_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => new IdeaController(_groqApiServiceMock.Object, _loggerMock.Object, _ideaServiceMock.Object, _imageServiceMock.Object, _mapper, new OpenAiApiConfig()));
        }

        [Test]
        public void Constructor_WithNullGroqApiService_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new IdeaController(null, _loggerMock.Object, _ideaServiceMock.Object, _imageServiceMock.Object, _mapper, new OpenAiApiConfig()));
            Assert.That(ex.ParamName, Is.EqualTo("groqApiService"));
        }

        [Test]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new IdeaController(_groqApiServiceMock.Object, null, _ideaServiceMock.Object, _imageServiceMock.Object, _mapper, new OpenAiApiConfig()));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }

        [Test]
        public void Constructor_WithNullIdeaService_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new IdeaController(_groqApiServiceMock.Object, _loggerMock.Object, null, _imageServiceMock.Object, _mapper, new OpenAiApiConfig()));
            Assert.That(ex.ParamName, Is.EqualTo("ideaService"));
        }

        [Test]
        public void Constructor_WithNullImageService_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new IdeaController(_groqApiServiceMock.Object, _loggerMock.Object, _ideaServiceMock.Object, null, _mapper, new OpenAiApiConfig()));
            Assert.That(ex.ParamName, Is.EqualTo("imageService"));
        }

        [Test]
        public void Constructor_WithNullMapper_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new IdeaController(_groqApiServiceMock.Object, _loggerMock.Object, _ideaServiceMock.Object, _imageServiceMock.Object, null, new OpenAiApiConfig()));
            Assert.That(ex.ParamName, Is.EqualTo("mapper"));
        }

        [Test]
        public void Constructor_WithNullConfig_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new IdeaController(_groqApiServiceMock.Object, _loggerMock.Object, _ideaServiceMock.Object, _imageServiceMock.Object, _mapper, null));
            Assert.That(ex.ParamName, Is.EqualTo("config"));
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
                Choices = new List<Choice> { new Choice { Message = new Message { Content = string.Empty } } }
            };
            var responseContent = JsonConvert.SerializeObject(response);

            _groqApiServiceMock.Setup(service => service.GenerateIdea(It.IsAny<string>())).ReturnsAsync(responseContent);

            // Act
            var result = await _controller.GenerateIdea(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("No idea generated."));
        }

        [Test]
        public async Task GenerateIdea_ReturnsCreated_WhenIdeaIsGenerated()
        {
            // Arrange
            var request = new GenerateIdeaRequestModel { Message = "Test message" };
            var response = new ChatResponseModel
            {
                Choices = new List<Choice> { new Choice { Message = new Message { Content = "{\"Idea\":\"Generated idea\",\"Pasos\":[\"Step 1\",\"Step 2\"]}" } } },
                Usage = new Usage { TotalTokens = 100, TotalTime = 1.2 }
            };
            var responseContent = JsonConvert.SerializeObject(response);

            _groqApiServiceMock.Setup(service => service.GenerateIdea(It.IsAny<string>())).ReturnsAsync(responseContent);

            // Act
            var result = await _controller.GenerateIdea(request);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var createdResult = result as ObjectResult;
            var ideaResponse = createdResult.Value as GenerateIdeaResponseModel;

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
            _groqApiServiceMock.Setup(service => service.GenerateIdea(It.IsAny<string>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GenerateIdea(request);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.Multiple(() =>
            {
                Assert.That(objectResult.StatusCode, Is.EqualTo(500));
                Assert.That(objectResult.Value, Is.EqualTo("Internal server error"));
            });
        }

        [Test]
        public async Task SaveIdea_ReturnsBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = await _controller.SaveIdea(null);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("Invalid request payload"));
        }

        [Test]
        public async Task SaveIdea_ReturnsCreated_WhenRequestIsValid()
        {
            // Arrange
            var ideaRequest = new IdeaRequestModel
            {
                UsuarioId = 1,
                Titulo = "Test Idea"
            };
            _ideaServiceMock.Setup(service => service.SaveIdeaAsync(It.IsAny<IdeaDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SaveIdea(ideaRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult.ActionName, Is.EqualTo(nameof(_controller.SaveIdea)));
            Assert.That(createdResult.RouteValues["id"], Is.EqualTo(ideaRequest.UsuarioId));
            Assert.That(createdResult.Value, Is.EqualTo(ideaRequest));
        }

        [Test]
        public async Task SaveIdea_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var ideaRequest = new IdeaRequestModel
            {
                UsuarioId = 1,
                Titulo = "Test Idea"
            };
            _ideaServiceMock.Setup(service => service.SaveIdeaAsync(It.IsAny<IdeaDto>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.SaveIdea(ideaRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.Multiple(() =>
            {
                Assert.That(objectResult.StatusCode, Is.EqualTo(500));
                Assert.That(objectResult.Value, Is.EqualTo("Internal server error"));
            });
        }

        [Test]
        public async Task GetIdeasByUsuarioId_ReturnsOkResult_WithIdeas()
        {
            // Arrange
            var usuarioId = 1;
            var ideasList = new List<IdeaDto>
        {
            new IdeaDto { Id = 1, Titulo = "Idea 1", UsuarioId = usuarioId },
            new IdeaDto { Id = 2, Titulo = "Idea 2", UsuarioId = usuarioId }
        };
            _ideaServiceMock.Setup(service => service.GetIdeasByUserIdAsync(usuarioId)).ReturnsAsync(ideasList);

            // Act
            var result = await _controller.GetIdeasByUsuarioId(usuarioId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<List<IdeaResponseModel>>());
            var returnValue = okResult.Value as List<IdeaResponseModel>;
            Assert.That(returnValue.Count, Is.EqualTo(2));
            Assert.That(returnValue[0].UsuarioId, Is.EqualTo(usuarioId));
            Assert.That(returnValue[1].UsuarioId, Is.EqualTo(usuarioId));
        }

        [Test]
        public async Task GetIdeasByUsuarioId_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var usuarioId = 1;
            _ideaServiceMock.Setup(service => service.GetIdeasByUserIdAsync(usuarioId)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetIdeasByUsuarioId(usuarioId);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetIdeaById_ReturnsOkResult_WithIdea()
        {
            // Arrange
            var ideaId = 1;
            var idea = new IdeaDto { Id = ideaId, Titulo = "Test Idea", UsuarioId = 1 };
            _ideaServiceMock.Setup(service => service.GetIdeaByIdAsync(ideaId)).ReturnsAsync(idea);

            // Act
            var result = await _controller.GetIdeaById(ideaId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;

            Assert.That(okResult.Value, Is.InstanceOf<IdeaResponseModel>());
            var returnValue = okResult.Value as IdeaResponseModel;
            Assert.That(returnValue.Id, Is.EqualTo(ideaId));
        }

        [Test]
        public async Task GetIdeaById_ReturnsNotFound_WhenIdeaNotFound()
        {
            // Arrange
            var ideaId = 1;
            _ideaServiceMock.Setup(service => service.GetIdeaByIdAsync(ideaId)).ReturnsAsync((IdeaDto)null);

            // Act
            var result = await _controller.GetIdeaById(ideaId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetIdeaById_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var ideaId = 1;
            _ideaServiceMock.Setup(service => service.GetIdeaByIdAsync(ideaId)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetIdeaById(ideaId);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task DeleteIdea_ReturnsOk_WhenDeletionIsSuccessful()
        {
            // Arrange
            var ideaId = 1;
            _ideaServiceMock.Setup(service => service.DeleteIdeaByIdAsync(ideaId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteIdea(ideaId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public async Task DeleteIdea_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var ideaId = 1;
            _ideaServiceMock.Setup(service => service.DeleteIdeaByIdAsync(ideaId)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.DeleteIdea(ideaId);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
