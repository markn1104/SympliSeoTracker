using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using SympliSeoTracker.Domain.Interfaces;
using SympliSeoTracker.Infrastructure.Services;
using Xunit;

namespace SympliSeoTracker.Tests.Services
{
    public class BingSearchServiceTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILoggerManager> _mockLogger;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;

        public BingSearchServiceTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILoggerManager>();
            _mockCacheService = new Mock<ICacheService>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            
            _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(_httpClient);
        }

        [Fact]
        public async Task FindPositionsAsync_WhenCacheHasValue_ReturnsCachedResult()
        {
            // Arrange
            string keywords = "test keywords";
            string url = "example.com";
            string cachedResult = "1, 5, 10";
            
            _mockCacheService.Setup(c => c.TryGetValue(It.IsAny<string>(), out cachedResult))
                .Returns(true);
            
            var bingSearchService = new BingSearchService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _mockCacheService.Object);

            // Act
            var result = await bingSearchService.FindPositionsAsync(keywords, url);

            // Assert
            Assert.Equal(cachedResult, result);
            _mockLogger.Verify(l => l.LogInfo(It.Is<string>(s => 
                s.Contains("Retrieved Bing search results from cache"))), Times.Once);
            _mockHttpClientFactory.Verify(f => f.CreateClient(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task FindPositionsAsync_WhenNoCache_FetchesAndProcessesResults()
        {
            // Arrange
            string keywords = "test keywords";
            string url = "example.com";
            string htmlContent = @"
                <html>
                <body>
                    <li class=""b_algo"">
                        <a href=""http://example.com/page1"">Example Page 1</a>
                    </li>
                    <li class=""b_algo"">
                        <a href=""http://othersite.com/page2"">Other Site</a>
                    </li>
                    <div>Next page</div>
                </body>
                </html>";

            // Setup mock HTTP handler
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(htmlContent)
                });

            string outValue = null;
            _mockCacheService.Setup(c => c.TryGetValue(It.IsAny<string>(), out outValue))
                .Returns(false);

            var bingSearchService = new BingSearchService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _mockCacheService.Object);

            // Act
            var result = await bingSearchService.FindPositionsAsync(keywords, url);

            // Assert
            Assert.Equal("1, 11, 21, 31, 41, 51, 61, 71, 81, 91", result);

            _mockLogger.Verify(l => l.LogInfo(It.Is<string>(s => 
                s.Contains("Searching Bing for keywords"))), Times.Once);
            _mockCacheService.Verify(c => c.Set(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<TimeSpan?>()), Times.Once);
        }

        [Fact]
        public async Task FindPositionsAsync_WhenNoMatches_ReturnsZero()
        {
            // Arrange
            string keywords = "test keywords";
            string url = "example.com";
            string htmlContent = @"
                <html>
                <body>
                    <li class=""b_algo"">
                        <a href=""http://othersite1.com/page1"">Other Site 1</a>
                    </li>
                    <li class=""b_algo"">
                        <a href=""http://othersite2.com/page2"">Other Site 2</a>
                    </li>
                </body>
                </html>";

            // Setup mock HTTP handler
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(htmlContent)
                });

            string outValue = null;
            _mockCacheService.Setup(c => c.TryGetValue(It.IsAny<string>(), out outValue))
                .Returns(false);

            var bingSearchService = new BingSearchService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _mockCacheService.Object);

            // Act
            var result = await bingSearchService.FindPositionsAsync(keywords, url);

            // Assert
            Assert.Equal("0", result);
        }

        [Fact]
        public async Task FindPositionsAsync_WhenHttpRequestFails_ThrowsException()
        {
            // Arrange
            string keywords = "test keywords";
            string url = "example.com";

            // Setup mock HTTP handler to throw exception
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Simulated HTTP error"));

            string outValue = null;
            _mockCacheService.Setup(c => c.TryGetValue(It.IsAny<string>(), out outValue))
                .Returns(false);

            var bingSearchService = new BingSearchService(
                _mockHttpClientFactory.Object,
                _mockLogger.Object,
                _mockCacheService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                bingSearchService.FindPositionsAsync(keywords, url));
            
            _mockLogger.Verify(l => l.LogError(It.IsAny<string>()), Times.Once);
        }
    }
}