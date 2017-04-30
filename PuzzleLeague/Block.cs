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

      // Defines the color to draw (start at white)
      private Color drawColor = Color.White;

      // Defines whether this block has been matched
      private bool isMatched = false;

      // Defines the X,Y position of this Block
      private Vector2 position;

      // Defines who this Block belongs to (used for positioning)
      private Row parent;

      // Timer used to time the blinking/disappearing
      private Timer blockStateTimer;

      // Defines the current "state" of the block
      private BlockState state;

      // Defines which type of Block this is (cannot change)
      private BlockType type;

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
      private enum BlockState
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

      // Public property that holds the conditions in which this block can be matched
      public bool CanBeMatched
      {
         get
         {
            // Not already matched
            if (!(isMatched)
               // Not an empty block
               && !(type == BlockType.Empty)
               // Is active
               && parent.isActive == true)
            {
               return true;
            }

            return false;
         }
      }

      // Public property that holds the conditions in which this block can be swapped
      public bool CanBeSwapped
      {
         get
         {
            // Not already matched
            if (!(isMatched))
            {
               return true;
            }
            return false;
         }
      }

      // Public accessor for 'type' field 
      public BlockType Type
      {
         get { return type; }
      }

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
         position.X = parent.Position.X + (parent.IndexOf(this)) * BlockWidth;

         // Init the timer
         blockStateTimer = new Timer();
         blockStateTimer.OnComplete += SetStateDisappearing;
         blockStateTimer.SetTimer(1);
      }

      // Alter the parent of this block
      public void SetParent(Row parent)
      {
         this.parent = parent;
      }

      // Code to handle when this block gets matched
      public void OnMatched()
      {
         // Make sure we don't do the same thing twice
         if (!(isMatched))
         {
            isMatched = true;
            state = BlockState.Blinking;

            // Start the countdown for disppearing
            blockStateTimer.StartTimer();
         }
      }

      // State changing event used to fade the block out
      public void SetStateDisappearing(object sender, EventArgs e)
      {
         state = BlockState.Disappearing;
         blockStateTimer.StopTimer();
      }

      // Return a random block (doesn't include empty)
      public static Block RndBlockExcEmpty(Row parent)
      {
         return new Block(parent, (BlockType)RandomHelper.Next(1, 6));
      }

      // Return a random block (includes empty)
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
            position.X = position.X = parent.Position.X + (parent.IndexOf(this)) * BlockWidth;

            if (state == BlockState.Removed)
            {
               type = BlockType.Empty;
               parent.RemoveBlock(parent.IndexOf(this));
               parent = null;
            }
         }
      }


      // Main draw method
      public void Draw(SpriteBatch spriteBatch)
      {
         if (type == BlockType.Empty)
            return; // Never draw empty blocks

         // Decide if we should draw depending on the current block state
         bool draw = true;
         switch (state)
         {
            // Draw default block
            case BlockState.None:
               break;
            // Draw falling block
            case BlockState.Falling:
               break;
            // Draw blinking block every other frame
            case BlockState.Blinking:
               if (Time.FrameCount % 2 == 0)
                  draw = false;
               break;
            // Change draw color to slowly fade
            case BlockState.Disappearing:
               drawColor.A = (byte)Math.Max(0, drawColor.A - 15);
               // drawColor = new Color(alpha, alpha, alpha, alpha);
               if (drawColor.A == 0)
               {
                  state = BlockState.Removed;
                  draw = false;
               }
               break;
            // Do not draw "removed" blocks
            case BlockState.Removed:
               draw = false; break;
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
