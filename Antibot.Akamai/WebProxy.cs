using System;
using System.Net;

namespace DripSolutions.Antibot.Akamai
{
  public class WebProxy : IWebProxy
  {
    public WebProxy(string proxy)
    {
      string[] proxyParts = proxy.Split(':');
      m_ProxyUri = new Uri($"http://{proxyParts[0]}:{proxyParts[1]}");
      m_Credentials = new NetworkCredential(proxyParts[2], proxyParts[3]);
    }

    private Uri m_ProxyUri;

    private NetworkCredential m_Credentials;
    public ICredentials Credentials
    {
      get => m_Credentials;
      set => throw new NotImplementedException();
    }

    public Uri GetProxy(Uri destination)
    {
      return m_ProxyUri;
    }

    public bool IsBypassed(Uri host)
    {
      return false;
    }
  }
}
