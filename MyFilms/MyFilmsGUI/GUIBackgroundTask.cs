﻿using System;
using System.Threading;
using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;

namespace MyFilmsPlugin.MyFilms.MyFilmsGUI
{
  internal class GUIBackgroundTask
  {
    # region Singleton

    protected GUIBackgroundTask()
    {
      timeoutTimer.Elapsed += TaskWatcherTimerElapsed;
    }

    protected static GUIBackgroundTask instance = null;
    internal static GUIBackgroundTask Instance
    {
      get
      {
        return instance ?? (instance = new GUIBackgroundTask());
      }
    }

    #endregion

    private static NLog.Logger LogMyFilms = NLog.LogManager.GetCurrentClassLogger();  //log

    internal bool IsBusy { get; private set; }

    Action<bool, object> _CurrentResultHandler = null;
    object _CurrentResult = null;
    bool? _CurrentTaskSuccess = null;
    Exception _CurrentError = null;
    string _CurrentTaskDescription = null;
    Thread backgroundThread = null;
    bool abortedByUser = false;
    System.Timers.Timer timeoutTimer = new System.Timers.Timer(90000) { AutoReset = false };

    public void StopBackgroundTask()
    {
      StopBackgroundTask(true);
    }

    void StopBackgroundTask(bool byUserRequest)
    {
      if (IsBusy && _CurrentTaskSuccess == null && backgroundThread != null && backgroundThread.IsAlive)
      {
        LogMyFilms.Info("Aborting background thread: {0}", _CurrentTaskDescription);
        backgroundThread.Abort();
        abortedByUser = byUserRequest;
      }
    }

    void TaskWatcherTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      StopBackgroundTask(false);
    }

    /// <summary>
    /// This method should be used to call methods in GUI that might take a few seconds.
    /// The Wait Cursor will be shown while executing the task and the result handler will be called on MediaPortal's Main thread.
    /// </summary>
    /// <param name="task">method to invoke on a background thread</param>
    /// <param name="resultHandler">method to invoke on the GUI Thread with the result of the task</param>
    /// <param name="taskDescription">description of the task to be invoked - will be shown in the error message if execution fails or times out</param>
    /// <param name="timeout">true: use the timeout, or false: wait forever</param>
    /// <param name="searchanimation">Animation as waiting symbol</param>
    /// <returns>true, if the task could be successfully started in the background</returns>
    internal bool ExecuteInBackgroundAndCallback(Func<object> task, Action<bool, object> resultHandler, string taskDescription, bool timeout, GUIAnimation searchanimation)
    {
      // make sure only one background task can be executed at a time
      if (!IsBusy && Monitor.TryEnter(this))
      {
        try
        {
          IsBusy = true;
          abortedByUser = false;
          _CurrentResultHandler = resultHandler;
          _CurrentTaskDescription = taskDescription;
          _CurrentResult = null;
          _CurrentError = null;

          // while this is null the task has not finished (or later on timeout), 
          // true indicates successfull completion and false error
          _CurrentTaskSuccess = null;

          // init and show the wait cursor in MediaPortal
          // GUIWaitCursor.Init(); GUIWaitCursor.Show();
          MyFilmsDetail.SetProcessAnimationStatus(true, searchanimation);

          backgroundThread = new Thread(delegate()
          {
            try
            {
              _CurrentResult = task.Invoke();
              _CurrentTaskSuccess = true;
            }
            catch (ThreadAbortException)
            {
              if (!abortedByUser)
                LogMyFilms.Info("Timeout waiting for results.");

              Thread.ResetAbort();
            }
            catch (Exception threadException)
            {
              _CurrentError = threadException;
              LogMyFilms.Info(threadException.ToString());
              _CurrentTaskSuccess = false;
            }
            timeoutTimer.Stop();

            // hide the wait cursor
            // GUIWaitCursor.Hide();
            MyFilmsDetail.SetProcessAnimationStatus(false, searchanimation);

            // execute the ResultHandler on the Main Thread
            GUIWindowManager.SendThreadCallbackAndWait((p1, p2, o) =>
            {
              ExecuteTaskResultHandler();
              return 0;
            }, 0, 0, null);
          })
          {
            Name = "MyFilmsGUIBackgroundTask",
            IsBackground = true
          };

          // disable timeout when debugging
          if (timeout && !System.Diagnostics.Debugger.IsAttached)
            timeoutTimer.Start();

          // start background task
          backgroundThread.Start();

          // successfully started the background task
          return true;
        }
        catch (Exception ex)
        {
          LogMyFilms.Error(ex.Message);
          IsBusy = false;
          _CurrentResultHandler = null;

          // hide the wait cursor
          // GUIWaitCursor.Hide();
          MyFilmsDetail.SetProcessAnimationStatus(false, searchanimation);

          // could not start the background task
          return false;
        }
      }
      else
      {
        LogMyFilms.Info("Another thread tried to execute a task in background.");
        return false;
      }
    }

    void ExecuteTaskResultHandler()
    {
      if (!IsBusy) return;

      // show an error message if task was not completed successfully
      if (_CurrentTaskSuccess != true)
      {
        if (_CurrentError != null)
        {
          string lines = string.Format("{0} {1}\\n{2}", "Error", _CurrentTaskDescription, _CurrentError.Message);
          GUIUtils.ShowOKDialog(GUIUtils.PluginName(), lines);
        }
        else
        {
          if (!abortedByUser)
          {
            GUIUtils.ShowNotifyDialog(GUIUtils.PluginName(), _CurrentTaskSuccess.HasValue ? string.Format("{0} {1}", "Error", this._CurrentTaskDescription) : string.Format("{0} {1}", "Timeout", this._CurrentTaskDescription));
          }
        }
      }

      // store info needed to invoke the result handler
      bool storedTaskSuccess = _CurrentTaskSuccess == true;
      var storedHandler = _CurrentResultHandler;
      object storedResultObject = _CurrentResult;

      // clear all fields and allow execution of another background task 
      // before actually executing the result handler,
      // this way a result handler can also inovke another background task
      _CurrentResultHandler = null;
      _CurrentResult = null;
      _CurrentTaskSuccess = null;
      _CurrentError = null;
      backgroundThread = null;
      abortedByUser = false;
      IsBusy = false;
      timeoutTimer.Stop();
      Monitor.Exit(this);

      // execute the result handler
      if (storedHandler != null)
        storedHandler.Invoke(storedTaskSuccess, storedResultObject);
    }
  }
}