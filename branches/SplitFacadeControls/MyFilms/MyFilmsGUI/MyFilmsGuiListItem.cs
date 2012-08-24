using System;
using MediaPortal.GUI.Library;

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
    /// <summary>
    /// Extends a <see cref="GUIListItem"/> with properties and methods only used in OnlineVideos.
    /// </summary>
    public class MyFilmsGuiListItem : GUIListItem
    {
        #region Constructors
        public MyFilmsGuiListItem(string strLabel) : base(strLabel) { }

        //public MyFilmsGuiListItem(SitesGroup item) : base(item.Name)
        //{
        //    Label2 = item.Sites.Count.ToString();
        //    IsFolder = true;
        //    Item = item;
        //    if (!string.IsNullOrEmpty(item.Thumbnail))
        //    {
        //        ThumbnailImage = item.Thumbnail;
        //        IconImage = item.Thumbnail;
        //        IconImageBig = item.Thumbnail;
        //    }
        //    else
        //    {
        //        MediaPortal.Util.Utils.SetDefaultIcons(this);
        //    }
        //}

        //public MyFilmsGuiListItem(Sites.SiteUtilBase item) : base(item.Settings.Name)
        //{
        //    Label2 = item.Settings.Language;
        //    IsFolder = true;
        //    Item = item;
        //    // use Icon with the same name as the Site
        //    string image = GUIOnlineVideos.GetImageForSite(item.Settings.Name, item.Settings.UtilName, "Icon");
        //    if (!string.IsNullOrEmpty(image))
        //    {
        //        ThumbnailImage = image;
        //        IconImage = image;
        //        IconImageBig = image;
        //    }
        //    else
        //    {
        //        MediaPortal.Util.Utils.SetDefaultIcons(this);
        //    }
        //}

        //public MyFilmsGuiListItem(Category item) : base(item.Name)
        //{
        //    Label2 = item is RssLink && (item as RssLink).EstimatedVideoCount > 0 ? (item as RssLink).EstimatedVideoCount.ToString() : 
        //        item is Group && (item as Group).Channels.Count > 0 ? (item as Group).Channels.Count.ToString() : Label2;
        //    IsFolder = true;
        //    Item = item;
        //    MediaPortal.Util.Utils.SetDefaultIcons(this);
        //}

        //public MyFilmsGuiListItem(VideoInfo item, bool useTitle2 = false)
        //{
        //    Label = useTitle2 ? item.Title2 : item.Title;
        //    Label2 = !string.IsNullOrEmpty(item.Length) ? VideoInfo.GetDuration(item.Length) : item.Airdate;
        //    Item = item;
        //    IconImage = "defaultVideo.png";
        //    IconImageBig = "defaultVideoBig.png";
        //}
        #endregion

		    protected PropertyChangedDelegator eventDelegator = null;
        protected object _Item;
        /// <summary>
        /// The <see cref="SiteUtilBase"/>, <see cref="Category"/>, <see cref="VideoInfo"/> or <see cref="SitesGroup"/> that belongs to this object.
        /// </summary>
        public object Item 
        {
            get { return _Item; }
            internal set
            {
                _Item = value;
				System.ComponentModel.INotifyPropertyChanged notifier = value as System.ComponentModel.INotifyPropertyChanged;
				if (notifier != null)
				{
					eventDelegator = OnlineVideosAppDomain.Domain.CreateInstanceAndUnwrap(typeof(PropertyChangedDelegator).Assembly.FullName, typeof(PropertyChangedDelegator).FullName) as PropertyChangedDelegator;
					eventDelegator.InvokeTarget = new PropertyChangedExecutor() { InvokeHandler = (s, e) =>
					{
						if (s is VideoInfo && e.PropertyName == "ThumbnailImage") SetImageToGui((s as VideoInfo).ThumbnailImage);
						else if (s is Category && e.PropertyName == "ThumbnailImage") SetImageToGui((s as Category).ThumbnailImage);
						else if (s is VideoInfo && e.PropertyName == "Length") Label2 = (s as VideoInfo).Length;
					} };
					notifier.PropertyChanged += eventDelegator.EventDelegate;
				}
            }
        }
		
        public string Description
        {
            get
            {
                string desc = null;
                if (Item != null)
                {
                            Category cat = Item as Category;
                            if (cat != null) desc = cat.Description;
                            else
                            {
                                VideoInfo vid = Item as VideoInfo;
                                if (vid != null) desc = vid.Description;
                            }
                }
                return desc ?? string.Empty;
            }
        }

        protected void SetImageToGui(string imageFilePath)
        {
            ThumbnailImage = imageFilePath;
            IconImage = imageFilePath;
            IconImageBig = imageFilePath;

            // if selected and OnlineVideos is current window force an update of #selectedthumb
            GUIOnlineVideos ovsWindow = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow) as GUIOnlineVideos;
            if (ovsWindow != null)
            {
                int listControlId = ovsWindow.CurrentState == GUIOnlineVideos.State.details ? 51 : 50;
                GUIListItem selectedItem = GUIControl.GetSelectedListItem(GUIOnlineVideos.WindowId, listControlId);
                if (selectedItem == this)
                {
                    GUIWindowManager.SendThreadMessage(new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GUIWindowManager.ActiveWindow, 0, listControlId, ItemId, 0, null));
                }
            }
        }
    }

    public class PropertyChangedExecutor : MarshalByRefObject
    {
      public System.ComponentModel.PropertyChangedEventHandler InvokeHandler { get; set; }

      public void Execute(object s, string propertyName)
      {
        InvokeHandler.Invoke(s, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
      }

      #region MarshalByRefObject overrides
      public override object InitializeLifetimeService()
      {
        // In order to have the lease across appdomains live forever, we return null.
        return null;
      }
      #endregion
    }

    public class PropertyChangedDelegator : MarshalByRefObject
    {
      public PropertyChangedExecutor InvokeTarget { get; set; }

      public void EventDelegate(object s, System.ComponentModel.PropertyChangedEventArgs e)
      {
        InvokeTarget.Execute(s, e.PropertyName);
      }

      #region MarshalByRefObject overrides
      public override object InitializeLifetimeService()
      {
        // In order to have the lease across appdomains live forever, we return null.
        return null;
      }
      #endregion
    }
}
