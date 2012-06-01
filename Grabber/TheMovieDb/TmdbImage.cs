namespace Grabber.TheMovieDb
{
  using System.Xml.Serialization;

  [XmlType("image")]
    public class TmdbImage
    {
        [XmlAttribute("type")]
        public TmdbImageType Type { get; set; }

        [XmlAttribute("size")]
        public TmdbImageSize Size { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }

        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
