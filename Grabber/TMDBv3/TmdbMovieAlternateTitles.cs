namespace Grabber.TMDBv3
{
  using System.Collections.Generic;

  public class AlternateTitle
    {
        public string iso_3166_1 { get; set; }
        public string title { get; set; }
    }

    public class TmdbMovieAlternateTitles
    {
        public int id { get; set; }
        public List<AlternateTitle> titles { get; set; }
    }
}
