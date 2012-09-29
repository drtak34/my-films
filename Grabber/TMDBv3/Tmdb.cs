using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp.Deserializers;
using System.Net;
using RestSharp;

namespace WatTmdb.V3
{
  using Grabber.TMDBv3.Utility;

  public partial class Tmdb
    {
        private const string BASE_URL = "http://api.themoviedb.org/3";

        public string ApiKey { get; set; }
        public string Language { get; set; }
        private JsonDeserializer jsonDeserializer = new JsonDeserializer();

        /// <summary>
        /// Current count of outstanding Async calls awaiting callback response
        /// </summary>
        public int AsyncCount = 0;

        public TmdbError Error { get; set; }

        /// <summary>
        /// String representation of response content
        /// </summary>
        public string ResponseContent { get; set; }

        /// <summary>
        /// Dictionary of Header values in response
        /// http://help.themoviedb.org/kb/api/content-versioning
        /// </summary>
        public Dictionary<string, object> ResponseHeaders { get; set; }

        /// <summary>
        /// ETag pulled out of the response header for the last request
        /// http://help.themoviedb.org/kb/api/content-versioning
        /// </summary>
        public string ResponseETag
        {
            get
            {
                if (ResponseHeaders == null || ResponseHeaders.ContainsKey("ETag") == false)
                    return null;

                return Convert.ToString(ResponseHeaders["ETag"]).Trim('"');
            }
        }

#if !WINDOWS_PHONE
        /// <summary>
        /// Proxy to use for requests made.  Passed on to underying WebRequest if set.
        /// </summary>
        public IWebProxy Proxy { get; set; }
#endif
        /// <summary>
        /// Timeout in milliseconds to use for requests made.
        /// </summary>
        public int? Timeout { get; set; }

        public Tmdb(string apiKey)
        {
            Error = null;
            ApiKey = apiKey;
            Language = null;
            Timeout = null;
        }

        public Tmdb(string apiKey, string language)
        {
            Error = null;
            ApiKey = apiKey;
            Language = language;
            Timeout = null;
        }


        #region Build Requests


        #region Configuration Methods

    private static RestRequest BuildGetConfigurationRequest()
    {
      return BuildGetConfigurationRequest(null);
    }

    private static RestRequest BuildGetConfigurationRequest(object userState)
        {
            var request = new RestRequest("configuration", Method.GET);
            if (userState != null)
                request.UserState = userState;

            return request;
        }
        #endregion


        #region Search Methods

    private static RestRequest BuildSearchMovieRequest(string query, int page, string language, bool? includeAdult, int? year)
    {
      return BuildSearchMovieRequest(query, page, language, includeAdult, year, null);
    }

    private static RestRequest BuildSearchMovieRequest(string query, int page, string language, bool? includeAdult, int? year, object userState)
        {
            var request = new RestRequest("search/movie", Method.GET);
            request.AddParameter("query", query.EscapeString());
            request.AddParameter("page", page);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            if (includeAdult.HasValue)
                request.AddParameter("include_adult", includeAdult.Value ? "true" : "false");
            if (year.HasValue)
                request.AddParameter("year", year.Value);
            if (userState != null)
                request.UserState = userState;

            return request;
        }

    private static RestRequest BuildSearchPersonRequest(string query, int page, string language)
    {
      return BuildSearchPersonRequest(query, page, language, null);
    }

    private static RestRequest BuildSearchPersonRequest(string query, int page, string language, object userState)
        {
            var request = new RestRequest("search/person", Method.GET);
            request.AddParameter("query", query.EscapeString());
            request.AddParameter("page", page);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            if (userState != null)
                request.UserState = userState;

            return request;
        }

    private static RestRequest BuildSearchCompanyRequest(string query, int page)
    {
      return BuildSearchCompanyRequest(query, page, null);
    }

    private static RestRequest BuildSearchCompanyRequest(string query, int page, object userState)
        {
            var request = new RestRequest("search/company", Method.GET);
            request.AddParameter("query", query.EscapeString());
            request.AddParameter("page", page);
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }
        #endregion


        #region Collections

    private static RestRequest BuildGetCollectionInfoRequest(int CollectionID, string language)
    {
      return BuildGetCollectionInfoRequest(CollectionID, language, null);
    }

