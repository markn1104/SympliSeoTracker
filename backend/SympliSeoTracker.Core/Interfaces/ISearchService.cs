using System.Threading.Tasks;

namespace SympliSeoTracker.Core.Interfaces
{
    public interface ISearchService
    {
        Task<string> FindPositionsAsync(string keywords, string url, int maxResults = 100);
    }
}