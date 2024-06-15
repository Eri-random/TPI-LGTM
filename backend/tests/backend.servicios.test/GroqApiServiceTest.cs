using backend.servicios.Config;
using backend.servicios.Servicios;
using Moq;
using Moq.Protected;
using System.Net;

namespace backend.servicios.test
{
    [TestFixture]
    public class GroqApiServiceTests
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private GroqApiConfig _config;
        private GroqApiService _groqApiService;

        [SetUp]
        public void SetUp()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _config = new GroqApiConfig
            {
                ApiKey = "test_api_key",
                RequestUri = "https://api.example.com",
                LlmModel = "test_model"
            };

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _groqApiService = new GroqApiService(_httpClientFactoryMock.Object, _config);
        }

        [Test]
        public async Task GenerateIdea_ReturnsResponse()
        {
            // Arrange
            var expectedResponse = "{\"Idea\":\"Generated idea\",\"Pasos\":[\"Step 1\",\"Step 2\"]}";
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedResponse)
                });

            // Act
            var result = await _groqApiService.GenerateIdea("Test message");

            // Assert
            Assert.That(result, Is.EqualTo(expectedResponse));
        }


        [Test]
        public void Constructor_NullHttpClientFactory_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GroqApiService(null, _config));
        }

        [Test]
        public void Constructor_NullConfig_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GroqApiService(_httpClientFactoryMock.Object, null));
        }

        [Test]
        public void GetHttpClient_ReturnsConfiguredHttpClient()
        {
            // Act
            var httpClient = ((GroqApiService)_groqApiService).GetType().GetMethod("GetHttpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(_groqApiService, null) as HttpClient;

            // Assert
            Assert.That(httpClient, Is.Not.Null);
            Assert.That(httpClient.DefaultRequestHeaders.Contains("Authorization"), Is.True);
        }

        [Test]
        public void GetHttpMessageContent_ReturnsCorrectContent()
        {
            // Arrange
            var userMessage = "Test message";

            // Act
            var methodInfo = typeof(GroqApiService).GetMethod("GetHttpMessageContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var content = methodInfo.Invoke(_groqApiService, new object[] { userMessage }) as StringContent;

            // Assert
            var contentString = content.ReadAsStringAsync().Result;
            StringAssert.Contains(userMessage, contentString);
            StringAssert.Contains("system", contentString);
            StringAssert.Contains("user", contentString);
        }

        [Test]
        public void GetSystemMessage_ReturnsCorrectMessage()
        {
            // Act
            var methodInfo = typeof(GroqApiService).GetMethod("GetSystemMessage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var systemMessage = methodInfo.Invoke(null, null) as string;

            // Assert
            StringAssert.Contains("JSON", systemMessage);
            StringAssert.Contains("\"Idea\"", systemMessage);
            StringAssert.Contains("\"Pasos\"", systemMessage);
        }
    }
}
