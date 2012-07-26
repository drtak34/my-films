namespace Grabber.TMDBv3
{
  public class TmdbAsyncResult<T>
    {
        public T Data { get; set; }
        public TmdbError Error { get; set; }
        public object UserState { get; set; }
    }

    public class TmdbAsyncETagResult
    {
        public string ETag { get; set; }
        public object UserState { get; set; }
    }
}
