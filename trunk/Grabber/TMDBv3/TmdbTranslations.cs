namespace Grabber.TMDBv3
{
  using System.Collections.Generic;

  public class Translation
    {
        public string iso_639_1 { get; set; }
        public string name { get; set; }
        public string english_name { get; set; }
    }

    public class TmdbTranslations
    {
        public int id { get; set; }
        public List<Translation> translations { get; set; }
    }
}
