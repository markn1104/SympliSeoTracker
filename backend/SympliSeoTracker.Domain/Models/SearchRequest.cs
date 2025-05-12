using MediatR;
using SympliSeoTracker.Domain.Enums;

namespace SympliSeoTracker.Domain.Models
{
    public class SearchRequest : IRequest<SearchResponse>
    {
        public string Keywords { get; set; }
        public string Url { get; set; }
        public SearchProviderType Provider { get; set; } = SearchProviderType.Google;
    }
}