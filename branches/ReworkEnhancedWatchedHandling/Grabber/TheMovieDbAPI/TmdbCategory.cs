namespace Grabber.TheMovieDbAPI
{
  using System.Xml.Serialization;

  [XmlType("category")]
    public class TmdbCategory
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }
    }
}
