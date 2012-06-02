namespace Grabber.TheMovieDbAPI
{
  using System.Xml.Serialization;

  [XmlType("studio")]
    public class TmdbStudio
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }
    }
}
