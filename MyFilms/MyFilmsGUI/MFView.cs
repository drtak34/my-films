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

    public MFView()
    {
      CurrentContext = MyFilms.ViewContext.Menu;
    }

    public MyFilms.ViewContext CurrentContext { get; set; }  // "Boolselect" - or Films, Views, Persons, Herarchies ... GetSelectFromDivX or GetFilmList ...
    public int ID { get; set; }
    public int PrevID { get; set; }  // to access  a state via "PrevID"
    public View StartSettings { get; set; }
    public View CurrentSettings { get; set; }
  }

  public class View
  {
    public View()
    {
      FilmsSortDirection = string.Empty;
      FilmsSortItemFriendlyName = string.Empty;
      FilmsSortItem = string.Empty;
      FilmsLayout = 0;
      HierarchySortDirection = string.Empty;
      HierarchySortItemFriendlyName = string.Empty;
      HierarchySortItem = string.Empty;
      HierarchyLayout = 0;
      PersonsSortDirection = string.Empty;
      PersonsSortItemFriendlyName = string.Empty;
      PersonsSortType = MyFilms.ViewSortType.Name;
      PersonsLayout = 0;
      ViewFilter = string.Empty;
      ViewSortDirection = string.Empty;
      ViewSortType = MyFilms.ViewSortType.Name;
      ViewLayout = 0;
      ViewDBItemValue = string.Empty;
      ViewDBItem = string.Empty;
      ViewDisplayName = string.Empty;
      ViewContext = MyFilms.ViewContext.Menu;
      ID = -1;
    }

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    #region public vars

    public int ID { get; set; }
    public MyFilms.ViewContext ViewContext { get; set; }
    public string ViewDisplayName { get; set; }
    public string ViewDBItem { get; set; }
    public string ViewDBItemValue { get; set; }
    public MyFilms.Layout ViewLayout { get; set; }
    public MyFilms.ViewSortType ViewSortType { get; set; }
    public string ViewSortDirection { get; set; }
    public string ViewFilter { get; set; }
    public MyFilms.Layout PersonsLayout { get; set; }
    public MyFilms.ViewSortType PersonsSortType { get; set; }
    public string PersonsSortItemFriendlyName { get; set; }
    public string PersonsSortDirection { get; set; }
    public MyFilms.Layout HierarchyLayout { get; set; }
    public string HierarchySortItem { get; set; }
    public string HierarchySortItemFriendlyName { get; set; }
    public string HierarchySortDirection { get; set; }
    public MyFilms.Layout FilmsLayout { get; set; }
    public string FilmsSortItem { get; set; }
    public string FilmsSortItemFriendlyName { get; set; }
    public string FilmsSortDirection { get; set; }

    public void InitDefaults()
    {
      this.ID = -1;
      this.ViewContext = MyFilms.ViewContext.Menu;
      this.ViewDisplayName = "Films";
      this.ViewDBItem = "OriginalTitle";
      this.ViewDBItemValue = "";
      this.ViewLayout = MyFilms.Layout.List;
      this.ViewSortType = MyFilms.ViewSortType.Name;
      this.ViewSortDirection = " ASC";
      this.ViewFilter = string.Empty;
      this.PersonsLayout = MyFilms.Layout.List;
      this.PersonsSortType = MyFilms.ViewSortType.Name;
      this.PersonsSortItemFriendlyName = string.Empty;
      this.PersonsSortDirection = " ASC";
      this.HierarchyLayout = MyFilms.Layout.List;
      this.HierarchySortItem = "OriginalTitle";
      this.HierarchySortItemFriendlyName = string.Empty;
      this.HierarchySortDirection = " ASC";
      this.FilmsLayout = MyFilms.Layout.List;
      this.FilmsSortItem = "SortTitle";
      this.FilmsSortItemFriendlyName = string.Empty;
      this.FilmsSortDirection = " ASC";
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
