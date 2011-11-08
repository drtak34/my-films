#region Copyright (C) 2005-2008 Team MediaPortal

/* 
 *	Copyright (C) 2005-2008 Team MediaPortal
 *	http://www.team-mediaportal.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *   
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *   
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA. 
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */

#endregion

//using Grabber.IMDB;
//using MediaPortal.Video.Database;

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  using System;
  using System.Collections.Generic;
  using System.IO;

  using MediaPortal.Dialogs;
  using MediaPortal.GUI.Library;
  using MediaPortal.GUI.Pictures;
  using MediaPortal.Util;
  using MediaPortal.Video.Database;
  using Action = MediaPortal.GUI.Library.Action;
  using MyFilmsPlugin.MyFilms;

  using NLog;

  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;

  /// <summary>
    /// Opens a separate Dialog to display Actor Infos - based on original IMDB actor Dialog
    /// </summary>
  public class MyFilmsActorInfo : GUIDialogWindow
    {
    #region skin
    [SkinControl(2)]
    protected GUIButtonControl btnViewAs = null;
    //[SkinControl(3)]
    //protected GUISortButtonControl btnSortBy = null;
    [SkinControl(3)]
    protected GUIButtonControl btnBiography = null;
    [SkinControl(4)]
    protected GUIButtonControl btnMovies = null;
    //[SkinControl(4)]
    //protected GUIToggleButtonControl btnMovies = null;
    [SkinControl(20)]
    protected GUITextScrollUpControl tbPlotArea = null;
    [SkinControl(21)]
    protected GUIImage imgCoverArt = null;
    [SkinControl(22)]
    protected GUITextControl tbTextArea = null;
    [SkinControl(50)]
    protected GUIFacadeControl facadeView = null;
    #endregion

    private static Logger LogMyFilms = LogManager.GetCurrentClassLogger();  //log
    private int selectedItemIndex = -1;
    View currentView = View.List;
    private List<string> list;

    enum View : int
    {
        List = 0,
        Icons = 1,
        LargeIcons = 2,
    }


    private enum ViewMode
    {
        Biography,
        Movies,
    }

    private ViewMode viewmode = ViewMode.Biography;

    private MediaPortal.Video.Database.IMDBActor currentActor = null;
    //private bool _prevOverlay = false;
    private string imdbCoverArtUrl = string.Empty;

           

    public MyFilmsActorInfo()
    {
        GetID = MyFilms.ID_MyFilmsActorsInfo;
    }

    public override string GetModuleName()
    {
      return GUILocalizeStrings.Get(MyFilms.ID_MyFilmsActorsInfo); // return localized string for dialog's ID
    }

    public override void PreInit() { }

    public override bool Init()
    {
        return Load(GUIGraphicsContext.Skin + @"\MyFilmsActorsInfo.xml");
    }

    public override void DoModal(int ParentID)
    {
      AllocResources();
      InitControls();

      base.DoModal(ParentID);
    }

     public override void OnAction(Action action)
     {
      //if (action.wID == Action.ActionType.ACTION_PREVIOUS_MENU)
      //{
      //  Close();
      //  return;
      //}
      base.OnAction(action);
    }

    protected override void OnPageLoad()
    {
      base.OnPageLoad();
      list = new List<string>();
      GUIControl.HideControl(GetID, 50);
      viewmode = ViewMode.Biography;
      Update();
    }

    protected override void OnPageDestroy(int newWindowId)
    {
      currentActor = null;
      facadeView.Clear();
      base.OnPageDestroy(newWindowId);
    }


    protected override void OnClicked(int controlId, GUIControl control, Action.ActionType actionType)
        {
            base.OnClicked(controlId, control, actionType);

            if (control == btnMovies)
            {
                viewmode = ViewMode.Movies;
                Update();
            }
            if (control == btnBiography)
            {
                viewmode = ViewMode.Biography;
                Update();
            }

            #region ViewAs
            if (control == btnViewAs)
            {
                bool shouldContinue = false;
                do
                {
                    shouldContinue = false;
                    switch (currentView)
#if MP11
                    {
                        case View.List:
                            currentView = View.Icons;
                            if (facadeView.ThumbnailView == null)
                                shouldContinue = true;
                            else
                                facadeView.View = GUIFacadeControl.ViewMode.SmallIcons;
                            break;

                        case View.Icons:
                            currentView = View.LargeIcons;
                            if (facadeView.ThumbnailView == null)
                                shouldContinue = true;
                            else
                                facadeView.View = GUIFacadeControl.ViewMode.LargeIcons;
                            break;

                        case View.LargeIcons:
                            currentView = View.List;
                            if (facadeView.ListView == null)
                                shouldContinue = true;
                            else
                                facadeView.View = GUIFacadeControl.ViewMode.List;
                            break;
                    }
#else
                    {
                        case View.List:
                            currentView = View.Icons;
                            if (facadeView.ThumbnailLayout == null)
                                shouldContinue = true;
                            else
                                facadeView.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
                            break;

                        case View.Icons:
                            currentView = View.LargeIcons;
                            if (facadeView.ThumbnailLayout == null)
                                shouldContinue = true;
                            else
                                facadeView.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
                            break;

                        case View.LargeIcons:
                            currentView = View.List;
                            if (facadeView.ListLayout == null)
                                shouldContinue = true;
                            else
                                facadeView.CurrentLayout = GUIFacadeControl.Layout.List;
                            break;
                    }


#endif
                } while (shouldContinue);
                
                SelectCurrentItem();
                GUIControl.FocusControl(GetID, controlId);
                return;
            }
            #endregion

            if (control == facadeView)
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, facadeView.GetID, 0, 0,
                                                null);
                OnMessage(msg);
                int itemIndex = (int)msg.Param1;

                if (actionType == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM)
                {
                    OnClick(itemIndex);
                }
            }

        }

        public IMDBActor Actor
        {
            get { return currentActor; }
            set { currentActor = value; }
        }

        private void Update()
        {
            if (currentActor == null)
            {
                return;
            }

            //cast->image
            if (viewmode == ViewMode.Movies)
            {
                tbPlotArea.IsVisible = false;
                tbTextArea.IsVisible = true;
                imgCoverArt.IsVisible = true;
                GUIControl.ShowControl(GetID, 50);
                btnBiography.Selected = false;
                btnMovies.Selected = true;
                //GUIControl.ShowControl(GetID, (int)Controls.CTRL_MovieThumb1);
            }
            //cast->plot
            if (viewmode == ViewMode.Biography)
            {
                tbPlotArea.IsVisible = true;
                tbTextArea.IsVisible = false;
                imgCoverArt.IsVisible = true;
                GUIControl.HideControl(GetID, 50);
                btnBiography.Selected = true;
                btnMovies.Selected = false;
            }
            GUIPropertyManager.SetProperty("#Actor.Name", currentActor.Name);
            GUIPropertyManager.SetProperty("#Actor.DateOfBirth", currentActor.DateOfBirth);
            GUIPropertyManager.SetProperty("#Actor.PlaceOfBirth", currentActor.PlaceOfBirth);
            string biography = currentActor.Biography;
            if ((biography == string.Empty) || (biography == Strings.Unknown))
            {
                biography = currentActor.MiniBiography;
                if (biography == Strings.Unknown)
                {
                    biography = "";
                }
            }
            GUIPropertyManager.SetProperty("#Actor.Biography", biography);

            string movies = "";
            facadeView.Clear();
            for (int i = 0; i < currentActor.Count; ++i)
            {
                string line = String.Format("{0}. {1} - {2}", i + 1, currentActor[i].Year, currentActor[i].MovieTitle);
                //string line = String.Format("{0}. {1} ({2})\n            {3}\n", i + 1, currentActor[i].Year, currentActor[i].MovieTitle, currentActor[i].Role);
                movies += line;
                GUIListItem item = new GUIListItem();
                item.Label = line;
                if (!string.IsNullOrEmpty(currentActor[i].Role))
                  item.Label2 = "(" + currentActor[i].Role.ToString() + ")";
                else 
                  item.Label2 = "";
                item.Label3 = "";
                //item.Path = f;
                
                
                string coverArtImage = string.Empty;

                coverArtImage = MyFilms.conf.DefaultCover;

                //if (System.IO.File.Exists(Config.GetDirectoryInfo(Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups\NoPhoto.jpg"))
                //    item.ThumbnailImage = (Config.Dir.Thumbs) + @"\MyFilms\Thumbs\MyFilms_Groups\NoPhoto.jpg"; 
                if (System.IO.File.Exists(MediaPortal.Util.Utils.GetCoverArt(Thumbs.MovieTitle, currentActor[i].MovieTitle)))
                        coverArtImage = MediaPortal.Util.Utils.GetCoverArt(Thumbs.MovieTitle, currentActor[i].MovieTitle);
                LogMyFilms.Debug("MyFilmsActors (Coverartimage) - CoverartImage = '" + coverArtImage + "'");
                if (File.Exists(coverArtImage))
                {
                  item.ThumbnailImage = coverArtImage;
                  item.IconImageBig = coverArtImage;
                  item.IconImage = coverArtImage;
                  //else if (movie.Actor != string.Empty)
                  //{            
                  //  coverArtImage = MediaPortal.Util.Utils.GetCoverArt(Thumbs.MovieActors, movie.Actor);
                  //  if (System.IO.File.Exists(coverArtImage))
                  //  {
                  //    listItem.ThumbnailImage = coverArtImage;
                  //    listItem.IconImageBig = coverArtImage;
                  //    listItem.IconImage = coverArtImage;
                  //  }
                  //}
                  //else if (movie.SingleGenre != string.Empty)
                  //{
                  //  coverArtImage = MediaPortal.Util.Utils.GetCoverArt(Thumbs.MovieGenre, movie.SingleGenre);
                  //  if (System.IO.File.Exists(coverArtImage))
                  //  {
                  //    listItem.ThumbnailImage = coverArtImage;
                  //    listItem.IconImageBig = coverArtImage;
                  //    listItem.IconImage = coverArtImage;
                  //  }
                  //}
                }
                // let's try to assign better covers
                if (!string.IsNullOrEmpty(coverArtImage))
                {
                  coverArtImage = MediaPortal.Util.Utils.ConvertToLargeCoverArt(coverArtImage);
                  if (File.Exists(coverArtImage))
                  {
                    item.ThumbnailImage = coverArtImage;
                  }
                }
                              
                //item.ThumbnailImage = Utils.GetLargeCoverArtName(Thumbs.MovieActors, currentActor.Name);
                facadeView.Add(item);
            }
            //SwitchView();
            //Nötig?
            //base.OnPageLoad();
            GUIPropertyManager.SetProperty("#Actor.Movies", movies);

            string largeCoverArtImage = MyFilms.conf.StrPathArtist + @"\" + currentActor.Name;
            if (!System.IO.File.Exists(largeCoverArtImage))
              largeCoverArtImage = MediaPortal.Util.Utils.GetLargeCoverArtName(Thumbs.MovieActors, currentActor.Name);
            if (imgCoverArt != null)
            {
                imgCoverArt.Dispose();
                imgCoverArt.SetFileName(largeCoverArtImage);
                imgCoverArt.AllocResources();
            }
        }

        //Added for ListControl
        private void OnClick(int itemIndex)
        {
            GUIListItem item = GetSelectedItem();
            if (item == null)
            {
                return;
            }

            selectedItemIndex = GetSelectedItemNo();
            OnShowPicture(item.Path);
        }
        private void OnShowPicture(string strFile)
        {
            GUISlideShow SlideShow = (GUISlideShow)GUIWindowManager.GetWindow((int)Window.WINDOW_SLIDESHOW);
            if (SlideShow == null)
            {
                return;
            }

            SlideShow.Reset();

            foreach (string url in list)
            {
                string fname = Path.GetFileName(url);
                SlideShow.Add(fname);
            }

            if (SlideShow.Count > 0)
            {
                GUIWindowManager.ActivateWindow((int)Window.WINDOW_SLIDESHOW);
                SlideShow.Select(strFile);
            }
        }

        private int GetSelectedItemNo()
        {
            return facadeView.SelectedListItemIndex;
        }
        private GUIListItem GetSelectedItem()
        {
            return facadeView.SelectedListItem;
        }

        void SelectCurrentItem()
        {
            int iItem = facadeView.SelectedListItemIndex;
            if (iItem > -1)
            {
                GUIControl.SelectItemControl(GetID, facadeView.GetID, iItem);
            }
            UpdateButtonStates();
        }
        void UpdateButtonStates()
        {
            facadeView.IsVisible = false;
            facadeView.IsVisible = true;
            GUIControl.FocusControl(GetID, facadeView.GetID);

            string strLine = string.Empty;
            View view = currentView;
            switch (view)
            {
                case View.List:
                    strLine = GUILocalizeStrings.Get(101);
                    break;
                case View.Icons:
                    strLine = GUILocalizeStrings.Get(100);
                    break;
                case View.LargeIcons:
                    strLine = GUILocalizeStrings.Get(417);
                    break;
            }
            if (btnViewAs != null)
            {
                btnViewAs.Label = strLine;
            }
        }
        void SwitchView()
        {
            switch (currentView)
#if MP11
            {
                case View.List:
                    facadeView.View = GUIFacadeControl.ViewMode.List;
                    break;
                case View.Icons:
                    facadeView.View = GUIFacadeControl.ViewMode.SmallIcons;
                    break;
                case View.LargeIcons:
                    facadeView.View = GUIFacadeControl.ViewMode.LargeIcons;
                    break;
            }
#else
            {
                case View.List:
                    facadeView.CurrentLayout = GUIFacadeControl.Layout.List;
                    break;
                case View.Icons:
                    facadeView.CurrentLayout = GUIFacadeControl.Layout.SmallIcons;
                    break;
                case View.LargeIcons:
                    facadeView.CurrentLayout = GUIFacadeControl.Layout.LargeIcons;
                    break;
            }
#endif

            UpdateButtonStates(); // Ensure "View: xxxx" button label is updated to suit
        }

    }
}