using System.Net;
using System.Text;
using System.Text.Json;

namespace PhpBot.Bot.HttpRequestHelper
{
    public class HttpRequestHelper : IHttpRequestHelper
    {
        public HttpRequestHelper(HttpRequestHelperConfiguration httpRequestHelperConfiguration)
        {
            _httpClient = new HttpClient();
            _apiPath = httpRequestHelperConfiguration.ApiHost;
        }

        public async Task<TResponse> SendRequest<TBody, TResponse>(string path, HttpMethod method, string telegramUsername, TBody body = default)
        {
            using var request = new HttpRequestMessage(method, _apiPath + path);
            if (body != null && method != HttpMethod.Get && method != HttpMethod.Delete)
            {
                var jsonBody = JsonSerializer.Serialize(body);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }
            request.Headers.Add("X-TelegramUsername", telegramUsername);
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
            {
                var data = await response.Content.ReadAsStringAsync();
                if (data != "")
                {
                    var result = JsonSerializer.Deserialize<TResponse>(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    return result;
                }
                else
                {
                    return default;
                }
            }
            throw new InvalidOperationException();
        }

        private readonly HttpClient _httpClient;
        private readonly string _apiPath;
    }
}