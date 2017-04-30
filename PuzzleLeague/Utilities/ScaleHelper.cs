using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PuzzleLeague.Utilities
{
   public static class ScaleHelper
   {
      //
      // Private fields
      //

      // Stores the back buffer height of the game window
      private static int backBufferHeight;

      // Stores the back buffer width of the game window
      private static int backBufferWidth;

      // Stores the const height of the "true" scale of the game
      private const int PreferredHeight = 720;

      // Stores the const width of the "true" scale of the game
      private const int PreferredWidth = 1280;

      // The height scale used to determine scaled rectangles/positions
      private static float HeightScale
      {
         get { return (float)backBufferHeight / PreferredHeight; }
      }

      // The width scale used to determine scaled rectangles/positions
      private static float WidthScale
      {
         get { return (float)backBufferWidth / PreferredWidth; }
      }

      //
      // Public properties
      //
      
      // Static access to the current backbufferheight
      public static int BackBufferHeight
      {
         get { return backBufferHeight; }
      }

      // Static access to the current backbufferWidth
      public static int BackBufferWidth
      {
         get { return backBufferWidth; }
      }

      //
      // Static functions
      //

      // Used to set the values used when scaling graphics (should be done AFTER applying changes to game window)
      public static void UpdateBufferValues(int newBufferHeight, int newBufferWidth)
      {
         backBufferHeight = newBufferHeight;
         backBufferWidth = newBufferWidth;
      }

      // Used to scale a point
      public static Point ScalePoint(Point point)
      {
         return new Point(
            (int)Math.Ceiling(point.X * WidthScale),
            (int)Math.Ceiling(point.Y * HeightScale)
            );
      }

      // Used to scale a width
      public static int ScaleWidth(int width)
      {
         return (int)Math.Ceiling(width * WidthScale);
      }

      // Used to scale a height
      public static int ScaleHeight(int height)
      {
         return (int)Math.Ceiling(height * HeightScale);
      }
   }
}
