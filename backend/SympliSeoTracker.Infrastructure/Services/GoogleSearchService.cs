using System.Text.RegularExpressions;
using SympliSeoTracker.Domain.Constants;
using SympliSeoTracker.Domain.Interfaces;

namespace SympliSeoTracker.Infrastructure.Services
{
    public class GoogleSearchService : ISearchService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerManager _logger;
        private readonly ICacheService _cacheService;

        public GoogleSearchService(
            IHttpClientFactory httpClientFactory,
            ILoggerManager logger,
            ICacheService cacheService)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<string> FindPositionsAsync(string keywords, string url, int maxResults = 100)
        {
            string cacheKey = $"search_{keywords}_{url}_{maxResults}";

            if (_cacheService.TryGetValue<string>(cacheKey, out var cachedResult))
            {
                _logger.LogInfo($"Retrieved search results from cache for keywords: {keywords}, url: {url}");
                return cachedResult;
            }

            _logger.LogInfo($"Searching for keywords: {keywords}, url: {url}");

            try
            {
                var htmlContent = await FetchSearchHtmlAsync(keywords, maxResults);
                var resultContainer = ExtractResultContainer(htmlContent);
                var anchorTags = ExtractAnchorTags(resultContainer);
                var matchedPositions = FindUrlPositions(anchorTags, url);

                string result = matchedPositions.Any() ? string.Join(", ", matchedPositions) : "0";
                
                _cacheService.Set(cacheKey, result, TimeSpan.FromHours(1));
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching Google: {ex.Message}");
                throw;
            }
        }

        private async Task<string> FetchSearchHtmlAsync(string keywords, int maxResults)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", SearchConstants.UserAgent);
            
            int numResults = Math.Min(maxResults, SearchConstants.PageSize * SearchConstants.TotalPage);
            
            var requestUrl = $"{SearchConstants.GoogleBaseUrl}/search?q={Uri.EscapeDataString(keywords)}&num={numResults}";
            
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsStringAsync();
        }

        private string ExtractResultContainer(string htmlContent)
        {
            const string startTag = @"<div[^>]*id=""rso""[^>]*>"; 
            const string openDivTag = @"<div[^>]*>"; 
            const string closeDivTag = @"<\/div>"; 
 
            var regexPattern = $@"{startTag}((?'nested'{openDivTag})|{closeDivTag}(?'-nested')|[\w\W]*?)*{closeDivTag}"; 
            var match = Regex.Match(htmlContent, regexPattern, RegexOptions.Singleline); 
 
            return match.Success ? match.Value : string.Empty;
        }

        private List<string> ExtractAnchorTags(string resultContainer)
        {
            const string anchorPattern = @"<div[^>]*>([\s\S]*?)(<a[^>]*jsname\s*=\s*""[^""]+""[^>]*href\s*=\s*""[^""]+""[^>]*>[\s\S]*?<\/a>)[\s\S]*?<\/div>"; 
 
            var matches = Regex.Matches(resultContainer, anchorPattern, RegexOptions.Singleline); 
            return matches.Select(match => match.Groups[2].Value).ToList();
        }

        private List<int> FindUrlPositions(List<string> anchorTags, string targetUrl)
        {
            var positions = new List<int>();
            var cleanUrl = targetUrl.Replace("www.", "")
                                   .Replace("http://", "")
                                   .Replace("https://", "")
                                   .TrimEnd('/');

            for (int i = 0; i < anchorTags.Count && i < 100; i++)
            {
                if (anchorTags[i].Contains(cleanUrl, StringComparison.OrdinalIgnoreCase))
                {
                    positions.Add(i + 1);
                }
            }

            return positions;
        }
    }
}