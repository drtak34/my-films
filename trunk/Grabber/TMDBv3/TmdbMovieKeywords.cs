namespace Grabber.TMDBv3
{
  using System.Collections.Generic;

  public class MovieKeyword
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class TmdbMovieKeywords
    {
        public int id { get; set; }
        public List<MovieKeyword> keywords { get; set; }
    }
}
