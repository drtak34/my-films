﻿using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using NLog;

namespace Grabber
{
  public class WebGrabber
  {
    #region Private variables
    private static Logger LogMyFilms = LogManager.GetCurrentClassLogger();
    private static int unsafeHeaderUserCount;
    private static object lockingObj;
    private string requestUrl;
    #endregion

    #region Ctor
    static WebGrabber()
    {
      unsafeHeaderUserCount = 0;
      lockingObj = new object();
    }

    public WebGrabber(string url)
    {
      AllowUnsafeHeader = false;
      Debug = false;
      Method = "GET";
      UserAgent = "MF/" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
      TimeoutIncrement = 1000;
      Timeout = 5000;
      MaxRetries = 3;
      requestUrl = url;
      this.Request = (HttpWebRequest)WebRequest.Create(requestUrl);
    }

    public WebGrabber(Uri uri)
    {
      AllowUnsafeHeader = false;
      Debug = false;
      Method = "GET";
      UserAgent = "MF/" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
      TimeoutIncrement = 1000;
      Timeout = 5000;
      MaxRetries = 3;
      requestUrl = uri.OriginalString;
      this.Request = (HttpWebRequest)WebRequest.Create(uri);
    }

    ~WebGrabber()
    {
      this.Request = null;
      if (this.Response != null)
      {
        this.Response.Close();
        this.Response = null;
      }
    }
    #endregion

    #region Public properties
    public HttpWebRequest Request { get; private set; }
    public HttpWebResponse Response { get; private set; }
    public Encoding Encoding { get; set; }
    public int MaxRetries { get; set; }
    public int Timeout { get; set; }
    public int TimeoutIncrement { get; set; }
    public string UserAgent { get; set; }
    public string CookieHeader { get; set; }
    public string Method { get; set; }
    public bool Debug { get; set; }
    public bool AllowUnsafeHeader { get; set; }
    #endregion

    #region Public methods

    public bool GetResponse()
    {
      try
      {

        bool completed = false;
        int tryCount = 0;

        // enable unsafe header parsing if needed
        if (this.AllowUnsafeHeader) SetAllowUnsafeHeaderParsing(true);

        // setup some request properties
        this.Request.Proxy = WebRequest.DefaultWebProxy;
        this.Request.Proxy.Credentials = CredentialCache.DefaultCredentials;
        this.Request.UserAgent = this.UserAgent;
        this.Request.Method = this.Method;
        this.Request.CookieContainer = new CookieContainer();

        while (!completed)
        {
          tryCount++;

          this.Request.Timeout = this.Timeout + (this.TimeoutIncrement * tryCount);
          if (this.CookieHeader != null)
            this.Request.CookieContainer.SetCookies(this.Request.RequestUri, this.CookieHeader.Replace(';', ','));

          try
          {
            this.Response = (HttpWebResponse)this.Request.GetResponse();
            completed = true;
          }
          catch (WebException e)
          {

            // Skip retry logic on protocol errors
            if (e.Status == WebExceptionStatus.ProtocolError)
            {
              HttpStatusCode statusCode = ((HttpWebResponse)e.Response).StatusCode;
              switch (statusCode)
              {
                // Currently the only exception is the service temporarily unavailable status
                // So keep retrying when this is the case
                case HttpStatusCode.ServiceUnavailable:
                  break;
                // all other status codes mostly indicate problems that won't be
                // solved within the retry period so fail these immediatly
                default:
                  LogMyFilms.Error("Connection failed: URL={0}, Status={1}, Description={2}.", requestUrl, statusCode, ((HttpWebResponse)e.Response).StatusDescription);
                  return false;
              }
            }

            // Return when hitting maximum retries.
            if (tryCount == this.MaxRetries)
            {
              LogMyFilms.Warn("Connection failed: Reached retry limit of " + this.MaxRetries + ". URL=" + requestUrl);
              return false;
            }

            // If we did not experience a timeout but some other error
            // use the timeout value as a pause between retries
            if (e.Status != WebExceptionStatus.Timeout)
            {
              Thread.Sleep(this.Timeout + (this.TimeoutIncrement * tryCount));
            }
          }
          catch (NotSupportedException e)
          {
            LogMyFilms.Error("Connection failed.", e);
            return false;
          }
          catch (ProtocolViolationException e)
          {
            LogMyFilms.Error("Connection failed.", e);
            return false;
          }
          catch (InvalidOperationException e)
          {
            LogMyFilms.Error("Connection failed.", e);
            return false;
          }
          finally
          {
            // disable unsafe header parsing if it was enabled
            if (this.AllowUnsafeHeader) SetAllowUnsafeHeaderParsing(false);
          }
        }

        // persist the cookie header
        this.CookieHeader = this.Request.CookieContainer.GetCookieHeader(this.Request.RequestUri);

        // Debug
        if (this.Debug) LogMyFilms.Debug("GetResponse: URL={0}, UserAgent={1}, CookieHeader={3}", requestUrl, this.UserAgent, this.CookieHeader);

        // disable unsafe header parsing if it was enabled
        if (this.AllowUnsafeHeader) SetAllowUnsafeHeaderParsing(false);

        return true;
      }
      catch (Exception e)
      {
        LogMyFilms.Warn("Unexpected error getting http response from '{0}': {1}", requestUrl, e.Message);
        return false;
      }
    }

