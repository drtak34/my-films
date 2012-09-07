using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace grabber
{
  public class DBMovieInfo
  {
    public string Identifier { get; set; }
    public string Name { get; set; }
    public string TranslatedTitle { get; set; }
    public string AlternativeTitle { get; set; }
    public int Year { get; set; }
    public string ImdbID { get; set; }
    public string DetailsURL { get; set; }
    public string Summary { get; set; }
    public List<string> Posters { get; set; }
    public List<string> PersonIDs { get; set; }
    public List<string> Backdrops { get; set; }
    public float Score { get; set; }
    public string Certification { get; set; }
    public List<string> Languages { get; set; }
    public int Popularity { get; set; }
    public int Runtime { get; set; }
    public List<string> Producers { get; set; }
    public List<string> Directors { get; set; }
    public List<string> Writers { get; set; }
    public List<string> Actors { get; set; }
    public List<string> Country { get; set; }
    public List<string> Genres { get; set; }
    public List<DBPersonInfo> Persons { get; set; }
  }

  public class DBPersonInfo
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string AlternateName { get; set; }
    public string Job { get; set; }
    public string Biography { get; set; }
    public string Birthday { get; set; }
    public string Birthplace { get; set; }
    public string DetailsURL { get; set; }
    public int Popularity { get; set; }
    public List<string> Images { get; set; }
    public int Known_movies { get; set; }
    public List<DBMovieInfo> Movies { get; set; }
  }

  public class TheMoviedb
  {
    //private const string apiSearchMovie = "http://api.themoviedb.org/2.0/Movie.search?api_key=1e66c0cc99696feaf2ea56695e134eae&title=";
    private const string apiSearchMovie = "http://api.themoviedb.org/2.1/Movie.search/en/xml/1e66c0cc99696feaf2ea56695e134eae/";
    private const string apiSearchMovieByIMDB = "http://api.themoviedb.org/2.1/Movie.imdbLookup/en/xml/1e66c0cc99696feaf2ea56695e134eae/"; // add tt-nbr to search for movie ... tt0137523
    private const string apiSearchPerson = "http://api.themoviedb.org/2.1/Person.search/en/xml/1e66c0cc99696feaf2ea56695e134eae/";
    //private const string apiGetMovieInfo = "http://api.themoviedb.org/2.0/Movie.getInfo?api_key=1e66c0cc99696feaf2ea56695e134eae&id=";
    private const string apiGetMovieInfo = "http://api.themoviedb.org/2.1/Movie.getInfo/en/xml/1e66c0cc99696feaf2ea56695e134eae/";
    private const string apiGetPersonInfo = "http://api.themoviedb.org/2.1/Person.getInfo/en/xml/1e66c0cc99696feaf2ea56695e134eae/";


    public List<DBMovieInfo> getMoviesByTitles(string title, string ttitle, int year, string director, string imdbid, bool choose)
    {
      return getMoviesByTitles(title, ttitle, year, director, imdbid, choose, "en"); // set "en" as default
    }

    public List<DBMovieInfo> getMoviesByTitles(string title, string ttitle, int year, string director, string imdbid, bool choose, string language)
    {
      List<DBMovieInfo> results = new List<DBMovieInfo>();
      results = getMoviesByTitle(title, year, director, imdbid, choose, language);
      if (results.Count == 1)
        return results;
      List<DBMovieInfo> results2 = new List<DBMovieInfo>();
      results2 = getMoviesByTitle(ttitle, year, director, imdbid, choose, language);
      return results2.Count == 1 ? results2 : results;
    }

    public List<DBMovieInfo> getMoviesByTitle(string title, int year, string director, string imdbid, bool choose, string language)
    {
      //title = Grabber.GrabUtil.normalizeTitle(title);
      string id = string.Empty;
      string apiSearchLanguage = apiSearchMovie;
      string apiGetMovieInfoLanguage = apiGetMovieInfo;
      string apiGetPersonInfoLanguage = apiGetPersonInfo;
      if (language.Length == 2)
      {
        apiSearchLanguage = apiSearchMovie.Replace("/en/", "/" + language + "/");
        apiGetMovieInfoLanguage = apiGetMovieInfo.Replace("/en/", "/" + language + "/");
        apiGetPersonInfoLanguage = apiGetPersonInfo.Replace("/en/", "/" + language + "/");
      }

      List<DBMovieInfo> results = new List<DBMovieInfo>();
      List<DBMovieInfo> resultsdet = new List<DBMovieInfo>();
      XmlNodeList xml = null;
      if (!string.IsNullOrEmpty(imdbid))
        xml = getXML(apiSearchMovieByIMDB + imdbid);
      if (xml == null) // if imdb search was unsuccessful use normal search...
        xml = getXML(apiSearchLanguage + Grabber.GrabUtil.RemoveDiacritics(title.Trim().ToLower()).Replace(" ", "+"));

      if (xml == null)
        return results;

      XmlNodeList movieNodes = xml.Item(0).SelectNodes("//movie");
      foreach (XmlNode node in movieNodes)
      {
        DBMovieInfo movie = getMovieInformation(node);
        if (movie != null && Grabber.GrabUtil.normalizeTitle(movie.Name.ToLower()).Contains(Grabber.GrabUtil.normalizeTitle(title.ToLower())))
          if (year > 0 && movie.Year > 0 && !choose)
          {
            if ((year >= movie.Year - 2) && (year <= movie.Year + 2))
              results.Add(movie);
          }
          else
            results.Add(movie);
      }
      if (results.Count > 0)
      {
        // Replace non-descriptive characters with spaces
        director = System.Text.RegularExpressions.Regex.Replace(director, "( et | and | & | und )", ",");
        if (director.IndexOf(",") > 0)
          director = director.Substring(0, director.IndexOf(","));
        foreach (DBMovieInfo movie in results)
        {
          if (movie.Identifier != null)
          {
            try { xml = getXML(apiGetMovieInfoLanguage + movie.Identifier); }
            catch { xml = null; }
            if (xml != null)
            {
              movieNodes = xml.Item(0).SelectNodes("//movie");
              foreach (XmlNode node in movieNodes)
              {
                DBMovieInfo movie2 = getMovieInformation(node);
                if (movie2 != null && Grabber.GrabUtil.normalizeTitle(movie2.Name.ToLower()).Contains(Grabber.GrabUtil.normalizeTitle(title.ToLower())) && movie2.Directors.Contains(director))
                  if (year > 0 && movie2.Year > 0 && !choose)
                  {
                    if ((year >= movie2.Year - 2) && (year <= movie2.Year + 2))
                      resultsdet.Add(movie2);
                  }
                  else
                    resultsdet.Add(movie2);
                else
                  if (choose)
                    resultsdet.Add(movie2);
              }
            }
          }
        }
      }
      return resultsdet.Count > 0 ? resultsdet : results;
    }

    public List<DBPersonInfo> getPersonsByName(string name, bool choose, string language)
    {
      //name = Grabber.GrabUtil.normalizeTitle(name);
      string id = string.Empty;
      string apiSearchLanguage = apiSearchPerson;
      string apiGetMovieInfoLanguage = apiGetMovieInfo;
      string apiGetPersonInfoLanguage = apiGetPersonInfo;
      if (language.Length == 2)
      {
        apiSearchLanguage = apiSearchPerson.Replace("/en/", "/" + language + "/");
        apiGetMovieInfoLanguage = apiGetMovieInfo.Replace("/en/", "/" + language + "/");
        apiGetPersonInfoLanguage = apiGetPersonInfo.Replace("/en/", "/" + language + "/");
      }

      List<DBPersonInfo> results = new List<DBPersonInfo>();
      List<DBPersonInfo> resultsdet = new List<DBPersonInfo>();
      XmlNodeList xml = getXML(apiSearchLanguage + Grabber.GrabUtil.RemoveDiacritics(name.Trim().ToLower()).Replace(" ", "+"));
      if (xml == null)
        return results;

      XmlNodeList personNodes = xml.Item(0).SelectNodes("//person");
      foreach (XmlNode node in personNodes)
      {
        DBPersonInfo person = getPersonInformation(node);
        if (person != null && Grabber.GrabUtil.normalizeTitle(person.Name.ToLower()).Contains(Grabber.GrabUtil.normalizeTitle(name.ToLower())))
          results.Add(person);
      }
      if (results.Count > 0)
      {
        foreach (DBPersonInfo person in results)
        {
          if (person.Id != null)
          {
            try { xml = getXML(apiGetPersonInfoLanguage + person.Id); }
            catch { xml = null; }
            if (xml != null)
            {
              personNodes = xml.Item(0).SelectNodes("//person");
              foreach (XmlNode node in personNodes)
              {
                DBPersonInfo person2 = getPersonInformation(node);
                if (person2 != null && Grabber.GrabUtil.normalizeTitle(person2.Name.ToLower()).Contains(Grabber.GrabUtil.normalizeTitle(name.ToLower())))
                  if (!choose)
                  {
                    resultsdet.Add(person2);
                  }
                  else
                    resultsdet.Add(person2);
                else
                  if (choose)
                    resultsdet.Add(person2);
              }
            }
          }
        }
      }
      return resultsdet.Count > 0 ? resultsdet : results;
    }

    public DBPersonInfo getPersonsById(string id, string language)
    {
      string apiSearchLanguage = apiSearchPerson;
      string apiGetMovieInfoLanguage = apiGetMovieInfo;
      string apiGetPersonInfoLanguage = apiGetPersonInfo;
      if (language.Length == 2)
      {
        apiSearchLanguage = apiSearchPerson.Replace("/en/", "/" + language + "/");
        apiGetMovieInfoLanguage = apiGetMovieInfo.Replace("/en/", "/" + language + "/");
        apiGetPersonInfoLanguage = apiGetPersonInfo.Replace("/en/", "/" + language + "/");
      }
      DBPersonInfo result = new DBPersonInfo();
      XmlNodeList xml = getXML(apiGetPersonInfoLanguage + id);
      if (xml == null)
        return result;

      XmlNodeList personNodes = xml.Item(0).SelectNodes("//person");
      foreach (XmlNode node in personNodes)
      {
        DBPersonInfo person = getPersonInformation(node);
        if (person != null)
          result = person;
      }
      return result;
    }

    // given a url, retrieves the xml result set and returns the nodelist of Item objects
    public XmlNodeList getXML(string url)
    {
      Cornerstone.Tools.WebGrabber grabber = new Cornerstone.Tools.WebGrabber(url);
      grabber.MaxRetries = 10;
      grabber.Timeout = 5000;
      grabber.TimeoutIncrement = 1000;
      grabber.Encoding = Encoding.UTF8;

      return grabber.GetResponse() ? grabber.GetXML() : null;
    }
    private DBMovieInfo getMovieInformation(XmlNode movieNode)
    {
      if (movieNode == null)
        return null;

      if (movieNode.ChildNodes.Count < 2 || movieNode.Name != "movie")
        return null;

      List<String> producers = new List<string>();
      List<String> directors = new List<string>();
      List<String> writers = new List<string>();
      List<String> actors = new List<string>();
      List<String> backdrops = new List<string>();
      List<String> posters = new List<string>();
      List<DBPersonInfo> persons = new List<DBPersonInfo>();
      DBMovieInfo movie = new DBMovieInfo();
      foreach (XmlNode node in movieNode.ChildNodes)
      {
        string value = node.InnerText;
        switch (node.Name)
        {
          case "id":
            movie.Identifier = value;
            break;
          case "name":
          case "title":
            movie.Name = value;
            break;
          case "alternative_name":
            if (value.Trim() != "None found." && value.Trim().Length > 0)
              movie.AlternativeTitle = value;
            break;
          case "original_name":
            if (value.Trim() != "None found." && value.Trim().Length > 0)
              movie.TranslatedTitle = value;
            break;
          case "released":
          case "release":
            DateTime date;
            if (DateTime.TryParse(value, out date))
              movie.Year = date.Year;
            break;
          case "imdb":
          case "imdb_id":
            movie.ImdbID = value;
            break;
          case "url":
            movie.DetailsURL = value;
            break;
          case "overview":
          case "short_overview":
            movie.Summary = value;
            break;
          case "rating":
          case "score":
            float rating = 0;
            if (float.TryParse(value, out rating))
              movie.Score = rating;
            break;
          case "certification":
            movie.Certification = value;
            break;
          case "languages_spoken":
            foreach (XmlNode language in node.SelectNodes("language_spoken/name"))
            {
              movie.Languages.Add(language.InnerText.Trim());
            }
            break;
          case "popularity":
            int popularity = 0;
            if (int.TryParse(value, out popularity))
              movie.Popularity = popularity;
            break;
          case "runtime":
            int runtime = 0;
            if (int.TryParse(value, out runtime))
              movie.Runtime = runtime;
            break;
          case "people":
          case "cast":
            foreach (XmlNode person in node.ChildNodes)
            {
              string name = person.Attributes["name"].Value;
              string character = person.Attributes["character"].Value;
              string thumb = person.Attributes["thumb"].Value;
              string job = person.Attributes["job"].Value;
              string id = person.Attributes["id"].Value;
              string url = person.Attributes["url"].Value;
              DBPersonInfo personToAdd = new DBPersonInfo();
              personToAdd.Id = id;
              personToAdd.Name = name;
              personToAdd.DetailsURL = url;
              personToAdd.Job = job;
              persons.Add(personToAdd);
              switch (job)
              {
                case "Producer":
                  producers.Add(name);
                  break;
                case "Director":
                  directors.Add(name);
                  break;
                case "Actor":
                  if (character.Length > 0)
                    name = name + " (" + character + ")";
                  actors.Add(name);
                  break;
                case "Screenplay":
                  writers.Add(name);
                  break;
              }
            }
            //foreach (XmlNode person in node.SelectNodes("person[@job='Director']/name"))
            //{
            //    directors.Add(person.InnerText.Trim());
            //}
            //foreach (XmlNode person in node.SelectNodes("person[@job='screenplay']/name"))
            //{
            //    writers.Add(person.InnerText.Trim());
            //}
            //foreach (XmlNode person in node.SelectNodes("person[@job='actor']/name"))
            //{
            //    actors.Add(person.InnerText.Trim());
            //}
            break;
          case "countries":
            foreach (XmlNode country in node.SelectNodes("country/name"))
            {
              movie.Country.Add(country.InnerText.Trim());
            }
            break;
          case "categories":
            foreach (XmlNode category in node.SelectNodes("category/name"))
            {
              movie.Genres.Add(category.InnerText.Trim());
            }
            break;
          case "poster":
            if (node.OuterXml.Contains("\"original\""))
              posters.Add(value);
            break;
          case "backdrop":
            if (node.OuterXml.Contains("\"original\""))
              backdrops.Add(value);
            break;
          case "images":
            if (node.OuterXml.Contains("\"original\"") && node.OuterXml.Contains("\"backdrop\"") && node.OuterXml.Contains("url="))
            {
              //                            int start = node.OuterXml.IndexOf("url=") + 5;
              foreach (XmlNode image in node.SelectNodes("image"))
              {
                if (image.OuterXml.Contains("\"original\"") && image.OuterXml.Contains("\"backdrop\"") && image.OuterXml.Contains("url="))
                {
                  int start = image.OuterXml.IndexOf("url=") + 5;
                  string zvalue = image.OuterXml.Substring(start);
                  zvalue = zvalue.Substring(0, zvalue.IndexOf("\""));
                  backdrops.Add(zvalue);
                }
              }
              //                           string zvalue = node.OuterXml.Substring(start);
              //                           zvalue = zvalue.Substring(0, zvalue.IndexOf("\""));
              //                           backdrops.Add(zvalue);
            }
            if (node.OuterXml.Contains("\"original\"") && node.OuterXml.Contains("\"poster\"") && node.OuterXml.Contains("url="))
            {
              //                            int start = node.OuterXml.IndexOf("url=") + 5;
              foreach (XmlNode image in node.SelectNodes("image"))
              {
                if (image.OuterXml.Contains("\"original\"") && image.OuterXml.Contains("\"poster\"") && image.OuterXml.Contains("url="))
                {
                  int start = image.OuterXml.IndexOf("url=") + 5;
                  string zvalue = image.OuterXml.Substring(start);
                  zvalue = zvalue.Substring(0, zvalue.IndexOf("\""));
                  posters.Add(zvalue);
                }
              }
              //                           string zvalue = node.OuterXml.Substring(start);
              //                           zvalue = zvalue.Substring(0, zvalue.IndexOf("\""));
              //                           backdrops.Add(zvalue);
            }
            break;
        }
      }
      movie.Producers = producers;
      movie.Directors = directors;
      movie.Writers = writers;
      movie.Actors = actors;
      movie.Backdrops = backdrops;
      movie.Posters = posters;
      movie.Persons = persons;
      return movie;
    }
    private DBPersonInfo getPersonInformation(XmlNode personNode)
    {
      if (personNode == null)
        return null;

      if (personNode.ChildNodes.Count < 2 || personNode.Name != "person")
        return null;

      List<String> images = new List<string>();
      List<DBMovieInfo> movies = new List<DBMovieInfo>();
      //DBMovieInfo movie = new DBMovieInfo();
      DBPersonInfo person = new DBPersonInfo();
      foreach (XmlNode node in personNode.ChildNodes)
      {
        string value = node.InnerText;
        switch (node.Name)
        {
          case "id":
            person.Id = value;
            break;
          case "name":
            person.Name = value;
            break;
          case "also_known_as":
            if (value.Trim() != "None found." && value.Trim().Length > 0)
              person.AlternateName = value;
            break;
          case "birthday":
            person.Birthday = value;
            break;
          case "birthplace":
            person.Birthplace = value;
            break;
          case "url":
            person.DetailsURL = value;
            break;
          case "biography":
            person.Biography = value;
            break;
          case "popularity":
            int popularity = 0;
            if (!string.IsNullOrEmpty(value) && value.Length > 0)
            {
              if (int.TryParse(value, out popularity))
              { }
              else popularity = 0;
            }
            person.Popularity = popularity;
            break;
          case "known_movies":
            int known_movies = 0;
            if (int.TryParse(value, out known_movies))
              person.Known_movies = known_movies;
            break;
          case "filmography":
            foreach (XmlNode movie in node.ChildNodes)
            {
              DBMovieInfo movieToAdd = new DBMovieInfo();
              string id = movie.Attributes["id"].Value;
              string name = movie.Attributes["name"].Value;
              string character = movie.Attributes["character"].Value;
              string job = movie.Attributes["job"].Value;
              string url = movie.Attributes["url"].Value;
              string release = movie.Attributes["release"].Value;
              string posterUrl = movie.Attributes["poster"].Value;
              List<String> posters = new List<string>();
              posters.Add(posterUrl);
              int year = 1900;
              if (!string.IsNullOrEmpty(release) && release.Length > 3)
              {
                if (int.TryParse(movie.Attributes["release"].Value.Substring(0, 4), out year))
                { }
                else
                  year = 1900;
              }
              movieToAdd.Identifier = id;
              movieToAdd.Name = name;
              movieToAdd.DetailsURL = url;
              movieToAdd.Year = year;
              movieToAdd.Posters = posters;
              movies.Add(movieToAdd);
            }
            break;
          case "image":
            if (node.OuterXml.Contains("\"original\""))
              images.Add(value);
            break;
          case "version":
          case "last_modified_at":
            break;
          case "images":
            if (node.OuterXml.Contains("\"original\"") && node.OuterXml.Contains("\"profile\"") && node.OuterXml.Contains("url="))
            {
              foreach (XmlNode image in node.SelectNodes("image"))
              {
                if (image.OuterXml.Contains("\"original\"") && image.OuterXml.Contains("\"profile\"") && image.OuterXml.Contains("url="))
                {
                  int start = image.OuterXml.IndexOf("url=") + 5;
                  string zvalue = image.OuterXml.Substring(start);
                  zvalue = zvalue.Substring(0, zvalue.IndexOf("\""));
                  images.Add(zvalue);
                }
              }
            }
            break;
        }
      }
      person.Movies = movies;
      person.Images = images;
      return person;
    }

  }

}
