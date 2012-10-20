namespace Grabber.TheMovieDbAPI
{
  using System;
  using System.Xml.Serialization;

  [XmlType("movie")]
    public class TmdbMovie
    {
        [XmlElement("popularity")]
        public decimal Popularity { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("alternative_name")]
        public string AlternativeName { get; set; }

        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("id")]
        public int Id { get; set; }

        [XmlElement("imdb_id")]
        public string ImdbId { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement("overview")]
        public string Overview { get; set; }

        [XmlElement("rating")]
        public decimal Rating { get; set; }

        [XmlElement("released")]
        public string ReleasedString { get; set; }

        public DateTime? Released
        {
            get
            {
                DateTime d;
                if (string.IsNullOrEmpty(this.ReleasedString) || !DateTime.TryParse(this.ReleasedString, out d))
                    return null;
                else
                    return d;
            }
        }

        [XmlElement("runtime")]
        public int Runtime { get; set; }

        [XmlElement("budget")]
        public decimal Budget { get; set; }

        [XmlElement("revenue")]
        public decimal Revenue { get; set; }

        [XmlElement("homepage")]
        public string Homepage { get; set; }

        [XmlElement("trailer")]
        public string Trailer { get; set; }

        [XmlArray("categories")]
        public TmdbCategory[] Categories { get; set; }

        [XmlArray("studios")]
        public TmdbStudio[] Studios { get; set; }

        [XmlArray("countries")]
        public TmdbCountry[] Countries { get; set; }

        [XmlArray("images")]
        public TmdbImage[] Images { get; set; }

        [XmlArray("cast")]
        public TmdbCastPerson[] Cast { get; set; }
    }
}
