using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace grabber
{
  using System.Linq;

  public class DbMovieInfo
  {
    public string Identifier { get; set; }
    public string Name { get; set; }
    public string TranslatedTitle { get; set; }
    public string AlternativeTitle { get; set; }
    public int Year { get; set; }
    public string ImdbId { get; set; }
    public string DetailsUrl { get; set; }
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
    public List<DbPersonInfo> Persons { get; set; }
  }

  public class DbPersonInfo
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string AlternateName { get; set; }
    public string Job { get; set; }
    public string Biography { get; set; }
    public string Birthday { get; set; }
    public string Birthplace { get; set; }
    public string DetailsUrl { get; set; }
    public int Popularity { get; set; }
    public List<string> Images { get; set; }
    public int KnownMovies { get; set; }
    public List<DbMovieInfo> Movies { get; set; }
  }

  public class TheMoviedb
  {
    // private const string apiSearchMovie = "http://api.themoviedb.org/2.0/Movie.search?api_key=1e66c0cc99696feaf2ea56695e134eae&title=";
    private const string ApiSearchMovie = "http://api.themoviedb.org/2.1/Movie.search/en/xml/1e66c0cc99696feaf2ea56695e134eae/";
    private const string ApiSearchMovieByImdb = "http://api.themoviedb.org/2.1/Movie.imdbLookup/en/xml/1e66c0cc99696feaf2ea56695e134eae/"; // add tt-nbr to search for movie ... tt0137523
    private const string ApiSearchPerson = "http://api.themoviedb.org/2.1/Person.search/en/xml/1e66c0cc99696feaf2ea56695e134eae/";
    // private const string apiGetMovieInfo = "http://api.themoviedb.org/2.0/Movie.getInfo?api_key=1e66c0cc99696feaf2ea56695e134eae&id=";
    private const string ApiGetMovieInfo = "http://api.themoviedb.org/2.1/Movie.getInfo/en/xml/1e66c0cc99696feaf2ea56695e134eae/";
    private const string ApiGetPersonInfo = "http://api.themoviedb.org/2.1/Person.getInfo/en/xml/1e66c0cc99696feaf2ea56695e134eae/";


    public List<DbMovieInfo> GetMoviesByTitles(string title, string ttitle, int year, string director, string imdbid, bool choose)
    {
      return this.GetMoviesByTitles(title, ttitle, year, director, imdbid, choose, "en"); // set "en" as default
    }

    public List<DbMovieInfo> GetMoviesByTitles(string title, string ttitle, int year, string director, string imdbid, bool choose, string language)
    {
      List<DbMovieInfo> results = this.GetMoviesByTitle(title, year, director, imdbid, choose, language);
      if (results.Count == 1)
        return results;
      List<DbMovieInfo> results2 = this.GetMoviesByTitle(ttitle, year, director, imdbid, choose, language);
      return results2.Count == 1 ? results2 : results;
    }

    public List<DbMovieInfo> GetMoviesByTitle(string title, int year, string director, string imdbid, bool choose, string language)
    {
      //title = Grabber.GrabUtil.normalizeTitle(title);
      string id = string.Empty;
      string apiSearchLanguage = ApiSearchMovie;
      string apiGetMovieInfoLanguage = ApiGetMovieInfo;
      string apiGetPersonInfoLanguage = ApiGetPersonInfo;
      if (language.Length == 2)
      {
        apiSearchLanguage = ApiSearchMovie.Replace("/en/", "/" + language + "/");
        apiGetMovieInfoLanguage = ApiGetMovieInfo.Replace("/en/", "/" + language + "/");
        apiGetPersonInfoLanguage = ApiGetPersonInfo.Replace("/en/", "/" + language + "/");
      }

      var results = new List<DbMovieInfo>();
      var resultsdet = new List<DbMovieInfo>();
      XmlNodeList xml = null;
      if (!string.IsNullOrEmpty(imdbid))
        xml = this.GetXml(ApiSearchMovieByImdb + imdbid);
      if (xml == null) // if imdb search was unsuccessful use normal search...
        xml = this.GetXml(apiSearchLanguage + Grabber.GrabUtil.RemoveDiacritics(title.Trim().ToLower()).Replace(" ", "+"));

      if (xml == null)
        return results;

      XmlNodeList movieNodes = xml.Item(0).SelectNodes("//movie");
      foreach (XmlNode node in movieNodes)
      {
        DbMovieInfo movie = GetMovieInformation(node);
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
        if (director.IndexOf(",", System.StringComparison.Ordinal) > 0) director = director.Substring(0, director.IndexOf(",", System.StringComparison.Ordinal));
        foreach (DbMovieInfo movie in results.Where(movie => movie.Identifier != null))
        {
          try { xml = this.GetXml(apiGetMovieInfoLanguage + movie.Identifier); }
          catch { xml = null; }
          if (xml != null)
          {
            movieNodes = xml.Item(0).SelectNodes("//movie");
            foreach (DbMovieInfo movie2 in from XmlNode node in movieNodes select GetMovieInformation(node))
            {
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
      return resultsdet.Count > 0 ? resultsdet : results;
    }

    public List<DbPersonInfo> GetPersonsByName(string name, bool choose, string language)
    {
      //name = Grabber.GrabUtil.normalizeTitle(name);
      string id = string.Empty;
      string apiSearchLanguage = ApiSearchPerson;
      string apiGetMovieInfoLanguage = ApiGetMovieInfo;
      string apiGetPersonInfoLanguage = ApiGetPersonInfo;
      if (language.Length == 2)
      {
        apiSearchLanguage = ApiSearchPerson.Replace("/en/", "/" + language + "/");
        apiGetMovieInfoLanguage = ApiGetMovieInfo.Replace("/en/", "/" + language + "/");
        apiGetPersonInfoLanguage = ApiGetPersonInfo.Replace("/en/", "/" + language + "/");
      }

      var results = new List<DbPersonInfo>();
      var resultsdet = new List<DbPersonInfo>();
      XmlNodeList xml = this.GetXml(apiSearchLanguage + Grabber.GrabUtil.RemoveDiacritics(name.Trim().ToLower()).Replace(" ", "+"));
      if (xml == null) return results;

      XmlNodeList personNodes = xml.Item(0).SelectNodes("//person");
      results.AddRange(personNodes.Cast<XmlNode>().Select(node => GetPersonInformation(node)).Where(person => person != null && Grabber.GrabUtil.normalizeTitle(person.Name.ToLower()).Contains(Grabber.GrabUtil.normalizeTitle(name.ToLower()))));
      if (results.Count > 0)
      {
        foreach (DbPersonInfo person in results.Where(person => person.Id != null))
        {
          try { xml = this.GetXml(apiGetPersonInfoLanguage + person.Id); }
          catch { xml = null; }
          if (xml != null)
          {
            personNodes = xml.Item(0).SelectNodes("//person");
            foreach (DbPersonInfo person2 in from XmlNode node in personNodes select GetPersonInformation(node))
            {
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
      return resultsdet.Count > 0 ? resultsdet : results;
    }

    public DbPersonInfo GetPersonsById(string id, string language)
    {
      string apiSearchLanguage = ApiSearchPerson;
      string apiGetMovieInfoLanguage = ApiGetMovieInfo;
      string apiGetPersonInfoLanguage = ApiGetPersonInfo;
      if (language.Length == 2)
      {
        apiSearchLanguage = ApiSearchPerson.Replace("/en/", "/" + language + "/");
        apiGetMovieInfoLanguage = ApiGetMovieInfo.Replace("/en/", "/" + language + "/");
        apiGetPersonInfoLanguage = ApiGetPersonInfo.Replace("/en/", "/" + language + "/");
      }
      var result = new DbPersonInfo();
      XmlNodeList xml = this.GetXml(apiGetPersonInfoLanguage + id);
      if (xml == null)
        return result;

      XmlNodeList personNodes = xml.Item(0).SelectNodes("//person");
      foreach (DbPersonInfo person in personNodes.Cast<XmlNode>().Select(node => GetPersonInformation(node)).Where(person => person != null))
      {
        result = person;
      }
      return result;
    }

    public XmlNodeList GetXml(string url)
    {
      // given a url, retrieves the xml result set and returns the nodelist of Item objects
      var grabber = new Cornerstone.Tools.WebGrabber(url);
      grabber.MaxRetries = 10;
      grabber.Timeout = 5000;
      grabber.TimeoutIncrement = 1000;
      grabber.Encoding = Encoding.UTF8;
      return grabber.GetResponse() ? grabber.GetXML() : null;
    }

    private static DbMovieInfo GetMovieInformation(XmlNode movieNode)
    {
      if (movieNode == null)
        return null;

      if (movieNode.ChildNodes.Count < 2 || movieNode.Name != "movie")
        return null;

      var producers = new List<string>();
      var directors = new List<string>();
      var writers = new List<string>();
      var actors = new List<string>();
      var backdrops = new List<string>();
      var posters = new List<string>();
      var persons = new List<DbPersonInfo>();
      var movie = new DbMovieInfo();
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
            movie.ImdbId = value;
            break;
          case "url":
            movie.DetailsUrl = value;
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
              var personToAdd = new DbPersonInfo { Id = id, Name = name, DetailsUrl = url, Job = job };
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
              backdrops.AddRange(from XmlNode image in node.SelectNodes("image") where image.OuterXml.Contains("\"original\"") && image.OuterXml.Contains("\"backdrop\"") && image.OuterXml.Contains("url=") let start = image.OuterXml.IndexOf("url=") + 5 select image.OuterXml.Substring(start) into zvalue select zvalue.Substring(0, zvalue.IndexOf("\"")));
            }
            if (node.OuterXml.Contains("\"original\"") && node.OuterXml.Contains("\"poster\"") && node.OuterXml.Contains("url="))
            {
              posters.AddRange(from XmlNode image in node.SelectNodes("image") where image.OuterXml.Contains("\"original\"") && image.OuterXml.Contains("\"poster\"") && image.OuterXml.Contains("url=") let start = image.OuterXml.IndexOf("url=") + 5 select image.OuterXml.Substring(start) into zvalue select zvalue.Substring(0, zvalue.IndexOf("\"")));
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

    private static DbPersonInfo GetPersonInformation(XmlNode personNode)
    {
      if (personNode == null)
        return null;

      if (personNode.ChildNodes.Count < 2 || personNode.Name != "person")
        return null;

      var images = new List<string>();
      var movies = new List<DbMovieInfo>();
      var person = new DbPersonInfo();
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
            person.DetailsUrl = value;
            break;
          case "biography":
            person.Biography = value;
            break;
          case "popularity":
            int popularity = 0;
            if (!string.IsNullOrEmpty(value) && value.Length > 0)
            {
              if (!int.TryParse(value, out popularity)) popularity = 0;
            }
            person.Popularity = popularity;
            break;
          case "known_movies":
            int knownMovies = 0;
            if (int.TryParse(value, out knownMovies))
              person.KnownMovies = knownMovies;
            break;
          case "filmography":
            foreach (XmlNode movie in node.ChildNodes)
            {
              var movieToAdd = new DbMovieInfo();
              string id = movie.Attributes["id"].Value;
              string name = movie.Attributes["name"].Value;
              string character = movie.Attributes["character"].Value;
              string job = movie.Attributes["job"].Value;
              string url = movie.Attributes["url"].Value;
              string release = movie.Attributes["release"].Value;
              string posterUrl = movie.Attributes["poster"].Value;
              var posters = new List<string> { posterUrl };
              int year = 1900;
              if (!string.IsNullOrEmpty(release) && release.Length > 3)
              {
                if (!int.TryParse(movie.Attributes["release"].Value.Substring(0, 4), out year)) year = 1900;
              }
              movieToAdd.Identifier = id;
              movieToAdd.Name = name;
              movieToAdd.DetailsUrl = url;
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
              images.AddRange(from XmlNode image in node.SelectNodes("image") where image.OuterXml.Contains("\"original\"") && image.OuterXml.Contains("\"profile\"") && image.OuterXml.Contains("url=") let start = image.OuterXml.IndexOf("url=") + 5 select image.OuterXml.Substring(start) into zvalue select zvalue.Substring(0, zvalue.IndexOf("\"")));
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
