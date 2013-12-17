using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net;
using System.Web;
using System.Xml;
using System.Linq;
using System.IO;
using System.Text;

using MediaPortal.Util;
using MediaPortal.GUI.Library;
using MediaPortal.Configuration;
using MediaPortal.Services;

using NLog;
using grabber;

namespace Grabber
{
  public class GrabberScript
  {
    string m_strFilename = "";

    // XmlNode Node;

    public void Load(string strFilename)
    {
      //Loading the configuration file
      var doc = new XmlDocument();
      doc.Load(strFilename);
      XmlNode n = doc.ChildNodes[1].FirstChild;

      try { this.DBName = n.SelectSingleNodeFast("DBName").InnerText; }
      catch (Exception) { this.URLPrefix = ""; }

      try { this.URLPrefix = n.SelectSingleNodeFast("URLPrefix").InnerText; }
      catch (Exception) { this.URLPrefix = ""; }

      try { this.Language = n.SelectSingleNodeFast("Language").InnerText; }
      catch (Exception) { this.Language = ""; }

      try { this.Type = n.SelectSingleNodeFast("Type").InnerText; }
      catch (Exception) { this.Type = ""; }

      try { this.Version = n.SelectSingleNodeFast("Version").InnerText; }
      catch (Exception) { this.Version = ""; }

      try { this.Encoding = n.SelectSingleNodeFast("Encoding").InnerText; }
      catch (Exception) { this.Encoding = ""; }
    }

    public GrabberScript(string strFilename)
    {
      DBName = "";
      Language = "";
      Type = "";
      Encoding = "";
      Version = "";
      URLPrefix = "";
      m_strFilename = strFilename;
      if (!string.IsNullOrEmpty(m_strFilename))
        Load(m_strFilename);
    }

    public string DBName { get; set; }
    public string URLPrefix { get; set; }
    public string Language { get; set; }
    public string Type { get; set; }
    public string Version { get; set; }
    public string Encoding { get; set; }

    //public XmlNode URLSearch
    //{
    //  get { return Node; }
    //  set { Node = value; }
    //}

    //public XmlNode Details
    //{
    //  get { return Node; }
    //  set { Node = value; }
    //}

    //public XmlNode Mapping
    //{
    //  get { return Node; }
    //  set { Node = value; }
    //}
  };

  public class Grabber_URLClass
  {
    private static Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    ArrayList elements = new ArrayList();

    public enum Grabber_Output
    {
      OriginalTitle = 0,
      TranslatedTitle = 1,
      PicturePathLong = 2,
      Description = 3,
      Rating = 4,
      Actors = 5,
      Director = 6,
      Producer = 7,
      Year = 8,
      Country = 9,
      Category = 10,
      URL = 11,
      PicturePathShort = 12,
      Writer = 13,
      Comments = 14,
      Language = 15,
      Tagline = 16,
      Certification = 17,
      IMDB_Id = 18,
      IMDB_Rank = 19,
      Studio = 20,
      Edition = 21,
      Fanart = 22,
      Generic1 = 23,
      Generic2 = 24,
      Generic3 = 25,
      TranslatedTitleAllNames = 26,
      TranslatedTitleAllValues = 27,
      CertificationAllNames = 28,
      CertificationAllValues = 29,

      MultiPosters = 30,
      Photos = 31,
      PersonImages = 32,
      MultiFanart = 33,
      Trailer = 34,
      TMDB_Id = 35,
      Runtime = 36,
      Collection = 37,
      CollectionImage = 38,
      PictureURL = 39,

      OriginalTitle_Transformed = 40,
      TranslatedTitle_Transformed = 41,
      PicturePathLong_Transformed = 42,
      Description_Transformed = 43,
      Rating_Transformed = 44,
      Actors_Transformed = 45,
      Director_Transformed = 46,
      Producer_Transformed = 47,
      Year_Transformed = 48,
      Country_Transformed = 49,
      Category_Transformed = 50,
      URL_Transformed = 51,
      PicturePathShort_Transformed = 52,
      Writer_Transformed = 53,
      Comments_Transformed = 54,
      Language_Transformed = 55,
      Tagline_Transformed = 56,
      Certification_Transformed = 57,
      IMDB_Id_Transformed = 58,
      IMDB_Rank_Transformed = 59,
      Studio_Transformed = 60,
      Edition_Transformed = 61,
      Fanart_Transformed = 62,
      Generic1_Transformed = 63,
      Generic2_Transformed = 64,
      Generic3_Transformed = 65,
      TranslatedTitleAllNames_Transformed = 66,
      TranslatedTitleAllValues_Transformed = 67,
      CertificationAllNames_Transformed = 68,
      CertificationAllValues_Transformed = 69,
      MultiPosters_Transformed = 70,
      Photos_Transformed = 71,
      PersonImages_Transformed = 72,
      MultiFanart_Transformed = 73,
      Trailer_Transformed = 74,
      TMDB_Id_Transformed = 75,
      Runtime_Transformed = 76,
      Collection_Transformed = 77,
      CollectionImage_Transformed = 78,
      PictureURL_Transformed = 79
    }

    public static List<string> FieldList()
    {
      var list = new List<string>();
      list.Add("OriginalTitle");
      list.Add("TranslatedTitle");
      list.Add("Picture");
      list.Add("Description");
      list.Add("Rating");
      list.Add("Actors");
      list.Add("Director");
      list.Add("Producer");
      list.Add("Year");
      list.Add("Country");
      list.Add("Category");
      list.Add("URL Redirection Picture");
      list.Add("ImageURL");
      list.Add("Writer");
      list.Add("Comment");
      list.Add("Language");
      list.Add("Tagline");
      list.Add("Certification");
      list.Add("IMDB_Id");
      list.Add("IMDB_Rank");
      list.Add("Studio");
      list.Add("Edition");
      list.Add("Fanart");
      list.Add("Generic1");
      list.Add("Generic2");
      list.Add("Generic3");
      list.Add("TranslatedTitle - All Names");
      list.Add("TranslatedTitle - All Values");
      list.Add("Certification - All Names");
      list.Add("Certification - All Values");
      list.Add("MultiPosters");
      list.Add("Photos");
      list.Add("PersonImages");
      list.Add("MultiFanart");
      list.Add("Trailer");
      list.Add("TMDB_Id");
      list.Add("Runtime");
      list.Add("Collection");
      list.Add("CollectionImageURL");
      list.Add("PictureURL");
      return list;
    }

    #region internal vars

    // list of the search results, containts objects of IMDBUrl
    private ArrayList _elements = new ArrayList();

    private string BodyDetail2 = string.Empty;
    private string BodyLinkGeneric1 = string.Empty;
    private string BodyLinkGeneric2 = string.Empty;
    private string BodyLinkImg = string.Empty;
    private string BodyLinkPersons = string.Empty;
    private string BodyLinkTitles = string.Empty;
    private string BodyLinkCertification = string.Empty;
    private string BodyLinkComment = string.Empty;
    private string BodyLinkSyn = string.Empty;
    private string BodyLinkMultiPosters = string.Empty;
    private string BodyLinkPhotos = string.Empty;
    private string BodyLinkPersonImages = string.Empty;
    private string BodyLinkMultiFanart = string.Empty;
    private string BodyLinkTrailer = string.Empty;

    private string BodyDetail = string.Empty;
    private string BodyLinkDetailsPath = string.Empty;

    private string MovieDirectory = string.Empty;
    private string MovieFilename = string.Empty;

    #endregion

