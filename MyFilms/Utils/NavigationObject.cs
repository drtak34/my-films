using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;


namespace MyFilmsPlugin.Utils
{
  using MyFilmsPlugin.MyFilms.MyFilmsGUI;
  using MyFilmsPlugin.MyFilmsGUI;

  public class NavigationObject
  {
    public List<GUIListItem> Items { get; set; }

    public ViewState ViewStatus { get; set; }

    public string Title { get; set; }
    public string ItemType { get; set; }
    public string NbObjects { get; set; }

    // images
    public CoverState CoverStatus { get; set; }

    public bool SortButtonEnabled { get; set; }
    public bool SortButtonASC { get; set; }
    public string SortButtonLabel { get; set; }

    public string DbDfltSelect { get; set; }
    public string DbSelect { get; set; }
    public string DbField { get; set; }
    public string DbSort { get; set; }
    public bool DbShowAll { get; set; }
    public bool DbExtraSort { get; set; }
    
    public NavigationObject()
    {
      Items = new List<GUIListItem>();
      Title = string.Empty;
      // ViewStatus = new ViewState();
    }

    public int Position { get; set; }

    public MyFilms.Layout CurrentView { get; set; }

    public NavigationObject(GUIListControl control, string title, string itemtype, string nbobjects, int pos, MyFilms.Layout curview, Configuration curconf, GUISortButtonControl srtButton, CoverState coverstate)
    {
      Items = new List<GUIListItem>();
      GetItems(control, title, itemtype, pos, curview);
      ViewStatus = new ViewState();
      GetViewStatus(curconf);

      DbDfltSelect = curconf.DbSelection[0];
      DbSelect = curconf.DbSelection[1];
      DbField = curconf.DbSelection[2];
      DbSort = curconf.DbSelection[3];
      bool showall;
      DbShowAll = (Boolean.TryParse(curconf.DbSelection[4], out showall)) && showall;
      bool extrasort;
      DbExtraSort = (Boolean.TryParse(curconf.DbSelection[4], out extrasort)) && extrasort;

      NbObjects = nbobjects;

      CurrentView = curview;

      SortButtonEnabled = srtButton.IsEnabled;
      SortButtonASC = srtButton.IsAscending;
      SortButtonLabel = srtButton.Label;
      CoverStatus = coverstate;
    }

    public void GetItems(GUIListControl control, string title, string itemtype, int pos, MyFilms.Layout curview)
    {
      Title = title;
      Position = pos;
      CurrentView = curview;
      ItemType = itemtype;
      Items = control.ListItems.GetRange(0, control.ListItems.Count);
    }

    public void SetItems(GUIFacadeControl control)
    {
      foreach (GUIListItem item in Items)
      {
        control.Add(item);
      }
    }

    public void GetViewStatus(Configuration conf)
    {
      ViewStatus.Boolselect = conf.Boolselect;
      ViewStatus.Boolreturn = conf.Boolreturn;
      ViewStatus.Boolindexed = conf.Boolindexed;
      ViewStatus.Boolindexedreturn = conf.Boolindexedreturn;
      ViewStatus.IndexedChars = conf.IndexedChars;
      ViewStatus.BoolReverseNames = conf.BoolReverseNames;
      ViewStatus.BoolShowEmptyValuesInViews = conf.BoolShowEmptyValuesInViews;
      ViewStatus.BoolSkipViewState = conf.BoolSkipViewState;

      ViewStatus.StrSelect = conf.StrSelect;
      ViewStatus.StrPersons = conf.StrPersons;
      ViewStatus.StrTitleSelect = conf.StrTitleSelect;
      ViewStatus.StrFilmSelect = conf.StrFilmSelect;
      ViewStatus.ViewContext = conf.ViewContext;
      ViewStatus.StrTxtView = conf.StrTxtView;
      ViewStatus.StrTxtSelect = conf.StrTxtSelect;

      ViewStatus.Wselectedlabel = conf.Wselectedlabel;
      ViewStatus.WStrSort = conf.WStrSort;
      ViewStatus.WStrSortSensCount = conf.WStrSortSensCount;
      ViewStatus.BoolSortCountinViews = conf.BoolSortCountinViews;
      ViewStatus.Wstar = conf.Wstar;

      ViewStatus.StrLayOut = conf.StrLayOut;
      ViewStatus.WStrLayOut = conf.WStrLayOut;
      ViewStatus.StrLayOutInHierarchies = conf.StrLayOutInHierarchies;
      ViewStatus.LastID = conf.LastID;

      ViewStatus.CurrentView = conf.CurrentView;

      //ViewStatus.IndexItem = (this.facadeFilms.SelectedItem > -1) ? ((MyFilms.conf.Boolselect) ? this.facadeFilms.SelectedListItemIndex : 0) : -1; //may need to check if there is no item selected and so save -1
      //ViewStatus.TitleItem = (this.facadeFilms.SelectedItem > -1) ? ((MyFilms.conf.Boolselect) ? this.facadeFilms.SelectedItem.ToString() : this.facadeFilms.SelectedListItem.Label) : string.Empty; //may need to check if there is no item selected and so save ""
    }

