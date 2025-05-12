using MediatR;
using SympliSeoTracker.Domain.Interfaces;
using SympliSeoTracker.Domain.Models;

namespace SympliSeoTracker.Application.Features.Search.Queries
{
    public class SearchRequestHandler : IRequestHandler<SearchRequest, SearchResponse>
    {
        private readonly ISearchServiceFactory _searchServiceFactory;
        private readonly ILoggerManager _logger;

        public SearchRequestHandler(
            ISearchServiceFactory searchServiceFactory,
            ILoggerManager logger)
        {
            _searchServiceFactory = searchServiceFactory;
            _logger = logger;
        }

        public async Task<SearchResponse> Handle(SearchRequest request, CancellationToken cancellationToken)
        {
            // No need for manual validation here - FluentValidation will handle it
            
            _logger.LogInfo($"Processing search request for keywords: {request.Keywords}, url: {request.Url}, provider: {request.Provider}");
            
            var searchService = _searchServiceFactory.CreateSearchService(request.Provider);
            var positions = await searchService.FindPositionsAsync(request.Keywords, request.Url);
            
            return new SearchResponse
            {
                BrowserName = request.Provider.ToString(),
                Positions = positions
            };
        }
    }
}