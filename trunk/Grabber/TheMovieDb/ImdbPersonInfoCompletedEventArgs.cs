namespace Grabber.TheMovieDb
{
  using System;

  public class ImdbPersonInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        private TmdbPerson person;
        public TmdbPerson Person
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return this.person;
            }
        }

        public ImdbPersonInfoCompletedEventArgs(
        TmdbPerson person,
        Exception e,
        bool canceled,
        object state)
            : base(e, canceled, state)
        {
            this.person = person;
        }
    }
}
