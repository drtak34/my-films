namespace Grabber.TheMovieDbAPI
{
  using System.Xml.Serialization;

  [XmlRoot("OpenSearchDescription")]
    public class TmdbMovieSearchResults
    {
        [XmlArray("movies")]
        public TmdbMovie[] Movies { get; set; }        
    }
}
