namespace Grabber.TMDBv3
{
  using System;
  using System.Linq;

  using RestSharp;

  public partial class Tmdb
    {
        private void ProcessAsyncRequest<T>(RestRequest request, Action<TmdbAsyncResult<T>> callback)
            where T : new()
        {
            var client = new RestClient(BASE_URL);
            client.AddHandler("application/json", new WatJsonDeserializer());
            if (this.Timeout.HasValue)
                client.Timeout = this.Timeout.Value;

#if !WINDOWS_PHONE
            if (this.Proxy != null)
                client.Proxy = this.Proxy;
#endif

            this.Error = null;

            request.AddHeader("Accept", "application/json");
            request.AddParameter("api_key", this.ApiKey);

            var asyncHandle = client.ExecuteAsync<T>(request, resp =>
                {
                    var result = new TmdbAsyncResult<T>
                    {
                        Data = resp.Data,
                        UserState = request.UserState
                    };

                    this.ResponseContent = resp.Content;
                    this.ResponseHeaders = resp.Headers.ToDictionary(k => k.Name, v => v.Value);

                    if (resp.ResponseStatus != ResponseStatus.Completed)
                    {
                        if (resp.Content.Contains("status_message"))
                            result.Error = this.jsonDeserializer.Deserialize<TmdbError>(resp);
                        else if (resp.ErrorException != null)
                            throw resp.ErrorException;
                        else
                            result.Error = new TmdbError { status_message = resp.Content };
                    }

                    this.Error = result.Error;

                    callback(result);
                });
        }

        private void ProcessAsyncRequestETag(RestRequest request, Action<TmdbAsyncETagResult> callback)
        {
            var client = new RestClient(BASE_URL);
            if (this.Timeout.HasValue)
                client.Timeout = this.Timeout.Value;

#if !WINDOWS_PHONE
            if (this.Proxy!=null)
                client.Proxy = this.Proxy;
#endif

            this.Error = null;

            request.Method = Method.HEAD;
            request.AddHeader("Accept", "application/json");
            request.AddParameter("api_key", this.ApiKey);

            var asyncHandle = client.ExecuteAsync(request, resp =>
                {
                    this.ResponseContent = resp.Content;
                    this.ResponseHeaders = resp.Headers.ToDictionary(k => k.Name, v => v.Value);

                    var result = new TmdbAsyncETagResult
                    {
                        ETag = this.ResponseETag,
                        UserState = request.UserState
                    };

                    if (resp.ResponseStatus != ResponseStatus.Completed && resp.ErrorException != null)
                        throw resp.ErrorException;

                    callback(result);
                });
        }

        #region Configuration
        /// <summary>
        /// Retrieve configuration data from TMDB
        /// (http://help.themoviedb.org/kb/api/configuration)
        /// </summary>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetConfiguration(object UserState, Action<TmdbAsyncResult<TmdbConfiguration>> callback)
        {
            this.ProcessAsyncRequest<TmdbConfiguration>(BuildGetConfigurationRequest(UserState), callback);
        }

        public void GetConfigurationETag(object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetConfigurationRequest(UserState), callback);
        }
        #endregion


        #region Search
        /// <summary>
        /// Search for movies that are listed in TMDB
        /// (http://help.themoviedb.org/kb/api/search-movies)
        /// </summary>
        /// <param name="query">Is your search text.</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="includeAdult">optional - include adult items in your search, (Default=false)</param>
        /// <param name="year">optional - to get a closer result</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void SearchMovie(string query, int page, string language, bool? includeAdult, int? year, object UserState, Action<TmdbAsyncResult<TmdbMovieSearch>> callback)
        {
            if (string.IsNullOrEmpty(query))
            {
                callback(new TmdbAsyncResult<TmdbMovieSearch>
                {
                    Data = null,
                    Error = new TmdbError { status_message = "Search cannot be empty" },
                    UserState = UserState
                });
                return;
            }

            this.ProcessAsyncRequest<TmdbMovieSearch>(BuildSearchMovieRequest(query, page, language, includeAdult, year, UserState), callback);
        }

