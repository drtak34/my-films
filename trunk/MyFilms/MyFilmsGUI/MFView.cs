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

    public int ID { get; set; }

    public int PrevID { get; set; }  // to access  a state via "PrevID"

    public View StartSettings { get; set; }

    public View CurrentSettings { get; set; }
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
    public ViewState()
    {
      StrSelect = string.Empty;
      StrPersons = string.Empty;
      StrTitleSelect = string.Empty;
      StrFilmSelect = string.Empty;
      ViewContext = MyFilms.ViewContext.Menu;
      CurrentView = string.Empty;
      StrTxtView = string.Empty;
      StrTxtSelect = string.Empty;
      WStrSort = string.Empty;
      TitleItem = string.Empty;
      IndexItem = 0;
      LastID = 0;
      StrLayOutInHierarchies = 0;
      WStrLayOut = 0;
      StrLayOut = 0;
      Wstar = string.Empty;
      WStrSortSensCount = string.Empty;
      Wselectedlabel = string.Empty;
      IndexedChars = 0;
      BoolSkipViewState = false;
    }

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    #region public vars
    public string StrSelect { get; set; }
    public string StrPersons { get; set; }
    public string StrTitleSelect { get; set; }
    public string StrFilmSelect { get; set; }
    public MyFilms.ViewContext ViewContext { get; set; }
    public string CurrentView { get; set; }
    public string StrTxtView { get; set; }
    public string StrTxtSelect { get; set; }
    public bool Boolselect { get; set; }
    public bool Boolreturn { get; set; }
    public bool Boolindexed { get; set; }
    public bool Boolindexedreturn { get; set; }
    public int IndexedChars { get; set; }
    public bool BoolReverseNames { get; set; }
    public bool BoolVirtualPathBrowsing { get; set; }
    public bool BoolShowEmptyValuesInViews { get; set; }
    public bool BoolSkipViewState { get; set; }
    public string Wselectedlabel { get; set; }
    public string WStrSort { get; set; }
    public string WStrSortSensCount { get; set; }
    public bool BoolSortCountinViews { get; set; }
    public string Wstar { get; set; }
    public int StrLayOut { get; set; }
    public int WStrLayOut { get; set; }
    public int StrLayOutInHierarchies { get; set; }
    public int LastID { get; set; }
    public int IndexItem { get; set; }
    public string TitleItem { get; set; }
    // CurrentView", MyFilms.conf.CurrentView.SaveToString());
    #endregion

    public void InitDefaults()
    {
      this.StrSelect = string.Empty;
      this.StrPersons = string.Empty;
      this.StrTitleSelect = string.Empty;
      this.StrFilmSelect = string.Empty;
      this.ViewContext = MyFilms.ViewContext.Menu;
      this.StrTxtView = string.Empty;
      this.StrTxtSelect = string.Empty;
      this.Boolselect = false;
      this.Boolreturn = false;
      this.Boolindexed = false;
      this.Boolindexedreturn = false;
      this.IndexedChars = 0;
      this.BoolReverseNames = false;
      this.BoolVirtualPathBrowsing = false;
      this.BoolShowEmptyValuesInViews = false;
      this.Wselectedlabel = string.Empty;
      this.WStrSort = string.Empty;
      this.WStrSortSensCount = string.Empty;
      this.BoolSortCountinViews = false;
      this.Wstar = string.Empty;
      this.StrLayOut = 0;
      this.WStrLayOut = 0;
      this.StrLayOutInHierarchies = 0;
      this.LastID = 0;
      this.IndexItem = 0;
      this.TitleItem = string.Empty;
    }

    public string SaveToString()
    {
      string savestring =
        this.StrSelect + "|" +
        this.StrPersons + "|" +
        this.StrTitleSelect + "|" +
        this.StrFilmSelect + "|" +
        Enum.GetName(typeof(MyFilms.Layout), this.ViewContext) + "|" +
        this.StrTxtView + "|" +
        this.StrTxtSelect + "|" +
        this.Boolselect.ToString() + "|" +
        this.Boolreturn.ToString() + "|" +
        this.Boolindexed.ToString() + "|" +
        this.Boolindexedreturn.ToString() + "|" +
        this.IndexedChars.ToString() + "|" +
        this.BoolReverseNames.ToString() + "|" +
        this.BoolShowEmptyValuesInViews.ToString() + "|" +
        this.Wselectedlabel + "|" +
        this.WStrSort + "|" +
        this.WStrSortSensCount + "|" +
        this.BoolSortCountinViews.ToString() + "|" +
        this.Wstar + "|" +
        this.StrLayOut.ToString() + "|" +
        this.WStrLayOut.ToString() + "|" +
        this.StrLayOutInHierarchies.ToString() + "|" +
        this.LastID.ToString() + "|" +
        this.IndexItem.ToString() + "|" +
        this.TitleItem + "|" +
        this.BoolVirtualPathBrowsing.ToString() ;
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
      this.StrSelect = split[0];
      this.StrPersons = split[1];
      this.StrTitleSelect = split[2];
      this.StrFilmSelect = split[3];
      this.ViewContext = (MyFilms.ViewContext)Enum.Parse(typeof(MyFilms.ViewContext), split[4], true); // MyFilms.ViewContext.Menu;
      this.StrTxtView = split[5];
      this.StrTxtSelect = split[6];
      this.Boolselect = bool.Parse(split[7]);
      this.Boolreturn = bool.Parse(split[8]);
      this.Boolindexed = bool.Parse(split[9]);
      this.Boolindexedreturn = bool.Parse(split[10]);
      this.IndexedChars = int.Parse(split[11]);
      this.BoolReverseNames = bool.Parse(split[12]);
      this.BoolShowEmptyValuesInViews = bool.Parse(split[13]);
      this.Wselectedlabel = split[14];
      this.WStrSort = split[15];
      this.WStrSortSensCount = split[16];
      this.BoolSortCountinViews = bool.Parse(split[17]);
      this.Wstar = split[18];
      this.StrLayOut = int.Parse(split[19]);
      this.WStrLayOut = int.Parse(split[20]);
      this.StrLayOutInHierarchies = int.Parse(split[21]);
      this.LastID = int.Parse(split[22]);
      this.IndexItem = int.Parse(split[23]);
      this.TitleItem = split[24];
      this.BoolVirtualPathBrowsing = bool.Parse(split[25]);
    }
  }

}
