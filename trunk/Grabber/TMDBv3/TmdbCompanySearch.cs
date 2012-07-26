namespace Grabber.TMDBv3
{
  using System.Collections.Generic;

  public class CompanyResult
    {
        public int id { get; set; }
        public string logo_path { get; set; }
        public string name { get; set; }
    }

    public class TmdbCompanySearch
    {
        public int page { get; set; }
        public List<CompanyResult> results { get; set; }
        public int total_pages { get; set; }
        public int total_results { get; set; }
    }
}
