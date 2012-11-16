namespace Grabber.TheMovieDbAPI
{
  using System;
  using System.Xml.Serialization;

  [XmlType("person")]
    public class TmdbPerson
    {
        [XmlElement("score")]
        public decimal Score { get; set; }

        [XmlElement("popularity")]
        public decimal Popularity { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("also_known_as")]
        public TmdbAlsoKnownAs AlsoKnownAs { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement("id")]
        public int Id { get; set; }

        [XmlElement("known_movies")]
        public int KnownMovies { get; set; }

        [XmlElement("birthday")]
        public string BirthdayString { get; set; }

        public DateTime? Birthday
        {
            get
            {
                DateTime d;
                if (string.IsNullOrEmpty(this.BirthdayString) || !DateTime.TryParse(this.BirthdayString, out d))
                    return null;
                else
                    return d;
            }
        }

        [XmlElement("birthplace")]
        public string Birthplace { get; set; }

        [XmlArray("images")]
        public TmdbImage[] Images { get; set; }

        [XmlArray("filmography")]
        public TmdbPersonFilm[] Filmography { get; set; }
    }
}
