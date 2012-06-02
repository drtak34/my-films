namespace Grabber.TheMovieDbAPI
{
  using System;

  public class ImdbMovieInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        private TmdbMovie movie;
        public TmdbMovie Movie
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return this.movie;
            }
        }

        public ImdbMovieInfoCompletedEventArgs(
        TmdbMovie movie,
        Exception e,
        bool canceled,
        object state)
            : base(e, canceled, state)
        {
            this.movie = movie;
        }
    }
}
