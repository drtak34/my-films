namespace Grabber.TMDBv3
{
  using System.Collections.Generic;

  public class ImageConfiguration
    {
        public string base_url { get; set; }
        public List<string> poster_sizes { get; set; }
        public List<string> backdrop_sizes { get; set; }
        public List<string> profile_sizes { get; set; }
        public List<string> logo_sizes { get; set; }
    }

    public class TmdbConfiguration
    {
        public ImageConfiguration images { get; set; }
    }
}
