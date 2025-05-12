using SympliSeoTracker.Core.Enums;

namespace SympliSeoTracker.Core.Interfaces
{
    public interface ISearchServiceFactory
    {
        ISearchService CreateSearchService(SearchProviderType providerType);
    }
}