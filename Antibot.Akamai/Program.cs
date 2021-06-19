using RandomUserAgent;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DripSolutions.Antibot.Akamai
{
  class Program
  {
    static async Task Main(string[] args)
    {
      await ZalandoSavedTest();
    }

    private static async Task ZalandoTest()
    {
      string proxy = GetRandomProxy();
      string userAgent = RandomUa.RandomUserAgent;

      //HttpClientFactory

      var httpClientHandler = new HttpClientHandler();
      httpClientHandler.Proxy = new WebProxy(proxy);
      httpClientHandler.UseCookies = true;
      httpClientHandler.CookieContainer = new CookieContainer();
      httpClientHandler.AllowAutoRedirect = false;
      var httpClient = new HttpClient(httpClientHandler);
      //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Host", "https://www.zalando.de");
      httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);
      httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
      httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
      httpClient.DefaultRequestHeaders.TryAddWithoutValidation("DNT", "1");
      httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referrer", "https://www.google.com");
      httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
      //httpClient.DefaultRequestHeaders.TryAddWithoutValidation

      //var response = await httpClient.GetAsync("https://www.zalando-lounge.de");
      //var stringResponse = await response.Content.ReadAsStringAsync();

      var response = await httpClient.GetAsync("https://www.zalando.de");
      var stringResponse = await response.Content.ReadAsStringAsync();
    }

    private static async Task ZalandoSavedTest()
    {
      var akamaiPayloadCreator = new AkamaiPayload();

      string userAgent = RandomUa.RandomUserAgent;
      string jsFile = ReadFile("file.js");
      var postRequest = await akamaiPayloadCreator.CreateRequest(jsFile, "https://www.zalando.de/", userAgent);
    }

    private static string ReadFile(string fileName)
    {
      string content = "";
      using (var stream = File.Open("../../../" + fileName, FileMode.Open))
      {
        using (var streamReader = new StreamReader(stream))
        {
          content = streamReader.ReadToEnd();
        }
      }

      return content;
    }

    private static string GetRandomProxy()
    {
      string proxies = ReadFile("proxies.txt");
      string[] proxiesSplit = proxies.Split("\n");
      var r = new Random();
      int randomIndex = r.Next(0, proxiesSplit.Length);

      return proxiesSplit[randomIndex];
    }
  }
}
