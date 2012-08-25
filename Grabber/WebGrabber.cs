using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using NLog;

namespace Cornerstone.Tools {

    public class WebGrabber {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region Private variables
        private static Logger LogMyFilms = LogManager.GetCurrentClassLogger();
        private static int unsafeHeaderUserCount;
        private static object lockingObj;
        private string requestUrl;

        #endregion

        #region Ctor

        static WebGrabber() {
            unsafeHeaderUserCount = 0;
            lockingObj = new object();
        }

        public WebGrabber(string url) {
            requestUrl = url;
            request = (HttpWebRequest)WebRequest.Create(requestUrl);
        }

        public WebGrabber(Uri uri) {
            requestUrl = uri.OriginalString;
            request = (HttpWebRequest)WebRequest.Create(uri);
        }

        ~WebGrabber() {
            request = null;
            if (response != null) {
                response.Close();
                response = null;
            }
        }

        #endregion

        #region Public properties

        public HttpWebRequest Request {
            get { return request; }
        } private HttpWebRequest request;

        public HttpWebResponse Response {
            get { return response; }
        } private HttpWebResponse response;

        public Encoding Encoding {
            get { return encoding; }
            set { encoding = value; }
        } private Encoding encoding;

        public int MaxRetries {
            get { return maxRetries; }
            set { maxRetries = value; }
        } private int maxRetries = 3;

        public int Timeout {
            get { return timeout; }
            set { timeout = value; }
        } private int timeout = 5000;

        public int TimeoutIncrement {
            get { return timeoutIncrement; }
            set { timeoutIncrement = value; }
        } private int timeoutIncrement = 1000;

        public string UserAgent {
            get { return userAgent; }
            set { userAgent = value; }
        } private string userAgent = "MF/" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string CookieHeader {
            get { return cookieHeader; }
            set { cookieHeader = value; }
        } private string cookieHeader;

        public string Method {
            get { return _method; }
            set { _method = value; }
        } private string _method = "GET";

        public bool Debug {
            get { return _debug; }
            set { _debug = value; }
        } private bool _debug = false;

        public bool AllowUnsafeHeader {
            get { return _allowUnsafeHeader; }
            set { _allowUnsafeHeader = value; }
        } private bool _allowUnsafeHeader = false;

        #endregion

        #region Public methods

