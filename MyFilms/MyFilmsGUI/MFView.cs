using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;


namespace MyFilmsPlugin.MyFilmsGUI
{

  public class MFView
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    public MFView() { }
    public ViewContext CurrentContext // "Boolselect" - or Films, Views, Persons, Herarchies ... GetSelectFromDivX or GetFilmList ...
    {
      get { return _CurrentContext; }
      set { _CurrentContext = value; }
    }
    private ViewContext _CurrentContext;

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

  public enum ViewContext
  {
    Movie,
    MovieCollection,
    Group,
    Person,
    None
  }
  public enum ViewSortType
  {
    Name,
    Frequency
  }

  public enum Layout
  {
    List,
    SmallThumbs,
    BigThumbs,
    FilmStrip,
    CoverFlow
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

    public Layout ViewLayout
    {
      get { return viewLayout; }
      set { viewLayout = value; }
    }
    private Layout viewLayout = 0;

    public ViewSortType ViewSortType // Name or Occurencies
    {
      get { return viewSortType; }
      set { viewSortType = value; }
    }
    private ViewSortType viewSortType = ViewSortType.Name;

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

    public Layout PersonsLayout
    {
      get { return personsLayout; }
      set { personsLayout = value; }
    }
    private Layout personsLayout = 0;

    public ViewSortType PersonsSortType // Name or Occurencies
    {
      get { return personsSortType; }
      set { personsSortType = value; }
    }
    private ViewSortType personsSortType = ViewSortType.Name;

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

    public Layout HierarchyLayout
    {
      get { return hierarchyLayout; }
      set { hierarchyLayout = value; }
    }
    private Layout hierarchyLayout = 0;

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

    public Layout FilmsLayout
    {
      get { return filmsLayout; }
      set { filmsLayout = value; }
    }
    private Layout filmsLayout = 0;

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

    
    public string SaveString
    {
      get
      {
        _mSaveString = _ID + "|" + viewDisplayName + "|" + _mContext;
        return _mSaveString;
      }
      set
      {
        _mSaveString = value;
        string[] split = _mSaveString.Split(new char[]{'|'}, StringSplitOptions.None);
        _ID = int.Parse(split[0]);
        viewDisplayName = split[1];
        _mContext = (ViewContext)Enum.Parse(typeof(ViewContext), split[2]);
      }
    }
    private string _mSaveString = string.Empty;

    public ViewContext Context
    {
      get { return _mContext; }
      set { _mContext = value; }
    }
    private ViewContext _mContext = ViewContext.Movie;

    public void InitDefaults()
    {
      _ID = -1;
      viewDisplayName = "Films";
      viewDBItem = "OriginalTitle";
      viewDBItemValue = "";
      viewLayout = Layout.List;
      viewSortType = ViewSortType.Name;
      viewSortDirection = " ASC";
      viewFilter = string.Empty;
      personsLayout = Layout.List;
      personsSortType = ViewSortType.Name;
      personsSortItemFriendlyName = string.Empty;
      personsSortDirection = " ASC";
      hierarchyLayout = Layout.List;
      hierarchySortItem = "OriginalTitle";
      hierarchySortItemFriendlyName = string.Empty;
      hierarchySortDirection = " ASC";
      filmsLayout = Layout.List;
      filmsSortItem = "SortTitle";
      filmsSortItemFriendlyName = string.Empty;
      filmsSortDirection = " ASC";
    }

    #endregion

  }

}
