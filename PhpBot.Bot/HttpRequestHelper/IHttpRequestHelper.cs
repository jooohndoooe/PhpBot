namespace PhpBot.Bot.HttpRequestHelper
{
    public interface IHttpRequestHelper
    {
        Task<TResponse> SendRequest<TBody, TResponse>(string path, HttpMethod method, string telegramUsername, TBody body = default);
    }
}
