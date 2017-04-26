using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleLeague
{
   public static class Time
   {
      // Holds the total amount of time that has elapsed since the game started
      private static float elapsedTime;

      // Holds the total number of frames since the game started 
      private static float frameCount;

      // Holds the deltaTime (time elapsed in the previous frame)
      private static float deltaTime;

      // Public access to elapsedTime
      public static float ElapsedTime
      {
         get { return elapsedTime; }
      }

      // Public access to deltaTime
      public static float DeltaTime
      {
         get { return deltaTime; }
      }

      // Public access to frameCount
      public static float FrameCount
      {
         get { return frameCount; }
      }

      // Update (called once per frame in Game1.cs)
      public static void Update(float dt)
      {
         // Add to elapsedTime
         elapsedTime += dt;
         // Update deltaTime
         deltaTime = dt;
         // Add to frameCount
         frameCount++;
      }

   }
}
