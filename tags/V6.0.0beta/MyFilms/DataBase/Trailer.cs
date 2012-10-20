namespace MyFilmsPlugin.MyFilms
{
  using System;

  [Serializable]
  public class Trailer
  {
    public string MovieTitle { get; set; }
    public string Trailername { get; set; }
    public string DestinationDirectory { get; set; }
    public string OriginalUrl { get; set; }
    public string SourceUrl { get; set; }
    public string Quality { get; set; }
  }
}