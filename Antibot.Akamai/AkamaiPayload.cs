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

    public HttpContent CreatePayload(string jsFile, string scriptSourceUrl, string targetUrl, string userAgent)
    {
      Dictionary<string, string> form = new Dictionary<string, string>();
      form.Add("sensor_data", CreateSensorData(jsFile, scriptSourceUrl, targetUrl, userAgent));
      return new FormUrlEncodedContent(form);
    }

    private string CreateSensorData(string jsFile, string scriptSourceUrl, string targetUrl, string userAgent)
    {
      var targetUri = new Uri(targetUrl);

      dynamic location = new ExpandoObject();
      location.hash = "";
      location.host = targetUri.Host;
      location.hostname = targetUri.Host;
      location.href = targetUri.OriginalString;
      location.origin = targetUri.Scheme + "://" + targetUri.Host;
      location.pathname = targetUri.LocalPath;
      location.port = "";
      location.protocol = targetUri.Scheme + ":";
      location.search = "";

      dynamic currentScript = new ExpandoObject();
      currentScript.src = scriptSourceUrl;

      dynamic documentElement = new ExpandoObject();
      documentElement.getAttribute = new Func<string, object>(GetAttribute);

      dynamic document = new ExpandoObject();
      document.location = location;
      document.getElementsByTagName = new Func<string, object[]>(GetElementsByTagName);
      document.addEventListener = new Action<string, Func<JsValue, JsValue[], JsValue>>(AddEventListener);
      document.currentScript = currentScript;
      document.createElement = new Func<string, object>(CreateElement);
      document.URL = targetUri.OriginalString;
      document.documentElement = documentElement;

      dynamic navigator = new ExpandoObject();
      navigator.appCodeName = "Mozilla";
      navigator.appName = "Netscape";
      navigator.cookieEnabled = true;
      navigator.deviceMemory = 8;
      navigator.doNotTrack = "1";
      navigator.hardwareConcurrency = 4;
      navigator.language = "de-DE";
      navigator.product = "Gecko";
      navigator.productSub = "20030107";
      navigator.userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.106 Safari/537.36";
      navigator.vendor = "Google Inc.";
      navigator.webdriver = false;

      dynamic screenOrientation = new ExpandoObject();
      screenOrientation.angle = 0;
      screenOrientation.type = "landscape-primary";

      dynamic screen = new ExpandoObject();
      screen.availHeight = 1040;
      screen.availLeft = 0;
      screen.availTop = 0;
      screen.availWidth = 1920;
      screen.colorDepth = 24;
      screen.height = 1080;
      screen.orientation = screenOrientation;
      screen.pixelDepth = 24;
      screen.width = 1920;

      dynamic window = new ExpandoObject();
      window.location = location;
      window.navigator = navigator;
      window.document = document;
      window.screen = screen;
      window.mozInnerScreenY = 0;
      window.innerWidth = 762;
      window.innerHeight = 937;
      window.outerWidth = 1920;
      window.outerHeight = 1040;

      dynamic math = new ExpandoObject();
      math.imul = true;
      math.hypot = true;
      math.abs = new Func<double, double>(Math.Abs);
      math.acos = new Func<double, double>(Math.Acos);
      math.asin = new Func<double, double>(Math.Asin);
      math.atanh = new Func<double, double>(Math.Atanh);
      math.cbrt = new Func<double, double>(Math.Cbrt);
      math.exp = new Func<double, double>(Math.Exp);
      math.random = new Func<double>(MathRandom);
      math.round = new Func<double, int>(MathRound);
      math.sqrt = new Func<double, double>(Math.Sqrt);
      math.floor = new Func<double, double>(Math.Floor);

      dynamic performance = new ExpandoObject();
      performance.now = new Func<double>(PerformanceNow);

      dynamic json = new ExpandoObject();
      json.parse = new Func<string, ExpandoObject>(JsonParse);

      _engine = new Engine()
        .SetValue("window", window)
        .SetValue("document", document)
        .SetValue("location", location)
        .SetValue("navigator", navigator)
        .SetValue("Math", math)
        .SetValue("performance", performance)
        .SetValue("JSON", json)
        .SetValue("isFinite", new Func<double, bool>(IsFinite))
        .SetValue("isNaN", new Func<double, bool>(IsNan))
        .SetValue("parseFloat", new Func<string, double>(ParseFloat))
        .SetValue("parseInt", new Func<string, int>(ParseInt))
        .SetValue("console", new JsConsole())
        .SetValue("setTimeout", new Func<Func<JsValue, JsValue[], JsValue>, object, double>(SetTimeout))
        .SetValue("setInterval", new Func<Func<JsValue, JsValue[], JsValue>, object, double>(SetInterval));

      _engine.Execute(jsFile);
      _engine.Execute("bmak[\"bpd\"]()");

      var bmak = _engine.GetValue("bmak").ToObject() as IDictionary<string, object>;

      object sensorData;
      bmak.TryGetValue("sensor_data", out sensorData);

      if (sensorData != null && sensorData.GetType() == typeof(string))
      {
        return sensorData as string;
      }
      else
      {
        return null;
      }
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

    private double SetInterval(Func<JsValue, JsValue[], JsValue> func, object d)
    {
      Console.WriteLine("setInterval " + func + " " + d);
      return 1;
    }

    private object CreateElement(string name)
    {
      Console.WriteLine("createElement " + name);
      return null;
    }

    private object GetAttribute(string name)
    {
      if (name == "webdriver" || name == "driver" || name == "selenium")
      {
        return null;
      }
      else
      {
        Console.WriteLine("getAttribute " + name);
        return null;
      }
    }
  
    private double MathRandom()
    {
      var r = new Random();
      return r.NextDouble();
    }

    private int MathRound(double value)
    {
      return (int)Math.Round(value, 0);
    }

    private double PerformanceNow()
    {
      return 2428136.1000000006;
    }

    private bool IsFinite(double value)
    {
      Console.WriteLine("isFinite" + value);
      return true;
    }

    private bool IsNan(double value)
    {
      Console.WriteLine("isNan" + value);
      return true;
    }

    private double ParseFloat(string value)
    {
      Console.WriteLine("parseFloat" + value);
      return double.Parse(value);
    }

    private int ParseInt(string value)
    {
      Console.WriteLine("parseInt" + value);
      double valueDouble = double.Parse(value);
      return (int)Math.Round(valueDouble, 0);
    }

    private ExpandoObject JsonParse(string value)
    {
      Console.WriteLine("JSON.parse" + value);
      return null;
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
