﻿using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Cornerstone.Tools {

    public class WebGrabber {
        
        private string requestUrl;
        private string data;

        public WebGrabber(string url) {
            requestUrl = url;
            request = (HttpWebRequest)WebRequest.Create(requestUrl);
        }

        ~WebGrabber() {
            data = null;
            request = null;
            if (response != null) {
                response.Close();
                response = null;
            }
        }

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
        } private int timeout = 6000;

        public int TimeoutIncrement {
            get { return timeoutIncrement; }
            set { timeoutIncrement = value; }
        } private int timeoutIncrement = 1500;

        public string UserAgent {
            get { return userAgent; }
            set { userAgent = value; }
        } private string userAgent = "Cornerstone/" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string CookieHeader {
            get { return cookieHeader; }
            set { cookieHeader = value; }
        } private string cookieHeader;

        public bool Debug {
            get { return _debug; }
            set { _debug = value; }
        } private bool _debug;

        public bool GetResponse() {
            data = string.Empty;
            int tryCount = 0;
            while (data == string.Empty) {
                tryCount++;
                try {
                    request.UserAgent = userAgent;
                    request.Timeout = timeout + (timeoutIncrement * tryCount);
                    request.CookieContainer = new CookieContainer();
                    if (cookieHeader != null)
                        request.CookieContainer.SetCookies(request.RequestUri, cookieHeader);
                    response = (HttpWebResponse)request.GetResponse();
                    cookieHeader = request.CookieContainer.GetCookieHeader(request.RequestUri);

                    // Get result as stream
                    Stream resultData = response.GetResponseStream();

                    // Detect or force encoding
                    if (encoding == null)
                        encoding = Encoding.GetEncoding(response.CharacterSet);

                    //// Debug
                    //if (_debug) {
                    //   System..Log.Debug("URL: {0}", requestUrl);
                    //    log.Debug("UserAgent: {0}", userAgent);
                    //    logger.Debug("CookieHeader: {0}", cookieHeader);
                    //    logger.Debug("Encoding: {0}", encoding.EncodingName);
                    //}

                    // Converts the stream to a string
                    StreamReader reader = new StreamReader(resultData, encoding, true);
                    data = reader.ReadToEnd(); 

                    // Close stream and response objects
                    resultData.Close();
                    reader.Close();
                    response.Close();                
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
                                //logger.Error("Connection failed: URL={0}, Status={1}, Description={2}.", requestUrl, statusCode, ((HttpWebResponse)e.Response).StatusDescription);
                                return false;
                        }
                    }
                    
                    // Return when hitting maximum retries.
                    if (tryCount == maxRetries) {
                        //logger.ErrorException("Connection failed: Reached retry limit of " + maxRetries + ". URL=" + requestUrl, e);
                        return false;
                    }
                }
            }
            return true;
        }

        public string GetString() {
            return data;
        }

        public XmlNodeList GetXML() {
            return GetXML(null);
        }

        public XmlNodeList GetXML(string rootNode) {
            // if there's no data return nothing
            if (String.IsNullOrEmpty(data))
                return null;

            // attempts to convert data into an XmlDocument
            try {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(data);
                XmlNode xmlRoot = xml.FirstChild.NextSibling;

                // if a root node name is given check for it
                // return null when the root name doesn't match
                if (rootNode != null)
//                    if (xmlRoot.Name != rootNode)
                    if (!xmlRoot.HasChildNodes)
                        return null;

                // return the node list
                return xmlRoot.ChildNodes;
            }
            catch (XmlException) {
                //logger.ErrorException("XML Parse error: URL=" + requestUrl, e);
                return null;
            }          
        }
    }
}
