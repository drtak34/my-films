namespace MyFilmsPlugin.MyFilms.Trakt.User
{
  using System.Runtime.Serialization;

  [DataContract]
    public class TraktWatchedMovieHistory : TraktResponse
    {
        [DataMember(Name = "watched")]
        public string WatchedID { get; set; }

        [DataMember(Name = "movie")]
        public Movie Movie { get; set; }
    }

    [DataContract]
    public class Movie
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "year")]
        public string Year { get; set; }

        [DataMember(Name = "imdb_id")]
        public string IMDBID { get; set; }

        [DataMember(Name = "tmdb_id")]
        public string TMDBID { get; set; }
    }
}