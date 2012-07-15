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

    public string Title { get; set; }
    public string ItemType { get; set; }

    public NavigationObject()
    {
      Items = new List<GUIListItem>();
      Title = string.Empty;
    }

    public int Position { get; set; }

    public NavigationObject(GUIListControl control, string title, string itemtype, int pos, MyFilms.Layout curview)
    {
      Items = new List<GUIListItem>();
      GetItems(control, title, itemtype, pos, curview);
    }

    public void GetItems(GUIListControl control, string title, string itemtype, int pos, MyFilms.Layout curview)
    {
      Title = title;
      Position = pos;
      CurrentView = curview;
      ItemType = itemtype;
      Items = control.ListItems.GetRange(0, control.ListItems.Count);
    }

    public MyFilms.Layout CurrentView { get; set; }


    public void SetItems(GUIFacadeControl control)
    {
      foreach (GUIListItem item in Items)
      {
        control.Add(item);
      }
    }
  }
}
