using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;


namespace MyFilmsPlugin.MyFilmsGUI
{
  public class MFView
  {
    public MFView()
    {
      
    }
    
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    public enum ViewContext
    {
      Film,
      Person,
      Group,
      View
    }
    #region public vars
    public int ID
    {
      get { return _mID; }
      set { _mID = value; }
    }
    private int _mID = -1;

    public string Name
    {
      get { return _mName; }
      set { _mName = value; }
    }
    private string _mName = string.Empty;

    public string SaveString
    {
      get
      {
        _mSaveString = _mID + "|" + _mName + "|" + _mContext.ToString();
        return _mSaveString;
      }
      set
      {
        _mSaveString = value;
        string[] split = _mSaveString.Split(new char[]{'|'}, StringSplitOptions.None);
        _mID = int.Parse(split[0]);
        _mName = split[1];
        _mContext = (ViewContext)Enum.Parse(typeof(ViewContext), split[2]);
      }
    }
    private string _mSaveString = string.Empty;

    public ViewContext Context
    {
      get { return _mContext; }
      set { _mContext = value; }
    }
    private ViewContext _mContext = ViewContext.Film;
    #endregion

  }
}
