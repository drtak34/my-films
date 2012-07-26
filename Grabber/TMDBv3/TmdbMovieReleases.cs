namespace Grabber.TMDBv3
{
  using System.Collections.Generic;

  public class ReleaseCountry
    {
        public string iso_3166_1 { get; set; }
        public string certification { get; set; }
        public string release_date { get; set; }
    }

    public class TmdbMovieReleases
    {
        public int id { get; set; }
        public List<ReleaseCountry> countries { get; set; }
    }
}
