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

}