        /// <summary>
        /// Search for movies that are listed in TMDB
        /// (http://help.themoviedb.org/kb/api/search-movies)
        /// </summary>
        /// <param name="query">Is your search text.</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void SearchMovie(string query, int page, object UserState, Action<TmdbAsyncResult<TmdbMovieSearch>> callback)
        {
            this.SearchMovie(query, page, this.Language, null, null, UserState, callback);
        }

        /// <summary>
        /// Search for people that are listed in TMDB.
        /// (http://help.themoviedb.org/kb/api/search-people)
        /// </summary>
        /// <param name="query">Is your search text.</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void SearchPerson(string query, int page, string language, object UserState, Action<TmdbAsyncResult<TmdbPersonSearch>> callback)
        {
            if (string.IsNullOrEmpty(query))
            {
                callback(new TmdbAsyncResult<TmdbPersonSearch>
                {
                    Data = null,
                    Error = new TmdbError { status_message = "Search cannot be empty" },
                    UserState = UserState
                });
                return;
            }

            this.ProcessAsyncRequest<TmdbPersonSearch>(BuildSearchPersonRequest(query, page, language, UserState), callback);
        }

        /// <summary>
        /// Search for people that are listed in TMDB.
        /// (http://help.themoviedb.org/kb/api/search-people)
        /// </summary>
        /// <param name="query">Is your search text.</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void SearchPerson(string query, int page, object UserState, Action<TmdbAsyncResult<TmdbPersonSearch>> callback)
        {
            this.SearchPerson(query, page, this.Language, UserState, callback);
        }

        /// <summary>
        /// Search for production companies that are part of TMDB.
        /// (http://help.themoviedb.org/kb/api/search-companies)
        /// </summary>
        /// <param name="query">Is your search text.</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void SearchCompany(string query, int page, object UserState, Action<TmdbAsyncResult<TmdbCompanySearch>> callback)
        {
            if (string.IsNullOrEmpty(query))
            {
                callback(new TmdbAsyncResult<TmdbCompanySearch>
                {
                    Data = null,
                    Error = new TmdbError { status_message = "Search cannot be empty" },
                    UserState = UserState
                });
                return;
            }

            this.ProcessAsyncRequest<TmdbCompanySearch>(BuildSearchCompanyRequest(query, page, UserState), callback);
        }
        #endregion


        #region Collections
        /// <summary>
        /// Get all of the basic information about a movie collection.
        /// (http://help.themoviedb.org/kb/api/collection-info)
        /// </summary>
        /// <param name="CollectionID">Collection ID, available in TmdbMovie::belongs_to_collection</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetCollectionInfo(int CollectionID, string language, object UserState, Action<TmdbAsyncResult<TmdbCollection>> callback)
        {
            this.ProcessAsyncRequest<TmdbCollection>(BuildGetCollectionInfoRequest(CollectionID, language, UserState), callback);
        }

