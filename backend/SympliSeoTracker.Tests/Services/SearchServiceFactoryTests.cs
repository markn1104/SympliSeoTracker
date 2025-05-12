using System;
using System.Net.Http;
using Moq;
using SympliSeoTracker.Domain.Enums;
using SympliSeoTracker.Domain.Interfaces;
using SympliSeoTracker.Infrastructure.Services;
using Xunit;

namespace SympliSeoTracker.Tests.Services
{
    public class SearchServiceFactoryTests
    {
        [Fact]
        public void CreateSearchService_WhenGoogleProviderType_ReturnsGoogleSearchService()
        {
            // Arrange
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockGoogleSearchService = new Mock<GoogleSearchService>(
                Mock.Of<IHttpClientFactory>(),
                Mock.Of<ILoggerManager>(),
                Mock.Of<ICacheService>());
            
            mockServiceProvider
                .Setup(sp => sp.GetService(typeof(GoogleSearchService)))
                .Returns(mockGoogleSearchService.Object);
            
            var factory = new SearchServiceFactory(mockServiceProvider.Object);

            // Act
            var result = factory.CreateSearchService(SearchProviderType.Google);

            // Assert
            Assert.IsAssignableFrom<GoogleSearchService>(result);
        }

        [Fact]
        public void CreateSearchService_WhenBingProviderType_ReturnsBingSearchService()
        {
            // Arrange
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockBingSearchService = new Mock<BingSearchService>(
                Mock.Of<IHttpClientFactory>(),
                Mock.Of<ILoggerManager>(),
                Mock.Of<ICacheService>());
            
            mockServiceProvider
                .Setup(sp => sp.GetService(typeof(BingSearchService)))
                .Returns(mockBingSearchService.Object);
            
            var factory = new SearchServiceFactory(mockServiceProvider.Object);

            // Act
            var result = factory.CreateSearchService(SearchProviderType.Bing);

            // Assert
            Assert.IsAssignableFrom<BingSearchService>(result);

        }

        [Fact]
        public void CreateSearchService_WhenUnsupportedProviderType_ThrowsArgumentException()
        {
            // Arrange
            var mockServiceProvider = new Mock<IServiceProvider>();
            var factory = new SearchServiceFactory(mockServiceProvider.Object);
            
            // Use an invalid enum value by casting
            var invalidProviderType = (SearchProviderType)999;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => 
                factory.CreateSearchService(invalidProviderType));
            
            Assert.Contains("Unsupported search provider", exception.Message);
        }
    }
}