    public ArrayList ReturnURL(string strSearch, string strConfigFile, int strPage, bool alwaysAsk, string strMediaPath)
    {
      string strFileBasedReader = string.Empty;
      string strURLFile = string.Empty;
      string strDBName = string.Empty;
      var doc = new XmlDocument();
      doc.Load(strConfigFile);
      XmlNode n = doc.ChildNodes[1].FirstChild;
      try { strDBName = XmlConvert.DecodeName(n.SelectSingleNodeFast("DBName").InnerText); }
      catch { strDBName = "ERROR"; }
      try { strURLFile = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/URL").InnerText); }
      catch { strURLFile = ""; }
      try { strFileBasedReader = XmlConvert.DecodeName(n.SelectSingleNodeFast("FileBasedReader").InnerText); }
      catch { strFileBasedReader = "false"; }
      if (strFileBasedReader == "true" || strSearch.Contains("\\")) // if a mediapath is given and file name is part of the search expression... assume it's nfo/xml/xbmc reader request and return the proper file to read in details
      {
        #region read from file - mediapath

        if (string.IsNullOrEmpty(strMediaPath) && strSearch.Contains("\\")) strMediaPath = strSearch;

        elements.Clear();
        if (!string.IsNullOrEmpty(strURLFile))
        {
          try
          {
            string directory = Path.GetDirectoryName(strMediaPath); // get directory name of media file
            string filename = Path.GetFileNameWithoutExtension(strMediaPath); // get filename without extension
            //strSearch = GrabUtil.encodeSearch(strSearch);
            strURLFile = strURLFile.Replace("#Filename#", filename);
            string[] files = Directory.GetFiles(directory, strURLFile, SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
              string fileShortname = Path.GetFileName(file);
              var url = new IMDBUrl(file, fileShortname, strDBName, n, "1", "", "", "", "", "", "", "", "");
              LogMyFilms.Debug("ReturnURL() - Found and adding nfo file '" + fileShortname + "', fullpath = '" + file + "'");
              elements.Add(url);
            }
          }
          catch (Exception e)
          {
            LogMyFilms.DebugException("ReturnURL() - Catched Exception: " + e.Message, e);
          }
        }
        #endregion
        return elements;
      }

      // if no local grabbing, do web grabbing:
      if (strPage == -1)
      {
        // First run, finding the key starting page number
        //Loading the configuration file
        //XmlDocument doc = new XmlDocument();
        //doc.Load(strConfigFile);
        //XmlNode n = doc.ChildNodes[1].FirstChild;
        //Gets Key to the first page if it exists (not required)
        try
        { strPage = Convert.ToInt16(XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartPage").InnerText)); }
        catch
        { strPage = 1; }
      }
      var grab = new Grabber_URLClass();
      Int16 wIndex;
      do
      {
        if (strSearch.LastIndexOf(".", System.StringComparison.Ordinal) == strSearch.Length - 1)
          strSearch = strSearch.Substring(0, strSearch.Length - 1);
        else
          break;
      } while (true);
      grab.FindMovies(strSearch, strConfigFile, strPage, alwaysAsk, out elements, out wIndex);
      if (wIndex >= 0)
      {
        var wurl = (IMDBUrl)elements[wIndex];
        elements.Clear();
        elements.Add(wurl);
      }
      return elements;
    }

    public class IMDBUrl
    {
      // Guzzi Added for better matching internet searches:

      public IMDBUrl(string strURL, string strTitle, string strDB, XmlNode pNode)
      {
        Thumb = "";
        Akas = "";
        Options = "";
        ID = "";
        TMDB_ID = "";
        IMDB_ID = "";
        Director = "";
        Year = "";
        this.URL = strURL;
        this.Title = strTitle;
        this.Database = strDB;
        this.CurNode = pNode;
        this.IMDBURL = string.Empty;
      }
      public IMDBUrl(string strURL, string strTitle, string strDB, XmlNode pNode, string strIMDBURL)
      {
        Thumb = "";
        Akas = "";
        Options = "";
        ID = "";
        TMDB_ID = "";
        IMDB_ID = "";
        Director = "";
        Year = "";
        this.URL = strURL;
        this.Title = strTitle;
        this.Database = strDB;
        this.CurNode = pNode;
        this.IMDBURL = strIMDBURL;
      }
      public IMDBUrl(string strURL, string strTitle, string strDB, XmlNode pNode, string strIMDBURL, string strYear, string strDirector, string strIMDB_Id, string strTMDB_Id, string strID, string strOptions, string strAkas, string strThumb)
      {
        this.URL = strURL;
        this.Title = strTitle;
        this.Database = strDB;
        this.CurNode = pNode;
        this.IMDBURL = strIMDBURL;
        this.Year = strYear;
        this.Director = strDirector;
        this.IMDB_ID = strIMDB_Id;
        this.TMDB_ID = strTMDB_Id;
        this.ID = strID;
        this.Options = strOptions;
        this.Akas = strAkas;
        this.Thumb = strThumb;
      }

      public IMDBUrl(string strURL, string strTitle, string strDB)
      {
        IMDBURL = "";
        Thumb = "";
        Akas = "";
        Options = "";
        ID = "";
        TMDB_ID = "";
        IMDB_ID = "";
        Director = "";
        Year = "";
        URL = strURL;
        Title = strTitle;
        Database = strDB;
      }

      public string URL { get; set; }
      public string Title { get; set; }
      public string Database { get; set; }
      public string IMDBURL { get; set; }
      public XmlNode CurNode { get; set; }
      public string Year { get; set; }
      public string Director { get; set; }
      public string IMDB_ID { get; set; }
      public string TMDB_ID { get; set; }
      public string ID { get; set; }
      public string Options { get; set; }
      public string Akas { get; set; }
      public string Thumb { get; set; }
    };

    public void FindMovies(string strSearchInit, string strConfigFile, int strPage, bool AlwaysAsk, out ArrayList ListUrl, out short WIndex)
    {
      #region variables
      WIndex = -1;
      string strSearch = strSearchInit;
      string strTemp = string.Empty;
      string strBody = string.Empty;
      string strItem = string.Empty;
      string strURL;
      string strEncoding = string.Empty;
      string strSearchCleanup = string.Empty;
      string strAccept = string.Empty;
      string strUserAgent = string.Empty;
      string strHeaders = string.Empty;
      string strLanguage = string.Empty;
      string strType = string.Empty;
      string strVersion = string.Empty;
      string strStart = string.Empty;
      string strEnd = string.Empty;
      string strNext = string.Empty;
      string absoluteUri;
      string strStartItem = string.Empty; // selected item for grabbing
      string strStartTitle = string.Empty;
      string strEndTitle = string.Empty;
      string strStartYear = string.Empty;
      string strEndYear = string.Empty;
      string strStartDirector = string.Empty;
      string strEndDirector = string.Empty;
      string strStartLink = string.Empty;
      string strEndLink = string.Empty;
      string strStartID = string.Empty;
      string strEndID = string.Empty;
      string strStartOptions = string.Empty;
      string strEndOptions = string.Empty;
      string strStartAkas = string.Empty;
      string strEndAkas = string.Empty;
      string strKeyAkasRegExp = string.Empty;
      string strStartThumb = string.Empty;
      string strEndThumb = string.Empty;

      string strTitle = string.Empty;
      string strYear = string.Empty;
      string strDirector = string.Empty;
      string strID = string.Empty;
      string strOptions = string.Empty;
      string strAkas = string.Empty;
      string strThumb = string.Empty;

      string strIMDB_Id = string.Empty;
      string strTMDB_Id = string.Empty;
      string strLink = string.Empty;
      string strDBName;
      string strStartPage = string.Empty;
      int wStepPage = 0;
      int iFinItem = 0;
      int iStartItemLength = 0;
      int iStartTitle = 0;
      int iStartYear = 0;
      int iStartDirector = 0;
      int iStartID = 0;
      int iStartOptions = 0;
      int iStartAkas = 0;
      int iStartThumb = 0;
      int iStartUrl = 0;
      int iStart = 0;
      int iEnd = 0;
      int iLength = 0;
      string strRedir = string.Empty;
      string strParam1 = string.Empty;
      string strParam2 = string.Empty;
      #endregion

      elements.Clear();

      MediaPortal.Util.Utils.RemoveStackEndings(ref strSearchInit);

      #region Regex creation with name of movie file
      byte[] bytes = System.Text.Encoding.GetEncoding(1251).GetBytes(strSearchInit.ToLower());
      string file = System.Text.Encoding.ASCII.GetString(bytes);
      file = MediaPortal.Util.Utils.FilterFileName(file);
      file = file.Replace("-", " ");
      file = file.Replace("+", " ");
      file = file.Replace("!", " ");
      file = file.Replace("#", " ");
      file = file.Replace(";", " ");
      file = file.Replace(".", " ");
      file = file.Replace(",", " ");
      file = file.Replace("=", " ");
      file = file.Replace("&", " ");
      file = file.Replace("(", " ");
      file = file.Replace(")", " ");
      file = file.Replace("@", " ");
      file = file.Replace("%", " ");
      file = file.Replace("$", " ");
      file = file.Replace(":", " ");
      file = file.Replace("_", " ");
      file = file.Trim();
      var oRegex = new Regex(" +");
      file = oRegex.Replace(file, ":");
      file = file.Replace(":", ".*");
      oRegex = new Regex(file);
      #endregion

      #region Loading the configuration file
      var doc = new XmlDocument();
      doc.Load(strConfigFile);
      XmlNode n = doc.ChildNodes[1].FirstChild;

      strDBName = n.SelectSingleNodeFast("DBName").InnerText;
      if (strDBName.ToLower().StartsWith("ofdb") && strSearchInit.Length > 3) // Optimization for searches with ofdb
      {
        string strLeft = "";
        strLeft = strSearchInit.Substring(0, 3);
        if (strLeft.ToLower().Contains("der") || strLeft.ToLower().Contains("die") || strLeft.ToLower().Contains("das") || strLeft.ToLower().Contains("the"))
        {
          strSearchInit = strSearchInit.Substring(3).Trim() + ", " + strLeft.Trim();
          strSearch = strSearchInit;
        }
      }

      // retrieve manual encoding override, if any
      try { strEncoding = n.SelectSingleNodeFast("Encoding").InnerText; }
      catch (Exception) { strEncoding = ""; }

      try // retrieve language, if any
      { strLanguage = n.SelectSingleNodeFast("Language").InnerText; }
      catch (Exception) { strLanguage = ""; }

      try // retrieve type, if any
      { strType = n.SelectSingleNodeFast("Type").InnerText; }
      catch (Exception) { strType = ""; }

      try // retrieve version, if any
      { strVersion = n.SelectSingleNodeFast("Version").InnerText; }
      catch (Exception) { strVersion = ""; }

      try // retrieve SearchCleanupDefinition, if any
      { strSearchCleanup = n.SelectSingleNodeFast("SearchCleanup").InnerText; }
      catch (Exception) { strSearchCleanup = ""; }

      try // retrieve SearchCleanupDefinition, if any
      { strAccept = n.SelectSingleNodeFast("Accept").InnerText; }
      catch (Exception) { strAccept = ""; }

      try // retrieve SearchCleanupDefinition, if any
      { strUserAgent = n.SelectSingleNodeFast("UserAgent").InnerText; }
      catch (Exception) { strUserAgent = ""; }

      try // retrieve SearchCleanupDefinition, if any
      { strHeaders = n.SelectSingleNodeFast("Headers").InnerText; }
      catch (Exception) { strHeaders = ""; }
      #endregion

      #region Retrieves the URL
      strURL = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/URL").InnerText);
      strRedir = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/URL").Attributes["Param1"].InnerText);

      strSearch = GrabUtil.CleanupSearch(strSearch, strSearchCleanup); // process SearchCleanup
      strSearch = GrabUtil.encodeSearch(strSearch, strEncoding); // Process Encoding of Search Expression

      strURL = strURL.Replace("#Search#", strSearch);

      //Retrieves the identifier of the next page
      strNext = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyNextPage").InnerText);
      strNext = strNext.Replace("#Search#", strSearch);
      #endregion

      #region Load number of first and following pages
      //Récupère Le n° de la première page
      strStartPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartPage").InnerText);
      //Récupère Le step de page
      try { wStepPage = Convert.ToInt16(XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStepPage").InnerText)); }
      catch { wStepPage = 1; }

      int wpage = strPage;
      int wpagedeb;
      int wpageprev = 0;
      //Fetch The No. of the first page
      try { wpagedeb = Convert.ToInt16(XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartPage").InnerText)); }
      catch { wpagedeb = 1; }
      if (wpage - wStepPage < wpagedeb)
        wpageprev = -1;
      else
        wpageprev = wpage - wStepPage;
      #endregion

      /******************************/
      /* Search titles and links
      /******************************/

      //Gets Key to the first page if it exists (not required)...
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartList").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyEndList").InnerText);

      strStartTitle = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartTitle").InnerText);
      strEndTitle = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyEndTitle").InnerText);
      strStartYear = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartYear").InnerText);
      strEndYear = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyEndYear").InnerText);
      strStartDirector = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartDirector").InnerText);
      strEndDirector = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyEndDirector").InnerText);
      strStartLink = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartLink").InnerText);
      strEndLink = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyEndLink").InnerText);
      strStartID = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartID").InnerText);
      strEndID = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyEndID").InnerText);
      strStartOptions = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartOptions").InnerText);
      strEndOptions = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyEndOptions").InnerText);
      strStartAkas = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartAkas").InnerText);
      strEndAkas = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyEndAkas").InnerText);
      strKeyAkasRegExp = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyAkasRegExp").InnerText);
      try { strStartThumb = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartThumb").InnerText); }
      catch (Exception) { strStartThumb = ""; }
      try { strEndThumb = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyEndThumb").InnerText); }
      catch (Exception) { strEndThumb = ""; }

      var cook = new CookieContainer();

      //Récupère la page wpage
      strBody = GrabUtil.GetPage(strURL.Replace("#Page#", wpage.ToString()), strEncoding, out absoluteUri, cook, strHeaders, strAccept, strUserAgent);
      //redirection auto : 1 résult
      if (!absoluteUri.Equals(strURL.Replace("#Page#", wpage.ToString())))
      {
        var url = new IMDBUrl(absoluteUri, strSearchInit + " (AutoRedirect)", strDBName, null, wpage.ToString());
        elements.Add(url);
        ListUrl = elements;
        WIndex = 0;
        return;
      }

      if (strRedir.Length > 0)
        strBody = GrabUtil.GetPage(strRedir, strEncoding, out absoluteUri, cook, strHeaders, strAccept, strUserAgent);

      wpage += wStepPage;


      /******************************/
      /* Cutting the list
      /******************************/
      // If you have at least the key to start, we cut strBody
      iStart = 0;
      iEnd = 0;
      iLength = 0;

      if (strStart.Length > 0)
      {
        iStart = GrabUtil.FindPosition(strBody, strStart, iStart, ref iLength, true, true);
        if (iStart < 0) iStart = 0;
      }
      if (strEnd.Length > 0)
      {
        iEnd = GrabUtil.FindPosition(strBody, strEnd, iStart, ref iLength, true, false);
        if (iEnd <= 0) iEnd = strBody.Length;
      }

      // Cutting the body
      try { strBody = strBody.Substring(iStart, iEnd - iStart); }
      catch { }

      // Now grab the search data from stripped search page !
      iStart = 0;
      iFinItem = 0;
      iStartTitle = 0;
      iStartUrl = 0;
      iLength = 0;
      var urlprev = new IMDBUrl(strURL.Replace("#Page#", wpageprev.ToString()), "---", strDBName, n, wpageprev.ToString());

      if (strBody != "")
      {
        // Comparison between the position of URL and title to boundary elements //if (strBody.IndexOf(strStartTitle, 0) > strBody.IndexOf(strStartLink, 0))
        int iPosStartTitle = 0; iPosStartTitle = GrabUtil.FindPosition(strBody, strStartTitle, iPosStartTitle, ref iLength, true, false);
        int iPosStartLink = 0; iPosStartLink = GrabUtil.FindPosition(strBody, strStartLink, iPosStartLink, ref iLength, true, false);
        strStartItem = iPosStartTitle > iPosStartLink ? strStartLink : strStartTitle;

        // set start position for all elements (lowest possible position found)
        iFinItem = GrabUtil.FindPosition(strBody, strStartItem, iFinItem, ref iLength, true, false);
        iStartItemLength = iLength;

        // iFinItem += strStartItem.Length;
        iStartYear = iStartDirector = iStartUrl = iStartTitle = iStartID = iStartOptions = iStartAkas = iStartThumb = iFinItem;

        while (true)
        {
          // determining the end of nth Item (if the index fields are higher then found => no info for this item
          if (iFinItem <= 0) break;
          //iFinItem = GrabUtil.FindPosition(strBody, strStartItem, iFinItem + strStartItem.Length, ref iLength, true, false);
          iFinItem = GrabUtil.FindPosition(strBody, strStartItem, iFinItem + iStartItemLength, ref iLength, true, false);
          iStartItemLength = iLength;
          // Initialisation 

          #region Read Movie Title
          strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartTitle").Attributes["Param1"].InnerText);
          strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartTitle").Attributes["Param2"].InnerText);

          strTitle = strParam1.Length > 0 ? GrabUtil.FindWithAction(strBody, strStartTitle, ref iStartTitle, strEndTitle, strParam1, strParam2).Trim() : GrabUtil.Find(strBody, strStartTitle, ref iStartTitle, strEndTitle).Trim();

          if (strTitle.Length == 0)
            break;

          // Reorder article for ofdb to beginning
          if (strDBName.ToLower().StartsWith("ofdb") && strTitle.Length > 3) // Optimization for searches with ofdb
          {
            string strRight = "";
            strRight = strTitle.Substring(strTitle.Length - 3);
            if (strRight.ToLower().Contains("der") || strRight.ToLower().Contains("die") || strRight.ToLower().Contains("das") || strRight.ToLower().Contains("the"))
            {

              strTitle = strRight.Trim() + " " + strTitle.Substring(0, strTitle.Length - 3).Trim().Trim(',');
            }
          }
          #endregion

          #region Title outbound range Item re-delimit range item
          if ((iStartTitle > iFinItem) && (iFinItem != -1))
          {
            iStartYear = iStartDirector = iStartUrl = iStartTitle = iStartID = iStartOptions = iStartAkas = iStartThumb = iFinItem;
            //iFinItem = strBody.IndexOf(strStartItem, iFinItem + strStartItem.Length);
            //iFinItem = GrabUtil.FindPosition(strBody, strStartItem, iFinItem + strStartItem.Length, ref iLength, true, false);
            iFinItem = GrabUtil.FindPosition(strBody, strStartItem, iFinItem + iStartItemLength, ref iLength, true, false);
            iStartItemLength = iLength;
            if (iFinItem <= 0)
              break;
          }
          #endregion

          #region  read movie year
          strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartYear").Attributes["Param1"].InnerText);
          strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartYear").Attributes["Param2"].InnerText);

          strYear = strParam1.Length > 0 ? GrabUtil.FindWithAction(strBody, strStartYear, ref iStartYear, strEndYear, strParam1, strParam2).Trim() : GrabUtil.Find(strBody, strStartYear, ref iStartYear, strEndYear).Trim();

          if ((strYear.Length == 0) || ((iStartYear > iFinItem) && iFinItem != -1))
            strYear = string.Empty;
          #endregion

          #region read movie director
          strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartDirector").Attributes["Param1"].InnerText);
          strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartDirector").Attributes["Param2"].InnerText);

          strDirector = strParam1.Length > 0 ? GrabUtil.FindWithAction(strBody, strStartDirector, ref iStartDirector, strEndDirector, strParam1, strParam2).Trim() : GrabUtil.Find(strBody, strStartDirector, ref iStartDirector, strEndDirector).Trim();

          if ((strDirector.Length == 0) || ((iStartDirector > iFinItem) && iFinItem != -1))
            strDirector = string.Empty;
          #endregion

          #region read movie ID
          strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartID").Attributes["Param1"].InnerText);
          strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartID").Attributes["Param2"].InnerText);

          strID = strParam1.Length > 0 ? GrabUtil.FindWithAction(strBody, strStartID, ref iStartID, strEndID, strParam1, strParam2).Trim() : GrabUtil.Find(strBody, strStartID, ref iStartID, strEndID).Trim();

          if (strID.Length == 0 || (iStartID > iFinItem && iFinItem != -1))
            strID = string.Empty;
          #endregion

          #region read movie Options
          strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartOptions").Attributes["Param1"].InnerText);
          strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartOptions").Attributes["Param2"].InnerText);

          strOptions = strParam1.Length > 0 ? GrabUtil.FindWithAction(strBody, strStartOptions, ref iStartOptions, strEndOptions, strParam1, strParam2).Trim() : GrabUtil.Find(strBody, strStartOptions, ref iStartOptions, strEndOptions).Trim();

          if ((strOptions.Length == 0) || ((iStartOptions > iFinItem) && iFinItem != -1))
            strOptions = string.Empty;
          #endregion

          #region read movie Akas
          strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartAkas").Attributes["Param1"].InnerText);
          strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartAkas").Attributes["Param2"].InnerText);

          strAkas = strParam1.Length > 0 ? GrabUtil.FindWithAction(strBody, strStartAkas, ref iStartAkas, strEndAkas, strParam1, strParam2).Trim() : GrabUtil.Find(strBody, strStartAkas, ref iStartAkas, strEndAkas).Trim();

          if (strAkas.Length == 0 || (iStartAkas > iFinItem && iFinItem != -1))
            strAkas = string.Empty;
          #endregion

          #region read movie Thumb
          strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartThumb").Attributes["Param1"].InnerText);
          strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartThumb").Attributes["Param2"].InnerText);

          strThumb = strParam1.Length > 0 ? GrabUtil.FindWithAction(strBody, strStartThumb, ref iStartThumb, strEndThumb, strParam1, strParam2).Trim() : GrabUtil.Find(strBody, strStartThumb, ref iStartThumb, strEndThumb).Trim();

          if ((strThumb.Length == 0) || ((iStartThumb > iFinItem) && iFinItem != -1))
            strThumb = string.Empty;
          #endregion

          #region create akas string with titles
          if (!String.IsNullOrEmpty(strKeyAkasRegExp)) // strKeyAkasRegExp = @"aka." + "\"" + ".*?" + "\"" + ".-";
          {
            strTemp = strAkas;
            strTemp = HttpUtility.HtmlDecode(strTemp);
            strTemp = HttpUtility.HtmlDecode(strTemp).Replace("\n", "");
            Regex p = new Regex(strKeyAkasRegExp, RegexOptions.Singleline);
            iLength = 0;

            MatchCollection MatchList = p.Matches(strTemp);
            if (MatchList.Count > 0)
            {
              string matchstring = "";
              foreach (Match match in MatchList)
              {
                if (matchstring.Length > 0) matchstring += " | " + match.Groups["aka"].Value;
                else matchstring = match.Groups["aka"].Value;
              }
              if (matchstring.Length > 0)
                strAkas = matchstring;
            }
            // else strAkas = ""; 
          }
          #endregion

          #region read movie url
          strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartLink").Attributes["Param1"].InnerText);
          strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLSearch/KeyStartLink").Attributes["Param2"].InnerText);

          strLink = strParam1.Length > 0 ? GrabUtil.FindWithAction(strBody, strStartLink, ref iStartUrl, strEndLink, strParam1, strParam2).Trim() : GrabUtil.Find(strBody, strStartLink, ref iStartUrl, strEndLink).Trim();

          if (strLink.Length != 0)
          {
            // check, if IMDB id is existing
            if (strLink.Contains(@"/tt"))
            {
              strIMDB_Id = strLink.Substring(strLink.IndexOf(@"/tt") + 1);
              strIMDB_Id = strIMDB_Id.Substring(0, strIMDB_Id.IndexOf(@"/"));
              // Fix for redirection on AKAS site on IMDB
              if (strLink.Contains("onclick"))
                strLink = strLink.Substring(0, strLink.IndexOf("onclick"));
            }
            // check, if TMDB id is existing
            if (strLink.Contains(@"themoviedb.org/movie/"))
            {
              strIMDB_Id = strLink.Substring(strLink.IndexOf(@"themoviedb.org/movie/") + 20); // assign TMDB ID
            }
            if (!strLink.StartsWith("http://") && !strLink.StartsWith("www."))
            {
              //si les liens sont relatifs on rajoute le préfix (domaine)
              strLink = XmlConvert.DecodeName(n.SelectSingleNodeFast("URLPrefix").InnerText + strLink);
            }

            //Ajout http:// s'il n'y est pas (Pour GetPage)
            if (strLink.StartsWith("www."))
              strLink = "http://" + strLink;

            //Added new element, we pass the node of xml file to find the details

            //IMDBUrl url = new IMDBUrl(strLink, strTitle + " (" + strYear + ") " + strDirector, strDBName, n, wpage.ToString(), strYear, strDirector, strIMDB_Id, strTMDB_Id) ;
            IMDBUrl url = new IMDBUrl(strLink, strTitle, strDBName, n, wpage.ToString(), strYear, strDirector, strIMDB_Id, strTMDB_Id, strID, strOptions, strAkas, strThumb);
            bytes = System.Text.Encoding.GetEncoding(1251).GetBytes(strTitle.ToLower());
            System.Text.RegularExpressions.MatchCollection oMatches = oRegex.Matches(System.Text.Encoding.ASCII.GetString(bytes));
            if (oMatches.Count > 0)
              if (AlwaysAsk)
                WIndex = -2;
              else
                if (WIndex == -1)
                  WIndex = (short)elements.Count;
                else
                  WIndex = -2;

            if ((elements.Count == 0) && (strNext.Length > 0) && (strBody.Contains(strNext.Replace("#Page#", wpageprev.ToString()))) && !(wpageprev < 0))
              elements.Add(urlprev);
            elements.Add(url);
          }
          #endregion

          // init new search indexes
          iStartYear = iStartDirector = iStartUrl = iStartTitle = iStartID = iStartOptions = iStartAkas = iStartThumb = iFinItem;
        }
      }
      var urlsuite = new IMDBUrl(strURL.Replace("#Page#", wpage.ToString()), "+++", strDBName, n, wpage.ToString());

      if ((strBody.Contains(strNext.Replace("#Page#", wpage.ToString()))) && (strNext.Length > 0))
        elements.Add(urlsuite);

      ListUrl = elements;
    }

    public string[] GetDetail(string strUrl, string strPathImg, string strConfigFile, bool saveImage, string preferredLanguage, string personLimit, string titleLimit, string getRoles)
    {
      return GetDetail(strUrl, strPathImg, strConfigFile, saveImage, preferredLanguage, personLimit, titleLimit, getRoles, null);
    }
    public string[] GetDetail(string strUrl, string strPathImg, string strConfigFile, bool saveImage, string preferredLanguage, string personLimit, string titleLimit, string getRoles, string mediafile)
    {
      var datas = new string[80]; // 0-39 = original fields, 40-79 mapped fields
      elements.Clear();
      string strTemp = string.Empty;

      //string strBody = string.Empty;
      //string strBodyDetails2 = string.Empty;
      //string strBodyPersons = string.Empty;
      //string strBodyTitles = string.Empty;
      //string strBodyCertification = string.Empty;
      //string strBodyComment = string.Empty;
      //string strBodyDescription = string.Empty;
      //string strBodyCover = string.Empty;

      string strEncoding = string.Empty; // added for manual encoding override (global setting for all webgrabbing)
      string strEncodingSubPage = string.Empty; // added to override Sub Page encoding

      string strAccept = string.Empty;
      string strUserAgent = string.Empty;
      string strHeaders = string.Empty;

      string strStart = string.Empty;
      string strEnd = string.Empty;
      string strIndex = string.Empty;
      string strPage = string.Empty;

      string strActivePage = string.Empty;
      string absoluteUri;
      string strTitle = string.Empty;
      string strRate = string.Empty;
      string strRate2 = string.Empty;
      string strBasedRate = string.Empty;
      string strParam1 = string.Empty;
      string strParam3 = string.Empty;
      string strParam2 = string.Empty;
      string strMaxItems = string.Empty;
      string strLanguage = string.Empty;
      string strGrabActorRoles = string.Empty;
      bool boolGrabActorRoles = false;
      string allNames = string.Empty;
      string allRoles = string.Empty;
      int iStart = 0;
      int iEnd = 0;

      // Reset all webpage content
      BodyDetail = string.Empty;
      BodyDetail2 = string.Empty;
      BodyLinkGeneric1 = string.Empty;
      BodyLinkGeneric2 = string.Empty;
      BodyLinkImg = string.Empty;
      BodyLinkPersons = string.Empty;
      BodyLinkTitles = string.Empty;
      BodyLinkCertification = string.Empty;
      BodyLinkComment = string.Empty;
      BodyLinkSyn = string.Empty;
      BodyLinkMultiPosters = string.Empty;
      BodyLinkPhotos = string.Empty;
      BodyLinkPersonImages = string.Empty;
      BodyLinkMultiFanart = string.Empty;
      BodyLinkTrailer = string.Empty;
      BodyLinkDetailsPath = string.Empty;

      // reset filesystem content
      MovieDirectory = string.Empty;
      MovieFilename = string.Empty;

      // Recovery parameters
      // Load the configuration file
      var doc = new XmlDocument();
      doc.Load(strConfigFile);
      XmlNode n = doc.ChildNodes[1].FirstChild;

      try { strEncoding = n.SelectSingleNodeFast("Encoding").InnerText; }
      catch (Exception) { strEncoding = ""; }
      try { strAccept = n.SelectSingleNodeFast("Accept").InnerText; }
      catch (Exception) { strAccept = ""; }
      try { strUserAgent = n.SelectSingleNodeFast("UserAgent").InnerText; }
      catch (Exception) { strUserAgent = ""; }
      try { strHeaders = n.SelectSingleNodeFast("Headers").InnerText; }
      catch (Exception) { strHeaders = ""; }

      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartBody").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndBody").InnerText);

      // Set DetailsPath
      BodyLinkDetailsPath = "<url>" + strUrl + "</url>";
      if (strUrl.LastIndexOf("/", System.StringComparison.Ordinal) > 0)
      {
        BodyLinkDetailsPath += Environment.NewLine;
        BodyLinkDetailsPath += "<baseurl>" + strUrl.Substring(0, strUrl.LastIndexOf("/", System.StringComparison.Ordinal)) + "</baseurl>";
        BodyLinkDetailsPath += Environment.NewLine;
        BodyLinkDetailsPath += "<pageurl>" + strUrl.Substring(strUrl.LastIndexOf("/", System.StringComparison.Ordinal) + 1) + "</pageurl>";
        BodyLinkDetailsPath += Environment.NewLine;
        BodyLinkDetailsPath += "<replacement>" + strUrl.Substring(0, strUrl.LastIndexOf("/", System.StringComparison.Ordinal)) + "%replacement%" + strUrl.Substring(strUrl.LastIndexOf("/", System.StringComparison.Ordinal) + 1) + "</replacement>";
      }

      //Fetch the basic Details page and update DetailsPath, if possible
      if (strUrl.ToLower().StartsWith("http"))
      {
        BodyDetail = GrabUtil.GetPage(strUrl, strEncoding, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
      {
        BodyDetail = GrabUtil.GetFileContent(strUrl, strEncoding); // Read page from file !
        string physicalFile = (mediafile != null && File.Exists(mediafile)) ? mediafile : strUrl;
        if (File.Exists(physicalFile))
        {
          MovieDirectory = Path.GetDirectoryName(physicalFile);
          MovieFilename = Path.GetFileNameWithoutExtension(physicalFile);
          // Set DetailsPath
          BodyLinkDetailsPath += Environment.NewLine;
          BodyLinkDetailsPath += "<directory>" + MovieDirectory + "</directory>";
          BodyLinkDetailsPath += Environment.NewLine;
          BodyLinkDetailsPath += "<filename>" + MovieFilename + "</filename>";
          if (MovieDirectory != null)
          {
            string[] files = Directory.GetFiles(MovieDirectory, "*", SearchOption.AllDirectories);

            //foreach (string extension in files.Select(file => Path.GetExtension(file)).Distinct().ToList())
            //{
            //  BodyLinkDetailsPath += Environment.NewLine;
            //  BodyLinkDetailsPath += "<" + extension + "-files>";
            //  foreach (string file in files.Where(file => file.EndsWith("." + extension)).ToList())
            //  {
            //    BodyLinkDetailsPath += Environment.NewLine;
            //    BodyLinkDetailsPath += "<" + extension + ">" + file + "</" + extension + ">";
            //  }
            //  BodyLinkDetailsPath += Environment.NewLine;
            //  BodyLinkDetailsPath += "</" + extension + "-files>";
            //}

            BodyLinkDetailsPath += Environment.NewLine;
            BodyLinkDetailsPath += "<jpg-files>";
            foreach (string file in files.Where(file => file.EndsWith(".jpg")).ToList())
            {
              BodyLinkDetailsPath += Environment.NewLine;
              BodyLinkDetailsPath += "<jpg>" + file + "</jpg>";
            }
            BodyLinkDetailsPath += Environment.NewLine;
            BodyLinkDetailsPath += "</jpg-files>";

            BodyLinkDetailsPath += Environment.NewLine;
            BodyLinkDetailsPath += "<other-files>";
            foreach (string file in files.Where(file => !file.EndsWith(".jpg")).ToList())
            {
              BodyLinkDetailsPath += Environment.NewLine;
              BodyLinkDetailsPath += "<other>" + file + "</other>";
            }
            BodyLinkDetailsPath += Environment.NewLine;
            BodyLinkDetailsPath += "</other-files>";
          }
        }
      }

      var htmlUtil = new HTMLUtil();

      //Si on a au moins la clé de début, on découpe StrBody
      if (strStart != "")
      {
        iStart = BodyDetail.IndexOf(strStart);

        //Si la clé de début a été trouvé
        if (iStart > 0)
        {
          //Si une clé de fin a été paramétrée, on l'utilise si non on prend le reste du body
          iEnd = strEnd != "" ? BodyDetail.IndexOf(strEnd, iStart) : this.BodyDetail.Length;

          //Découpage du body
          iStart += strStart.Length;
          if (iEnd - iStart > 0)
            BodyDetail = BodyDetail.Substring(iStart, iEnd - iStart);
        }
      }

      #region Load Sub Pages into Memory ***** // Will be used for Secondary Website Infos!

      // ***** URL Redirection Details 2 Base Page ***** // Will be used for Secondary Website Infos!
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartDetails2").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndDetails2").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartDetails2").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartDetails2").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyDetails2Index").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyDetails2Page").InnerText);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingDetails2").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      strActivePage = this.LoadPage(strPage);
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyDetail2 = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyDetail2 = "";

      // ***** URL Redirection Generic 1 *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkGeneric1").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLinkGeneric1").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkGeneric1").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkGeneric1").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkGeneric1Index").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkGeneric1Page").InnerText);
      strActivePage = this.LoadPage(strPage);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingLinkGeneric1").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyLinkGeneric1 = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyLinkGeneric1 = "";

      // ***** URL Redirection Generic 2 *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkGeneric2").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLinkGeneric2").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkGeneric2").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkGeneric2").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkGeneric2Index").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkGeneric2Page").InnerText);
      strActivePage = this.LoadPage(strPage);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingLinkGeneric2").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyLinkGeneric2 = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyLinkGeneric2 = "";

      // ***** URL Redirection IMG *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkImg").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLinkImg").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkImg").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkImg").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkImgIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkImgPage").InnerText);
      strActivePage = this.LoadPage(strPage);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingLinkImg").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyLinkImg = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyLinkImg = "";

      // ***** URL Redirection Persons ***** // Will be used for persons, if available !
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkPersons").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLinkPersons").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkPersons").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkPersons").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkPersonsIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkPersonsPage").InnerText);
      strActivePage = this.LoadPage(strPage);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingLinkPersons").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyLinkPersons = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyLinkPersons = "";

      // ***** URL Redirection Titles ***** // Will be used for TTitle, if available !
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkTitles").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLinkTitles").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkTitles").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkTitles").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkTitlesIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkTitlesPage").InnerText);
      strActivePage = this.LoadPage(strPage);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingLinkTitles").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyLinkTitles = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyLinkTitles = "";

