using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PuzzleLeague.Utilities
{
   /// <summary>
   /// Helper class in order to contain all loaded content, and provide static access to it
   /// </summary>
   public static class ContentHelper
   {
      // The dictionary that is holding the string/texture combinations
      private static Dictionary<string,Texture2D> textures;

      // Static constructor
      static ContentHelper()
      {
         textures = new Dictionary<string, Texture2D>();
      }

      /// <summary>
      /// Method for adding a texture to the contenthelper
      /// </summary>
      /// <param name="name">The name of this texture (same as content name)</param>
      /// <param name="texture">The loaded texture (use Main.Content to load)</param>
      public static void AddTexture(string name, Texture2D texture)
      {
         if (!(textures.ContainsKey(name)))
            textures.Add(name, texture);
         else
            throw new Exception("Attempting to add a texture twice in ContentHelper");
      }

      /// <summary>
      /// Method to access a loaded texture from a given string name
      /// </summary>
      /// <param name="name">The name of this texture (same as content name)</param>
      /// <returns>The associated texture</returns>
      public static Texture2D GetTexture(string name)
      {
         if (textures.ContainsKey(name))
            return textures[name];
         else
            throw new IndexOutOfRangeException("Attempting to retrieve a texture is not loaded in ContentHelper");
      }
   }
}