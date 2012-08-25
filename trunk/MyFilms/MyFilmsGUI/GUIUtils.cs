using System;
using System.Reflection;
using System.Collections.Generic;
using MediaPortal.Configuration;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using System.Threading;

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  using System.Collections;
  using System.Reflection;

  using MediaPortal.Video.Database;

  using MyFilmsPlugin.MyFilms.Utils;

  public static class GUIUtils
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger(); //log

    private delegate bool ShowCustomYesNoDialogDelegate(
      string heading, string lines, string yesLabel, string noLabel, bool defaultYes);

    private delegate void ShowOKDialogDelegate(string heading, string lines);

    private delegate void ShowNotifyDialogDelegate(
      string heading, string text, string image, string buttonText, int timeOut);

    private delegate int ShowMenuDialogDelegate(string heading, List<GUIListItem> items);

    private delegate List<MultiSelectionItem> ShowMultiSelectionDialogDelegate(
      string heading, List<MultiSelectionItem> items);

    private delegate void ShowTextDialogDelegate(string heading, string text);

    private delegate string ShowRateDialogDelegate<T>(T rateObject);

    private delegate bool GetStringFromKeyboardDelegate(ref string strLine, bool isPassword);

    public static readonly string MyFilmsLogo = GUIGraphicsContext.Skin + "\\Media\\MyFilms\\myfilms.png";

    public static string PluginName()
    {
      return "MyFilms";
    }

    #region wrapper for MP1.2-MP1.3 compatibility in VideoDatabase

    public static bool GetActorDetails(IMDB imdb, object imdbActorUrl, object director, out IMDBActor imdbActor)
    {
      // MP1.3 _imdb.GetActorDetails(_imdb[0], out imdbActor);
      // MP1.2 _imdb.GetActorDetails(_imdb[0], false, out person);
      imdbActor = new IMDBActor();
      MethodInfo mi = imdb.GetType().GetMethod("GetActorDetails");
      var iParams = mi.GetParameters();
      switch (iParams.Length)
      {
        case 2:
          {
            //var paramTypes = new Type[] { typeof(IMDB.IMDBUrl), typeof(IMDBActor).MakeByRefType() }; // var paramTypes = new Type[] { typeof(IMDB.IMDBUrl) Type.GetType("MediaPortal.Video.Database.IMDB.IMDBUrl"), Type.GetType("MediaPortal.Video.Database.IMDBActor&") };
            //MethodInfo mi = imdb.GetType().GetMethod("GetActorDetails", paramTypes);
            object[] arrParms = new object[] { imdbActorUrl, imdbActor };
            bool returnValue = (bool)mi.Invoke(imdb, arrParms); // return (bool)mi.Invoke(imdb, new[] { imdbActorUrl, imdbActor });
            imdbActor = (IMDBActor)arrParms[1];
            return returnValue;
          }
        case 3:
          {
            //var paramTypes = new Type[] { typeof(IMDB.IMDBUrl), typeof(bool), typeof(IMDBActor).MakeByRefType() }; //var paramTypes = new Type[] { Type.GetType("MediaPortal.Video.Database.IMDB.IMDBUrl"), typeof(bool), Type.GetType("MediaPortal.Video.Database.IMDBActor&") };
            //MethodInfo mi = imdb.GetType().GetMethod("GetActorDetails", paramTypes);
            object[] arrParms = new object[] { imdbActorUrl, director, imdbActor };
            bool returnValue = (bool)mi.Invoke(imdb, arrParms);
            imdbActor = (IMDBActor)arrParms[2];
            return returnValue;
          }
      }
      throw new Exception("Error invoking VideoDatabase method 'GetActorDetails'");
    }

    public static void GetFilesForMovie(int id, ref ArrayList movies)
    {
      // MP1.2 VideoDatabase.GetFiles(iidMovie, ref movies);
      // MP1.3 VideoDatabase.GetFilesForMovie(iidMovie, ref movies);

      //System.Type[] arrTypes = new System.Type[2];
      //arrTypes.SetValue(Type.GetType("System.Int32"), 0);
      //arrTypes.SetValue(Type.GetType("System.ArrayList&"), 1);
      //MethodInfo miCusNm = imdb.GetType().GetMethod("GetFiles", arrTypes);

      // package parameters
      object[] arrParms = new object[2];
      arrParms.SetValue(id, 0);
      arrParms.SetValue(movies, 1);
      // bool success = (bool)miCusNm.Invoke(oClsX, arrParms);

      Type[] paramTypes = new Type[] { typeof(int), typeof(ArrayList).MakeByRefType() }; // Type[] paramTypes = new Type[] { typeof(int), Type.GetType("System.ArrayList&") };
      
      // Get methods.
      MethodInfo[] methods = typeof(VideoDatabase).GetMethods();
      bool success = false;
      foreach (MethodInfo info in methods)
      {
        switch (info.Name)
        {
          case "GetFiles":
            {
              MethodInfo mi = typeof(VideoDatabase).GetMethod("GetFiles", paramTypes);
              mi.Invoke(null, arrParms);
              success = true;
            }
            break;
          case "GetFilesForMovie":
            {
              MethodInfo mi = typeof(VideoDatabase).GetMethod("GetFilesForMovie", paramTypes);
              mi.Invoke(null, arrParms);
              success = true;
            }
            break;
        }
      }
      if (!success)
      {
        throw new Exception("Error invoking GetFiles() form Videodatabase !");
      }
    }

    public static int AddActor(object role, object name)
    {
      // int iiActor = VideoDatabase.AddActor(string.Empty, "");
      var mi = typeof(VideoDatabase).GetMethod("AddActor");
      var iParams = mi.GetParameters();
      switch (iParams.Length)
      {
        case 1:
          return (int)mi.Invoke(null, new[] { name });
        case 2:
          return (int)mi.Invoke(null, new[] { role, name });
        default:
          throw new Exception("Error invoking VideoDatabase method 'AddActor'");
      }
    }

    public static void AddActorToMovie(object iMovieId, object iActorId, object role)
    {
      // VideoDatabase.AddActorToMovie(iidmovie, iiActor, "actor");
      // public static void AddActorToMovie(int lMovieId, int lActorId, string role)
      var mi = typeof(VideoDatabase).GetMethod("AddActorToMovie");
      var iParams = mi.GetParameters();
      switch (iParams.Length)
      {
        case 2:
          mi.Invoke(null, new[] { iMovieId, iActorId });
          break;
        case 3:
          mi.Invoke(null, new[] { iMovieId, iActorId, role });
          break;
        default:
          throw new Exception("Error invoking VideoDatabase method 'AddActorToMovie'");
      }
    }

    #endregion

    #region GUI property setter
    public static void SetProperty(string property, string value)
    {
      SetProperty(property, value, false);
    }

    public static void SetProperty(string property, string value, bool log)
    {
      // prevent ugly display of property names
      if (string.IsNullOrEmpty(value)) value = " ";

      GUIPropertyManager.SetProperty(property, value);

      if (log)
      {
        if (GUIPropertyManager.Changed) LogMyFilms.Debug("Set property \"" + property + "\" to \"" + value + "\" successful");
        else LogMyFilms.Warn("Set property \"" + property + "\" to \"" + value + "\" failed");
      }
    }
    #endregion

    #region dialogs
    /// <summary>
    /// Displays a yes/no dialog.
    /// </summary>
    /// <returns>True if yes was clicked, False if no was clicked</returns>
    public static bool ShowYesNoDialog(string heading, string lines)
    {
      return ShowCustomYesNoDialog(heading, lines, null, null, false);
    }

    /// <summary>
    /// Displays a yes/no dialog.
    /// </summary>
    /// <returns>True if yes was clicked, False if no was clicked</returns>
    public static bool ShowYesNoDialog(string heading, string lines, bool defaultYes)
    {
      return ShowCustomYesNoDialog(heading, lines, null, null, defaultYes);
    }

    /// <summary>
    /// Displays a yes/no dialog with custom labels for the buttons.
    /// This method may become obsolete in the future if media portal adds more dialogs.
    /// </summary>
    /// <returns>True if yes was clicked, False if no was clicked</returns>
    public static bool ShowCustomYesNoDialog(string heading, string lines, string yesLabel, string noLabel)
    {
      return ShowCustomYesNoDialog(heading, lines, yesLabel, noLabel, false);
    }

    /// <summary>
    /// Displays a yes/no dialog with custom labels for the buttons.
    /// This method may become obsolete in the future if media portal adds more dialogs.
    /// </summary>
    /// <returns>True if yes was clicked, False if no was clicked</returns>
    public static bool ShowCustomYesNoDialog(
      string heading, string lines, string yesLabel, string noLabel, bool defaultYes)
    {
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowCustomYesNoDialogDelegate d = ShowCustomYesNoDialog;
        return (bool)GUIGraphicsContext.form.Invoke(d, heading, lines, yesLabel, noLabel, defaultYes);
      }

      GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);

      try
      {
        dlgYesNo.Reset();
        dlgYesNo.SetHeading(heading);
        string[] linesArray = lines.Split(new string[] { "\\n", "\n" }, StringSplitOptions.None);
        if (linesArray.Length > 0) dlgYesNo.SetLine(1, linesArray[0]);
        if (linesArray.Length > 1) dlgYesNo.SetLine(2, linesArray[1]);
        if (linesArray.Length > 2) dlgYesNo.SetLine(3, linesArray[2]);
        if (linesArray.Length > 3) dlgYesNo.SetLine(4, linesArray[3]);
        dlgYesNo.SetDefaultToYes(defaultYes);

        foreach (GUIControl item in dlgYesNo.GetControlList())
        {
          if (item is GUIButtonControl)
          {
            GUIButtonControl btn = (GUIButtonControl)item;
            if (btn.GetID == 11 && !string.IsNullOrEmpty(yesLabel)) // Yes button
              btn.Label = yesLabel;
            else if (btn.GetID == 10 && !string.IsNullOrEmpty(noLabel)) // No button
              btn.Label = noLabel;
          }
        }
        dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
        return dlgYesNo.IsConfirmed;
      }
      finally
      {
        // set the standard yes/no dialog back to it's original state (yes/no buttons)
        if (dlgYesNo != null)
        {
          dlgYesNo.ClearAll();
        }
      }
    }

    #endregion

    /// <summary>
    /// Same as Children and controlList but used for backwards compatibility between mediaportal 1.1 and 1.2
    /// </summary>
    /// <param name="self"></param>
    /// <returns>IEnumerable of GUIControls</returns>
    public static IEnumerable GetControlList(this GUIWindow self)
    {
      PropertyInfo property = GetPropertyInfo<GUIWindow>("Children", null);
      return (IEnumerable)property.GetValue(self, null);
    }


    private static Dictionary<string, PropertyInfo> propertyCache = new Dictionary<string, PropertyInfo>();

    /// <summary>
    /// Gets the property info object for a property using reflection.
    /// The property info object will be cached in memory for later requests.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="newName">The name of the property in 1.2</param>
    /// <param name="oldName">The name of the property in 1.1</param>
    /// <returns>instance PropertyInfo or null if not found</returns>
    public static PropertyInfo GetPropertyInfo<T>(string newName, string oldName)
    {
      PropertyInfo property = null;
      Type type = typeof(T);
      string key = type.FullName + "|" + newName;

      if (!propertyCache.TryGetValue(key, out property))
      {
        property = type.GetProperty(newName) ?? type.GetProperty(oldName);

        propertyCache[key] = property;
      }

      return property;
    }

    /// <summary>
    /// Displays a OK dialog with heading and up to 4 lines.
    /// </summary>
    public static void ShowOKDialog(string heading, string line1, string line2, string line3, string line4)
    {
      ShowOKDialog(heading, string.Concat(line1, line2, line3, line4));
    }

    /// <summary>
    /// Displays a OK dialog with heading and up to 4 lines split by \n in lines string.
    /// </summary>
    public static void ShowOKDialog(string heading, string lines)
    {
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowOKDialogDelegate d = ShowOKDialog;
        GUIGraphicsContext.form.Invoke(d, heading, lines);
        return;
      }

      GUIDialogOK dlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);

      dlgOK.Reset();
      dlgOK.SetHeading(heading);

      int lineid = 1;
      foreach (string line in lines.Split(new string[] { "\\n", "\n" }, StringSplitOptions.None))
      {
        dlgOK.SetLine(lineid, line);
        lineid++;
      }
      for (int i = lineid; i <= 4; i++) dlgOK.SetLine(i, string.Empty);

      dlgOK.DoModal(GUIWindowManager.ActiveWindow);
    }

    /// <summary>
    /// Displays a notification dialog.
    /// </summary>
    public static void ShowNotifyDialog(string heading, string text)
    {
      ShowNotifyDialog(heading, text, MyFilmsLogo, Translation.OK, -1);
    }

    /// <summary>
    /// Displays a notification dialog.
    /// </summary>
    public static void ShowNotifyDialog(string heading, string text, int timeOut)
    {
      ShowNotifyDialog(heading, text, MyFilmsLogo, Translation.OK, timeOut);
    }

    /// <summary>
    /// Displays a notification dialog.
    /// </summary>
    public static void ShowNotifyDialog(string heading, string text, string image)
    {
      ShowNotifyDialog(heading, text, image, Translation.OK, -1);
    }

    /// <summary>
    /// Displays a notification dialog.
    /// </summary>
    public static void ShowNotifyDialog(string heading, string text, string image, string buttonText, int timeout)
    {
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowNotifyDialogDelegate d = ShowNotifyDialog;
        GUIGraphicsContext.form.Invoke(d, heading, text, image, buttonText, timeout);
        return;
      }

      GUIDialogNotify pDlgNotify =
        (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
      if (pDlgNotify == null) return;

      try
      {
        pDlgNotify.Reset();
        pDlgNotify.SetHeading(heading);
        pDlgNotify.SetImage(image);
        pDlgNotify.SetText(text);
        if (timeout >= 0) pDlgNotify.TimeOut = timeout;

        foreach (GUIControl item in pDlgNotify.GetControlList())
        {
          if (item is GUIButtonControl)
          {
            GUIButtonControl btn = (GUIButtonControl)item;
            if (btn.GetID == 4 && !string.IsNullOrEmpty(buttonText) && !string.IsNullOrEmpty(btn.Label))
            {
              // Only if ID is 4 and we have our custom text and if button already has label (in case the skin "hides" the button by emtying the label)
              btn.Label = buttonText;
            }
          }
        }

        pDlgNotify.DoModal(GUIWindowManager.ActiveWindow);
      }
      finally
      {
        if (pDlgNotify != null) pDlgNotify.ClearAll();
      }
    }

    /// <summary>
    /// Displays a menu dialog from list of items
    /// </summary>
    /// <returns>Selected item index, -1 if exited</returns>
    public static int ShowMenuDialog(string heading, List<GUIListItem> items)
    {
      return ShowMenuDialog(heading, items, -1);
    }

    /// <summary>
    /// Displays a menu dialog from list of items
    /// </summary>
    /// <returns>Selected item index, -1 if exited</returns>
    public static int ShowMenuDialog(string heading, List<GUIListItem> items, int selectedItemIndex)
    {
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowMenuDialogDelegate d = ShowMenuDialog;
        return (int)GUIGraphicsContext.form.Invoke(d, heading, items);
      }

      GUIDialogMenu dlgMenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);

      dlgMenu.Reset();
      dlgMenu.SetHeading(heading);

      foreach (GUIListItem item in items)
      {
        dlgMenu.Add(item);
      }

      if (selectedItemIndex >= 0) dlgMenu.SelectedLabel = selectedItemIndex;

      dlgMenu.DoModal(GUIWindowManager.ActiveWindow);

      if (dlgMenu.SelectedLabel < 0)
      {
        return -1;
      }

      return dlgMenu.SelectedLabel;
    }

    /// <summary>
    /// Displays a menu dialog from list of items
    /// </summary>
    public static List<MultiSelectionItem> ShowMultiSelectionDialog(string heading, List<MultiSelectionItem> items)
    {
      List<MultiSelectionItem> result = new List<MultiSelectionItem>();
      if (items == null) return result;

      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowMultiSelectionDialogDelegate d = ShowMultiSelectionDialog;
        return (List<MultiSelectionItem>)GUIGraphicsContext.form.Invoke(d, heading, items);
      }

      GUIWindow dlgMultiSelectOld = (GUIWindow)GUIWindowManager.GetWindow(2100);
      GUIDialogMultiSelect dlgMultiSelect = new GUIDialogMultiSelect();
      dlgMultiSelect.Init();
      GUIWindowManager.Replace(2100, dlgMultiSelect);

      try
      {
        dlgMultiSelect.Reset();
        dlgMultiSelect.SetHeading(heading);

        foreach (MultiSelectionItem multiSelectionItem in items)
        {
          GUIListItem item = new GUIListItem();
          item.Label = multiSelectionItem.ItemTitle;
          item.Label2 = multiSelectionItem.ItemTitle2;
          item.MusicTag = multiSelectionItem.Tag;
          item.Selected = multiSelectionItem.Selected;

          dlgMultiSelect.Add(item);
        }

        dlgMultiSelect.DoModal(GUIWindowManager.ActiveWindow);

        if (dlgMultiSelect.DialogModalResult == ModalResult.OK)
        {
          for (int i = 0; i < items.Count; i++)
          {
            MultiSelectionItem item = items[i];
            MultiSelectionItem newMultiSelectionItem = new MultiSelectionItem();
            newMultiSelectionItem.ItemTitle = item.ItemTitle;
            newMultiSelectionItem.ItemTitle2 = item.ItemTitle2;
            newMultiSelectionItem.ItemID = item.ItemID;
            newMultiSelectionItem.Tag = item.Tag;
            try
            {
              newMultiSelectionItem.Selected = dlgMultiSelect.ListItems[i].Selected;
            }
            catch
            {
              newMultiSelectionItem.Selected = item.Selected;
            }

            result.Add(newMultiSelectionItem);
          }
        }
        else return null;

        return result;
      }
      finally
      {
        GUIWindowManager.Replace(2100, dlgMultiSelectOld);
      }
    }

    /// <summary>
    /// Displays a text dialog.
    /// </summary>
    public static void ShowTextDialog(string heading, List<string> text)
    {
      if (text == null || text.Count == 0) return;
      ShowTextDialog(heading, string.Join("\n", text.ToArray()));
    }

    /// <summary>
    /// Displays a text dialog.
    /// </summary>
    public static void ShowTextDialog(string heading, string text)
    {
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        ShowTextDialogDelegate d = ShowTextDialog;
        GUIGraphicsContext.form.Invoke(d, heading, text);
        return;
      }

      GUIDialogText dlgText = (GUIDialogText)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_TEXT);

      dlgText.Reset();
      dlgText.SetHeading(heading);
      dlgText.SetText(text);

      dlgText.DoModal(GUIWindowManager.ActiveWindow);
    }

    public static bool GetStringFromKeyboard(ref string strLine)
    {
      return GetStringFromKeyboard(ref strLine, false);
    }

    /// <summary>
    /// Gets the input from the virtual keyboard window.
    /// </summary>
    public static bool GetStringFromKeyboard(ref string strLine, bool isPassword)
    {
      if (GUIGraphicsContext.form.InvokeRequired)
      {
        GetStringFromKeyboardDelegate d = GetStringFromKeyboard;
        object[] args = { strLine, isPassword };
        bool result = (bool)GUIGraphicsContext.form.Invoke(d, args);
        strLine = (string)args[0];
        return result;
      }

      VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
      if (keyboard == null) return false;

      keyboard.Reset();
      keyboard.Text = strLine;
      keyboard.Password = isPassword;
      keyboard.DoModal(GUIWindowManager.ActiveWindow);

      if (keyboard.IsConfirmed)
      {
        strLine = keyboard.Text;
        return true;
      }

      return false;
    }
  }

}
