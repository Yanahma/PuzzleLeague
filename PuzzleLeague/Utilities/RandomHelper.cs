using System;

namespace PuzzleLeague.Utilities
{
   /// <summary>
   /// Helper class in order to provide static access to a single "Random" object
   /// </summary>
   public static class RandomHelper
   {
      // Private, static random object
      private static Random random;

      // Private, static constructor to initialize the random object
      static RandomHelper()
      {
         random = new Random();
      }

      /// <summary>
      /// Static access to "Next" function
      /// </summary>
      /// <returns>A random integer</returns>
      public static int Next() => random.Next();

      /// <summary>
      /// Static access to "Next" function with a maximum value
      /// </summary>
      /// <param name="max">The maximum value of the random integer</param>
      /// <returns>A random integer within the selected range</returns>
      public static int Next(int max) => random.Next(max);

      /// <summary>
      /// Static access to "Next" function with a min/max range
      /// </summary>
      /// <param name="min">The minimum value of the random integer</param>
      /// <param name="max">The maximum value of the random integer</param>
      /// <returns>A random integer within the selected range</returns>
      public static int Next(int min, int max) => random.Next(min, max);
   }
}
