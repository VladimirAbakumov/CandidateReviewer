using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;

namespace VA.Candidate.Reviewer.Frameworks
{
  public interface IHttpClientFactory
  {
    HttpClient Get();

    void Release(HttpClient client);
  }

  public class HttpClientFactory : IHttpClientFactory
  {
    private readonly ConcurrentQueue<HttpClient> _httpQueue = new();
    
    public HttpClientFactory() => ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;

    public HttpClient Get() => _httpQueue.TryDequeue(out var client) ? client : CreateHttpClient();

    public void Release(HttpClient client)
    {
      client.DefaultRequestHeaders.Clear();
      _httpQueue.Enqueue(client);
    }

    protected virtual HttpClientHandler CreateHandler() => new() { UseCookies = false };

    private HttpClient CreateHttpClient() =>
      new(CreateHandler())
      {
        Timeout = TimeSpan.FromSeconds(30),
        DefaultRequestHeaders =
        {
          ConnectionClose = false
        }
      };
  }
}