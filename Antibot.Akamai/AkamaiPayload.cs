using Jint;
using Jint.Native;
using Jint.Native.Function;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DripSolutions.Antibot.Akamai
{
  public class AkamaiPayload
  {
    private Engine _engine;

    public async Task<HttpRequestMessage> CreateRequest(string jsFile, string url, string userAgent)
    {
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);

      Dictionary<string, string> form = new Dictionary<string, string>();

      var payload = await CreatePayload(jsFile, userAgent);

      request.Content = new FormUrlEncodedContent(form);
      return request;
    }

    private async Task<string> CreatePayload(string jsFile, string userAgent)
    {
      var payload = "";

      dynamic location = new ExpandoObject();
      location.hash = "";
      location.host = "www.zalando.de";
      location.hostname = "www.zalando.de";
      location.href = "https://www.zalando.de/";
      location.origin = "https://www.zalando.de";
      location.pathname = "/";
      location.port = "";
      location.protocol = "https:";
      location.search = "";

      dynamic document = new ExpandoObject();
      document.location = location;
      document.getElementsByTagName = new Func<string, object[]>(GetElementsByTagName);
      document.addEventListener = new Action<string, Func<JsValue, JsValue[], JsValue>>(AddEventListener);

      dynamic window = new ExpandoObject();
      window.location = location;

      //dynamic navigator = new ExpandoObject();
      //navigator.appCodeName = "Mozilla";
      //navigator.appName = "Netscape";
      //navigator.userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.106 Safari/537.36";

      ////var setTimeout = new Func<Func<Jint.Native.JsValue, Jint.Native.JsValue[], Jint.Native.JsValue>, double, int>(__setTimeout__);
      ////var clearTimeout = new Action<int>(__clearTimeout__);
      ////var setInterval = new Func<Func<Jint.Native.JsValue, Jint.Native.JsValue[], Jint.Native.JsValue>, double, int>(__setInterval__);
      ////var clearInterval = new Action<int>(__clearInterval__);

      //_engine = new Engine()
      //  .SetValue("window", window)
      //  .SetValue("document", document)
      //  .SetValue("location", location)
      //  .SetValue("navigator", navigator)
      //  .SetValue("console", new JsConsole())
      //  .SetValue("atob", new Func<string, string>(Atob))
      //  .SetValue("setTimeout", new Func<Func<JsValue, JsValue[], JsValue>, object, double>(SetTimeout));
      ////   .SetValue("setTimeout", new Func<Func<Jint.Native.JsValue, Jint.Native.JsValue[], Jint.Native.JsValue>, double?, int>((f, d) => __setTimeout__(f, d != null ? d.Value: 0)))
      //// .SetValue("clearTimeout", new Action<int>(__clearTimeout__))
      ////.SetValue("setInterval", new Func<Func<Jint.Native.JsValue, Jint.Native.JsValue[], Jint.Native.JsValue>, double?, int>((f, d) => __setInterval__(f, d != null ? d.Value : 0)))
      //// .SetValue("clearInterval", new Action<int>(__clearInterval__));
      ////.SetValue("setInterval", new Action<object, object>((s, t) => {
      ////  Console.WriteLine("setInterval " + s + " " + t);
      ////  if (s.GetType() == typeof(Func<Jint.Native.JsValue, Jint.Native.JsValue[], Jint.Native.JsValue>))
      ////  {
      ////    var f = s as Func<Jint.Native.JsValue, Jint.Native.JsValue[], Jint.Native.JsValue>;
      ////    var res = f.Invoke(new Jint.Native.JsValue(""), new Jint.Native.JsValue[0]);
      ////  }
      ////}))
      ////.SetValue("setTimeout", new Action<object, object>((s, t) => {
      ////  Console.WriteLine("setTimeout " + s + " " + t);
      ////  if (s.GetType() == typeof(Func<Jint.Native.JsValue, Jint.Native.JsValue[], Jint.Native.JsValue>))
      ////  {
      ////    var f = s as Func<Jint.Native.JsValue, Jint.Native.JsValue[], Jint.Native.JsValue>;

      ////    var res = f.Invoke(new Jint.Native.JsValue(""), new Jint.Native.JsValue[0]);
      ////  }
      ////}))
      ////.SetValue("clearInterval", new Action<object, object>((s, t) => {
      ////  Console.WriteLine("clearInterval " + s + " " + t);
      ////}));

      _engine = new Engine()
        .SetValue("window", window)
        .SetValue("document", document)
        .SetValue("location", location)
        .SetValue("console", new JsConsole())
        .SetValue("setInterval", new Action<object, object>((s, t) => {
          Console.WriteLine("setInterval " + s + " " + t);
          if (s.GetType() == typeof(Func<JsValue, Jint.Native.JsValue[], Jint.Native.JsValue>))
          {
            var f = s as Func<JsValue, JsValue[], JsValue>;
            var res = f.Invoke(new JsValue(""), new JsValue[0]);
          }
        }))
        .SetValue("setTimeout", new Func<Func<JsValue, JsValue[], JsValue>, object, double>(SetTimeout)); ;

      _engine.Execute(jsFile);
      _engine.Execute("bmak[\"bpd\"]()");
      var o = _engine.GetValue("bmak").ToObject() as IDictionary<string, object>;

      object sensorData;
      o.TryGetValue("sensor_data", out sensorData);

      return payload;
    }

    private string Atob(string baseText)
    {
      byte[] data = Convert.FromBase64String(baseText);
      string decoded = Encoding.UTF8.GetString(data);
      Console.WriteLine("atob " + decoded);
      return decoded;
    }

    private object[] GetElementsByTagName(string name)
    {
      Console.WriteLine(name);
      if (name == "script")
      {
        return new Script[] { new Script(), new Script() };
      }
      else if (name == "input")
      {
        return new Script[] { new Script(), new Script() };
      }
      {
        return new object[] { "" };
      }
    }

    private void AddEventListener(string eventName, Func<JsValue, JsValue[], JsValue> func)
    {
      Console.WriteLine("addEventListener " + eventName + " " + func);
      if (eventName == "mousemove")
      {
        dynamic mouseMoveEvent = new ExpandoObject();
        mouseMoveEvent.clientX = 544;
        mouseMoveEvent.clientY = 544;
        JsValue mouseMoveEventJs = JsValue.FromObject(_engine, mouseMoveEvent);
        func.Invoke(mouseMoveEventJs, new JsValue[1] { mouseMoveEventJs });
      }
      //try
      //{
      //  func.Invoke(new Jint.Native.JsValue("1"), new Jint.Native.JsValue[0]);
      //}
      //catch
      //{

      //}
    }

    private double SetTimeout(Func<JsValue, JsValue[], JsValue> func, object d)
    {
      double delay = 0;

      if (d != null && d.GetType() == typeof(double))
      {
        delay = (double)d;
      }

      var t = "setTimeout " + delay + " ";

      if (func.Target != null && func.Target.GetType() == typeof(ScriptFunctionInstance))
      {
        var funcTarget = (ScriptFunctionInstance)func.Target;

        t += $"{funcTarget.FormalParameters.Length} ";

        for (int i = 0; i < funcTarget.FormalParameters.Length; i++)
        {
          t += $"{funcTarget.FormalParameters[i]} ";
        }
      }
      else
      {
        t += "no ";
      }

      Console.WriteLine(t);

      if (delay == 0)
      {
        func.Invoke(new JsValue(""), new JsValue[0]);
      }

      return 1;
    }
  }

  public class DataDomeOptions
  {
    public bool abortAsyncOnCaptchaDisplay { get; set; }
    public string ajaxListenerPath { get; set; }
    public bool allowHtmlContentTypeOnCaptcha { get; set; }
    public Delegate check { get; set; }
    public object customParam { get; set; }
    public string ddResponsePage { get; set; }
    public string endpoint { get; set; }
    public bool eventsTrackingEnabled { get; set; }
    public bool exposeCaptchaFunction { get; set; }
    public bool isSalesforce { get; set; }
    public bool overrideAbortFetch { get; set; }
    public string patternToRemoveFromReferrerUrl { get; set; }
    public string version { get; set; }
  }

  public class Document
  {
    public Location location { get; set; }
    public Delegate getElementsByTagName => new Func<string, object[]>(GetElementsByTagName);

    public static object[] GetElementsByTagName(string name)
    {
      Console.WriteLine(name);
      if (name == "script")
      {
        return new Script[] { new Script(), new Script() };
      }
      else if (name == "input")
      {
        return new Script[] { new Script(), new Script() };
      }
      {
        return new object[] { "" };
      }
    }
  }

  public class Location
  {
    public string hash { get; set; }
    public string host { get; set; }
    public string hostname { get; set; }
    public string href { get; set; }
    public string origin { get; set; }
    public string pathname { get; set; }
    public string port { get; set; }
    public string protocol { get; set; }
    public string search { get; set; }
  }

  public class Script
  {
    public Delegate getAttribute => new Action<object>((s) => Console.WriteLine("script.getAttribute " + s));
  }

  public class JsConsole
  {
    public Delegate log => new Action<object>((s) => Console.WriteLine("console.log " + s));
  }
}
