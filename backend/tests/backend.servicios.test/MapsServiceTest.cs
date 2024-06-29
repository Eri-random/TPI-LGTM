using backend.servicios.Servicios;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;

namespace backend.servicios.test
{
    [TestFixture]
    public class MapsServiceTest
    {
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<IConfiguration> _configurationMock;
        private MapsService _mapsService;
        private HttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(x => x["GoogleMapsApiKey"]).Returns("your_api_key");

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(mockHttpMessageHandler.Object);

            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(_httpClient);

            _mapsService = new MapsService(_httpClientFactoryMock.Object, _configurationMock.Object);
        }

        [Test]
        public async Task GetCoordinates_Returns_CorrectCoordinates()
        {
            // Arrange
            var responseContent = @"
        {
            ""status"": ""OK"",
            ""results"": [
                {
                    ""geometry"": {
                        ""location"": {
                            ""lat"": 40.7128,
                            ""lng"": -74.0060
                        }
                    }
                }
            ]
        }";

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            _httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(_httpClient);

            var mapsService = new MapsService(_httpClientFactoryMock.Object, _configurationMock.Object);

            // Act
            var (lat, lng) = await mapsService.GetCoordinates("1600 Amphitheatre Parkway", "Mountain View", "CA");

            // Assert
            Assert.AreEqual(40.7128, lat);
            Assert.AreEqual(-74.0060, lng);
        }

        [Test]
        public async Task GetCoordinates_Throws_Exception_On_Error()
        {
            // Arrange
            var responseContent = @"
        {
            ""status"": ""ERROR""
        }";

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            _httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(_httpClient);

            var mapsService = new MapsService(_httpClientFactoryMock.Object, _configurationMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => mapsService.GetCoordinates("1600 Amphitheatre Parkway", "Mountain View", "CA"));
        }

        [Test]
        public void Constructor_NullHttpClientFactory_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new MapsService(null, _configurationMock.Object));
            Assert.That(ex.ParamName, Is.EqualTo("factory"));
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
        }
    }
}
