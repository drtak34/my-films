namespace Grabber.TheMovieDbAPI
{
  using System.Xml.Serialization;

  [XmlType("movie")]
    public class TmdbPersonFilm
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("job")]
        public string Job { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("character")]
        public string Character { get; set; }

        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
