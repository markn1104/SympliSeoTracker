using System;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using SympliSeoTracker.Infrastructure.Caching;
using Xunit;

namespace SympliSeoTracker.Tests.Caching
{
    public class MemoryCacheServiceTests
    {
        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private readonly MemoryCacheService _cacheService;

        public MemoryCacheServiceTests()
        {
            _mockMemoryCache = new Mock<IMemoryCache>();
            _cacheService = new MemoryCacheService(_mockMemoryCache.Object);
        }

        [Fact]
        public void Get_CallsMemoryCacheGet()
        {
            // Arrange
            string key = "testKey";
            string expectedValue = "testValue";

            object outValue = expectedValue;

            _mockMemoryCache
                .Setup(m => m.TryGetValue(key, out outValue))
                .Returns(true);

            // Act
            var result = _cacheService.Get<string>(key);

            // Assert
            Assert.Equal(expectedValue, result);
            _mockMemoryCache.Verify(m => m.TryGetValue(key, out It.Ref<object>.IsAny), Times.Once);
        }

        [Fact]
        public void TryGetValue_WhenKeyExists_ReturnsTrue()
        {
            // Arrange
            string key = "nonExistentKey";
            object outValue = null; // mimic a cache miss

            _mockMemoryCache
                .Setup(m => m.TryGetValue(key, out outValue))
                .Returns(false);

            // Act
            var result = _cacheService.TryGetValue(key, out string actualValue);

            // Assert
            Assert.False(result);                      // Because we return false from TryGetValue
            Assert.Null(actualValue);                  // Because we set outValue = null
        }

        [Fact]
        public void TryGetValue_WhenKeyDoesNotExist_ReturnsFalse()
        {
            // Arrange
            string key = "nonExistentKey";
            string outValue = "null";
            object expectedValue = outValue;

            _mockMemoryCache
                .Setup(m => m.TryGetValue(key, out expectedValue))
                .Returns(false);

            // Act
            bool result = _cacheService.TryGetValue(key, out string actualValue);

            // Assert
            Assert.False(result);
            Assert.Null(actualValue);
        }

        [Fact]
        public void Set_WithoutExpiration_SetsDefaultExpiration()
        {
            // Arrange
            string key = "testKey";
            string value = "testValue";
            
            var cacheEntryMock = new Mock<ICacheEntry>();
            
            _mockMemoryCache
                .Setup(m => m.CreateEntry(key))
                .Returns(cacheEntryMock.Object);

            // Act
            _cacheService.Set(key, value);

            // Assert
            _mockMemoryCache.Verify(m => m.CreateEntry(key), Times.Once);
            cacheEntryMock.VerifySet(e => e.Value = value);
            cacheEntryMock.VerifySet(e => e.AbsoluteExpirationRelativeToNow = It.Is<TimeSpan>(t => 
                t.TotalHours == 1));
        }

        [Fact]
        public void Set_WithCustomExpiration_SetsSpecifiedExpiration()
        {
            // Arrange
            string key = "testKey";
            string value = "testValue";
            TimeSpan expiration = TimeSpan.FromMinutes(30);
            
            var cacheEntryMock = new Mock<ICacheEntry>();
            
            _mockMemoryCache
                .Setup(m => m.CreateEntry(key))
                .Returns(cacheEntryMock.Object);

            // Act
            _cacheService.Set(key, value, expiration);

            // Assert
            _mockMemoryCache.Verify(m => m.CreateEntry(key), Times.Once);
            cacheEntryMock.VerifySet(e => e.Value = value);
            cacheEntryMock.VerifySet(e => e.AbsoluteExpirationRelativeToNow = expiration);
        }

        [Fact]
        public void Remove_CallsMemoryCacheRemove()
        {
            // Arrange
            string key = "testKey";

            // Act
            _cacheService.Remove(key);

            // Assert
            _mockMemoryCache.Verify(m => m.Remove(key), Times.Once);
        }
    }
}