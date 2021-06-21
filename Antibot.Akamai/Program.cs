using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using RandomUserAgent;
using ScrapySharp.Network;
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
      //await AkamaiTest("https://www.nike.com/login");
      await AkamaiTest("https://www.zalando.de/damen-home/");
    }

    private static async Task AkamaiTest(string url)
    {
      Uri targetUri = new Uri(url);

      string proxy = GetRandomProxy();
      string userAgent = RandomUa.RandomUserAgent;

      var httpClientHandler = new HttpClientHandler();
      httpClientHandler.Proxy = new WebProxy(proxy);
      httpClientHandler.UseCookies = true;
      httpClientHandler.CookieContainer = new CookieContainer();
      httpClientHandler.AllowAutoRedirect = false;
      var httpClient = new HttpClient(httpClientHandler);

      //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Host", "https://www.zalando.de");
      //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);
      //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "PostmanRuntime/7.28.0"); 
      //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
      //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
      //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("DNT", "1");
      //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referrer", "https://www.google.com");
      //httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
      //httpClient.DefaultRequestHeaders.TryAddWithoutValidation

      var response = await httpClient.GetAsync(targetUri);
      var stringResponse = await response.Content.ReadAsStringAsync();

      var doc = new HtmlDocument();
      doc.LoadHtml(stringResponse);
      var scripts = doc.DocumentNode.QuerySelectorAll("script");

      if (scripts.Count == 0)
      {
        throw new Exception("No Scripts");
      }

      var akamaiScript = scripts[scripts.Count - 1];
      var akamaiScriptSrc = akamaiScript.GetAttributeValue("src", "");

      if (akamaiScriptSrc == "")
      {
        throw new Exception("Cant get Akamai Script Source");
      }

      var fullAkamaiScriptSrc = targetUri.Scheme + "://" + targetUri.Host + akamaiScriptSrc;

      var responseScript = await httpClient.GetAsync(fullAkamaiScriptSrc);
      var stringResponseScript = await responseScript.Content.ReadAsStringAsync();

      var akamaiPayloadCreator = new AkamaiPayload();
      var postRequest = akamaiPayloadCreator.CreatePayload(stringResponseScript, fullAkamaiScriptSrc, url, userAgent);

      var responsePost = await httpClient.PostAsync(fullAkamaiScriptSrc, postRequest);
      var stringResponsePost = await responsePost.Content.ReadAsStringAsync();
    }

    private static void ZalandoSavedTest()
    {
      var akamaiPayloadCreator = new AkamaiPayload();

      string userAgent = RandomUa.RandomUserAgent;
      string jsFile = ReadFile("file.js");
      var postRequest = akamaiPayloadCreator.CreatePayload(jsFile, "https://www.zalando.de/jcMTxdlJIlnmO/Hx8tKHXQft/Z3Qc/9mODwkbL/bTZxKC1lBA/dl/sPZl0UHVw", "https://www.zalando.de/", userAgent);
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

      return proxiesSplit[randomIndex].Trim();
    }
  }
}
