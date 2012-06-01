namespace Grabber.TheMovieDb
{
  using System.Xml.Serialization;

  [XmlType("also_known_as")]
    public class TmdbAlsoKnownAs
    {
        [XmlElement("name")]
        public string[] Names { get; set; }
    }
}
