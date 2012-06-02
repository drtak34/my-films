namespace Grabber.TheMovieDbAPI
{
  using System;

  public class ImdbPersonSearchCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        private TmdbPerson[] people;
        public TmdbPerson[] People
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return this.people;
            }
        }

        public ImdbPersonSearchCompletedEventArgs(
        TmdbPerson[] people,
        Exception e,
        bool canceled,
        object state)
            : base(e, canceled, state)
        {
            this.people = people;
        }
    }
}
