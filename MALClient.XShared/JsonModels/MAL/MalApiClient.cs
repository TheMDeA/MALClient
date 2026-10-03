using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MALClient.XShared.JsonModels.MAL
{
    /// <summary>
    /// Thin wrapper for official MAL API v2 GET calls. Deserializes directly from the
    /// response stream instead of buffering the whole payload into a string first
    /// (HttpClient.GetStringAsync), which cuts peak memory roughly in half for large
    /// responses such as seasonal lists and search results. Throws on non-success
    /// status codes, exactly like GetStringAsync did (including HttpRequestException
    /// carrying the status code, e.g. for 429 retry handling).
    /// </summary>
    internal static class MalApiClient
    {
        public static async Task<T> GetJsonAsync<T>(HttpClient client, string url)
        {
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            // NB: plain 'using', not 'await using' — this library targets netstandard2.0,
            // where Stream does not implement IAsyncDisposable yet.
            using var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
    }
}
