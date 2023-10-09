using System.Net;
using System.Net.Http;

namespace IntegradorLPR.Client
{
    public class IntelbrasHttpClient
    {
        private readonly HttpClient _httpClient;

        public IntelbrasHttpClient(string username, string password)
        {
            _httpClient = new HttpClient(new HttpClientHandler { Credentials = new NetworkCredential(username, password) });
            _httpClient.Timeout = Timeout.InfiniteTimeSpan;
        }

        public async Task<HttpResponseMessage> GetAsync(string url, string username, string password)

        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
               Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));

            return await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        }
    }
}