      // ***** URL Redirection Certification ***** // Will be used for Certification Details, if available !
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkCertification").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLinkCertification").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkCertification").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkCertification").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkCertificationIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkCertificationPage").InnerText);
      strActivePage = this.LoadPage(strPage);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingLinkCertification").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyLinkCertification = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyLinkCertification = "";

      // ***** URL Redirection Comment ***** // Will be used for Comment Details, if available !
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkComment").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLinkComment").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkComment").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkComment").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkCommentIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkCommentPage").InnerText);
      strActivePage = this.LoadPage(strPage);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingLinkComment").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyLinkComment = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyLinkComment = "";

      // ***** URL Redirection Description ***** // Will be used for Description Details, if available !
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkSyn").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLinkSyn").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkSyn").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkSyn").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkSynIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkSynPage").InnerText);
      strActivePage = this.LoadPage(strPage);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingLinkSyn").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyLinkSyn = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyLinkSyn = "";

      // ***** URL Redirection MultiPosters ***** // Will be used for MultiPosters Details, if available !
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkMultiPosters").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLinkMultiPosters").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkMultiPosters").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkMultiPosters").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkMultiPostersIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkMultiPostersPage").InnerText);
      strActivePage = this.LoadPage(strPage);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingLinkMultiPosters").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyLinkMultiPosters = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyLinkMultiPosters = "";

      // ***** URL Redirection Photos ***** // Will be used for Photos Details, if available !
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkPhotos").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLinkPhotos").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkPhotos").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkPhotos").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkPhotosIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkPhotosPage").InnerText);
      strActivePage = this.LoadPage(strPage);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingLinkPhotos").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyLinkPhotos = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyLinkPhotos = "";

      // ***** URL Redirection PersonImages ***** // Will be used for PersonImages Details, if available !
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkPersonImages").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLinkPersonImages").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkPersonImages").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkPersonImages").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkPersonImagesIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkPersonImagesPage").InnerText);
      strActivePage = this.LoadPage(strPage);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingLinkPersonImages").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyLinkPersonImages = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyLinkPersonImages = "";

      // ***** URL Redirection MultiFanart ***** // Will be used for MultiFanart Details, if available !
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkMultiFanart").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLinkMultiFanart").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkMultiFanart").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkMultiFanart").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkMultiFanartIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkMultiFanartPage").InnerText);
      strActivePage = this.LoadPage(strPage);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingLinkMultiFanart").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyLinkMultiFanart = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyLinkMultiFanart = "";

      // ***** URL Redirection Trailer ***** // Will be used for Trailer Details, if available !
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkTrailer").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLinkTrailer").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkTrailer").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLinkTrailer").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkTrailerIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLinkTrailerPage").InnerText);
      strActivePage = this.LoadPage(strPage);
      try { strEncodingSubPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEncodingLinkTrailer").InnerText); }
      catch (Exception) { strEncodingSubPage = ""; }
      if (strStart.Length > 0)
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
        BodyLinkTrailer = GrabUtil.GetPage(strTemp, (string.IsNullOrEmpty(strEncodingSubPage)) ? strEncoding : strEncodingSubPage, out absoluteUri, new CookieContainer(), strHeaders, strAccept, strUserAgent);
      }
      else
        BodyLinkTrailer = "";
      #endregion

      #region ************* Now get the detail fields ***************************

      // ***** Original TITLE *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartOTitle").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndOTitle").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartOTitle").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartOTitle").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyOTitleIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyOTitlePage").InnerText);
      strActivePage = this.LoadPage(strPage);
      strTitle = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTitle = strTitle.Replace("\n", "");

      if (strTitle.Length > 0)
        datas[(int)Grabber_Output.OriginalTitle] = strTitle;
      else
        datas[(int)Grabber_Output.OriginalTitle] = "";


      // ***** Translated TITLE *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTTitle").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndTTitle").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTTitle").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTTitle").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTTitleRegExp").InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTTitleIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTTitlePage").InnerText);
      try
      { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTTitleMaxItems").InnerText); }
      catch (Exception) { strMaxItems = ""; }
      try
      { strLanguage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTTitleLanguage").InnerText); }
      catch (Exception) { strLanguage = ""; }
      strActivePage = this.LoadPage(strPage);

      // Overrides from MyFilms plugin:
      if (!string.IsNullOrEmpty(preferredLanguage))
        strLanguage = preferredLanguage;
      if (!string.IsNullOrEmpty(titleLimit))
        strMaxItems = titleLimit.ToString();

      allNames = string.Empty;
      allRoles = string.Empty;
      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTitle = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles).Trim();
      else
        strTitle = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
      strTitle = strTitle.Replace("\n", "");
      if (strTitle.Length > 0)
        //Translated Title
        datas[(int)Grabber_Output.TranslatedTitle] = strTitle;
      else
        datas[(int)Grabber_Output.TranslatedTitle] = "";
      //else
      //    datas[(int)Grabber_Output.TranslatedTitle] = datas[(int)Grabber_Output.OriginalTitle];
      //if (datas[(int)Grabber_Output.OriginalTitle].Length == 0)
      //    datas[(int)Grabber_Output.OriginalTitle] = datas[(int)Grabber_Output.TranslatedTitle];
      datas[(int)Grabber_Output.TranslatedTitleAllNames] = allNames;
      datas[(int)Grabber_Output.TranslatedTitleAllValues] = allRoles;

      // ***** URL for Image *****
      // ***** Fanart ***** //

      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartImg").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndImg").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartImg").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartImg").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyImgIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyImgPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      datas[(int)Grabber_Output.Fanart] = ""; // Fanart - only for file grabber

      if (strStart.StartsWith("#NFO#") || strEnd.StartsWith("#NFO#")) // 
      {
        if (strStart.StartsWith("#NFO#")) // special handling for NFO files
        {
          string covername = strStart.Substring(strStart.IndexOf("#NFO#") + 5);
          if (covername.Contains("#Filename#"))
            covername = covername.Replace("#Filename#", MovieFilename);
          datas[(int)Grabber_Output.PicturePathLong] = MovieDirectory + "\\" + covername;
          datas[(int)Grabber_Output.PicturePathShort] = covername;
          if (saveImage == true)
          {
            if (string.IsNullOrEmpty(datas[(int)Grabber_Output.OriginalTitle]))
              GrabUtil.CopyCoverArt(strPathImg, MovieDirectory + "\\" + covername, datas[(int)Grabber_Output.TranslatedTitle], out strTemp);
            else
              GrabUtil.CopyCoverArt(strPathImg, MovieDirectory + "\\" + covername, datas[(int)Grabber_Output.OriginalTitle], out strTemp);
            datas[(int)Grabber_Output.PicturePathLong] = strPathImg + "\\" + strTemp;
          }
          datas[(int)Grabber_Output.PicturePathShort] = strTemp;
        }
        if (strEnd.StartsWith("#NFO#")) // special handling for NFO files
        {
          string fanartname = strEnd.Substring(strEnd.IndexOf("#NFO#") + 5);
          if (fanartname.Contains("#Filename#"))
            fanartname = fanartname.Replace("#Filename#", MovieFilename);
          datas[(int)Grabber_Output.Fanart] = MovieDirectory + "\\" + fanartname;
        }
      }
      else
      {
        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

        strTemp = strTemp.Replace("\n", "");
        if (strTemp.Length > 0)
        {
          datas[(int)Grabber_Output.PictureURL] = strTemp;
          //Picture
          if (saveImage == true)
          {
            if (string.IsNullOrEmpty(datas[(int)Grabber_Output.OriginalTitle]))
              GrabUtil.DownloadCoverArt(strPathImg, strTemp, datas[(int)Grabber_Output.TranslatedTitle], out strTemp);
            else
              GrabUtil.DownloadCoverArt(strPathImg, strTemp, datas[(int)Grabber_Output.OriginalTitle], out strTemp);
            // strTemp = MediaPortal.Util.Utils.FilterFileName(strTemp); // Guzzi: removed, as it could change the filename to an already loaded image, thus breaking the "link".
            datas[(int)Grabber_Output.PicturePathLong] = strPathImg + "\\" + strTemp;
          }
          datas[(int)Grabber_Output.PicturePathShort] = strTemp;
        }
      }

      // ***** Synopsis ***** Description
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartSyn").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndSyn").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartSyn").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartSyn").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeySynIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeySynPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");

      // make sure nonvalid Chars are encoded - e.g. 
      //< 	-> 	&lt;
      //> 	-> 	&gt;
      //" 	-> 	&quot;
      //' 	-> 	&apos;
      //& 	-> 	&amp;
      // strTemp = System.Web.HttpUtility.HtmlEncode(strTemp.Replace('’', '\''));
      // strTemp = System.Security.SecurityElement.Escape(strTemp); // alternative way to encode invalid Chars - avoids overhead of Web classes
      // strTemp = GrabUtil.StripIllegalXMLChars(strTemp, "1.1");
      strTemp = strTemp.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;"); // strTemp = strTemp.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
      // strTemp = strTemp.Replace("\"", "\'");

      if (strTemp.Length > 0) datas[(int)Grabber_Output.Description] = strTemp;

      NumberFormatInfo provider = new NumberFormatInfo();
      provider.NumberDecimalSeparator = ",";

      // ***** Base rating *****
      strBasedRate = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/BaseRating").InnerText);
      decimal wRate = 0;
      decimal wRate2 = 0;
      decimal wBasedRate = 10;
      try
      { wBasedRate = Convert.ToDecimal(strBasedRate, provider); }
      catch
      { }
      // ***** NOTE 1 ***** Rating 1
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRate").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndRate").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRate").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRate").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyRateIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyRatePage").InnerText);
      strActivePage = this.LoadPage(strPage);
      strRate = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
      strRate = GrabUtil.convertNote(strRate);
      try
      { wRate = (Convert.ToDecimal(strRate, provider) / wBasedRate) * 10; }
      catch
      { }

      // ***** NOTE 2 ***** Rating 2
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRate2").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndRate2").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRate2").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRate2").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyRate2Index").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyRate2Page").InnerText);
      strActivePage = this.LoadPage(strPage);
      strRate2 = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strRate2 = GrabUtil.convertNote(strRate2);

      try
      { wRate2 = (Convert.ToDecimal(strRate2, provider) / wBasedRate) * 10; }
      catch
      { }

      //Calcul de la moyenne des notes.
      decimal resultRate;
      if (wRate > 0 && wRate2 > 0)
        resultRate = ((wRate + wRate2) / 2);
      else
        if (wRate == 0 && wRate2 == 0)
          resultRate = -1;
        else
          resultRate = ((wRate + wRate2));

      resultRate = Math.Round(resultRate, 1);
      strRate = resultRate == -1 ? "" : Convert.ToString(resultRate);

      //Rating (calculated from Rating 1 and 2)
      strRate = strRate.Replace(",", ".");
      datas[(int)Grabber_Output.Rating] = strRate.Replace(",", ".");

      // ***** Acteurs ***** Actors
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCredits").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndCredits").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCredits").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCredits").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCreditsRegExp").InnerText);
      strMaxItems = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCreditsMaxItems").InnerText);
      strGrabActorRoles = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCreditsGrabActorRoles").InnerText);
      boolGrabActorRoles = strGrabActorRoles == "true";
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCreditsIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCreditsPage").InnerText);
      strActivePage = this.LoadPage(strPage);
      strLanguage = "";

      // Overrides from MyFilms plugin:
      if (!string.IsNullOrEmpty(personLimit))
        strMaxItems = personLimit.ToString();
      if (!string.IsNullOrEmpty(getRoles))
      {
        if (getRoles.ToLower() == "true") boolGrabActorRoles = true;
        if (getRoles.ToLower() == "false") boolGrabActorRoles = false;
      }

      allNames = string.Empty;
      allRoles = string.Empty;
      if (strParam1.Length > 0 || strParam3.Length > 0) // 
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out allNames, out allRoles, boolGrabActorRoles).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      // strTemp = strTemp.Replace("\n", ""); // removed, as it seems, the "newlines" replacements for #LF# didn't work in AMC and MP
      strTemp = GrabUtil.trimSpaces(strTemp);
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Actors] = strTemp;

      // ***** Réalisateur ***** = Director 
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRealise").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndRealise").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRealise").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRealise").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyRealiseRegExp").InnerText);
      strMaxItems = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyRealiseMaxItems").InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyRealiseIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyRealisePage").InnerText);
      strActivePage = this.LoadPage(strPage);

      // Overrides from MyFilms plugin:
      if (!string.IsNullOrEmpty(personLimit))
        strMaxItems = personLimit;

      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Director] = strTemp;

      // ***** Producteur ***** Producer // Producers also using MiltiPurpose Secondary page !
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartProduct").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndProduct").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartProduct").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartProduct").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyProductRegExp").InnerText);
      strMaxItems = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyProductMaxItems").InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyProductIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyProductPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      // Overrides from MyFilms plugin:
      if (!string.IsNullOrEmpty(personLimit))
        strMaxItems = personLimit.ToString();

      if (strParam1.Length > 0 || strParam3.Length > 0) // Guzzi: Added param3 to execute matchcollections also !
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Producer] = strTemp;

      // ***** Année ***** Year
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartYear").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndYear").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartYear").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartYear").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyYearIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyYearPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        if (strTemp.Length >= 4)
          datas[(int)Grabber_Output.Year] = strTemp.Substring(strTemp.Length - 4, 4);
        else
          datas[(int)Grabber_Output.Year] = strTemp; // fallback, if scraping failed

      // ***** Pays ***** Country
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCountry").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndCountry").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCountry").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCountry").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCountryRegExp").InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCountryIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCountryPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
      {
        strTemp = strTemp.Replace(".", " ");
        strTemp = GrabUtil.TransformCountry(strTemp);
        datas[(int)Grabber_Output.Country] = strTemp;
      }


      // ***** Genre *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartGenre").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndGenre").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartGenre").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartGenre").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGenreRegExp").InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGenreIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGenrePage").InnerText);
      strActivePage = this.LoadPage(strPage);

      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Category] = strTemp;

      // ***** URL *****
      datas[(int)Grabber_Output.URL] = strUrl;


      // ***** Writer ***** //
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartWriter").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndWriter").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartWriter").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartWriter").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyWriterRegExp").InnerText);
      strMaxItems = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyWriterMaxItems").InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyWriterIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyWriterPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      // Overrides from MyFilms plugin:
      if (!string.IsNullOrEmpty(personLimit))
        strMaxItems = personLimit.ToString();

      if (strParam1.Length > 0 || strParam3.Length > 0) // Guzzi: Added param3 to execute matchcollections also !
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Writer] = strTemp;

      // ***** Comment *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartComment").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndComment").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartComment").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartComment").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCommentRegExp").InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCommentIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCommentPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", " "); // Guzzi: Replace linebreaks with space
      strTemp = strTemp.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;"); // strTemp = strTemp.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
      // strTemp = System.Web.HttpUtility.HtmlEncode(strTemp.Replace('’', '\''));
      // strTemp = System.Security.SecurityElement.Escape(strTemp); // alternative way to encode invalid Chars - avoids overhead of Web classes
      // strTemp = strTemp.Replace("\"", "\'");

      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Comments] = strTemp;

      // ***** Language *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLanguage").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndLanguage").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLanguage").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLanguage").Attributes["Param2"].InnerText);
      try { strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLanguageRegExp").InnerText); }
      catch (Exception) { strParam3 = ""; }
      
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLanguageIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyLanguagePage").InnerText);
      strActivePage = this.LoadPage(strPage);

      strTemp = (strParam1.Length > 0 || strParam3.Length > 0) 
        ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3).Trim()
        : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Language] = strTemp;

      // ***** Tagline *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTagline").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndTagline").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTagline").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTagline").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTaglineIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTaglinePage").InnerText);
      strActivePage = this.LoadPage(strPage);

      strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Tagline] = strTemp;


      // ***** Certification *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCertification").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndCertification").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCertification").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCertification").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCertificationRegExp").InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCertificationIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCertificationPage").InnerText);
      strActivePage = this.LoadPage(strPage);
      try
      { strLanguage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCertificationLanguage").InnerText); }
      catch (Exception) { strLanguage = ""; }

      // Overrides from MyFilms plugin:
      if (!string.IsNullOrEmpty(preferredLanguage))
        strLanguage = preferredLanguage;

      allNames = string.Empty;
      allRoles = string.Empty;
      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, "", strLanguage, out allNames, out allRoles).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Certification] = strTemp;
      datas[(int)Grabber_Output.CertificationAllNames] = allNames;
      datas[(int)Grabber_Output.CertificationAllValues] = allRoles;

      // ***** IMDB_Id *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartIMDB_Id").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndIMDB_Id").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartIMDB_Id").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartIMDB_Id").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyIMDB_IdIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyIMDB_IdPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.IMDB_Id] = strTemp;

      // ***** IMDB_Rank *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartIMDB_Rank").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndIMDB_Rank").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartIMDB_Rank").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartIMDB_Rank").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyIMDB_RankIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyIMDB_RankPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.IMDB_Rank] = strTemp;


      // ***** TMDB_Id *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTMDB_Id").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndTMDB_Id").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTMDB_Id").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTMDB_Id").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTMDB_IdIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTMDB_IdPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.TMDB_Id] = strTemp;

      // ***** Studio *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartStudio").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndStudio").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartStudio").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartStudio").Attributes["Param2"].InnerText);
      try { strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStudioRegExp").InnerText); }
      catch (Exception) { strParam3 = ""; }
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStudioIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStudioPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      strTemp = (strParam1.Length > 0 || strParam3.Length > 0)
        ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3).Trim()
        : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Studio] = strTemp;

      // ***** Edition *****
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartEdition").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndEdition").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartEdition").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartEdition").Attributes["Param2"].InnerText);
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEditionIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEditionPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Edition] = strTemp;

      // ***** Collection *****
      try
      {
        strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCollection").InnerText);
        strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndCollection").InnerText);
        strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCollection").Attributes["Param1"].InnerText);
        strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCollection").Attributes["Param2"].InnerText);
        strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCollectionIndex").InnerText);
        strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCollectionPage").InnerText);
        strActivePage = this.LoadPage(strPage);

        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

        strTemp = strTemp.Replace("\n", "");
        if (strTemp.Length > 0)
          datas[(int)Grabber_Output.Collection] = strTemp;
      }
      catch (Exception)
      {
        datas[(int)Grabber_Output.Collection] = "";
      }

      // ***** Collection Image *****
      try
      {
        strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCollectionImageURL").InnerText);
        strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndCollectionImageURL").InnerText);
        strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCollectionImageURL").Attributes["Param1"].InnerText);
        strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCollectionImageURL").Attributes["Param2"].InnerText);
        strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCollectionImageURLIndex").InnerText);
        strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyCollectionImageURLPage").InnerText);
        strActivePage = this.LoadPage(strPage);

        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

        strTemp = strTemp.Replace("\n", "");
        if (strTemp.Length > 0)
        {
          datas[(int)Grabber_Output.CollectionImage] = strTemp;
          //Picture for Collections Download
          if (saveImage == true)
          {
            if (string.IsNullOrEmpty(datas[(int)Grabber_Output.OriginalTitle]))
              GrabUtil.DownloadCoverArt(strPathImg, strTemp, "Collection_" + datas[(int)Grabber_Output.TranslatedTitle], out strTemp);
            else
              GrabUtil.DownloadCoverArt(strPathImg, strTemp, "Collection_" + datas[(int)Grabber_Output.OriginalTitle], out strTemp);
            // datas[(int)Grabber_Output.PicturePathLong] = strPathImg + "\\" + strTemp;
          }
          // datas[(int)Grabber_Output.PicturePathShort] = strTemp;
        }
      }
      catch (Exception)
      {
        datas[(int)Grabber_Output.CollectionImage] = "";
      }

      // ***** Runtime *****
      try
      {
        strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRuntime").InnerText);
        strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndRuntime").InnerText);
        strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRuntime").Attributes["Param1"].InnerText);
        strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRuntime").Attributes["Param2"].InnerText);
        strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyRuntimeIndex").InnerText);
        strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyRuntimePage").InnerText);
        strActivePage = this.LoadPage(strPage);

        strTemp = strParam1.Length > 0 ? GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2).Trim() : GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();

        strTemp = strTemp.Replace("\n", "");
        if (strTemp.Length > 0)
          datas[(int)Grabber_Output.Runtime] = strTemp;
      }
      catch (Exception)
      {
        datas[(int)Grabber_Output.Runtime] = "";
      }

      // ***** Generic Field 1 ***** //
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartGeneric1").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndGeneric1").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartGeneric1").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartGeneric1").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric1RegExp").InnerText);
      try
      { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric1MaxItems").InnerText); }
      catch (Exception) { strMaxItems = ""; }
      try
      { strLanguage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric1Language").InnerText); }
      catch (Exception) { strLanguage = ""; }
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric1Index").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric1Page").InnerText);
      strActivePage = this.LoadPage(strPage);

      // Overrides from MyFilms plugin:
      //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
      //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
      //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

      allNames = string.Empty;
      allRoles = string.Empty;
      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
      strTemp = strTemp.Replace("\n", "");
      strTemp = GrabUtil.StripIllegalXMLChars(strTemp, "1.0"); // strTemp = strTemp.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;"); // strTemp = strTemp.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");

      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Generic1] = strTemp;
      else
        datas[(int)Grabber_Output.Generic1] = "";

      // ***** Generic Field 2 ***** //
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartGeneric2").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndGeneric2").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartGeneric2").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartGeneric2").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric2RegExp").InnerText);
      try
      { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric2MaxItems").InnerText); }
      catch (Exception) { strMaxItems = ""; }
      try
      { strLanguage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric2Language").InnerText); }
      catch (Exception) { strLanguage = ""; }
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric2Index").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric2Page").InnerText);
      strActivePage = this.LoadPage(strPage);

      // Overrides from MyFilms plugin:
      //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
      //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
      //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

      allNames = string.Empty;
      allRoles = string.Empty;
      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
      strTemp = strTemp.Replace("\n", "");
      strTemp = GrabUtil.StripIllegalXMLChars(strTemp, "1.0"); // strTemp = strTemp.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;"); // strTemp = strTemp.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Generic2] = strTemp;
      else
        datas[(int)Grabber_Output.Generic2] = "";


      // ***** Generic Field 3 ***** //
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartGeneric3").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndGeneric3").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartGeneric3").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartGeneric3").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric3RegExp").InnerText);
      try
      { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric3MaxItems").InnerText); }
      catch (Exception) { strMaxItems = ""; }
      try
      { strLanguage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric3Language").InnerText); }
      catch (Exception) { strLanguage = ""; }
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric3Index").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyGeneric3Page").InnerText);
      strActivePage = this.LoadPage(strPage);

      // Overrides from MyFilms plugin:
      //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
      //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
      //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

      allNames = string.Empty;
      allRoles = string.Empty;
      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
      strTemp = strTemp.Replace("\n", "");
      strTemp = GrabUtil.StripIllegalXMLChars(strTemp, "1.0"); // strTemp = strTemp.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;"); // strTemp = strTemp.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Generic3] = strTemp;
      else
        datas[(int)Grabber_Output.Generic3] = "";


      // ***********************************
      // new key-value listingoutputs
      // ***********************************

      // ***** MultiPosters ***** //
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartMultiPosters").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndMultiPosters").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartMultiPosters").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartMultiPosters").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyMultiPostersRegExp").InnerText);
      try
      { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyMultiPostersMaxItems").InnerText); }
      catch (Exception) { strMaxItems = ""; }
      try
      { strLanguage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyMultiPostersLanguage").InnerText); }
      catch (Exception) { strLanguage = ""; }
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyMultiPostersIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyMultiPostersPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      // Overrides from MyFilms plugin:
      //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
      //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
      //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

      allNames = string.Empty;
      allRoles = string.Empty;
      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles, true).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.MultiPosters] = strTemp;
      else
        datas[(int)Grabber_Output.MultiPosters] = "";

      // ***** Photos ***** //
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartPhotos").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndPhotos").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartPhotos").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartPhotos").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyPhotosRegExp").InnerText);
      try
      { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyPhotosMaxItems").InnerText); }
      catch (Exception) { strMaxItems = ""; }
      try
      { strLanguage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyPhotosLanguage").InnerText); }
      catch (Exception) { strLanguage = ""; }
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyPhotosIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyPhotosPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      // Overrides from MyFilms plugin:
      //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
      //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
      //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

      allNames = string.Empty;
      allRoles = string.Empty;
      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles, true).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Photos] = strTemp;
      else
        datas[(int)Grabber_Output.Photos] = "";

      // ***** PersonImages ***** //
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartPersonImages").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndPersonImages").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartPersonImages").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartPersonImages").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyPersonImagesRegExp").InnerText);
      try
      { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyPersonImagesMaxItems").InnerText); }
      catch (Exception) { strMaxItems = ""; }
      try
      { strLanguage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyPersonImagesLanguage").InnerText); }
      catch (Exception) { strLanguage = ""; }
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyPersonImagesIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyPersonImagesPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      // Overrides from MyFilms plugin:
      //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
      //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
      //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

      allNames = string.Empty;
      allRoles = string.Empty;
      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles, true).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.PersonImages] = strTemp;
      else
        datas[(int)Grabber_Output.PersonImages] = "";

      // ***** MultiFanart ***** //
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartMultiFanart").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndMultiFanart").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartMultiFanart").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartMultiFanart").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyMultiFanartRegExp").InnerText);
      try
      { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyMultiFanartMaxItems").InnerText); }
      catch (Exception) { strMaxItems = ""; }
      try
      { strLanguage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyMultiFanartLanguage").InnerText); }
      catch (Exception) { strLanguage = ""; }
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyMultiFanartIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyMultiFanartPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      // Overrides from MyFilms plugin:
      //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
      //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
      //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

      allNames = string.Empty;
      allRoles = string.Empty;
      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles, true).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.MultiFanart] = strTemp;
      else
        datas[(int)Grabber_Output.MultiFanart] = "";

      // ***** Trailer ***** //
      strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTrailer").InnerText);
      strEnd = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyEndTrailer").InnerText);
      strParam1 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTrailer").Attributes["Param1"].InnerText);
      strParam2 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTrailer").Attributes["Param2"].InnerText);
      strParam3 = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTrailerRegExp").InnerText);
      try
      { strMaxItems = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTrailerMaxItems").InnerText); }
      catch (Exception) { strMaxItems = ""; }
      try
      { strLanguage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTrailerLanguage").InnerText); }
      catch (Exception) { strLanguage = ""; }
      strIndex = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTrailerIndex").InnerText);
      strPage = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyTrailerPage").InnerText);
      strActivePage = this.LoadPage(strPage);

      // Overrides from MyFilms plugin:
      //if (!string.IsNullOrEmpty(PreferredLanguage)) strLanguage = PreferredLanguage;
      //if (!string.IsNullOrEmpty(TitleLimit)) strMaxItems = TitleLimit.ToString();
      //if (!string.IsNullOrEmpty(PersonLimit)) strMaxItems = PersonLimit.ToString();

      allNames = string.Empty;
      allRoles = string.Empty;
      if (strParam1.Length > 0 || strParam3.Length > 0)
        strTemp = GrabUtil.FindWithAction(ExtractBody(strActivePage, strIndex, n), strStart, strEnd, strParam1, strParam2, strParam3, strMaxItems, strLanguage, out  allNames, out allRoles, true).Trim();
      else
        strTemp = GrabUtil.Find(ExtractBody(strActivePage, strIndex, n), strStart, strEnd).Trim();
      strTemp = strTemp.Replace("\n", "");
      if (strTemp.Length > 0)
        datas[(int)Grabber_Output.Trailer] = strTemp;
      else
        datas[(int)Grabber_Output.Trailer] = "";
      #endregion

      #region mapping of fields to output
      // **********************************************************************************************************
      // Do mapping, if any configured...
      // **********************************************************************************************************
      string[] source = new string[40];
      string[] destination = new string[40];
      bool[] replace = new bool[40];
      bool[] addStart = new bool[40];
      bool[] addEnd = new bool[40];
      bool[] mergePreferSource = new bool[40];
      bool[] mergePreferDestination = new bool[40];

      List<string> fields = Grabber.Grabber_URLClass.FieldList();

      // Read Config
      for (int t = 0; t < 40; t++)
      {
        try
        {
          source[t] = XmlConvert.DecodeName(n.SelectSingleNodeFast("Mapping/Field_" + t.ToString()).Attributes["source"].InnerText);
          if (source[t] == "") source[t] = fields[t]; // replace with "right DB field name" if it is empty (for upgrade compatibility)
          destination[t] = XmlConvert.DecodeName(n.SelectSingleNodeFast("Mapping/Field_" + t.ToString()).Attributes["destination"].InnerText);
          replace[t] = Convert.ToBoolean(XmlConvert.DecodeName(n.SelectSingleNodeFast("Mapping/Field_" + t.ToString()).Attributes["replace"].InnerText));
          addStart[t] = Convert.ToBoolean(XmlConvert.DecodeName(n.SelectSingleNodeFast("Mapping/Field_" + t.ToString()).Attributes["addstart"].InnerText));
          addEnd[t] = Convert.ToBoolean(XmlConvert.DecodeName(n.SelectSingleNodeFast("Mapping/Field_" + t.ToString()).Attributes["addend"].InnerText));
          mergePreferSource[t] = Convert.ToBoolean(XmlConvert.DecodeName(n.SelectSingleNodeFast("Mapping/Field_" + t.ToString()).Attributes["mergeprefersource"].InnerText));
          mergePreferDestination[t] = Convert.ToBoolean(XmlConvert.DecodeName(n.SelectSingleNodeFast("Mapping/Field_" + t.ToString()).Attributes["mergepreferdestination"].InnerText));
        }
        catch (Exception)
        {
          source[t] = fields[t];
          destination[t] = "";
          replace[t] = false;
          addStart[t] = false;
          addEnd[t] = false;
          mergePreferSource[t] = false;
          mergePreferDestination[t] = false;
        }
      }

      for (int t = 0; t < 40; t++) // set default values = source
      {
        datas[t + 40] = !string.IsNullOrEmpty(datas[t]) ? datas[t] : string.Empty; // set destination = source as default (base if no other transformations)
      }
      for (int t = 0; t < 40; t++) // replace values if configured
      {
        for (int i = 0; i < 40; i++) // search for mapping destination
        {
          if (destination[t] == source[i]) // found mapping destination -> datas[i] is destination object !
          {
            if (replace[t]) // replace action
              datas[i + 40] = !String.IsNullOrEmpty(datas[t]) ? datas[t] : string.Empty;
          }
        }
      }
      for (int t = 0; t < 40; t++) // merge prefer source - replace values only if source is not empty
      {
        for (int i = 0; i < 40; i++)
        {
          if (destination[t] == source[i])
          {
            if (mergePreferSource[t] && !string.IsNullOrEmpty(datas[t])) // replace action
              datas[i + 40] = datas[t];
          }
        }
      }
      for (int t = 0; t < 40; t++) // merge prefer destination - replace values only if destination empty
      {
        for (int i = 0; i < 40; i++)
        {
          if (destination[t] == source[i])
          {
            if (mergePreferDestination[t] && string.IsNullOrEmpty(datas[i])) // replace action
              datas[i + 40] = datas[t];
          }
        }
      }
      for (int t = 0; t < 40; t++) // insert or append values if configured
      {
        for (int i = 0; i < 40; i++) // search for mapping destination
        {
          if (destination[t] == source[i]) // found mapping destination -> datas[i] is destination object !
          {
            if (addStart[t] && !string.IsNullOrEmpty(datas[t])) // addStart action - only of not empty (avoid empty new line)
              datas[i + 40] = datas[t] + System.Environment.NewLine + datas[i + 40];
            if (addEnd[t] && !string.IsNullOrEmpty(datas[t])) // addEnd action - only if not empty (avoid empty new line)
              datas[i + 40] = datas[i + 40] + System.Environment.NewLine + datas[t];
          }
        }
      }
      #endregion

      return datas;
    }

    ///<summary>
    /// Liefert den Inhalt der Datei zurück.
    ///</summary>
    ///<param name="sFilename">Dateipfad</param>
    public string ReadFile(String sFilename)
    {
      string sContent = "";

      if (File.Exists(sFilename))
      {
        var myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
        sContent = myFile.ReadToEnd();
        myFile.Close();
      }
      return sContent;
    }

    public List<DbMovieInfo> GetFanart(string otitle, string ttitle, int year, string director, string imdbid, string fanartPath, bool multiImage, bool choose, string MasterTitle, string personArtworkPath)
    {
      return GetFanart(otitle, ttitle, year, director, imdbid, fanartPath, multiImage, choose, MasterTitle, "", 0, string.Empty, string.Empty);
    }
    public List<DbMovieInfo> GetFanart(string otitle, string ttitle, int year, string director, string imdbid, string fanartPath, bool multiImage, bool choose, string MasterTitle, string personArtworkPath, int downloadlimit, string resolutionMin, string resolutionMax)
    {
      var listemovies = new List<DbMovieInfo>();
      if (otitle.Length == 0) return listemovies;
      if (ttitle.Length == 0) ttitle = otitle;
      string wtitle1 = otitle;
      string wtitle2 = ttitle;
      if (otitle.IndexOf("\\") > 0) wtitle1 = wtitle1.Substring(wtitle1.IndexOf("\\") + 1);
      if (ttitle.IndexOf("\\") > 0) wtitle2 = wtitle2.Substring(wtitle2.IndexOf("\\") + 1);
      var theMoviedb = new TheMoviedb();
      listemovies = theMoviedb.GetMoviesByTitles(wtitle1, wtitle2, year, director, imdbid, choose);

      string filename = string.Empty;
      string filename1 = string.Empty;
      string filename2 = string.Empty;
      if (MasterTitle == "OriginalTitle")
        wtitle2 = wtitle1;
      if (listemovies.Count == 1 && listemovies[0].Backdrops != null && listemovies[0].Backdrops.Count > 0 && !choose)
      {
        // Download Fanart !!!
        bool first = true;
        foreach (string backdrop in listemovies[0].Backdrops)
        {
          // old: filename1 = GrabUtil.DownloadBacdropArt(fanartPath, backdrop, wtitle2, multiImage, first, out filename);
          filename1 = GrabUtil.DownloadBacdropArt(fanartPath, backdrop, wtitle2, multiImage, first, out filename, downloadlimit, resolutionMin, resolutionMax);
          //if (filename2 == string.Empty)
          //    filename2 = filename1;
          if ((filename2 != "added") && (filename1 != "already") && !filename1.StartsWith("numberlimit") && !filename1.StartsWith("resolution"))
          {
            filename2 = "added";
          }
          else
          {
            if (filename1.StartsWith("numberlimit"))
              filename2 = "numberlimit";
            else if (filename1.StartsWith("resolution"))
            {
              filename2 = "resolution";
            }
            else
            {
              filename2 = "already";
              first = false;
            }
          }

        }
        listemovies[0].Name = filename2;

        #region Download PersonArtwork (disabled)
        //// Get Actors from TMDB
        //string filenameperson = string.Empty;
        //string filename1person = string.Empty;
        //string filename2person = string.Empty;
        ////string ImdbBaseUrl = "http://www.imdb.com/";
        //if (!string.IsNullOrEmpty(personArtworkPath) && listemovies[0].Persons != null && listemovies[0].Persons.Count > 0)
        //{
        //  List<grabber.DBPersonInfo> listepersons = listemovies[0].Persons;
        //  foreach (grabber.DBPersonInfo person in listepersons)
        //  {
        //    bool firstpersonimage = true;
        //    grabber.DBPersonInfo persondetails = new DBPersonInfo();
        //    persondetails = TheMoviedb.getPersonsById(person.Id, string.Empty);
        //    foreach (var image in persondetails.Images)
        //    {
        //      filename1person = GrabUtil.DownloadPersonArtwork(personArtworkPath, image, persondetails.Name, multiImage, firstpersonimage, out filenameperson);
        //      if ((filename2person != "added") && (filename1person != "already"))
        //        filename2person = "added";
        //      else
        //        filename2person = "already";
        //      firstpersonimage = false;
        //    }
        //    //// Get further IMDB images
        //    //Grabber.MyFilmsIMDB _imdb = new Grabber.MyFilmsIMDB();
        //    //Grabber.MyFilmsIMDB.IMDBUrl wurl;
        //    //_imdb.FindActor(persondetails.Name);
        //    //IMDBActor imdbActor = new IMDBActor();

        //    //if (_imdb.Count > 0)
        //    //{
        //    //  string url = string.Empty;
        //    //  wurl = (Grabber.MyFilmsIMDB.IMDBUrl)_imdb[0]; // Assume first match is the best !
        //    //  if (wurl.URL.Length != 0)
        //    //  {
        //    //    url = wurl.URL;
        //    //    //url = wurl.URL + "videogallery"; // Assign proper Webpage for Actorinfos
        //    //    //url = ImdbBaseUrl + url.Substring(url.IndexOf("name"));
        //    //    this.GetActorDetails(url, persondetails.Name, false, out imdbActor);
        //    //    filename1person = GrabUtil.DownloadPersonArtwork(personArtworkPath, imdbActor.ThumbnailUrl, persondetails.Name, multiImage, firstpersonimage, out filenameperson);
        //    //    firstpersonimage = false;
        //    //  }
        //    //}
        //  }
        //  //// Get further Actors from IMDB
        //  //IMDBMovie MPmovie = new IMDBMovie();
        //  //MPmovie.Title = listemovies[0].Name;
        //  //MPmovie.IMDBNumber = listemovies[0].ImdbID;
        //  //FetchActorsInMovie(MPmovie, personArtworkPath);
        //}
        #endregion
      }
      else if (listemovies.Count > 1)
      {
        //listemovies[0].Name = "(toomany)";
        listemovies[0].Name = "(toomany) - (" + listemovies.Count.ToString() + " results) - " + listemovies[0].Name;
      }
      return listemovies;
    }

    public List<DbMovieInfo> GetTMDBinfos(string otitle, string ttitle, int year, string director, string fanartPath, bool multiImage, bool choose, string MasterTitle)
    {
      return GetTMDBinfos(otitle, ttitle, year, director, fanartPath, multiImage, choose, MasterTitle, "en");
    }
    public List<DbMovieInfo> GetTMDBinfos(string otitle, string ttitle, int year, string director, string fanartPath, bool multiImage, bool choose, string MasterTitle, string language)
    {

      var listemovies = new List<DbMovieInfo>();
      if (otitle.Length == 0) return listemovies;
      string wtitle1 = otitle;
      string wtitle2 = ttitle;
      if (otitle.IndexOf("\\") > 0)
        wtitle1 = wtitle1.Substring(wtitle1.IndexOf("\\") + 1);
      if (ttitle.IndexOf("\\") > 0)
        wtitle2 = wtitle2.Substring(wtitle2.IndexOf("\\") + 1);
      if (ttitle.Length == 0)
        ttitle = otitle;
      var theMoviedb = new TheMoviedb();
      listemovies = theMoviedb.GetMoviesByTitles(wtitle1, wtitle2, year, director, "", choose, language);

      string filename = string.Empty;
      string filename1 = string.Empty;
      string filename2 = string.Empty;
      if (MasterTitle == "OriginalTitle")
        wtitle2 = wtitle1;
      if (listemovies.Count == 1 && listemovies[0].Posters != null && listemovies[0].Posters.Count > 0 && !choose)
      {
        bool first = true;
        foreach (string backdrop in listemovies[0].Posters)
        {
          filename1 = GrabUtil.DownloadCovers(fanartPath, backdrop, wtitle2, multiImage, first, out filename);
          //if (filename2 == string.Empty)
          //    filename2 = filename1;
          if ((filename2 != "added") && (filename1 != "already"))
            filename2 = "added";
          else
            filename2 = "already";
          first = false;
        }
        listemovies[0].Name = filename2;
      }
      else if (listemovies.Count > 1)

        //listemovies[0].Name = "(toomany)";
        listemovies[0].Name = "(toomany) - (" + listemovies.Count + " results) - " + listemovies[0].Name;

      return listemovies;
    }

    public static string ExtractBody(string body, string paramStart, XmlNode n)
    {
      string strStart = string.Empty;
      switch (paramStart)
      {
        case "Original Title":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartOTitle").InnerText);
          break;
        case "Translated Title":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTTitle").InnerText);
          break;
        case "URL cover":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartImg").InnerText);
          break;
        case "Rate 1":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRate").InnerText);
          break;
        case "Rate 2":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRate2").InnerText);
          break;
        case "Synopsys":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartSyn").InnerText);
          break;
        case "Director":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartRealise").InnerText);
          break;
        case "Producer":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartProduct").InnerText);
          break;
        case "Actors":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCredits").InnerText);
          break;
        case "Country":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCountry").InnerText);
          break;
        case "Genre":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartGenre").InnerText);
          break;
        case "Year":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartYear").InnerText);
          break;
        case "Comment":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartComment").InnerText);
          break;
        case "Language":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartLanguage").InnerText);
          break;
        case "Tagline":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartTagline").InnerText);
          break;
        case "Certification":
          strStart = XmlConvert.DecodeName(n.SelectSingleNodeFast("Details/KeyStartCertification").InnerText);
          break;
        default:
          break;
      }
      if (strStart.Length > 0 && body.Length > 0) // Guzzi: Fix for exception when returning wrong webpage without content
        return body.Substring(body.IndexOf(strStart) + strStart.Length - 1);
      else
        return body;

    }

    /// <summary>
    /// count the elements
    /// </summary>
    public int Count
    {
      get { return _elements.Count; }
    }

    public IMDBUrl this[int index]
    {
      get { return (IMDBUrl)_elements[index]; }
    }

    #region helper methods to get infos

    /// <summary>
    /// trys to get a webpage from the specified url and returns the content as string
    /// </summary>
    private string GetPage(string strURL, string strEncode, out string absoluteUri)
    {
      string strBody = "";
      absoluteUri = string.Empty;
      Stream receiveStream = null;
      StreamReader sr = null;
      WebResponse result = null;
      try
      {
        // Make the Webrequest
        var req = WebRequest.Create(strURL);
        try
        {
          // Use the current user in case an NTLM Proxy or similar is used.
          // wr.Proxy = WebProxy.GetDefaultProxy();
          req.Proxy.Credentials = CredentialCache.DefaultCredentials;
        }
        catch (Exception) { }
        result = req.GetResponse();
        receiveStream = result.GetResponseStream();

        // Encoding: depends on selected page
        Encoding encode = Encoding.GetEncoding(strEncode);
        using (sr = new StreamReader(receiveStream, encode))
        {
          strBody = sr.ReadToEnd();
        }
        absoluteUri = result.ResponseUri.AbsoluteUri;
      }
      catch (Exception)
      {
        //Log.Error("Error retreiving WebPage: {0} Encoding:{1} err:{2} stack:{3}", strURL, strEncode, ex.Message, ex.StackTrace);
      }
      finally
      {
        if (sr != null)
        {
          try
          {
            sr.Close();
          }
          catch (Exception) { }
        }
        if (receiveStream != null)
        {
          try
          {
            receiveStream.Close();
          }
          catch (Exception) { }
        }
        if (result != null)
        {
          try
          {
            result.Close();
          }
          catch (Exception) { }
        }
      }
      return strBody;
    }

    /// <summary>
    /// cuts end of sting after strWord
    /// </summary>
    private void RemoveAllAfter(ref string strLine, string strWord)
    {
      int iPos = strLine.IndexOf(strWord);
      if (iPos > 0)
      {
        strLine = strLine.Substring(0, iPos);
      }
    }

    /// <summary>
    /// make a searchstring out of the filename
    /// </summary>
    private string GetSearchString(string strMovie)
    {
      string strUrl = strMovie;
      strUrl = strUrl.ToLower();
      strUrl = strUrl.Trim();

      RemoveAllAfter(ref strUrl, "divx");
      RemoveAllAfter(ref strUrl, "xvid");
      RemoveAllAfter(ref strUrl, "dvd");
      RemoveAllAfter(ref strUrl, " dvdrip");
      RemoveAllAfter(ref strUrl, "svcd");
      RemoveAllAfter(ref strUrl, "mvcd");
      RemoveAllAfter(ref strUrl, "vcd");
      RemoveAllAfter(ref strUrl, "cd");
      RemoveAllAfter(ref strUrl, "ac3");
      RemoveAllAfter(ref strUrl, "ogg");
      RemoveAllAfter(ref strUrl, "ogm");
      RemoveAllAfter(ref strUrl, "internal");
      RemoveAllAfter(ref strUrl, "fragment");
      RemoveAllAfter(ref strUrl, "proper");
      RemoveAllAfter(ref strUrl, "limited");
      RemoveAllAfter(ref strUrl, "rerip");
      RemoveAllAfter(ref strUrl, "bluray");
      RemoveAllAfter(ref strUrl, "brrip");
      RemoveAllAfter(ref strUrl, "hddvd");
      RemoveAllAfter(ref strUrl, "x264");
      RemoveAllAfter(ref strUrl, "mbluray");
      RemoveAllAfter(ref strUrl, "1080p");
      RemoveAllAfter(ref strUrl, "720p");
      RemoveAllAfter(ref strUrl, "480p");
      RemoveAllAfter(ref strUrl, "r5");

      RemoveAllAfter(ref strUrl, "+divx");
      RemoveAllAfter(ref strUrl, "+xvid");
      RemoveAllAfter(ref strUrl, "+dvd");
      RemoveAllAfter(ref strUrl, "+dvdrip");
      RemoveAllAfter(ref strUrl, "+svcd");
      RemoveAllAfter(ref strUrl, "+mvcd");
      RemoveAllAfter(ref strUrl, "+vcd");
      RemoveAllAfter(ref strUrl, "+cd");
      RemoveAllAfter(ref strUrl, "+ac3");
      RemoveAllAfter(ref strUrl, "+ogg");
      RemoveAllAfter(ref strUrl, "+ogm");
      RemoveAllAfter(ref strUrl, "+internal");
      RemoveAllAfter(ref strUrl, "+fragment");
      RemoveAllAfter(ref strUrl, "+proper");
      RemoveAllAfter(ref strUrl, "+limited");
      RemoveAllAfter(ref strUrl, "+rerip");
      RemoveAllAfter(ref strUrl, "+bluray");
      RemoveAllAfter(ref strUrl, "+brrip");
      RemoveAllAfter(ref strUrl, "+hddvd");
      RemoveAllAfter(ref strUrl, "+x264");
      RemoveAllAfter(ref strUrl, "+mbluray");
      RemoveAllAfter(ref strUrl, "+1080p");
      RemoveAllAfter(ref strUrl, "+720p");
      RemoveAllAfter(ref strUrl, "+480p");
      RemoveAllAfter(ref strUrl, "+r5");
      return strUrl;
    }

    private string LoadPage(string page)
    {
      string strActivePage;
      switch (page)
      {
        case "URL Gateway":
          strActivePage = BodyDetail2;
          break;
        case "URL Redirection Generic 1":
          strActivePage = BodyLinkGeneric1;
          break;
        case "URL Redirection Generic 2":
          strActivePage = BodyLinkGeneric2;
          break;
        case "URL Redirection Cover":
          strActivePage = BodyLinkImg;
          break;
        case "URL Redirection Persons":
          strActivePage = BodyLinkPersons;
          break;
        case "URL Redirection Title":
          strActivePage = BodyLinkTitles;
          break;
        case "URL Redirection Certification":
          strActivePage = BodyLinkCertification;
          break;
        case "URL Redirection Comment":
          strActivePage = BodyLinkComment;
          break;
        case "URL Redirection Description":
          strActivePage = BodyLinkSyn;
          break;
        case "URL Redirection Multi Posters":
          strActivePage = BodyLinkMultiPosters;
          break;
        case "URL Redirection Photos":
          strActivePage = BodyLinkPhotos;
          break;
        case "URL Redirection PersonImages":
          strActivePage = BodyLinkPersonImages;
          break;
        case "URL Redirection Multi Fanart":
          strActivePage = BodyLinkMultiFanart;
          break;
        case "URL Redirection Trailer":
          strActivePage = BodyLinkTrailer;
          break;
        case "DetailsPath":
          strActivePage = BodyLinkDetailsPath;
          break;
        default:
          strActivePage = BodyDetail;
          break;
      }
      return strActivePage;
    }

    #endregion
  }
}
