using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleLeague.Utilities
{
   /// <summary>
   /// Simple class to create & update Timers, and trigger events once they run down
   /// </summary>
   public class Timer
   {
      // Enum to determine if a timer is stopped or started
      private enum TimerState
      {
         Started,
         Stopped,
      }

      // Static "master list" of all the timers created
      private static List<Timer> timerList = new List<Timer>();

      // Determines if this is a "continuous" timer
      private bool isLooping;

      // The current state of the timer
      private TimerState state = TimerState.Stopped;
      
      // Property to contain set code for looping timers
      private TimerState State
      {
         get { return state; }
         set
         {
            if(value == TimerState.Stopped 
               && this.timeRemaining == 0
               && this.isLooping)
            {
               // Don't actually stop a looping timer when time remaining reaches 0
               timeRemaining = prevTimeRemaining;
            }
            else
            {
               this.state = value;
            }
         }
      }

      // Time remaining (ticks down every frame, triggers OnComplete event once 0)
      private float timeRemaining = 0;

      // Property to contain set code of timeRemaining for triggering OnComplete
      private float TimeRemaining
      {
         get { return timeRemaining; }
         set
         {
            timeRemaining = value;
            if (timeRemaining <= 0)
            {
               timeRemaining = 0;
               OnComplete(this, EventArgs.Empty);
               this.State = TimerState.Stopped;
            }
         }
      }

      // The last "timeRemaining" set on this timer - used for looping timers
      private float prevTimeRemaining;

      // Public event used to trigger events once the timer has been completed
      public event EventHandler OnComplete;

      //
      // Constructor
      //
      public Timer(bool isLooping = false)
      {
         this.isLooping = isLooping;
      }

      // Public method to set the time for the timer
      public void SetTimer(float time)
      {
         timeRemaining = time;
         prevTimeRemaining = timeRemaining;      
      }

      // Public method to start the timer
      public void StartTimer()
      {
         State = TimerState.Started;
         if (!(timerList.Contains(this)))
            timerList.Add(this);
      }

      // Public method to reset the timer
      public void ResetTimer()
      {
         timeRemaining = prevTimeRemaining;
      }

      // Public method to stop the timer
      public void StopTimer()
      {
         State = TimerState.Stopped;
         if(timerList.Contains(this))
            timerList.Remove(this);
      }

      // Update method to tick down all of the active timers
      public static void Update()
      {
         for(var i = timerList.Count - 1; i >= 0; i--)
         {
            timerList[i].TimeRemaining -= Time.DeltaTime;
         }
      }
   }
}