    public void SetViewStatus(Configuration conf)
    {
      conf.Boolselect = ViewStatus.Boolselect;
      conf.Boolreturn = ViewStatus.Boolreturn;
      conf.Boolindexed = ViewStatus.Boolindexed;
      conf.Boolindexedreturn = ViewStatus.Boolindexedreturn;
      conf.IndexedChars = ViewStatus.IndexedChars;
      // conf.BoolReverseNames = ViewStatus.BoolReverseNames; don't restore for Generic view state cache, as it is more a "global" setting
      // conf.BoolShowEmptyValuesInViews = ViewStatus.BoolShowEmptyValuesInViews;
      conf.BoolSkipViewState = ViewStatus.BoolSkipViewState;

      conf.StrSelect = ViewStatus.StrSelect;
      conf.StrPersons = ViewStatus.StrPersons;
      conf.StrTitleSelect = ViewStatus.StrTitleSelect;
      conf.StrFilmSelect = ViewStatus.StrFilmSelect;
      conf.ViewContext = ViewStatus.ViewContext;
      conf.StrTxtView = ViewStatus.StrTxtView;
      conf.StrTxtSelect = ViewStatus.StrTxtSelect;

      conf.Wselectedlabel = ViewStatus.Wselectedlabel;
      conf.WStrSort = ViewStatus.WStrSort;
      conf.WStrSortSensCount = ViewStatus.WStrSortSensCount;
      conf.BoolSortCountinViews = ViewStatus.BoolSortCountinViews;
      conf.Wstar = ViewStatus.Wstar;

      conf.StrLayOut = ViewStatus.StrLayOut;
      conf.WStrLayOut = ViewStatus.WStrLayOut;
      conf.StrLayOutInHierarchies = ViewStatus.StrLayOutInHierarchies;
      conf.LastID = ViewStatus.LastID;

      conf.CurrentView = ViewStatus.CurrentView;

      try { conf.StrIndex = ViewStatus.IndexItem; } catch { conf.StrIndex = -1; }
      conf.StrTIndex = ViewStatus.TitleItem;

      //int IndexItem = ViewStatus.IndexItem;
      //string TitleItem = ViewStatus.TitleItem;

      //IndexItem", (selectedItem > -1) ? ((MyFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItem.ToString()) : "-1"); //may need to check if there is no item selected and so save -1
      //TitleItem", (selectedItem > -1) ? ((MyFilms.conf.Boolselect) ? selectedItem.ToString() : selectedItemLabel) : string.Empty); //may need to check if there is no item selected and so save ""
    }

    public void SetCoverStatus(ref string menucover, ref string filmcover, ref string viewcover, ref string personcover, ref string groupcover)
    {
      menucover = CoverStatus.MenuCover;
      filmcover = CoverStatus.FilmCover;
      viewcover = CoverStatus.ViewCover;
      personcover = CoverStatus.PersonCover;
      groupcover = CoverStatus.GroupCover;
    }
  
  }
}
