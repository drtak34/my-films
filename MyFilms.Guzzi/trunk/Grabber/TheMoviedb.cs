using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace grabber
{
    public class DBMovieInfo
    {
        public string Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        } private string identifier;
        public string Name
        {
          get { return name; }
          set { name = value; }
        } private string name;
        public string TranslatedTitle
        {
          get { return translatedTitle; }
          set { translatedTitle = value; }
        } private string translatedTitle;
        public string AlternativeTitle
        {
          get { return alternativeTitle; }
          set { alternativeTitle = value; }
        }
        private string alternativeTitle;
        public int Year
        {
            get { return year; }
            set { year = value; }
        } private int year;
        public string ImdbID
        {
            get { return imdbID; }
            set { imdbID = value; }
        } private string imdbID;
        public string DetailsURL
        {
            get { return detailsURL; }
            set { detailsURL = value; }
        } private string detailsURL;
        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        } private string summary;
        public List<String> Posters
        {
            get { return posters; }
            set { posters = value; }
        } private List<String> posters;
        public List<String> Backdrops
        {
            get { return backdrops; }
            set { backdrops = value; }
        } private List<String> backdrops;
        public float Score
        {
          get { return score; }
          set { score = value; }
        } private float score;
        public string Certification
        {
          get { return certification; }
          set { certification = value; }
        } private string certification;
        public List<String> Languages
        {
          get { return languages; }
          set { languages = value; }
        } private List<String> languages;
        public int Popularity
        {
            get { return popularity; }
            set { popularity = value; }
        } private int popularity;
        public int Runtime
        {
            get { return runtime; }
            set { runtime = value; }
        } private int runtime;
        public List<String> Producers
        {
          get { return producers; }
          set { producers = value; }
        } private List<String> producers;
        public List<String> Directors
        {
          get { return directors; }
          set { directors = value; }
        } private List<String> directors;
        public List<String> Writers
        {
            get { return writers; }
            set { writers = value; }
        } private List<String> writers;
        public List<String> Actors
        {
          get { return actors; }
          set { actors = value; }
        }
        private List<String> actors;
        public List<String> Country
        {
          get { return country; }
          set { country = value; }
        }
        private List<String> country;
        public List<String> Genres
        {
          get { return genres; }
          set { genres = value; }
        }
        private List<String> genres;
    }
    public class TheMoviedb
    {
        //private const string apiSearch = "http://api.themoviedb.org/2.0/Movie.search?api_key=1e66c0cc99696feaf2ea56695e134eae&title=";
        private const string apiSearch = "http://api.themoviedb.org/2.1/Movie.search/en/xml/1e66c0cc99696feaf2ea56695e134eae/";
        //private const string apiGetMovieInfo = "http://api.themoviedb.org/2.0/Movie.getInfo?api_key=1e66c0cc99696feaf2ea56695e134eae&id=";
        private const string apiGetMovieInfo = "http://api.themoviedb.org/2.1/Movie.getInfo/en/xml/1e66c0cc99696feaf2ea56695e134eae/";
        private const string apiGetPersonInfo = "http://api.themoviedb.org/2.1/Person.getInfo/en/xml/1e66c0cc99696feaf2ea56695e134eae/";

        public List<DBMovieInfo> getMoviesByTitles(string title, string ttitle, int year, string director, bool choose)
        {
          return getMoviesByTitles(title, ttitle, year, director, choose, "en"); // set "en" as default
        }

        public List<DBMovieInfo> getMoviesByTitles(string title, string ttitle, int year, string director, bool choose, string language)
        {
            List<DBMovieInfo> results = new List<DBMovieInfo>();
            results = getMoviesByTitle(title, year, director, choose, language);
            if (results.Count == 1)
                return results;
            List<DBMovieInfo> results2 = new List<DBMovieInfo>();
            results2 = getMoviesByTitle(ttitle, year, director, choose, language);
            if (results2.Count == 1)
                return results2;
            return results;
        }

        public List<DBMovieInfo> getMoviesByTitle(string title, int year, string director, bool choose)
        {
          return getMoviesByTitle(title, year, director, choose, "en");
        }

        public List<DBMovieInfo> getMoviesByTitle(string title, int year, string director, bool choose, string language)
        {
            //title = Grabber.GrabUtil.normalizeTitle(title);
            string id = string.Empty;
            string apiSearchLanguage = "";
            string apiGetMovieInfoLanguage = "";
            string apiGetPersonInfoLanguage = "";
            if (language.Length == 2)
            {
              apiSearchLanguage = apiSearch.Replace("/en/", "/" + language + "/");
              apiGetMovieInfoLanguage = apiGetMovieInfo.Replace("/en/", "/" + language + "/");
              apiGetPersonInfoLanguage = apiGetPersonInfo.Replace("/en/", "/" + language + "/");
            }

            List<DBMovieInfo> results = new List<DBMovieInfo>();
            List<DBMovieInfo> resultsdet = new List<DBMovieInfo>();
            XmlNodeList xml = getXML(apiSearchLanguage + Grabber.GrabUtil.RemoveDiacritics(title.Trim().ToLower()).Replace(" ", "+"));
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
            if (resultsdet.Count > 0)
                return resultsdet;
            else
                return results;
        }
        // given a url, retrieves the xml result set and returns the nodelist of Item objects
        public XmlNodeList getXML(string url)
        {
            Cornerstone.Tools.WebGrabber grabber = new Cornerstone.Tools.WebGrabber(url);
            grabber.MaxRetries = 10;
            grabber.Timeout = 5000;
            grabber.TimeoutIncrement = 1000;
            grabber.Encoding = Encoding.UTF8;

            if (grabber.GetResponse())
                return grabber.GetXML();
            else
                return null;
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
                        foreach (XmlNode country in node.SelectNodes("country/name")) {
                            movie.Country.Add(country.InnerText.Trim());
                        }
                        break;
                    case "categories":
                        foreach (XmlNode category in node.SelectNodes("category/name")) {
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
            return movie;
        }

    }

}
