using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;


namespace MyFilmsPlugin.MyFilmsGUI
{
  using MediaPortal.GUI.Library;

  using MyFilmsPlugin.MyFilms.MyFilmsGUI;

  public class MFView
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    public MFView() { }
    public MyFilms.ViewContext CurrentContext // "Boolselect" - or Films, Views, Persons, Herarchies ... GetSelectFromDivX or GetFilmList ...
    {
      get { return currentContext; }
      set { currentContext = value; }
    }
    private MyFilms.ViewContext currentContext = MyFilms.ViewContext.Menu;

    public int ID // to access  a state via "PrevID"
    {
      get { return _ID; }
      set { _ID = value; }
    }
    private int _ID;

    public int PrevID // to access  a state via "PrevID"
    {
      get { return prevID; }
      set { prevID = value; }
    }
    private int prevID;

    public View StartSettings
    {
      get { return startSettings; }
      set { startSettings = value; }
    }
    private View startSettings;

    public View CurrentSettings
    {
      get { return currentSettings; }
      set { currentSettings = value; }
    }
    private View currentSettings;
  }

  public class View
  {
    public View() { }
   
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    #region public vars
    public int ID
    {
      get { return _ID; }
      set { _ID = value; }
    }
    private int _ID = -1;

    public MyFilms.ViewContext ViewContext
    {
      get { return viewContext; }
      set { viewContext = value; }
    }
    private MyFilms.ViewContext viewContext = MyFilms.ViewContext.Menu;

    public string ViewDisplayName
    {
      get { return viewDisplayName; }
      set { viewDisplayName = value; }
    }
    private string viewDisplayName = string.Empty;

    public string ViewDBItem
    {
      get { return viewDBItem; }
      set { viewDBItem = value; }
    }
    private string viewDBItem = string.Empty;

    public string ViewDBItemValue
    {
      get { return viewDBItemValue; }
      set { viewDBItemValue = value; }
    }
    private string viewDBItemValue = string.Empty;

    public MyFilms.Layout ViewLayout
    {
      get { return viewLayout; }
      set { viewLayout = value; }
    }
    private MyFilms.Layout viewLayout = 0;

    public MyFilms.ViewSortType ViewSortType // Name or Occurencies
    {
      get { return viewSortType; }
      set { viewSortType = value; }
    }
    private MyFilms.ViewSortType viewSortType = MyFilms.ViewSortType.Name;

    public string ViewSortDirection
    {
      get { return viewSortDirection; }
      set { viewSortDirection = value; }
    }
    private string viewSortDirection = string.Empty;

    public string ViewFilter  // to e.g. apply userdefined or global filters on view !
    {
      get { return viewFilter; }
      set { viewFilter = value; }
    }
    private string viewFilter = string.Empty;

    public MyFilms.Layout PersonsLayout
    {
      get { return personsLayout; }
      set { personsLayout = value; }
    }
    private MyFilms.Layout personsLayout = 0;

    public MyFilms.ViewSortType PersonsSortType // Name or Occurencies
    {
      get { return personsSortType; }
      set { personsSortType = value; }
    }
    private MyFilms.ViewSortType personsSortType = MyFilms.ViewSortType.Name;

    public string PersonsSortItemFriendlyName
    {
      get { return personsSortItemFriendlyName; }
      set { personsSortItemFriendlyName = value; }
    }
    private string personsSortItemFriendlyName = string.Empty;

    public string PersonsSortDirection
    {
      get { return personsSortDirection; }
      set { personsSortDirection = value; }
    }
    private string personsSortDirection = string.Empty;

    public MyFilms.Layout HierarchyLayout
    {
      get { return hierarchyLayout; }
      set { hierarchyLayout = value; }
    }
    private MyFilms.Layout hierarchyLayout = 0;

    public string HierarchySortItem
    {
      get { return hierarchySortItem; }
      set { hierarchySortItem = value; }
    }
    private string hierarchySortItem = string.Empty;

    public string HierarchySortItemFriendlyName
    {
      get { return hierarchySortItemFriendlyName; }
      set { hierarchySortItemFriendlyName = value; }
    }
    private string hierarchySortItemFriendlyName = string.Empty;

    public string HierarchySortDirection
    {
      get { return hierarchySortDirection; }
      set { hierarchySortDirection = value; }
    }
    private string hierarchySortDirection = string.Empty;

    public MyFilms.Layout FilmsLayout
    {
      get { return filmsLayout; }
      set { filmsLayout = value; }
    }
    private MyFilms.Layout filmsLayout = 0;

    public string FilmsSortItem
    {
      get { return filmsSortItem; }
      set { filmsSortItem = value; }
    }
    private string filmsSortItem = string.Empty;

    public string FilmsSortItemFriendlyName
    {
      get { return filmsSortItemFriendlyName; }
      set { filmsSortItemFriendlyName = value; }
    }
    private string filmsSortItemFriendlyName = string.Empty;

    public string FilmsSortDirection
    {
      get { return filmsSortDirection; }
      set { filmsSortDirection = value; }
    }
    private string filmsSortDirection = string.Empty;


    public void InitDefaults()
    {
      _ID = -1;
      viewContext = MyFilms.ViewContext.Menu;
      viewDisplayName = "Films";
      viewDBItem = "OriginalTitle";
      viewDBItemValue = "";
      viewLayout = MyFilms.Layout.List;
      viewSortType = MyFilms.ViewSortType.Name;
      viewSortDirection = " ASC";
      viewFilter = string.Empty;
      personsLayout = MyFilms.Layout.List;
      personsSortType = MyFilms.ViewSortType.Name;
      personsSortItemFriendlyName = string.Empty;
      personsSortDirection = " ASC";
      hierarchyLayout = MyFilms.Layout.List;
      hierarchySortItem = "OriginalTitle";
      hierarchySortItemFriendlyName = string.Empty;
      hierarchySortDirection = " ASC";
      filmsLayout = MyFilms.Layout.List;
      filmsSortItem = "SortTitle";
      filmsSortItemFriendlyName = string.Empty;
      filmsSortDirection = " ASC";
    }

    public string SaveToString()
    {
      string savestring = "";
      savestring = ID + "|" + ViewDisplayName + "|" + ViewDBItem + "|" + ViewDBItemValue + "|" +
                   Enum.GetName(typeof(MyFilms.Layout), ViewLayout) + "|" + Enum.GetName(typeof(MyFilms.ViewSortType), ViewSortType) + "|" + ViewSortDirection + "|" + ViewFilter + "|" + 
                   Enum.GetName(typeof(MyFilms.Layout), PersonsLayout) + "|" + Enum.GetName(typeof(MyFilms.ViewSortType), PersonsSortType) + "|" + PersonsSortItemFriendlyName + "|" + PersonsSortDirection + "|" +
                   Enum.GetName(typeof(MyFilms.Layout), HierarchyLayout) + "|" + HierarchySortItem + "|" + HierarchySortItemFriendlyName + "|" + HierarchySortDirection + "|" +
                   Enum.GetName(typeof(MyFilms.Layout), FilmsLayout) + "|" + FilmsSortItem + "|" + FilmsSortItemFriendlyName + "|" + FilmsSortDirection;
      LogMyFilms.Debug("SaveToString() - output = '" + savestring + "'");
      return savestring;
    }

    public void LoadFromString(string inputstring)
    {
      int i = 0;
      string[] split = inputstring.Split(new char[] { '|' }, StringSplitOptions.None);
      LogMyFilms.Debug("LoadFromString() - parsed '" + split.Length + "' elements from inputstring = '" + inputstring + "'");
      foreach (string s in split)
      {
        LogMyFilms.Debug("LoadFromString() - Parsed Value [" + i + "] = '" + s + "'");
        i++;
      }
      ID = int.Parse(split[0]);
      // viewContext = (MyFilms.ViewContext)Enum.Parse(typeof(MyFilms.ViewContext), split[2], true);
      ViewDisplayName = split[1];
      ViewDBItem = split[2];
      ViewDBItemValue = split[3];
      ViewLayout = (MyFilms.Layout)Enum.Parse(typeof(MyFilms.Layout), split[4], true);
      ViewSortType = (MyFilms.ViewSortType)Enum.Parse(typeof(MyFilms.ViewSortType), split[5], true);
      ViewSortDirection = split[6];
      ViewFilter = split[7];
      PersonsLayout = (MyFilms.Layout)Enum.Parse(typeof(MyFilms.Layout), split[8], true);
      PersonsSortType = (MyFilms.ViewSortType)Enum.Parse(typeof(MyFilms.ViewSortType), split[9], true);
      PersonsSortItemFriendlyName = split[10];
      PersonsSortDirection = split[11];
      HierarchyLayout = (MyFilms.Layout)Enum.Parse(typeof(MyFilms.Layout), split[12], true);
      HierarchySortItem = split[13];
      HierarchySortItemFriendlyName = split[14];
      HierarchySortDirection = split[15];
      FilmsLayout = (MyFilms.Layout)Enum.Parse(typeof(MyFilms.Layout), split[16], true);
      FilmsSortItem = split[17];
      FilmsSortItemFriendlyName = split[18];
      FilmsSortDirection = split[19];
    }
    #endregion
  }

  public class CoverState
  {
    public CoverState(){}

    public CoverState(string menucover, string filmcover, string viewcover, string personcover, string groupcover)
    {
      MenuCover = menucover;
      FilmCover = filmcover;
      ViewCover = viewcover;
      PersonCover = personcover;
      GroupCover = groupcover;
    }

    #region public vars

    public string MenuCover { get; set; }
    public string FilmCover { get; set; }
    public string ViewCover { get; set; }
    public string PersonCover { get; set; }
    public string GroupCover { get; set; }

    #endregion
  }

  
  public class ViewState
  {
    public ViewState() { }

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    #region public vars

    private string strSelect = string.Empty;
    public string StrSelect
    {
      get { return strSelect; }
      set { strSelect = value; }
    }

    private string strPersons = string.Empty;
    public string StrPersons
    {
      get { return strPersons; }
      set { strPersons = value; }
    }

    private string strTitleSelect = string.Empty;
    public string StrTitleSelect
    {
      get { return strTitleSelect; }
      set { strTitleSelect = value; }
    }

    private string strFilmSelect = string.Empty;
    public string StrFilmSelect
    {
      get { return strFilmSelect; }
      set { strFilmSelect = value; }
    }

    private MyFilms.ViewContext viewContext = MyFilms.ViewContext.Menu;
    public MyFilms.ViewContext ViewContext
    {
      get { return viewContext; }
      set { viewContext = value; }
    }

    private string currentView = string.Empty;
    public string CurrentView
    {
      get { return currentView; }
      set { currentView = value; }
    }

    private string strTxtView = string.Empty;
    public string StrTxtView
    {
      get { return strTxtView; }
      set { strTxtView = value; }
    }

    private string strTxtSelect = string.Empty;
    public string StrTxtSelect
    {
      get { return strTxtSelect; }
      set { strTxtSelect = value; }
    }

    private bool boolselect;
    public bool Boolselect
    {
      get { return boolselect; }
      set { boolselect = value; }
    }

    private bool boolreturn;
    public bool Boolreturn
    {
      get { return boolreturn; }
      set { boolreturn = value; }
    }

    private bool boolindexed;
    public bool Boolindexed
    {
      get { return boolindexed; }
      set { boolindexed = value; }
    }

    private bool boolindexedreturn;
    public bool Boolindexedreturn
    {
      get { return boolindexedreturn; }
      set { boolindexedreturn = value; }
    }

    private int indexedChars = 0;
    public int IndexedChars
    {
      get { return indexedChars; }
      set { indexedChars = value; }
    }

    private bool boolReverseNames;
    public bool BoolReverseNames
    {
      get { return boolReverseNames; }
      set { boolReverseNames = value; }
    }

    private bool boolShowEmptyValuesInViews;
    public bool BoolShowEmptyValuesInViews
    {
      get { return boolShowEmptyValuesInViews; }
      set { boolShowEmptyValuesInViews = value; }
    }

    private bool boolSkipViewState = false;
    public bool BoolSkipViewState
    {
      get { return boolSkipViewState; }
      set { boolSkipViewState = value; }
    }

    private string wselectedlabel = string.Empty;
    public string Wselectedlabel
    {
      get { return wselectedlabel; }
      set { wselectedlabel = value; }
    }

    private string wStrSort = string.Empty;
    public string WStrSort
    {
      get { return wStrSort; }
      set { wStrSort = value; }
    }

    private string wStrSortSensCount = string.Empty;
    public string WStrSortSensCount
    {
      get { return wStrSortSensCount; }
      set { wStrSortSensCount = value; }
    }

    private bool boolSortCountinViews;
    public bool BoolSortCountinViews
    {
      get { return boolSortCountinViews; }
      set { boolSortCountinViews = value; }
    }

    private string wstar = string.Empty;
    public string Wstar
    {
      get { return wstar; }
      set { wstar = value; }
    }

    private int strLayOut = 0;
    public int StrLayOut
    {
      get { return strLayOut; }
      set { strLayOut = value; }
    }

    private int wStrLayOut = 0;
    public int WStrLayOut
    {
      get { return wStrLayOut; }
      set { wStrLayOut = value; }
    }

    private int strLayOutInHierarchies = 0;
    public int StrLayOutInHierarchies
    {
      get { return this.strLayOutInHierarchies; }
      set { this.strLayOutInHierarchies = value; }
    }

    private int lastID = 0;
    public int LastID
    {
      get { return lastID; }
      set { lastID = value; }
    }

    private int indexItem = 0;
    public int IndexItem
    {
      get { return indexItem; }
      set { indexItem = value; }
    }

    private string titleItem = string.Empty;
    public string TitleItem
    {
      get { return titleItem; }
      set { titleItem = value; }
    }
    // CurrentView", MyFilms.conf.CurrentView.SaveToString());
    #endregion

    public void InitDefaults()
    {
      strSelect = string.Empty;
      strPersons = string.Empty;
      strTitleSelect = string.Empty;
      strFilmSelect = string.Empty;
      viewContext = MyFilms.ViewContext.Menu;
      strTxtView = string.Empty;
      strTxtSelect = string.Empty;
      boolselect = false;
      boolreturn = false;
      boolindexed = false;
      boolindexedreturn = false;
      indexedChars = 0;
      boolReverseNames = false;
      boolShowEmptyValuesInViews = false;
      wselectedlabel = string.Empty;
      wStrSort = string.Empty;
      wStrSortSensCount = string.Empty;
      boolSortCountinViews = false;
      wstar = string.Empty;
      strLayOut = 0;
      wStrLayOut = 0;
      strLayOutInHierarchies = 0;
      lastID = 0;
      indexItem = 0;
      titleItem = string.Empty;
    }

    public string SaveToString()
    {
      string savestring =
        strSelect + "|" +
        strPersons + "|" +
        strTitleSelect + "|" +
        strFilmSelect + "|" +
        Enum.GetName(typeof(MyFilms.Layout), viewContext) + "|" +
        strTxtView + "|" +
        strTxtSelect + "|" +
        boolselect.ToString() + "|" +
        boolreturn.ToString() + "|" +
        boolindexed.ToString() + "|" +
        boolindexedreturn.ToString() + "|" +
        indexedChars.ToString() + "|" +
        boolReverseNames.ToString() + "|" +
        boolShowEmptyValuesInViews.ToString() + "|" +
        wselectedlabel + "|" +
        wStrSort + "|" +
        wStrSortSensCount + "|" +
        boolSortCountinViews.ToString() + "|" +
        wstar + "|" +
        strLayOut.ToString() + "|" +
        wStrLayOut.ToString() + "|" +
        strLayOutInHierarchies.ToString() + "|" +
        lastID.ToString() + "|" +
        indexItem.ToString() + "|" +
        titleItem;
      LogMyFilms.Debug("SaveToString() - output = '" + savestring + "'");
      return savestring;
    }

    public void LoadFromString(string inputstring)
    {
      int i = 0;
      string[] split = inputstring.Split(new char[] { '|' }, StringSplitOptions.None);
      LogMyFilms.Debug("LoadFromString() - parsed '" + split.Length + "' elements from inputstring = '" + inputstring + "'");
      foreach (string s in split)
      {
        LogMyFilms.Debug("LoadFromString() - Parsed Value [" + i + "] = '" + s + "'");
        i++;
      }
      strSelect = split[0];
      strPersons = split[1];
      strTitleSelect = split[2];
      strFilmSelect = split[3];
      viewContext = (MyFilms.ViewContext)Enum.Parse(typeof(MyFilms.ViewContext), split[4], true); // MyFilms.ViewContext.Menu;
      strTxtView = split[5];
      strTxtSelect = split[6];
      boolselect = bool.Parse(split[7]);
      boolreturn = bool.Parse(split[8]);
      boolindexed = bool.Parse(split[9]);
      boolindexedreturn = bool.Parse(split[10]);
      indexedChars = int.Parse(split[11]);
      boolReverseNames = bool.Parse(split[12]);
      boolShowEmptyValuesInViews = bool.Parse(split[13]);
      wselectedlabel = split[14];
      wStrSort = split[15];
      wStrSortSensCount = split[16];
      boolSortCountinViews = bool.Parse(split[17]);
      wstar = split[18];
      strLayOut = int.Parse(split[19]);
      wStrLayOut = int.Parse(split[20]);
      strLayOutInHierarchies = int.Parse(split[21]);
      lastID = int.Parse(split[22]);
      indexItem = int.Parse(split[23]);
      titleItem = split[24];
    }
  }

}
