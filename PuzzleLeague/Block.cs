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
      //
      // Private Fields
      //

      // Defines the alpha of the drawColor (used to disappear)
      private int alpha = 255;

      // Defines the color to draw (start at white)
      private Color drawColor = Color.White;

      // Defines whether this block has been matched
      private bool isMatched = false;

      // Defines the X,Y position of this Block
      private Vector2 position;

      // Defines who this Block belongs to (used for positioning)
      private Row parent;

      // Defines the current "state" of the block
      private BlockState state;

      // Defines which type of Block this is (cannot change)
      private readonly BlockType type;

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

      //
      // Private enums
      //

      /// <summary>
      /// Private enum to define the different "states" a block can be in
      /// </summary>
      public enum BlockState
      {
         None,
         Falling,
         Blinking,
         Disappearing,
         Removed
      }

      //
      // Public Properties
      //

      // Public accessor for isMatched field
      public bool IsMatched
      {
         get { return isMatched; }
         set
         {
            if(value != isMatched && value == true)
            {
               isMatched = value;
               state = BlockState.Blinking;
            }
         }
      }

      // Public accessor for 'type' field 
      public BlockType Type { get { return type; } }

      //
      // Public constants
      //

      // Describes the default height of a block
      public const int BlockHeight = 54;

      // Describes the default width of a block
      public const int BlockWidth = 54;

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
         position.X = parent.Position.X + (parent.IndexOfBlock(this)) * BlockWidth;
      }

      /// <summary>
      /// Create a random block of any type, excluding empty
      /// </summary>
      /// <param name="parent">The parent row this block belongs to</param>
      /// <returns>A new block object of a random type (not empty)</returns>
      public static Block RndBlockExcEmpty(Row parent)
      {
         return new Block(parent, (BlockType)RandomHelper.Next(1, 6));
      }

      /// <summary>
      /// Create a random block of any type, including empty
      /// </summary>
      /// <param name="parent">The parent row this block belongs to</param>
      /// <returns>A new block object of a random type</returns>
      public static Block RndBlockIncEmpty(Row parent)
      {
         return new Block(parent, (BlockType)RandomHelper.Next(0, 6));
      }

      // Main update method
      public void Update()
      {
         if (parent != null)
         {
            position.Y = parent.Position.Y;
            position.X = position.X = parent.Position.X + (parent.IndexOfBlock(this)) * BlockWidth;
         }
      }

      private int count = 0;

      // Main draw method
      public void Draw(SpriteBatch spriteBatch)
      {
         // Do not do anything if the block is an empty block
         if (type == BlockType.Empty)
            return;

         // Decide if we should draw depending on the current block state
         bool draw = true;
         switch (state)
         {
            case BlockState.None:
               break; // Draw default block
            case BlockState.Falling:
               break; // Draw falling block
            case BlockState.Blinking:
               if (Time.FrameCount % 2 == 0)
                  draw = false; // Draw blinking block every other frame
               count += 1;
               if (count >= 200)
                  state = BlockState.Disappearing;
               break;
            case BlockState.Disappearing:
               alpha = alpha - 9 < 0 ? 0 : alpha - 9;
               drawColor = new Color(alpha, alpha, alpha, alpha); // Change draw color to slowly fade
               if (alpha == 0)
                  state = BlockState.Removed;
               break;
            case BlockState.Removed:
               draw = false; break; // Do not draw "removed" blocks
         }

         if (draw)
         {
            // Create a point for the "actual" position of the tile
            Point actualPosition = new Point(
               ScaleHelper.ScaleWidth((int)position.X),
               (int)position.Y);

            // Create a point for the "actual" scale of the tile
            Point actualScale = ScaleHelper.ScalePoint(new Point(BlockWidth,BlockHeight));

            // Draw the tile
            spriteBatch.Draw(TileTexture, new Rectangle(actualPosition, actualScale), drawColor);

            // Calculate the "actual" position of the symbol (add 1/4 of the block height)
            actualPosition.X += (int)Math.Ceiling((float)actualScale.X / 4);
            actualPosition.Y += (int)Math.Ceiling((float)actualScale.Y / 4);

            // Calculate the scale of the symbol (half of that of the block)
            actualScale.X /= 2;
            actualScale.Y /= 2;

            // Draw the symbol
            spriteBatch.Draw(SymbolTexture, new Rectangle(actualPosition, actualScale), drawColor);
         }
      }
   }
}
