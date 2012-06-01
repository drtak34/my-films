namespace Grabber.TheMovieDb
{
  using System;
  using System.Collections.Generic;
  using System.Net;
  using System.Xml.Serialization;
  using System.IO;
  using System.Xml.Linq;
  using System.ComponentModel;
  using System.Text.RegularExpressions;

  /// <summary>
    /// The main class for using TheMovieDb.org API.
    /// </summary>
    public class TmdbAPI
    {
        public delegate void MovieSearchAsyncCompletedEventHandler(object sender, ImdbMovieSearchCompletedEventArgs e);
        public delegate void MovieInfoAsyncCompletedEventHandler(object sender, ImdbMovieInfoCompletedEventArgs e);
        public delegate void PersonSearchAsyncCompletedEventHandler(object sender, ImdbPersonSearchCompletedEventArgs e);
        public delegate void PersonInfoAsyncCompletedEventHandler(object sender, ImdbPersonInfoCompletedEventArgs e);

        public event MovieSearchAsyncCompletedEventHandler MovieSearchCompleted;
        public event MovieSearchAsyncCompletedEventHandler MovieSearchByImdbCompleted;

        public event MovieInfoAsyncCompletedEventHandler GetMovieInfoCompleted;
        public event MovieInfoAsyncCompletedEventHandler GetMovieImagesCompleted;

        public event PersonSearchAsyncCompletedEventHandler PersonSearchCompleted;
        public event PersonInfoAsyncCompletedEventHandler GetPersonInfoCompleted;

        private const string MOVIE_GET_INFO_URL = "http://api.themoviedb.org/2.1/Movie.getInfo/en/xml/{0}/{1}";
        private const string MOVIE_IMDB_LOOKUP_URL = "http://api.themoviedb.org/2.1/Movie.imdbLookup/en/xml/{0}/{1}";
        private const string MOVIE_SEARCH_URL = "http://api.themoviedb.org/2.1/Movie.search/en/xml/{0}/{1}";
        private const string MOVIE_GET_IMAGES_URL = "http://api.themoviedb.org/2.1/Movie.getImages/en/xml/{0}/{1}";

        private const string PERSON_SEARCH_URL = "http://api.themoviedb.org/2.1/Person.search/en/xml/{0}/{1}";
        private const string PERSON_GET_INFO_URL = "http://api.themoviedb.org/2.1/Person.getInfo/en/xml/{0}/{1}";

        public string ApiKey { get; set; }

        public TmdbAPI(string apiKey)
        {
            this.ApiKey = apiKey;
        }

        /// <summary>
        /// the easiest and quickest way to search for a movie. It is a mandatory method in order to get the movie id to pass to the GetMovieInfo method.
        /// </summary>
        /// <param name="title">The title of the movie to search for.</param>
        /// <returns></returns>
        public TmdbMovie[] MovieSearch(string title)
        {
            TmdbMovieSearchResults search = new TmdbMovieSearchResults();
            WebClient webClient = new WebClient();

            XmlSerializer s = new XmlSerializer(typeof(TmdbMovieSearchResults));
            byte[] data = webClient.DownloadData(string.Format(MOVIE_SEARCH_URL, this.ApiKey, this.Escape(title)));
            string xml = System.Text.Encoding.UTF8.GetString(data);
            if (!xml.StartsWith("<?xml"))
                throw new Exception(xml.Replace("<p>", "").Replace("</p>", ""));

            TextReader r = new StringReader(xml);
            search = (TmdbMovieSearchResults)s.Deserialize(r);
            return search.Movies;

        }

        #region MovieSearchAsyncMethods
        private delegate void MovieSearchDelegate(string title, object userState, AsyncOperation asyncOp);
        public void MovieSearchAsync(string title)
        {
            this.MovieSearchAsync(title, null);
        }
        public void MovieSearchAsync(string title, object userState)
        {
            AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(null);
            MovieSearchDelegate worker = new MovieSearchDelegate(this.MovieSearchWorker);
            worker.BeginInvoke(title, userState, asyncOp, null, null);
        }
        private void MovieSearchWorker(string title, object userState, AsyncOperation asyncOp)
        {
            Exception exception = null;
            TmdbMovie[] movies = null;
            try
            {
                movies = this.MovieSearch(title);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            ImdbMovieSearchCompletedEventArgs args = new ImdbMovieSearchCompletedEventArgs(movies, exception, false, userState);
            asyncOp.PostOperationCompleted(
                delegate(object e) { this.OnMovieSearchCompleted((ImdbMovieSearchCompletedEventArgs)e); },
                args);
        }
        protected virtual void OnMovieSearchCompleted(ImdbMovieSearchCompletedEventArgs e)
        {
            if (this.MovieSearchCompleted != null)
                this.MovieSearchCompleted(this, e);
        }
        #endregion

        /// <summary>
        /// The easiest and quickest way to search for a movie based on it's IMDb ID. You can use Movie.imdbLookup method to get the TMDb id of a movie if you already have the IMDB id.
        /// </summary>
        /// <param name="imdbId">The IMDb ID of the movie you are searching for.</param>
        /// <returns></returns>
        public TmdbMovie[] MovieSearchByImdb(string imdbId)
        {
            TmdbMovieSearchResults search = new TmdbMovieSearchResults();
            WebClient webClient = new WebClient();

            XmlSerializer s = new XmlSerializer(typeof(TmdbMovieSearchResults));

            byte[] data = webClient.DownloadData(string.Format(MOVIE_IMDB_LOOKUP_URL, this.ApiKey, imdbId));
            string xml = System.Text.Encoding.UTF8.GetString(data);
            if (!xml.StartsWith("<?xml"))
                throw new Exception(xml.Replace("<p>", "").Replace("</p>", ""));

            TextReader r = new StringReader(xml);
            search = (TmdbMovieSearchResults)s.Deserialize(r);
            return search.Movies;

        }

        #region MovieSearchByImdbAsyncMethods
        private delegate void MovieSearchByImdbDelegate(string imdbId, object userState, AsyncOperation asyncOp);
        public void MovieSearchByImdbAsync(string imdbId)
        {
            this.MovieSearchByImdbAsync(imdbId, null);
        }
        public void MovieSearchByImdbAsync(string imdbId, object userState)
        {
            AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(null);
            MovieSearchDelegate worker = new MovieSearchDelegate(this.MovieSearchByImdbWorker);
            worker.BeginInvoke(imdbId, userState, asyncOp, null, null);
        }
        private void MovieSearchByImdbWorker(string imdbId, object userState, AsyncOperation asyncOp)
        {
            Exception exception = null;
            TmdbMovie[] movies = null;
            try
            {
                movies = this.MovieSearchByImdb(imdbId);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            ImdbMovieSearchCompletedEventArgs args = new ImdbMovieSearchCompletedEventArgs(movies, exception, false, userState);
            asyncOp.PostOperationCompleted(
                delegate(object e) { this.OnMovieSearchByImdbCompleted((ImdbMovieSearchCompletedEventArgs)e); },
                args);
        }
        protected virtual void OnMovieSearchByImdbCompleted(ImdbMovieSearchCompletedEventArgs e)
        {
            if (this.MovieSearchByImdbCompleted != null)
                this.MovieSearchByImdbCompleted(this, e);
        }
        #endregion

        /// <summary>
        /// retrieve specific information about a movie. Things like overview, release date, cast data, genre's, YouTube trailer link, etc...
        /// </summary>
        /// <param name="id">The ID of the TMDb movie you are searching for.</param>
        /// <returns></returns>
        public TmdbMovie GetMovieInfo(int id)
        {
            TmdbMovieSearchResults search = new TmdbMovieSearchResults();
            WebClient webClient = new WebClient();

            XmlSerializer s = new XmlSerializer(typeof(TmdbMovieSearchResults));

            byte[] data = webClient.DownloadData(string.Format(MOVIE_GET_INFO_URL, this.ApiKey, id));
            string xml = System.Text.Encoding.UTF8.GetString(data);
            if (!xml.StartsWith("<?xml"))
                throw new Exception(xml.Replace("<p>", "").Replace("</p>", ""));

            TextReader r = new StringReader(xml);
            search = (TmdbMovieSearchResults)s.Deserialize(r);

            if (search.Movies.Length == 1)
                return search.Movies[0];
            else
                return null;
        }

        #region GetMovieInfoAsyncMethods
        private delegate void GetMovieInfoDelegate(int id, object userState, AsyncOperation asyncOp);
        public void GetMovieInfoAsync(int id)
        {
            this.GetMovieInfoAsync(id, null);
        }
        public void GetMovieInfoAsync(int id, object userState)
        {
            AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(null);
            GetMovieInfoDelegate worker = new GetMovieInfoDelegate(this.GetMovieInfoWorker);
            worker.BeginInvoke(id, userState, asyncOp, null, null);
        }
        private void GetMovieInfoWorker(int id, object userState, AsyncOperation asyncOp)
        {
            Exception exception = null;
            TmdbMovie movie = null;
            try
            {
                movie = this.GetMovieInfo(id);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            ImdbMovieInfoCompletedEventArgs args = new ImdbMovieInfoCompletedEventArgs(movie, exception, false, userState);
            asyncOp.PostOperationCompleted(
                delegate(object e) { this.OnGetMovieInfoCompleted((ImdbMovieInfoCompletedEventArgs)e); },
                args);
        }
        protected virtual void OnGetMovieInfoCompleted(ImdbMovieInfoCompletedEventArgs e)
        {
            if (this.GetMovieInfoCompleted != null)
                this.GetMovieInfoCompleted(this, e);
        }
        #endregion

        /// <summary>
        /// Retrieve all of the backdrops and posters for a particular movie. This is useful to scan for updates, or new images if that's all you're after.
        /// </summary>
        /// <param name="id">TMDb ID (starting with tt) of the movie you are searching for.</param>
        /// <returns></returns>
        public TmdbMovie GetMovieImages(int id)
        {
            return this.GetMovieImages(id.ToString());
        }

        #region GetMovieImagesMethods
        private delegate void GetMovieImagesDelegate(string imdbId, object userState, AsyncOperation asyncOp);
        public void GetMovieImagesAsync(int id)
        {
            this.GetMovieImagesAsync(id, null);
        }
        public void GetMovieImagesAsync(int id, object userState)
        {
            AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(null);
            GetMovieImagesDelegate worker = new GetMovieImagesDelegate(this.GetMovieImagesWorker);
            worker.BeginInvoke(id.ToString(), userState, asyncOp, null, null);
        }
        private void GetMovieImagesWorker(string id, object userState, AsyncOperation asyncOp)
        {
            Exception exception = null;
            TmdbMovie movie = null;
            try
            {
                movie = this.GetMovieImages(id);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            ImdbMovieInfoCompletedEventArgs args = new ImdbMovieInfoCompletedEventArgs(movie, exception, false, userState);
            asyncOp.PostOperationCompleted(
                delegate(object e) { this.OnGetMovieImagesCompleted((ImdbMovieInfoCompletedEventArgs)e); },
                args);
        }
        protected virtual void OnGetMovieImagesCompleted(ImdbMovieInfoCompletedEventArgs e)
        {
            if (this.GetMovieImagesCompleted != null)
                this.GetMovieImagesCompleted(this, e);
        }
        #endregion

        /// <summary>
        /// Retrieve all of the backdrops and posters for a particular movie. This is useful to scan for updates, or new images if that's all you're after.
        /// </summary>
        /// <param name="imdbId">IMDB ID (starting with tt) of the movie you are searching for.</param>
        /// <returns></returns>
        public TmdbMovie GetMovieImages(string imdbId)
        {
            XDocument xdoc = XDocument.Load(string.Format(MOVIE_GET_IMAGES_URL, this.ApiKey, imdbId));

            foreach (XElement xmovie in xdoc.Descendants("movie"))
            {
                TmdbMovie movie = new TmdbMovie() { Name = xmovie.Element("name").Value };
                XElement ximages = xmovie.Element("images");
                if (ximages == null)
                    continue;

                List<TmdbImage> images = new List<TmdbImage>();
                foreach (XElement ximageType in ximages.Elements())
                {
                    TmdbImageType type = (TmdbImageType)Enum.Parse(typeof(TmdbImageType), ximageType.Name.ToString());
                    int imageId = int.Parse(ximageType.Attribute("id").Value);
                    foreach (XElement ximage in ximageType.Elements("image"))
                    {
                        TmdbImage image = new TmdbImage()
                        {
                            Id = imageId,
                            Size = (TmdbImageSize)Enum.Parse(typeof(TmdbImageSize), ximage.Attribute("size").Value),
                            Type = type,
                            Url = ximage.Attribute("url").Value
                        };
                        images.Add(image);
                    }
                }
                movie.Images = images.ToArray();
                return movie;
            }
            return null;
        }

        #region GetMovieImagesMethods
        public void GetMovieImagesAsync(string imdbId)
        {
            this.GetMovieImagesAsync(imdbId, null);
        }
        public void GetMovieImagesAsync(string imdbId, object userState)
        {
            AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(null);
            GetMovieImagesDelegate worker = new GetMovieImagesDelegate(this.GetMovieImagesWorker);
            worker.BeginInvoke(imdbId, userState, asyncOp, null, null);
        }

        #endregion

        /// <summary>
        /// Search for an actor, actress or production member.
        /// </summary>
        /// <param name="name">The name of the person you are searching for.</param>
        /// <returns>TmdbPerson[]</returns>
        public TmdbPerson[] PersonSearch(string name)
        {
            TmdbPersonSearchResults search = new TmdbPersonSearchResults();
            WebClient webClient = new WebClient();

            XmlSerializer s = new XmlSerializer(typeof(TmdbPersonSearchResults));
            byte[] data = webClient.DownloadData(string.Format(PERSON_SEARCH_URL, this.ApiKey, this.Escape(name)));
            string xml = System.Text.Encoding.UTF8.GetString(data);
            if (!xml.StartsWith("<?xml"))
                throw new Exception(xml.Replace("<p>", "").Replace("</p>", ""));

            TextReader r = new StringReader(xml);
            search = (TmdbPersonSearchResults)s.Deserialize(r);
            return search.People;
        }

        #region PersonSearchAsyncMethods
        private delegate void PersonSearchDelegate(string name, object userState, AsyncOperation asyncOp);
        public void PersonSearchAsync(string name)
        {
            this.PersonSearchAsync(name, null);
        }
        public void PersonSearchAsync(string name, object userState)
        {
            AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(null);
            PersonSearchDelegate worker = new PersonSearchDelegate(this.PersonSearchWorker);
            worker.BeginInvoke(name, userState, asyncOp, null, null);
        }
        private void PersonSearchWorker(string name, object userState, AsyncOperation asyncOp)
        {
            Exception exception = null;
            TmdbPerson[] people = null;
            try
            {
                people = this.PersonSearch(name);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            ImdbPersonSearchCompletedEventArgs args = new ImdbPersonSearchCompletedEventArgs(people, exception, false, userState);
            asyncOp.PostOperationCompleted(
                delegate(object e) { this.OnPersonSearchCompleted((ImdbPersonSearchCompletedEventArgs)e); },
                args);
        }
        protected virtual void OnPersonSearchCompleted(ImdbPersonSearchCompletedEventArgs e)
        {
            if (this.PersonSearchCompleted != null)
                this.PersonSearchCompleted(this, e);
        }
        #endregion

        /// <summary>
        /// Retrieve the full filmography, known movies, images and things like birthplace for a specific person in the TMDb database.
        /// </summary>
        /// <param name="id">The ID of the TMDb person you are searching for.</param>
        /// <returns>TmdbPerson</returns>
        public TmdbPerson GetPersonInfo(int id)
        {
            TmdbPersonSearchResults search = new TmdbPersonSearchResults();
            WebClient webClient = new WebClient();

            XmlSerializer s = new XmlSerializer(typeof(TmdbPersonSearchResults));
            byte[] data = webClient.DownloadData(string.Format(PERSON_GET_INFO_URL, this.ApiKey, id));
            string xml = System.Text.Encoding.UTF8.GetString(data);
            if (!xml.StartsWith("<?xml"))
                throw new Exception(xml.Replace("<p>", "").Replace("</p>", ""));

            TextReader r = new StringReader(xml);
            search = (TmdbPersonSearchResults)s.Deserialize(r);

            if (search.People.Length == 1)
                return search.People[0];
            else
                return null;
        }

        #region GetPersonInfoAsyncMethods
        private delegate void GetPersonInfoDelegate(int id, object userState, AsyncOperation asyncOp);
        public void GetPersonInfoAsync(int id)
        {
            this.GetPersonInfoAsync(id, null);
        }
        public void GetPersonInfoAsync(int id, object userState)
        {
            AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(null);
            GetPersonInfoDelegate worker = new GetPersonInfoDelegate(this.GetPersonInfoWorker);
            worker.BeginInvoke(id, userState, asyncOp, null, null);
        }
        private void GetPersonInfoWorker(int id, object userState, AsyncOperation asyncOp)
        {
            Exception exception = null;
            TmdbPerson person = null;
            try
            {
                person = this.GetPersonInfo(id);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            ImdbPersonInfoCompletedEventArgs args = new ImdbPersonInfoCompletedEventArgs(person, exception, false, userState);
            asyncOp.PostOperationCompleted(
                delegate(object e) { this.OnGetPersonInfoCompleted((ImdbPersonInfoCompletedEventArgs)e); },
                args);
        }
        protected virtual void OnGetPersonInfoCompleted(ImdbPersonInfoCompletedEventArgs e)
        {
            if (this.GetPersonInfoCompleted != null)
                this.GetPersonInfoCompleted(this, e);
        }
        #endregion


        private string Escape(string s)
        {
            return Regex.Replace(s, "[" + Regex.Escape(new String(Path.GetInvalidFileNameChars())) + "]", "-");
        }
    }
}
