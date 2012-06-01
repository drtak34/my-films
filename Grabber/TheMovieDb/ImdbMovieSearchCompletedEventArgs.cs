namespace Grabber.TheMovieDb
{
  using System;

  public class ImdbMovieSearchCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        private TmdbMovie[] movies;
        public TmdbMovie[] Movies
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return this.movies;
            }
        }

        public ImdbMovieSearchCompletedEventArgs(
        TmdbMovie[] movies,
        Exception e,
        bool canceled,
        object state)
            : base(e, canceled, state)
        {
            this.movies = movies;
        }
    }
}
