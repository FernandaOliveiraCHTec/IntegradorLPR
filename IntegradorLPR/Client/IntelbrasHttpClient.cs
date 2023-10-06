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


//using System.Net.Http.Headers;
//using System.Text;

//namespace IntegradorLPR.Client
//{
//    public class IntelbrasHttpClient
//    {
//        private readonly HttpClient _httpClient;

//        public IntelbrasHttpClient(string username, string password)
//        {
//            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
//            _httpClient = new HttpClient();
//            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
//            _httpClient.Timeout = Timeout.InfiniteTimeSpan;
//        }

//        public async Task<HttpResponseMessage> GetAsync(string url)
//        {
//            return await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
//        }
//    }
//}
