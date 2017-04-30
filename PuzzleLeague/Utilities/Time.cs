using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleLeague
{
   /// <summary>
   /// Static class to provide global access to useful time related fields
   /// </summary>
   public static class Time
   {
      // The total amount of time that has elapsed since the game started
      public static float ElapsedTime { get; private set; }

      // The time elapsed in the previous frame
      public static float DeltaTime { get; private set; }

      // The total number of frames since the game started
      public static float FrameCount { get; private set; }

      // Update (called once per frame in Game1.cs)
      public static void Update(float dt)
      {
         DeltaTime = dt;
         ElapsedTime += dt;
         FrameCount++;
      }

   }
}
