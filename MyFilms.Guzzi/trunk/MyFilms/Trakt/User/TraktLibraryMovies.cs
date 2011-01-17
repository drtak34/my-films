namespace MyFilmsPlugin.MyFilms.Trakt.User
{
  using System.Runtime.Serialization;

  [DataContract]
    public class TraktLibraryMovies : TraktResponse
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "year")]
        public string Year { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "imdb_id")]
        public string IMDBID { get; set; }

        [DataMember(Name = "tmdb_id")]
        public string TMDBID { get; set; }

        [DataMember(Name = "watched")]
        public bool Watched { get; set; }
    }
}
