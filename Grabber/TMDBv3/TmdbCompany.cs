namespace Grabber.TMDBv3
{
  public class ParentCompany
    {
        public string name { get; set; }
        public int id { get; set; }
        public string logo_path { get; set; }
    }

    public class TmdbCompany
    {
        public object description { get; set; }
        public object headquarters { get; set; }
        public object homepage { get; set; }
        public int id { get; set; }
        public string logo_path { get; set; }
        public string name { get; set; }
        public ParentCompany parent_company { get; set; }
    }
}
