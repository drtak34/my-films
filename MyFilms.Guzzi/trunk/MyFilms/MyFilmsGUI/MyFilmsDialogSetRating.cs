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

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  using MediaPortal.GUI.Library;

  using GUILocalizeStrings = MyFilmsPlugin.MyFilms.Utils.GUILocalizeStrings;

  /// <summary>
  /// 
  /// </summary>
  public class MyFilmsDialogSetRating : MediaPortal.Dialogs.GUIDialogWindow
  {
    public enum ResultCode
    {
      Close,
      Next,
      Previous
    };

    [SkinControl(2)]
    //protected GUILabelControl lblHeading = null;
    protected GUIFadeLabel lblHeading = null;
    [SkinControlAttribute(4)]
    protected GUILabelControl lblName = null;
    [SkinControlAttribute(5)]
    protected GUILabelControl lblRating = null;
    [SkinControlAttribute(10)]
    protected GUIButtonControl btnPlus = null;
    [SkinControlAttribute(11)]
    protected GUIButtonControl btnMin = null;
    [SkinControlAttribute(12)]
    protected GUIButtonControl btnOk = null;
    [SkinControlAttribute(100)]
    protected GUIImageList imgStar = null;

    decimal rating = 1;
    string fileName;
    ResultCode resultCode;
    public const int ID_MyFilmsDetail = 7988;

      public MyFilmsDialogSetRating()
    {
      GetID = 7988;
    }

    public override int GetID
    {
      get { return ID_MyFilmsDetail; }
    }

    //public override int GetID
    //{
    //  get { return ID_MyFilmsDetail; }
    //  set { base.GetID = value; }
    //}
    
    public override bool Init()
    {
      return Load(GUIGraphicsContext.Skin + @"\MyFilmsDialogRating.xml");
    }

    protected override void OnClicked(int controlId, GUIControl control, MediaPortal.GUI.Library.Action.ActionType actionType)
    {
      base.OnClicked(controlId, control, actionType);
      if (control == btnOk)
      {
        PageDestroy();
        resultCode = ResultCode.Close;
        return;
      }
      if (control == btnMin)
      {
        if (rating >= (decimal)0.1) 
            rating = rating - (decimal)0.1;
        UpdateRating();
        return;
      }
      if (control == btnPlus)
      {
        if (rating < 10)
            rating = rating + (decimal)0.1;
        UpdateRating();
        return;
      }
    }

    public override bool OnMessage(GUIMessage message)
    {
      switch (message.Message)
      {
        case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
          {
            resultCode = ResultCode.Close;
            base.OnMessage(message);
            UpdateRating();
          }
          return true;
      }

      return base.OnMessage(message);
    }
    public void SetHeading(string strLine)
    {
      LoadSkin();
      AllocResources();
      InitControls();
        
      lblHeading.Label = strLine;
    }

    public void SetHeading(int iString)
    {
      if (iString == 0) SetHeading(string.Empty);
      else SetHeading(GUILocalizeStrings.Get(iString));
    }

    public void SetTitle(string title)
    {
      LoadSkin();
      AllocResources();
      InitControls();
      //lblName.Label = title;
    }

    void UpdateRating()
    {
        MyFilmsDetail.setGUIProperty("rating", Rating.ToString());
    }

    public decimal Rating
    {
      get { return rating; }
      set { rating = value; }
    }
    public string FileName
    {
      get { return fileName; }
      set { fileName = value; }
    }

    public ResultCode Result
    {
      get { return resultCode; }
    }

  }
}
