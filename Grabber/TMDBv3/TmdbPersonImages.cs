namespace Grabber.TMDBv3
{
  using System.Collections.Generic;

  public class PersonImageProfile
    {
        public string file_path { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public object iso_639_1 { get; set; }
        public double aspect_ratio { get; set; }
    }

    public class TmdbPersonImages
    {
        public int id { get; set; }
        public List<PersonImageProfile> profiles { get; set; }
    }
}