        public bool GetResponse() {
            try {

                bool completed = false;
                int tryCount = 0;

                // enable unsafe header parsing if needed
                if (_allowUnsafeHeader) SetAllowUnsafeHeaderParsing(true);

                // setup some request properties
                request.Proxy = WebRequest.DefaultWebProxy;
                request.Proxy.Credentials = CredentialCache.DefaultCredentials;
                request.UserAgent = userAgent;
                request.Method = _method;
                request.CookieContainer = new CookieContainer();

                while (!completed) {
                    tryCount++;

                    request.Timeout = timeout + (timeoutIncrement * tryCount);
                    if (cookieHeader != null)
                        request.CookieContainer.SetCookies(request.RequestUri, cookieHeader.Replace(';', ','));

                    try {
                        response = (HttpWebResponse)request.GetResponse();
                        completed = true;
                    }
                    catch (WebException e) {

                        // Skip retry logic on protocol errors
                        if (e.Status == WebExceptionStatus.ProtocolError) {
                            HttpStatusCode statusCode = ((HttpWebResponse)e.Response).StatusCode;
                            switch (statusCode) {
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
                        if (tryCount == maxRetries) {
                            LogMyFilms.Warn("Connection failed: Reached retry limit of " + maxRetries + ". URL=" + requestUrl);
                            return false;
                        }

                        // If we did not experience a timeout but some other error
                        // use the timeout value as a pause between retries
                        if (e.Status != WebExceptionStatus.Timeout) {
                            Thread.Sleep(timeout + (timeoutIncrement * tryCount));
                        }
                    }
                    catch (NotSupportedException e) {
                        LogMyFilms.Error("Connection failed.", e);
                        return false;
                    }
                    catch (ProtocolViolationException e) {
                        LogMyFilms.Error("Connection failed.", e);
                        return false;
                    }
                    catch (InvalidOperationException e) {
                        LogMyFilms.Error("Connection failed.", e);
                        return false;
                    }
                    finally {
                        // disable unsafe header parsing if it was enabled
                        if (_allowUnsafeHeader) SetAllowUnsafeHeaderParsing(false);
                    }
                }

                // persist the cookie header
                cookieHeader = request.CookieContainer.GetCookieHeader(request.RequestUri);

                // Debug
                if (_debug) LogMyFilms.Debug("GetResponse: URL={0}, UserAgent={1}, CookieHeader={3}", requestUrl, userAgent, cookieHeader);

                // disable unsafe header parsing if it was enabled
                if (_allowUnsafeHeader) SetAllowUnsafeHeaderParsing(false);

                return true;
            }
            catch (Exception e) {
                LogMyFilms.Warn("Unexpected error getting http response from '{0}': {1}", requestUrl, e.Message);
                return false;
            }
        }

        public string GetString() {
            if (response == null)
                return null;

            // If encoding was not set manually try to detect it
            if (encoding == null) {
                try {
                    // Try to get the encoding using the characterset
                    encoding = Encoding.GetEncoding(response.CharacterSet);
                }
                catch (Exception e) {
                    // If this fails default to the system's default encoding
                    LogMyFilms.DebugException("Encoding could not be determined, using default.", e);
                    encoding = Encoding.Default;
                }
            }

            // Debug
            if (_debug) LogMyFilms.Debug("GetString: Encoding={2}", encoding.EncodingName);

            // Converts the stream to a string
            try {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, encoding, true);
                string data = reader.ReadToEnd();
                reader.Close();
                stream.Close();
                response.Close();

                // return the string data
                return data;
            }
            catch (Exception e) {
                if (e.GetType() == typeof(ThreadAbortException))
                    throw e;

                // There was an error reading the stream
                // todo: might have to retry
                LogMyFilms.ErrorException("Error while trying to read stream data: ", e);
            }

            // return nothing.
            return null;
        }

        public XmlNodeList GetXML() {
            return GetXML(null);
        }

        public XmlNodeList GetXML(string rootNode) {
            string data = GetString();
            
            // if there's no data return nothing
            if (String.IsNullOrEmpty(data))
                return null;

            XmlDocument xml = new XmlDocument();

            // attempts to convert data into an XmlDocument
            try {
                xml.LoadXml(data);
            }
            catch (XmlException e) {
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
        private bool SetAllowUnsafeHeaderParsing(bool setState) {
            try {
                lock (lockingObj) {
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
            catch (Exception e) {
                if (e.GetType() == typeof(ThreadAbortException))
                    throw e;

                LogMyFilms.Error("Unsafe header parsing setting change failed.");
                return false;
            }
        }

        #endregion
    }

    public class AdvancedStringComparer
    {
      private static Logger logger = LogManager.GetCurrentClassLogger();

      // Calculates the Levenshtein Distance between two strings. The least number of 
      // changes to make one string equal to the other. Useful for finding 
      // close matches.
      //
      // Thanks to Sten Hjelmqvist from codeproject.com for this algorithm.
      // http://www.codeproject.com/cs/algorithms/Levenshtein.asp
      //
      public static int Levenshtein(String strA, String strB)
      {
        // if string is null treat it as an empty string
        if (strA == null) strA = String.Empty;
        if (strB == null) strB = String.Empty;

        int RowLen = strA.Length;  // length of sRow
        int ColLen = strB.Length;  // length of sCol
        int RowIdx;                // iterates through sRow
        int ColIdx;                // iterates through sCol
        char Row_i;                // ith character of sRow
        char Col_j;                // jth character of sCol
        int cost;                   // cost

        // Test string length
        if (Math.Max(strA.Length, strB.Length) > Math.Pow(2, 31))
          throw (new Exception("\nMaximum string length in Levenshtein.iLD is " + Math.Pow(2, 31) + ".\nYours is " + Math.Max(strA.Length, strB.Length) + "."));

        // Step 1

        if (RowLen == 0)
        {
          return ColLen;
        }

        if (ColLen == 0)
        {
          return RowLen;
        }

        // Create the two vectors
        int[] v0 = new int[RowLen + 1];
        int[] v1 = new int[RowLen + 1];
        int[] vTmp;



        // Step 2
        // Initialize the first vector
        for (RowIdx = 1; RowIdx <= RowLen; RowIdx++)
        {
          v0[RowIdx] = RowIdx;
        }

        // Step 3

        // Fore each column
        for (ColIdx = 1; ColIdx <= ColLen; ColIdx++)
        {
          // Set the 0'th element to the column number
          v1[0] = ColIdx;

          Col_j = strB[ColIdx - 1];


          // Step 4

          // Fore each row
          for (RowIdx = 1; RowIdx <= RowLen; RowIdx++)
          {
            Row_i = strA[RowIdx - 1];


            // Step 5

            cost = Row_i == Col_j ? 0 : 1;

            // Step 6

            // Find minimum
            int m_min = v0[RowIdx] + 1;
            int b = v1[RowIdx - 1] + 1;
            int c = v0[RowIdx - 1] + cost;

            if (b < m_min)
            {
              m_min = b;
            }
            if (c < m_min)
            {
              m_min = c;
            }

            v1[RowIdx] = m_min;
          }

          // Swap the vectors
          vTmp = v0;
          v0 = v1;
          v1 = vTmp;

        }

        // Step 7

        // The vectors where swaped one last time at the end of the last loop,
        // that is why the result is now in v0 rather than in v1
        return v0[RowLen];
      }
    }

}
