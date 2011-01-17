namespace MyFilmsPlugin.MyFilms.Trakt
{
  using System.Runtime.Serialization;

  [DataContract]
    public class TraktResponse
    {
        [DataMember(Name = "status")]
        public string Status { get; set;}

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "error")]
        public string Error { get; set; }
    }
}
