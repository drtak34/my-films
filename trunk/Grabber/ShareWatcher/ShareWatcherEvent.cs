#region Copyright (C) 2005-2011 Team MediaPortal

// Copyright (C) 2005-2011 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#endregion

namespace ShareWatcherHelper
{
  internal class ShareWatcherEvent
  {
    #region Enums

    public enum EventType
    {
      Create,
      Change,
      Delete,
      Rename,
      DeleteDirectory
    }

    #endregion

    #region Variables

    private EventType m_Type;
    private string m_strFilename;
    private string m_strOldFilename;

    #endregion

    #region Constructors/Destructors

    public ShareWatcherEvent(EventType type, string strFilename)
    {
      this.m_Type = type;
      this.m_strFilename = strFilename;
      this.m_strOldFilename = null;
    }

    public ShareWatcherEvent(EventType type, string strFilename, string strOldFilename)
    {
      this.m_Type = type;
      this.m_strFilename = strFilename;
      this.m_strOldFilename = strOldFilename;
    }

    #endregion

    #region Properties

    public EventType Type
    {
      get { return this.m_Type; }
    }

    public string FileName
    {
      get { return this.m_strFilename; }
    }

    public string OldFileName
    {
      get { return this.m_strOldFilename; }
    }

    #endregion
  }
}