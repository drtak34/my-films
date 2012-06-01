namespace Grabber.TheMovieDb
{
  using System.Xml.Serialization;

  [XmlType("person")]
    public class TmdbCastPerson
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
