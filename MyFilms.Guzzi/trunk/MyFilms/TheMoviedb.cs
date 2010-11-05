using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Guzzi.grabber
{
    public class DBMovieInfo
    {
        public string Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        } private string identifier;
        public string Title
        {
            get { return title; }
            set { title = value; }
        } private string title;
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
        } private List<String> actors;
    }
    public class TheMoviedb
    {
        //private const string apiSearch = "http://api.themoviedb.org/2.0/Movie.search?api_key=1e66c0cc99696feaf2ea56695e134eae&title=";
        private const string apiSearch = "http://api.themoviedb.org/2.1/Movie.search/en/xml/1e66c0cc99696feaf2ea56695e134eae/";
        //private const string apiGetInfo = "http://api.themoviedb.org/2.0/Movie.getInfo?api_key=1e66c0cc99696feaf2ea56695e134eae&id=";
        private const string apiGetInfo = "http://api.themoviedb.org/2.1/Movie.getInfo/en/xml/1e66c0cc99696feaf2ea56695e134eae/";
        public List<DBMovieInfo> getMoviesByTitles(string title, string ttitle, int year, string director)
        {
            List<DBMovieInfo> results = new List<DBMovieInfo>();
            results = getMoviesByTitle(title, year, director);
            if (results.Count == 0)
                results = getMoviesByTitle(ttitle, year, director);
            return results;
        }

        public List<DBMovieInfo> getMoviesByTitle(string title,  int year, string director)
        {
            //title = Grabber.GrabUtil.normalizeTitle(title);
            string id = string.Empty;
            List<DBMovieInfo> results = new List<DBMovieInfo>();
            List<DBMovieInfo> resultsdet = new List<DBMovieInfo>();
            XmlNodeList xml = getXML(apiSearch + Grabber.GrabUtil.RemoveDiacritics(title.Trim().ToLower()));
            if (xml == null)
                    return results;
 
            XmlNodeList movieNodes = xml.Item(0).SelectNodes("//movie");
            foreach (XmlNode node in movieNodes)
            {
                DBMovieInfo movie = getMovieInformation(node);
                if (movie != null && Grabber.GrabUtil.normalizeTitle(movie.Title.ToLower()).Contains(Grabber.GrabUtil.normalizeTitle(title.ToLower())))
                    if (year > 0 && movie.Year > 0)
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
                    director = director.Substring(0,director.IndexOf(","));
                foreach (DBMovieInfo movie in results)
                {
                    if (movie.Identifier != null)
                    {
                        xml = getXML(apiGetInfo + movie.Identifier);
                        if (xml != null)
                        {
                            movieNodes = xml.Item(0).SelectNodes("//movie");
                            resultsdet.AddRange(from XmlNode node in movieNodes
                                                select getMovieInformation(node)
                                                into movie2 where movie2 != null && Grabber.GrabUtil.normalizeTitle(movie2.Title.ToLower()).Contains(Grabber.GrabUtil.normalizeTitle(title.ToLower())) && movie2.Directors.Contains(director) where year > 0 && movie2.Year > 0 where (year >= movie2.Year - 2) && (year <= movie2.Year + 2) select movie2);
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
        public XmlNodeList getXML(string url) {
            Cornerstone.Tools.WebGrabber grabber = new Cornerstone.Tools.WebGrabber(url);
            grabber.MaxRetries = 10;
            grabber.Timeout = 5000;
            grabber.TimeoutIncrement = 1000;
            grabber.Encoding = Encoding.UTF8;

            if (grabber.GetResponse())
                return grabber.GetXML("results");
            else
                return null;
        }
        private DBMovieInfo getMovieInformation(XmlNode movieNode)
        {
            if (movieNode == null)
                return null;

            if (movieNode.ChildNodes.Count < 2 || movieNode.Name != "movie")
                return null;

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
                    case "title":
                        movie.Title = value;
                        break;
                    //case "alternative_title":
                    //    // todo: remove this check when the api is fixed
                    //    if (value.Trim() != "None found." && value.Trim().Length > 0)
                    //        movie.AlternateTitles.Add(value);
                    //    break;
                    case "release":
                        DateTime date;
                        if (DateTime.TryParse(value, out date))
                            movie.Year = date.Year;
                        break;
                    case "imdb":
                        movie.ImdbID = value;
                        break;
                    case "url":
                        movie.DetailsURL = value;
                        break;
                    case "short_overview":
                        movie.Summary = value;
                        break;
                    case "rating":
                        float rating = 0;
                        if (float.TryParse(value, out rating))
                            movie.Score = rating;
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
                        directors.AddRange(from XmlNode person in node.SelectNodes("person[@job='director']/name")
                                           select person.InnerText.Trim());
                        writers.AddRange(from XmlNode person in node.SelectNodes("person[@job='screenplay']/name")
                                         select person.InnerText.Trim());
                        actors.AddRange(from XmlNode person in node.SelectNodes("person[@job='actor']/name")
                                        select person.InnerText.Trim());
                        break;
                    case "categories":
                        //todo: uncomment and adapt when the genres are implemented
                        //foreach (XmlNode category in node.SelectNodes("category/name")) {
                        //    movie.Genres.Add(category.InnerText.Trim());
                        //}
                        break;
                    case "poster":
                        if (node.OuterXml.Contains("\"original\""))
                            posters.Add(value);
                        break;
                    case "backdrop":
                        if (node.OuterXml.Contains("\"original\""))
                            backdrops.Add(value);
                        break;
                }
            }
            movie.Directors = directors;
            movie.Writers = writers;
            movie.Actors = actors;
            movie.Backdrops = backdrops;
            movie.Posters = posters;
            return movie;
        }

    }

}
