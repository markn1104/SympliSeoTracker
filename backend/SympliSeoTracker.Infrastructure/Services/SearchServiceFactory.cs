using Microsoft.Extensions.DependencyInjection;
using SympliSeoTracker.Domain.Enums;
using SympliSeoTracker.Domain.Interfaces;

namespace SympliSeoTracker.Infrastructure.Services
{
    public class SearchServiceFactory : ISearchServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SearchServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISearchService CreateSearchService(SearchProviderType providerType)
        {
            switch (providerType)
            {
                case SearchProviderType.Google:
                    return _serviceProvider.GetRequiredService<GoogleSearchService>();
                case SearchProviderType.Bing:
                    return _serviceProvider.GetRequiredService<BingSearchService>();                
                default:
                    throw new ArgumentException($"Unsupported search provider: {providerType}");
            }
        }
    }
}