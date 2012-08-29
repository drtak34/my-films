using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyFilmsPlugin.DataBase
{
  using System.Globalization;

  using MyFilmsPlugin.MyFilms.MyFilmsGUI;

  public class MultiUserData
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();

    public const decimal NoRating = -1;
    public static readonly DateTime NoWatchedDate = DateTime.MinValue;

    public string MultiUserStatusValue { get; set; }
    public List<UserState> MultiUserStatus { get; set; }
    public MultiUserData(string value)
    {
      this.MultiUserStatusValue = value;
      // if (!string.IsNullOrEmpty(value)) LogMyFilms.Debug("MultiUserData() - loaded with value = '" + value + "'");
      LoadUserStates();
    }

    public UserState GetUserState(string username)
    {
      UserState userstate;
      if (MultiUserStatus.Count(userState => userState.UserName == username) == 0)
      {
        userstate = new UserState(username);
        MultiUserStatus.Add(userstate);
      }
      userstate = MultiUserStatus.First(userState => userState.UserName == username);
      return userstate;
    }

    public UserState GetGlobalState()
    {
      // return this.MultiUserStatus.FirstOrDefault(userState => userState.UserName == MyFilms.GlobalUsername);
      var global = new UserState(MyFilms.GlobalUsername)
        { WatchedCount = 0, UserRating = NoRating, WatchedDate = NoWatchedDate };

      foreach (UserState userState in MultiUserStatus.FindAll(x => x.UserName != MyFilms.GlobalUsername))
      {
        global.WatchedCount += userState.WatchedCount;
        if (userState.WatchedDate > global.WatchedDate) global.WatchedDate = userState.WatchedDate;
        if (userState.UserRating > global.UserRating) global.UserRating = userState.UserRating;
      }
      global.Watched = global.WatchedCount > 0;
      return global;
    }

    public void SetWatched(string username, bool watched)
    {
      var userstate = GetUserState(username);
      if (watched)
      {
        userstate.WatchedCount = 1;
        userstate.Watched = true;
        userstate.WatchedDate = DateTime.Now;
      }
      else
      {
        userstate.WatchedCount = 0;
        userstate.Watched = false;
        userstate.WatchedDate = NoWatchedDate;
      }
    }

    public void SetWatchedCount(string username, int watchedcount)
    {
      var userstate = GetUserState(username);
      userstate.WatchedCount = watchedcount;
      userstate.Watched = (watchedcount > 0);
      userstate.WatchedDate = (watchedcount > 0) ? DateTime.Today : NoWatchedDate;
    }

    public void SetRating(string username, decimal rating)
    {
      var userstate = GetUserState(username);
      userstate.UserRating = (rating < 0) ? NoRating : rating;
    }

    public void AddWatchedCountByOne(string username)
    {
      var userstate = GetUserState(username);
      userstate.WatchedCount += 1;
      userstate.WatchedDate = DateTime.Now;
      userstate.Watched = true;
    }

    public string ResultValueString()
    {
      string resultValueString = string.Empty;
      foreach (var state in MultiUserStatus)
      {
        // LogMyFilms.Debug("LoadUserStates() - return state for user '" + state.UserName + "', rating = '" + state.UserRating + "', count = '" + state.WatchedCount + "', watched = '" + state.Watched + "', watcheddate = '" + state.WatchedDate + "'");
        var sNew = state.UserName + ":" + state.WatchedCount + ":" + state.UserRating.ToString(CultureInfo.InvariantCulture) + ":" + ((state.WatchedDate > DateTime.Parse("01/01/1900")) ? state.WatchedDate.ToShortDateString() : "");  // short date as invariant culture // var sNew = state.UserName + ":" + state.WatchedCount + ":" + state.UserRating.ToString(CultureInfo.InvariantCulture) + ":" + state.WatchedDate.ToString("d", invC);  // short date as invariant culture
        if (resultValueString.Length > 0) resultValueString += "|";
        resultValueString += sNew;
      }
      // if (MultiUserStatusValue != resultValueString) LogMyFilms.Debug("ResultValueString() - modified MUS value = '" + resultValueString + "'");
      return resultValueString;
    }

    private enum Type
    {
      Username,
      Count,
      Rating,
      Datewatched
    }
    
    private void LoadUserStates()
    {
      if (MultiUserStatus == null) MultiUserStatus = new List<UserState>();
      MultiUserStatus.Clear();
      string[] split = MultiUserStatusValue.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string s in split)
      {
        if (s.Contains(":"))
        {
          var userstate = new UserState((string)EnhancedWatchedValue(s, Type.Username));
          userstate.UserRating = (decimal)EnhancedWatchedValue(s, Type.Rating);
          userstate.WatchedCount = (int)EnhancedWatchedValue(s, Type.Count);
          userstate.Watched = (userstate.WatchedCount > 0);
          userstate.WatchedDate = (DateTime)(EnhancedWatchedValue(s, Type.Datewatched));
          // LogMyFilms.Debug("LoadUserStates() - loading state for user '" + userstate.UserName + "', rating = '" + userstate.UserRating + "', count = '" + userstate.WatchedCount + "', watched = '" + userstate.Watched + "', watcheddate = '" + userstate.WatchedDate + "'");
          MultiUserStatus.Add(userstate);
        }
      }
      if (MultiUserStatus.Count == 0) MultiUserStatus.Add(new UserState(MyFilms.GlobalUsername));
    }

    private static object EnhancedWatchedValue(string s, Type type)
    {
      // "Global:0:-1|MikePlanet:0:-1" or "Global:0:-1|MikePlanet:0:-1:2011-12-24"
      object value = "";
      string[] split = s.Split(new Char[] { ':' });
      switch (type)
      {
        case Type.Username:
          value = (split.Length > 0) ? split[0] : "";
          break;
        case Type.Count:
          int count = 0;
          value = (split.Length > 1 && int.TryParse(split[1], out count)) ? count : 0;
          break;
        case Type.Rating:
          decimal rating = NoRating;
          value = (split.Length > 2 && decimal.TryParse(split[2], NumberStyles.Any, CultureInfo.InvariantCulture, out rating)) ? rating : NoRating;
          break;
        case Type.Datewatched:
          DateTime datewatched;
          value = (split.Length > 3 && DateTime.TryParse(split[3], out datewatched)) ? datewatched : NoWatchedDate;
          break;
      }
      return value;
    }

    public string Remove(string input, string toRemove)
    {
      string output = "";
      string[] split = input.Split(new Char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
      List<string> itemList = split.Distinct().ToList();
      if (itemList.Contains(toRemove)) itemList.Remove(toRemove);
      foreach (string s in itemList)
      {
        if (output.Length > 0) output += ", ";
        output += s;
      }
      return output;
    }

    public string Add(string input, string toAdd)
    {
      string output = "";
      string[] split = input.Split(new Char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
      List<string> itemList = split.Distinct().ToList();
      if (!itemList.Contains(toAdd)) itemList.Add(toAdd);
      foreach (string s in itemList)
      {
        if (output.Length > 0) output += ", ";
        output += s;
      }
      return output;
    }
  }

  public class UserState
  {
    public UserState(string username)
    {
      this.UserName = username;
      this.UserRating = MultiUserData.NoRating;
      this.Watched = false;
      this.WatchedCount = 0;
      this.WatchedDate = MultiUserData.NoWatchedDate;
    }

    public string UserName { get; private set; }
    public bool Watched { get; set; }
    public int WatchedCount { get; set; }
    public DateTime WatchedDate { get; set; }
    public decimal UserRating { get; set; }
  }
}
