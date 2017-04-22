using System;
using System.Collections.Generic;
using PuzzleLeague.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PuzzleLeague
{
   /// <summary>
   /// Public enum to define the 5 different types of block (+ Empty)
   /// </summary>
   public enum BlockType
   {
      Empty = 0,
      Red,
      Pink,
      Yellow,
      Green,
      Blue 
   }

   /// <summary>
   /// Class that describes information associated with a single block on the GameBoard
   /// </summary>
   class Block
   {
      // Defines which type of Block this is (cannot change)
      private readonly BlockType type;

      // Defines the X,Y position of this Block
      private Vector2 position;

      // Defines who this Block belongs to (used for positioning)
      private Row parent;

      // Get-only access to the tile texture (based on type)
      private Texture2D TileTexture
      {
         get
         {
            switch (type)
            {
               case BlockType.Blue:
                  return ContentHelper.GetTexture("tileBlue_27");
               case BlockType.Green:
                  return ContentHelper.GetTexture("tileGreen_27");
               case BlockType.Red:
                  return ContentHelper.GetTexture("tileRed_27");
               case BlockType.Yellow:
                  return ContentHelper.GetTexture("tileYellow_27");
               case BlockType.Pink:
                  return ContentHelper.GetTexture("tilePink_27");
            }
            return null;
         }
      }

      // Get-only access to the symbol texture (based on type)
      private Texture2D SymbolTexture
      {
         get
         {
            switch (type)
            {
               case BlockType.Blue:
                  return ContentHelper.GetTexture("tileBlue_31");
               case BlockType.Green:
                  return ContentHelper.GetTexture("tileGreen_35");
               case BlockType.Red:
                  return ContentHelper.GetTexture("tileRed_36");
               case BlockType.Yellow:
                  return ContentHelper.GetTexture("tileYellow_33");
               case BlockType.Pink:
                  return ContentHelper.GetTexture("tilePink_30");
            }
            return null;
         }
      }

      // Public accessor for 'type' field 
      public BlockType Type { get { return type; } }

      //
      // Constructor
      //
      public Block(Row parent, BlockType type = BlockType.Empty)
      {
         // Assign parent
         this.parent = parent;

         // Assign block type
         this.type = type;

         // Set default position based on parent
         position.Y = parent.Position.Y;
         position.X = parent.Position.X + (parent.IndexOfBlock(this)) * 54;
      }

      /// <summary>
      /// Create a random block of any type, excluding empty
      /// </summary>
      /// <param name="parent">The parent row this block belongs to</param>
      /// <returns>A new block object of a random type (not empty)</returns>
      static Random rng = new Random();
      public static Block RndBlockExcEmpty(Row parent)
      {
         // Get a random block type from the enum
         //BlockType rndType = (BlockType)(Enum.GetValues(typeof(BlockType))).GetValue(rng.Next(1,6));
         BlockType rndType = (BlockType)rng.Next(1,6);
         return new Block(parent, rndType);
      }

      /// <summary>
      /// Create a random block of any type, including empty
      /// </summary>
      /// <param name="parent">The parent row this block belongs to</param>
      /// <returns>A new block object of a random type</returns>
      public static Block RndBlockIncEmpty(Row parent)
      {
         // Get a random block type from the enum
         BlockType rndType = (BlockType)(Enum.GetValues(typeof(BlockType)).GetValue(RandomHelper.Next(0,6)));
         return new Block(parent, rndType);
      }

      #region Game Loop lifecycle
      public void Update()
      {
         if (parent != null)
         {
            position.Y = parent.Position.Y;
            position.X = position.X = parent.Position.X + (parent.IndexOfBlock(this)) * 54;
         }                        
      }

      public void Draw(SpriteBatch spriteBatch)
      {
         if (type != BlockType.Empty)
         {
            spriteBatch.Draw(TileTexture, position, Color.White);
            spriteBatch.Draw(SymbolTexture, new Vector2(position.X + 14, position.Y + 14), Color.White);
         }    
      }
      #endregion
   }
}
