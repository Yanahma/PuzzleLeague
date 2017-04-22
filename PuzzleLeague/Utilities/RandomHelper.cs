using System;

namespace PuzzleLeague.Utilities
{
   /// <summary>
   /// Helper class in order to provide static access to a single "Random" object
   /// </summary>
   public static class RandomHelper
   {
      /// <summary>
      /// Private, static Random object
      /// </summary>
      private static Random rng;
      
      /// <summary>
      /// Private constructor to init Random object
      /// </summary>
      static RandomHelper()
      {
         rng = new Random();
      }

      /// <summary>
      /// Static access to "Next" function
      /// </summary>
      /// <returns>A random integer</returns>
      public static int Next() => rng.Next();

      /// <summary>
      /// Static access to "Next" function with a maximum value
      /// </summary>
      /// <param name="max">The maximum value of the random integer</param>
      /// <returns>A random integer within the selected range</returns>
      public static int Next(int max) => rng.Next(max);

      /// <summary>
      /// Static access to "Next" function with a min/max range
      /// </summary>
      /// <param name="min">The minimum value of the random integer</param>
      /// <param name="max">The maximum value of the random integer</param>
      /// <returns>A random integer within the selected range</returns>
      public static int Next(int min, int max) => rng.Next(min, max);
   }
}
