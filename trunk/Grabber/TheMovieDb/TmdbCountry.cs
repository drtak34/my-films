namespace Grabber.TheMovieDb
{
  using System.Xml.Serialization;

  [XmlType("country")]
    public class TmdbCountry
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("code")]
        public string Code { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }
    }
}
