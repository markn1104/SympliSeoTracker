using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SympliSeoTracker.Domain.Enums;
using SympliSeoTracker.Domain.Interfaces;

namespace SympliSeoTracker.API.Features.Search.Queries
{
    public class GetSearchPositionsQuery : IRequest<string>
    {
        public string Keywords { get; set; }
        public string Url { get; set; }
        public SearchProviderType Provider { get; set; }
    }

    public class GetSearchPositionsQueryHandler : IRequestHandler<GetSearchPositionsQuery, string>
    {
        private readonly ISearchServiceFactory _searchServiceFactory;
        private readonly ILoggerManager _logger;

        public GetSearchPositionsQueryHandler(
            ISearchServiceFactory searchServiceFactory,
            ILoggerManager logger)
        {
            _searchServiceFactory = searchServiceFactory ?? throw new ArgumentNullException(nameof(searchServiceFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> Handle(GetSearchPositionsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Processing search request for keywords: {request.Keywords}, URL: {request.Url}, Provider: {request.Provider}");
            
            try
            {
                var searchService = _searchServiceFactory.CreateSearchService(request.Provider);
                var result = await searchService.FindPositionsAsync(request.Keywords, request.Url);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing search request: {ex.Message}");
                throw;
            }
        }
    }
}