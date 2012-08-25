using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyFilmsPlugin.DataBase
{
  class MultiUserData
  {
    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    // public AntMovieCatalog.MovieRow AntMovie { get; set; }
    private string Value { get; set; }

    public MultiUserData(string value)
    {
      Value =  value;
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
}
