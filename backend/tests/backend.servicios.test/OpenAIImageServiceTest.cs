using backend.servicios.Config;
using backend.servicios.Models;
using backend.servicios.Servicios;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace backend.servicios.test
{
    [TestFixture]
    public class OpenAIImageServiceTest
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<ILogger<OpenAIImageService>> _loggerMock;
        private OpenAiApiConfig _config;
        private OpenAIImageService _openAIImageService;

        [SetUp]
        public void SetUp()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _loggerMock = new Mock<ILogger<OpenAIImageService>>();
            _config = new OpenAiApiConfig
            {
                ApiKey = "test_api_key",
                Url = "https://api.openai.com/v1/images"
            };

            _openAIImageService = new OpenAIImageService(_httpClientFactoryMock.Object, _config, _loggerMock.Object);
        }

        [Test]
        public async Task GenerateImageAsync_ReturnsResponse()
        {
            // Arrange
            var expectedResponse = new OpenAIImageResponse
            {
                Data = [new ImageData { Url = "https://example.com/image.png" }]
            };
            var responseContent = JsonSerializer.Serialize(expectedResponse);
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var request = new OpenAIImageRequest("test prompt");

            // Act
            var result = await _openAIImageService.GenerateImageAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse.Data.First().Url, result.Data.First().Url);
        }

        [Test]
        public async Task GenerateImageAsync_HandlesErrorResponse()
        {
            // Arrange
            var errorResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Bad request", Encoding.UTF8, "application/json")
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(errorResponse);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var request = new OpenAIImageRequest("test prompt");

            // Act
            var result = await _openAIImageService.GenerateImageAsync(request);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void Constructor_NullHttpClientFactory_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new OpenAIImageService(null, _config, _loggerMock.Object));
            Assert.That(ex.ParamName, Is.EqualTo("httpClientFactory"));
        }

        [Test]
        public void Constructor_NullConfig_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new OpenAIImageService(_httpClientFactoryMock.Object, null, _loggerMock.Object));
            Assert.That(ex.ParamName, Is.EqualTo("openAIConfig"));
        }

        [Test]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new OpenAIImageService(_httpClientFactoryMock.Object, _config, null));
            Assert.That(ex.ParamName, Is.EqualTo("logger"));
        }
    }
}
