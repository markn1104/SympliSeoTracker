using System.Text.RegularExpressions;
using SympliSeoTracker.Domain.Constants;
using SympliSeoTracker.Domain.Interfaces;

namespace SympliSeoTracker.Infrastructure.Services
{
    public class BingSearchService : ISearchService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerManager _logger;
        private readonly ICacheService _cacheService;

        public BingSearchService(
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
            string cacheKey = $"bing_search_{keywords}_{url}_{maxResults}";

            if (_cacheService.TryGetValue<string>(cacheKey, out var cachedResult))
            {
                _logger.LogInfo($"Retrieved Bing search results from cache for keywords: {keywords}, url: {url}");
                return cachedResult;
            }

            _logger.LogInfo($"Searching Bing for keywords: {keywords}, url: {url}");

            try
            {
                var positions = new List<int>();
                int pagesToFetch = Math.Min(SearchConstants.TotalPage, (int)Math.Ceiling((double)maxResults / SearchConstants.PageSize));

                for (int pageNum = 1; pageNum <= pagesToFetch; pageNum++)
                {
                    var pageResults = await GetBingSearchResultsPage(keywords, pageNum);

                    var pagePositions = FindUrlPositionsInBingPage(pageResults, url, pageNum);
                    positions.AddRange(pagePositions);

                    if (positions.Count >= maxResults)
                    {
                        positions = positions.Take(maxResults).ToList();
                        break;
                    }
                }

                string result = positions.Any() ? string.Join(", ", positions) : "0";

                _cacheService.Set(cacheKey, result, TimeSpan.FromHours(1));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching Bing: {ex.Message}");
                throw;
            }
        }

        private async Task<string> GetBingSearchResultsPage(string keywords, int pageNum)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", SearchConstants.UserAgent);

            var startParam = (pageNum - 1) * SearchConstants.PageSize + 1; // Bing uses 1-based indexing
            var url = $"{SearchConstants.BingBaseUrl}/search?q={Uri.EscapeDataString(keywords)}&first={startParam}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private List<int> FindUrlPositionsInBingPage(string pageContent, string url, int pageNum)
        {
            var positions = new List<int>();

            var cleanUrl = NormalizeUrl(url);

            var resultItems = Regex.Matches(pageContent, @"<li\s+class=""(?:[^""]*\s)?b_algo(?:\s[^""]*)?"".*?>.*?</li>", RegexOptions.Singleline);

            for (int i = 0; i < resultItems.Count; i++)
            {
                string itemContent = resultItems[i].Value;

                var anchorTags = ExtractAnchorTags(itemContent);

                foreach (var anchor in anchorTags)
                {
                    var normalizedAnchor = NormalizeUrl(anchor);
                    if (normalizedAnchor.Contains(cleanUrl) || cleanUrl.Contains(normalizedAnchor))
                    {
                        int position = (pageNum - 1) * 10 + (i + 1);
                        positions.Add(position);
                        break; 
                    }
                }
            }

            return positions;
        }

        private string NormalizeUrl(string url)
        {
            return url.ToLower()
                .Replace("http://", "")
                .Replace("https://", "")
                .Replace("www.", "")
                .TrimEnd('/');
        }

        private List<string> ExtractAnchorTags(string html)
        {
            var anchors = new List<string>();
            var matches = Regex.Matches(html, @"<a\s+(?:[^>]*?\s+)?href=""([^""]*)""", RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    anchors.Add(match.Groups[1].Value);
                }
            }

            return anchors;
        }
    }
}