    private static RestRequest BuildGetCollectionInfoRequest(int CollectionID, string language, object userState)
        {
            var request = new RestRequest("collection/{id}", Method.GET);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddUrlSegment("id", CollectionID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

    private static RestRequest BuildGetCollectionImagesRequest(int CollectionID, string language)
    {
      return BuildGetCollectionImagesRequest(CollectionID, language, null);
    }

    private static RestRequest BuildGetCollectionImagesRequest(int CollectionID, string language, object userState)
        {
            var request = new RestRequest("collection/{id}/images", Method.GET);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddUrlSegment("id", CollectionID.ToString());
            if (userState != null)
                request.UserState = userState;

            return request;
        }
        #endregion


        #region Movie Info

    private static RestRequest BuildGetMovieInfoRequest(int MovieID, string language)
    {
      return BuildGetMovieInfoRequest(MovieID, language, null);
    }

    private static RestRequest BuildGetMovieInfoRequest(int MovieID, string language, object userState)
        {
            var request = new RestRequest("movie/{id}", Method.GET);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddUrlSegment("id", MovieID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

    private static RestRequest BuildGetMovieByIMDBRequest(string IMDB_ID, string language)
    {
      return BuildGetMovieByIMDBRequest(IMDB_ID, language, null);
    }

    private static RestRequest BuildGetMovieByIMDBRequest(string IMDB_ID, string language, object userState)
        {
            var request = new RestRequest("movie/{id}", Method.GET);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddUrlSegment("id", IMDB_ID.EscapeString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

    private static RestRequest BuildGetMovieAlternateTitlesRequest(int MovieID, string Country)
    {
      return BuildGetMovieAlternateTitlesRequest(MovieID, Country, null);
    }

    private static RestRequest BuildGetMovieAlternateTitlesRequest(int MovieID, string Country, object userState)
        {
            var request = new RestRequest("movie/{id}/alternative_titles", Method.GET);
            if (string.IsNullOrEmpty(Country) == false)
                request.AddParameter("country", Country);
            request.AddUrlSegment("id", MovieID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

    private static RestRequest BuildGetMovieCastRequest(int MovieID)
    {
      return BuildGetMovieCastRequest(MovieID, null);
    }

    private static RestRequest BuildGetMovieCastRequest(int MovieID, object userState)
        {
            var request = new RestRequest("movie/{id}/casts", Method.GET);
            request.AddUrlSegment("id", MovieID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

    private static RestRequest BuildGetMovieImagesRequest(int MovieID, string language)
    {
      return BuildGetMovieImagesRequest(MovieID, language, null);
    }

    private static RestRequest BuildGetMovieImagesRequest(int MovieID, string language, object userState)
        {
            var request = new RestRequest("movie/{id}/images", Method.GET);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddUrlSegment("id", MovieID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

    private static RestRequest BuildGetMovieKeywordsRequest(int MovieID)
    {
      return BuildGetMovieKeywordsRequest(MovieID, null);
    }

    private static RestRequest BuildGetMovieKeywordsRequest(int MovieID, object userState)
        {
            var request = new RestRequest("movie/{id}/keywords", Method.GET);
            request.AddUrlSegment("id", MovieID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

    private static RestRequest BuildGetMovieReleasesRequest(int MovieID)
    {
      return BuildGetMovieReleasesRequest(MovieID, null);
    }

    private static RestRequest BuildGetMovieReleasesRequest(int MovieID, object userState)
        {
            var request = new RestRequest("movie/{id}/releases", Method.GET);
            request.AddUrlSegment("id", MovieID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

    private static RestRequest BuildGetMovieTrailersRequest(int MovieID, string language)
    {
      return BuildGetMovieTrailersRequest(MovieID, language, null);
    }

    private static RestRequest BuildGetMovieTrailersRequest(int MovieID, string language, object userState)
        {
            var request = new RestRequest("movie/{id}/trailers", Method.GET);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddUrlSegment("id", MovieID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

    private static RestRequest BuildGetSimilarMoviesRequest(int MovieID, int page, string language)
    {
      return BuildGetSimilarMoviesRequest(MovieID, page, language, null);
    }

    private static RestRequest BuildGetSimilarMoviesRequest(int MovieID, int page, string language, object userState)
        {
            var request = new RestRequest("movie/{id}/similar_movies", Method.GET);
            request.AddParameter("page", page);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddUrlSegment("id", MovieID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

    private static RestRequest BuildGetMovieTranslationsRequest(int MovieID)
    {
      return BuildGetMovieTranslationsRequest(MovieID, null);
    }

    private static RestRequest BuildGetMovieTranslationsRequest(int MovieID, object userState)
        {
            var request = new RestRequest("movie/{id}/translations", Method.GET);
            request.AddUrlSegment("id", MovieID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }
        #endregion


        #region Person Info

    private static RestRequest BuildGetPersonInfoRequest(int PersonID)
    {
      return BuildGetPersonInfoRequest(PersonID, null);
    }

    private static RestRequest BuildGetPersonInfoRequest(int PersonID, object userState)
        {
            var request = new RestRequest("person/{id}", Method.GET);
            request.AddUrlSegment("id", PersonID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

    private static RestRequest BuildGetPersonCreditsRequest(int PersonID, string language)
    {
      return BuildGetPersonCreditsRequest(PersonID, language, null);
    }

    private static RestRequest BuildGetPersonCreditsRequest(int PersonID, string language, object userState)
        {
            var request = new RestRequest("person/{id}/credits", Method.GET);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddUrlSegment("id", PersonID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

      private static RestRequest BuildGetPersonImagesRequest(int PersonID)
      {
        return BuildGetPersonImagesRequest(PersonID, null);
      }

      private static RestRequest BuildGetPersonImagesRequest(int PersonID, object userState)
        {
            var request = new RestRequest("person/{id}/images", Method.GET);
            request.AddUrlSegment("id", PersonID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }
        #endregion


        #region Miscellaneous Movie

    private static RestRequest BuildGetLatestMovieRequest()
    {
      return BuildGetLatestMovieRequest(null);
    }

    private static RestRequest BuildGetLatestMovieRequest(object userState)
        {
            var request = new RestRequest("movie/latest", Method.GET);
            if (userState != null)
                request.UserState = userState;

            return request;
        }

      private static RestRequest BuildGetNowPlayingMoviesRequest(int page, string language)
      {
        return BuildGetNowPlayingMoviesRequest(page, language, null);
      }

      private static RestRequest BuildGetNowPlayingMoviesRequest(int page, string language, object userState)
        {
            var request = new RestRequest("movie/now_playing", Method.GET);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddParameter("page", page);
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

      private static RestRequest BuildGetPopularMoviesRequest(int page, string language)
      {
        return BuildGetPopularMoviesRequest(page, language, null);
      }

      private static RestRequest BuildGetPopularMoviesRequest(int page, string language, object userState)
        {
            var request = new RestRequest("movie/popular", Method.GET);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddParameter("page", page);
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

      private static RestRequest BuildGetTopRatedMoviesRequest(int page, string language)
      {
        return BuildGetTopRatedMoviesRequest(page, language, null);
      }

      private static RestRequest BuildGetTopRatedMoviesRequest(int page, string language, object userState)
        {
            var request = new RestRequest("movie/top_rated", Method.GET);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddParameter("page", page);
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

      private static RestRequest BuildGetUpcomingMoviesRequest(int page, string language)
      {
        return BuildGetUpcomingMoviesRequest(page, language, null);
      }

      private static RestRequest BuildGetUpcomingMoviesRequest(int page, string language, object userState)
        {
            var request = new RestRequest("movie/upcoming", Method.GET);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddParameter("page", page);
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }
        #endregion


        #region Company Info

      private static RestRequest BuildGetCompanyInfoRequest(int CompanyID)
      {
        return BuildGetCompanyInfoRequest(CompanyID, null);
      }

      private static RestRequest BuildGetCompanyInfoRequest(int CompanyID, object userState)
        {
            var request = new RestRequest("company/{id}", Method.GET);
            request.AddUrlSegment("id", CompanyID.ToString());
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

      private static RestRequest BuildGetCompanyMoviesRequest(int CompanyID, int page, string language)
      {
        return BuildGetCompanyMoviesRequest(CompanyID, page, language, null);
      }

      private static RestRequest BuildGetCompanyMoviesRequest(int CompanyID, int page, string language, object userState)
        {
            var request = new RestRequest("company/{id}/movies", Method.GET);
            request.AddUrlSegment("id", CompanyID.ToString());
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddParameter("page", page);
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }
        #endregion


        #region Genre Info

      private static RestRequest BuildGetGenreListRequest(string language)
      {
        return BuildGetGenreListRequest(language, null);
      }

      private static RestRequest BuildGetGenreListRequest(string language, object userState)
        {
            var request = new RestRequest("genre/list", Method.GET);
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }

      private static RestRequest BuildGetGenreMoviesRequest(int GenreID, int page, string language)
      {
        return BuildGetGenreMoviesRequest(GenreID, page, language, null);
      }

      private static RestRequest BuildGetGenreMoviesRequest(int GenreID, int page, string language, object userState)
        {
            var request = new RestRequest("genre/{id}/movies", Method.GET);
            request.AddUrlSegment("id", GenreID.ToString());
            if (string.IsNullOrEmpty(language) == false)
                request.AddParameter("language", language);
            request.AddParameter("page", page);
            if (userState != null)
                request.UserState = userState;
            
            return request;
        }
        #endregion

        #endregion
    }

}