        public void GetCollectionInfoETag(int CollectionID, string language, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetCollectionInfoRequest(CollectionID, language, UserState), callback);
        }

        /// <summary>
        /// Get all of the basic information about a movie collection.
        /// (http://help.themoviedb.org/kb/api/collection-info)
        /// </summary>
        /// <param name="CollectionID">Collection ID, available in TmdbMovie::belongs_to_collection</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetCollectionInfo(int CollectionID, object UserState, Action<TmdbAsyncResult<TmdbCollection>> callback)
        {
            this.GetCollectionInfo(CollectionID, this.Language, UserState, callback);
        }

        public void GetCollectionInfoETag(int CollectionID, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.GetCollectionInfoETag(CollectionID, this.Language, UserState, callback);
        }
        #endregion


        #region Movie Info
        /// <summary>
        /// Retrieve all the basic movie information for a particular movie by TMDB reference.
        /// (http://help.themoviedb.org/kb/api/movie-info)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetMovieInfo(int MovieID, string language, object UserState, Action<TmdbAsyncResult<TmdbMovie>> callback)
        {
            this.ProcessAsyncRequest<TmdbMovie>(BuildGetMovieInfoRequest(MovieID, language, UserState), callback);
        }

        public void GetMovieInfoETag(int MovieID, string language, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetMovieInfoRequest(MovieID, language, UserState), callback);
        }

        /// <summary>
        /// Retrieve all the basic movie information for a particular movie by TMDB reference.
        /// (http://help.themoviedb.org/kb/api/movie-info)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetMovieInfo(int MovieID, object UserState, Action<TmdbAsyncResult<TmdbMovie>> callback)
        {
            this.GetMovieInfo(MovieID, this.Language, UserState, callback);
        }

        public void GetMovieInfoETag(int MovieID, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.GetMovieInfoETag(MovieID, this.Language, UserState, callback);
        }

        /// <summary>
        /// Retrieve all the basic movie information for a particular movie by IMDB reference.
        /// (http://help.themoviedb.org/kb/api/movie-info)
        /// </summary>
        /// <param name="IMDB_ID">IMDB movie id</param>
        /// <param name="callback"></param>
        public void GetMovieByIMDB(string IMDB_ID, object UserState, Action<TmdbAsyncResult<TmdbMovie>> callback)
        {
            if (string.IsNullOrEmpty(IMDB_ID))
            {
                callback(new TmdbAsyncResult<TmdbMovie>
                {
                    Data = null,
                    Error = new TmdbError { status_message = "Search cannot be empty" },
                    UserState = UserState
                });
                return;
            }

            this.ProcessAsyncRequest<TmdbMovie>(BuildGetMovieByIMDBRequest(IMDB_ID, UserState), callback);
        }

        /// <summary>
        /// Get list of all the alternative titles for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-alternative-titles)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="Country">ISO 3166-1 country code (optional)</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetMovieAlternateTitles(int MovieID, string Country, object UserState, Action<TmdbAsyncResult<TmdbMovieAlternateTitles>> callback)
        {
            this.ProcessAsyncRequest<TmdbMovieAlternateTitles>(BuildGetMovieAlternateTitlesRequest(MovieID, Country, UserState), callback);
        }

        public void GetMovieAlternateTitlesETag(int MovieID, string Country, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetMovieAlternateTitlesRequest(MovieID, Country, UserState), callback);
        }

        /// <summary>
        /// Get list of all the cast information for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-casts)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetMovieCast(int MovieID, object UserState, Action<TmdbAsyncResult<TmdbMovieCast>> callback)
        {
            this.ProcessAsyncRequest<TmdbMovieCast>(BuildGetMovieCastRequest(MovieID, UserState), callback);
        }

        public void GetMovieCastETag(int MovieID, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetMovieCastRequest(MovieID, UserState), callback);
        }

        /// <summary>
        /// Get list of all the images for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-images)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetMovieImages(int MovieID, string language, object UserState, Action<TmdbAsyncResult<TmdbMovieImages>> callback)
        {
            this.ProcessAsyncRequest<TmdbMovieImages>(BuildGetMovieImagesRequest(MovieID, language, UserState), callback);
        }

        public void GetMovieImagesETag(int MovieID, string language, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetMovieImagesRequest(MovieID, language, UserState), callback);
        }

        /// <summary>
        /// Get list of all the images for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-images)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetMovieImages(int MovieID, object UserState, Action<TmdbAsyncResult<TmdbMovieImages>> callback)
        {
            this.GetMovieImages(MovieID, this.Language, UserState, callback);
        }

        public void GetMovieImagesETag(int MovieID, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.GetMovieImagesETag(MovieID, this.Language, UserState, callback);
        }

        /// <summary>
        /// Get list of all the keywords that have been added to a particular movie.  Only English keywords exist currently.
        /// (http://help.themoviedb.org/kb/api/movie-keywords)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetMovieKeywords(int MovieID, object UserState, Action<TmdbAsyncResult<TmdbMovieKeywords>> callback)
        {
            this.ProcessAsyncRequest<TmdbMovieKeywords>(BuildGetMovieKeywordsRequest(MovieID, UserState), callback);
        }

        public void GetMovieKeywordsETag(int MovieID, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetMovieKeywordsRequest(MovieID, UserState), callback);
        }

        /// <summary>
        /// Get all the release and certification data in TMDB for a particular movie
        /// (http://help.themoviedb.org/kb/api/movie-release-info)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetMovieReleases(int MovieID, object UserState, Action<TmdbAsyncResult<TmdbMovieReleases>> callback)
        {
            this.ProcessAsyncRequest<TmdbMovieReleases>(BuildGetMovieReleasesRequest(MovieID, UserState), callback);
        }

        public void GetMovieReleasesETag(int MovieID, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetMovieReleasesRequest(MovieID, UserState), callback);
        }

        /// <summary>
        /// Get list of trailers for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-trailers)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetMovieTrailers(int MovieID, string language, object UserState, Action<TmdbAsyncResult<TmdbMovieTrailers>> callback)
        {
            this.ProcessAsyncRequest<TmdbMovieTrailers>(BuildGetMovieTrailersRequest(MovieID, language, UserState), callback);
        }

        public void GetMovieTrailersETag(int MovieID, string language, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetMovieTrailersRequest(MovieID, language, UserState), callback);
        }

        /// <summary>
        /// Get list of trailers for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-trailers)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetMovieTrailers(int MovieID, object UserState, Action<TmdbAsyncResult<TmdbMovieTrailers>> callback)
        {
            this.GetMovieTrailers(MovieID, this.Language, UserState, callback);
        }

        public void GetMovieTrailersETag(int MovieID, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.GetMovieTrailersETag(MovieID, this.Language, UserState, callback);
        }

        /// <summary>
        /// Get list of all available translations for a specific movie.
        /// (http://help.themoviedb.org/kb/api/movie-translations)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <returns></returns>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetMovieTranslations(int MovieID, object UserState, Action<TmdbAsyncResult<TmdbTranslations>> callback)
        {
            this.ProcessAsyncRequest<TmdbTranslations>(BuildGetMovieTranslationsRequest(MovieID, UserState), callback);
        }

        public void GetMovieTranslationsETag(int MovieID, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetMovieTranslationsRequest(MovieID, UserState), callback);
        }

        /// <summary>
        /// Get list of similar movies for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-similar-movies)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetSimilarMovies(int MovieID, int page, string language, object UserState, Action<TmdbAsyncResult<TmdbSimilarMovies>> callback)
        {
            this.ProcessAsyncRequest<TmdbSimilarMovies>(BuildGetSimilarMoviesRequest(MovieID, page, language, UserState), callback);
        }

        /// <summary>
        /// Get list of similar movies for a particular movie.
        /// (http://help.themoviedb.org/kb/api/movie-similar-movies)
        /// </summary>
        /// <param name="MovieID">TMDB movie id</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetSimilarMovies(int MovieID, int page, object UserState, Action<TmdbAsyncResult<TmdbSimilarMovies>> callback)
        {
            this.GetSimilarMovies(MovieID, page, this.Language, UserState, callback);
        }

        /// <summary>
        /// Get list of movies that are arriving to theatres in the next few weeks.
        /// (http://help.themoviedb.org/kb/api/upcoming-movies)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetUpcomingMovies(int page, string language, object UserState, Action<TmdbAsyncResult<TmdbUpcoming>> callback)
        {
            this.ProcessAsyncRequest<TmdbUpcoming>(BuildGetUpcomingMoviesRequest(page, language, UserState), callback);
        }

        /// <summary>
        /// Get list of movies that are arriving to theatres in the next few weeks.
        /// (http://help.themoviedb.org/kb/api/upcoming-movies)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetUpcomingMovies(int page, object UserState, Action<TmdbAsyncResult<TmdbUpcoming>> callback)
        {
            this.GetUpcomingMovies(page, this.Language, UserState, callback);
        }

        #endregion


        #region Person Info
        /// <summary>
        /// Get all of the basic information for a person.
        /// (http://help.themoviedb.org/kb/api/person-info)
        /// </summary>
        /// <param name="PersonID">Person ID</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetPersonInfo(int PersonID, object UserState, Action<TmdbAsyncResult<TmdbPerson>> callback)
        {
            this.ProcessAsyncRequest<TmdbPerson>(BuildGetPersonInfoRequest(PersonID, UserState), callback);
        }

        public void GetPersonInfoETag(int PersonID, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetPersonInfoRequest(PersonID, UserState), callback);
        }

        /// <summary>
        /// Get list of cast and crew information for a person.
        /// (http://help.themoviedb.org/kb/api/person-credits)
        /// </summary>
        /// <param name="PersonID">Person ID</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetPersonCredits(int PersonID, string language, object UserState, Action<TmdbAsyncResult<TmdbPersonCredits>> callback)
        {
            this.ProcessAsyncRequest<TmdbPersonCredits>(BuildGetPersonCreditsRequest(PersonID, language, UserState), callback);
        }

        public void GetPersonCreditsETag(int PersonID, string language, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetPersonCreditsRequest(PersonID, language, UserState), callback);
        }

        /// <summary>
        /// Get list of cast and crew information for a person.
        /// (http://help.themoviedb.org/kb/api/person-credits)
        /// </summary>
        /// <param name="PersonID">Person ID</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetPersonCredits(int PersonID, object UserState, Action<TmdbAsyncResult<TmdbPersonCredits>> callback)
        {
            this.GetPersonCredits(PersonID, this.Language, UserState, callback);
        }

        public void GetPersonCreditsETag(int PersonID, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.GetPersonCreditsETag(PersonID, this.Language, UserState, callback);
        }

        /// <summary>
        /// Get list of images for a person.
        /// (http://help.themoviedb.org/kb/api/person-images)
        /// </summary>
        /// <param name="PersonID">Person ID</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetPersonImages(int PersonID, object UserState, Action<TmdbAsyncResult<TmdbPersonImages>> callback)
        {
            this.ProcessAsyncRequest<TmdbPersonImages>(BuildGetPersonImagesRequest(PersonID, UserState), callback);
        }

        public void GetPersonImagesETag(int PersonID, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetPersonImagesRequest(PersonID, UserState), callback);
        }
        #endregion


        #region Miscellaneous Movie
        /// <summary>
        /// Get the newest movie added to the TMDB.
        /// (http://help.themoviedb.org/kb/api/latest-movie)
        /// </summary>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        //public void GetLatestMovie(object UserState, Action<TmdbAsyncResult<TmdbLatestMovie>> callback)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Get the list of movies currently in theatres.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/now-playing-movies)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetNowPlayingMovies(int page, string language, object UserState, Action<TmdbAsyncResult<TmdbNowPlaying>> callback)
        {
            this.ProcessAsyncRequest<TmdbNowPlaying>(BuildGetNowPlayingMoviesRequest(page, language, UserState), callback);
        }

        /// <summary>
        /// Get the list of movies currently in theatres.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/now-playing-movies)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetNowPlayingMovies(int page, object UserState, Action<TmdbAsyncResult<TmdbNowPlaying>> callback)
        {
            this.GetNowPlayingMovies(page, this.Language, UserState, callback);
        }

        /// <summary>
        /// Get the daily popularity list of movies.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/popular-movie-list)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetPopularMovies(int page, string language, object UserState, Action<TmdbAsyncResult<TmdbPopular>> callback)
        {
            this.ProcessAsyncRequest<TmdbPopular>(BuildGetPopularMoviesRequest(page, language, UserState), callback);
        }

        /// <summary>
        /// Get the daily popularity list of movies.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/popular-movie-list)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetPopularMovies(int page, object UserState, Action<TmdbAsyncResult<TmdbPopular>> callback)
        {
            this.GetPopularMovies(page, this.Language, UserState, callback);
        }

        /// <summary>
        /// Get list of movies that have over 10 votes on TMDB.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/top-rated-movies)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetTopRatedMovies(int page, string language, object UserState, Action<TmdbAsyncResult<TmdbTopRated>> callback)
        {
            this.ProcessAsyncRequest<TmdbTopRated>(BuildGetTopRatedMoviesRequest(page, language, UserState), callback);
        }

        /// <summary>
        /// Get list of movies that have over 10 votes on TMDB.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/top-rated-movies)
        /// </summary>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetTopRatedMovies(int page, object UserState, Action<TmdbAsyncResult<TmdbTopRated>> callback)
        {
            this.GetTopRatedMovies(page, this.Language, UserState, callback);
        }
        #endregion


        #region Company Info
        /// <summary>
        /// Get basic information about a production company from TMDB.
        /// (http://help.themoviedb.org/kb/api/company-info)
        /// </summary>
        /// <param name="CompanyID">Company ID</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetCompanyInfo(int CompanyID, object UserState, Action<TmdbAsyncResult<TmdbCompany>> callback)
        {
            this.ProcessAsyncRequest<TmdbCompany>(BuildGetCompanyInfoRequest(CompanyID, UserState), callback);
        }

        public void GetCompanyInfoETag(int CompanyID, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetCompanyInfoRequest(CompanyID, UserState), callback);
        }

        /// <summary>
        /// Get list of movies associated with a company.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/company-movies)
        /// </summary>
        /// <param name="CompanyID">Company ID</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetCompanyMovies(int CompanyID, int page, string language, object UserState, Action<TmdbAsyncResult<TmdbCompanyMovies>> callback)
        {
            this.ProcessAsyncRequest<TmdbCompanyMovies>(BuildGetCompanyMoviesRequest(CompanyID, page, language, UserState), callback);
        }

        public void GetCompanyMoviesETag(int CompanyID, int page, string language, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetCompanyMoviesRequest(CompanyID, page, language, UserState), callback);
        }

        /// <summary>
        /// Get list of movies associated with a company.  Response will contain 20 movies per page.
        /// (http://help.themoviedb.org/kb/api/company-movies)
        /// </summary>
        /// <param name="CompanyID">Company ID</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetCompanyMovies(int CompanyID, int page, object UserState, Action<TmdbAsyncResult<TmdbCompanyMovies>> callback)
        {
            this.GetCompanyMovies(CompanyID, page, this.Language, UserState, callback);
        }

        public void GetCompanyMoviesETag(int CompanyID, int page, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.GetCompanyMoviesETag(CompanyID, page, this.Language, UserState, callback);
        }
        #endregion


        #region Genre Info
        /// <summary>
        /// Get list of genres used in TMDB.  The ids will correspond to those found in movie calls.
        /// (http://help.themoviedb.org/kb/api/genre-list)
        /// </summary>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetGenreList(string language, object UserState, Action<TmdbAsyncResult<TmdbGenre>> callback)
        {
            this.ProcessAsyncRequest<TmdbGenre>(BuildGetGenreListRequest(language, UserState), callback);
        }

        public void GetGenreListETag(string language, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetGenreListRequest(language, UserState), callback);
        }

        /// <summary>
        /// Get list of genres used in TMDB.  The ids will correspond to those found in movie calls.
        /// (http://help.themoviedb.org/kb/api/genre-list)
        /// </summary>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetGenreList(object UserState, Action<TmdbAsyncResult<TmdbGenre>> callback)
        {
            this.GetGenreList(this.Language, UserState, callback);
        }

        public void GetGenreListETag(object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.GetGenreListETag(this.Language, UserState, callback);
        }

        /// <summary>
        /// Get list of movies in a Genre.  Note that only movies with more than 10 votes get listed.
        /// (http://help.themoviedb.org/kb/api/genre-movies)
        /// </summary>
        /// <param name="GenreID">TMDB Genre ID</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="language">optional - ISO 639-1 language code</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetGenreMovies(int GenreID, int page, string language, object UserState, Action<TmdbAsyncResult<TmdbGenreMovies>> callback)
        {
            this.ProcessAsyncRequest<TmdbGenreMovies>(BuildGetGenreMoviesRequest(GenreID, page, language, UserState), callback);
        }

        public void GetGenreMoviesETag(int GenreID, int page, string language, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.ProcessAsyncRequestETag(BuildGetGenreMoviesRequest(GenreID, page, language, UserState), callback);
        }

        /// <summary>
        /// Get list of movies in a Genre.  Note that only movies with more than 10 votes get listed.
        /// (http://help.themoviedb.org/kb/api/genre-movies)
        /// </summary>
        /// <param name="GenreID">TMDB Genre ID</param>
        /// <param name="page">Result page to retrieve (1 based)</param>
        /// <param name="UserState">User object to include in callback</param>
        /// <param name="callback"></param>
        public void GetGenreMovies(int GenreID, int page, object UserState, Action<TmdbAsyncResult<TmdbGenreMovies>> callback)
        {
            this.GetGenreMovies(GenreID, page, this.Language, UserState, callback);
        }

        public void GetGenreMoviesETag(int GenreID, int page, object UserState, Action<TmdbAsyncETagResult> callback)
        {
            this.GetGenreMoviesETag(GenreID, page, this.Language, UserState, callback);
        }
        #endregion
    }
}
