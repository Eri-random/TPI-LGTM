﻿using backend.data.DataContext;
using backend.data.Models;
using backend.servicios.DTOs;
using backend.servicios.Helpers;
using backend.servicios.Interfaces;
using backend.servicios.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Net;
using Moq.Protected;

namespace backend.servicios.test
{
    [TestFixture]
    public class MapsServiceTest
    {

        [Test]
        public async Task GetCoordinates_Returns_CorrectCoordinates()
        {
            // Arrange
            var httpClientMock = new Mock<HttpClient>();
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["GoogleMapsApiKey"]).Returns("your_api_key");

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

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("response content")
                });

            var httpClient = new HttpClient(httpMessageHandler.Object);

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var mapsService = new MapsService(httpClientFactory.Object, configurationMock.Object);

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
            var httpClientMock = new Mock<HttpClient>();
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["GoogleMapsApiKey"]).Returns("your_api_key");

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

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("response content")
                });

            var httpClient = new HttpClient(httpMessageHandler.Object);

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var mapsService = new MapsService(httpClientFactory.Object, configurationMock.Object);

            // Act & Assert

            Assert.ThrowsAsync<Exception>(() => mapsService.GetCoordinates("1600 Amphitheatre Parkway", "Mountain View", "CA"));
        }
    }
}