    public string GetString()
    {
      if (this.Response == null)
        return null;

      // If encoding was not set manually try to detect it
      if (this.Encoding == null)
      {
        try
        {
          // Try to get the encoding using the characterset
          this.Encoding = Encoding.GetEncoding(this.Response.CharacterSet);
        }
        catch (Exception e)
        {
          // If this fails default to the system's default encoding
          LogMyFilms.DebugException("Encoding could not be determined, using default.", e);
          this.Encoding = Encoding.Default;
        }
      }

      // Debug
      if (this.Debug) LogMyFilms.Debug("GetString: Encoding={2}", this.Encoding.EncodingName);

      // Converts the stream to a string
      try
      {
        Stream stream = this.Response.GetResponseStream();
        StreamReader reader = new StreamReader(stream, this.Encoding, true);
        string data = reader.ReadToEnd();
        reader.Close();
        stream.Close();
        this.Response.Close();

        // return the string data
        return data;
      }
      catch (Exception e)
      {
        if (e is ThreadAbortException)
          throw e;

        // There was an error reading the stream
        // todo: might have to retry
        LogMyFilms.ErrorException("Error while trying to read stream data: ", e);
      }

      // return nothing.
      return null;
    }

    public XmlNodeList GetXML()
    {
      return GetXML(null);
    }

    public XmlNodeList GetXML(string rootNode)
    {
      string data = GetString();

      // if there's no data return nothing
      if (String.IsNullOrEmpty(data))
        return null;

      var xml = new XmlDocument();

      // attempts to convert data into an XmlDocument
      try
      {
        xml.LoadXml(data);
      }
      catch (XmlException e)
      {
        LogMyFilms.ErrorException("XML Parse error: URL=" + requestUrl, e);
        return null;
      }

      // get the document root
      XmlElement xmlRoot = xml.DocumentElement;
      if (xmlRoot == null)
        return null;

      // if a root node name is given check for it
      // return null when the root name doesn't match
      if (rootNode != null && xmlRoot.Name != rootNode)
        return null;

      // return the node list
      return xmlRoot.ChildNodes;
    }

    #endregion

    #region Private methods

    //Method to change the AllowUnsafeHeaderParsing property of HttpWebRequest.
    private bool SetAllowUnsafeHeaderParsing(bool setState)
    {
      try
      {
        lock (lockingObj)
        {
          // update our counter of the number of requests needing 
          // unsafe header processing
          if (setState == true) unsafeHeaderUserCount++;
          else unsafeHeaderUserCount--;

          // if there was already a request using unsafe heaser processing, we
          // dont need to take any action.
          if (unsafeHeaderUserCount > 1)
            return true;

          // if the request tried to turn off unsafe header processing but it is
          // still needed by another request, we should wait.
          if (unsafeHeaderUserCount >= 1 && setState == false)
            return true;

          //Get the assembly that contains the internal class
          Assembly aNetAssembly = Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));
          if (aNetAssembly == null)
            return false;

          //Use the assembly in order to get the internal type for the internal class
          Type aSettingsType = aNetAssembly.GetType("System.Net.Configuration.SettingsSectionInternal");
          if (aSettingsType == null)
            return false;

          //Use the internal static property to get an instance of the internal settings class.
          //If the static instance isn't created allready the property will create it for us.
          object anInstance = aSettingsType.InvokeMember("Section",
                                                          BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic,
                                                          null, null, new object[] { });
          if (anInstance == null)
            return false;

          //Locate the private bool field that tells the framework is unsafe header parsing should be allowed or not
          FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
          if (aUseUnsafeHeaderParsing == null)
            return false;

          // and finally set our setting
          aUseUnsafeHeaderParsing.SetValue(anInstance, setState);
          return true;
        }

      }
      catch (Exception e)
      {
        if (e.GetType() == typeof(ThreadAbortException))
          throw e;

        LogMyFilms.Error("Unsafe header parsing setting change failed.");
        return false;
      }
    }

    #endregion
  }
}
