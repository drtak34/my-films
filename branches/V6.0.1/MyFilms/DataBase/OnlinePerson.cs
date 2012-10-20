namespace MyFilmsPlugin.DataBase
{
  using MediaPortal.Video.Database;

  using WatTmdb.V3;

  public class OnlinePerson
  {
     public IMDBActor ImdbActor { get; set; }
     public TmdbPerson Person { get; set; }
     public TmdbPersonImages PersonImages { get; set; }
     public TmdbPersonCredits PersonCredits { get; set; }
  }
}