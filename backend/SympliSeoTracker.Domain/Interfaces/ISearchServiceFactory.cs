using SympliSeoTracker.Domain.Enums;

namespace SympliSeoTracker.Domain.Interfaces
{
    public interface ISearchServiceFactory
    {
        ISearchService CreateSearchService(SearchProviderType providerType);
    }
}