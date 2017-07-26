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

      // Defines whether this block can be matched/moved etc.
      private bool isActive = true;

      // Defines whether or not to draw the block on this frame
      private bool drawThisFrame = false;

      // Defines the X,Y position of this Block
      private Point position;

      // Defines who this Block belongs to (used for positioning)
      private Row parent;

      // Timer used to time the blinking/disappearing
      private Timer blockStateTimer;

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
      private enum BlockState
      {
         None,
         Blinking,
         Disappearing,
         Removed
      }

      //
      // Public fields
      //

      // Whether or not to check for gravity this frame
      public bool GravityDirty = false;

      // Public property that holds the conditions in which this block can be matched
      public bool CanBeMatched
      {
         get
         {
            // Not already matched
            if (!(isMatched)
               // Not an empty block
               && !(type == BlockType.Empty)
               // Parent is active
               && parent.isActive
               // Block is active
               && isActive)
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
            if (!(isMatched)
            // Block is active
               && isActive)
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

      // Set default position based on parent
      public void ResetPosition()
      {
         if (parent != null)
         {
            position = new Point(
            parent.Position.X + (parent.IndexOf(this)) * BlockWidth,
            parent.Position.Y);
         }
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
            HandleDrawState();
            if (state != BlockState.Removed)
            {
               HandlePosition();
            }
         }
      }

      // Handle updates to the draw state on this frame
      private void HandleDrawState()
      {
         // If we are in "disappearing" mode and the alpha of our draw color is zero,
         // We have disappeared and are therefore removed from the gameboard
         if (state == BlockState.Disappearing
            && drawColor.A == 0)
         {
            // Emit particles
            // ParticleEmitter.CreateParticles(10, (ParticleType)type, ParticleMovement.Track, new Vector2(position.X, position.Y), new Vector2(500f, 500f), new Vector2(1100,50));

            // This means we should be remove ourselves from the parent row
            state = BlockState.Removed;
            parent.RemoveBlock(parent.IndexOf(this));
            parent = null;
         }

         // Decide if we should draw depending on the current block state
         switch (state)
         {
            // Draw default block
            case BlockState.None:
               drawThisFrame = true; break;
            // Draw blinking block every other frame
            case BlockState.Blinking:
               drawThisFrame = true;
               if (Time.FrameCount % 2 == 0)
                  drawThisFrame = false;
               break;
            // Change draw color to slowly fade
            case BlockState.Disappearing:
               drawThisFrame = true;
               drawColor.A = (byte)Math.Max(0, drawColor.A - 15);
               break;
            // Do not draw "removed" blocks
            case BlockState.Removed:
               drawThisFrame = false; break;
         }
      }

      int maximumFrameMove = 10;
      // Used to "animate" the block if the new position is too far to travel
      private void HandlePosition()
      {
         // Get where this block is "supposed" to be
         Point actualPosition = new Point (
            parent.Position.X + (parent.IndexOf(this)) * BlockWidth,
            parent.Position.Y);

         // If this is the same position, do nothing
         if (actualPosition == position)
            return;

         // If this is an empty block, don't do any of this
         if (type == BlockType.Empty)
         {
            position = actualPosition;
            isActive = true;
            return;
         }

         // Find the difference in x and y
         int xDiff = Math.Abs(actualPosition.X - position.X);
         int yDiff = Math.Abs(actualPosition.Y - position.Y);

         bool newIsActive = true;
         // Handle X move BEFORE Handling Y move
         if (xDiff > maximumFrameMove)
         {
            newIsActive = false;
            if (actualPosition.X > position.X)
               position.X += maximumFrameMove;
            else
               position.X -= maximumFrameMove;
         }
         else
         {
            position.X = actualPosition.X;

            // Handle Y move
            if (yDiff > maximumFrameMove)
            {
               newIsActive = false;
               if (actualPosition.Y > position.Y)
                  position.Y += maximumFrameMove;
               else
                  position.Y -= maximumFrameMove;
            }
            else
            {
               position.Y = actualPosition.Y;
            }
         }

         // Update the active flag
         if (newIsActive != isActive)
         {
            isActive = newIsActive;
            parent.DirtyRowAndColumn(parent.IndexOf(this));
         }
      }

      // Main draw method
      public void Draw(SpriteBatch spriteBatch)
      {
         if (type == BlockType.Empty)
            return; // Never draw empty blocks

         if (drawThisFrame)
         {
            // Create a point for the "actual" position of the tile
            Point drawPosition = new Point(
               ScaleHelper.ScaleWidth((int)position.X),
               (int)position.Y);

            // Create a point for the "actual" scale of the tile
            Point drawScale = ScaleHelper.ScalePoint(new Point(BlockWidth,BlockHeight));

            // Draw the tile
            spriteBatch.Draw(TileTexture, new Rectangle(drawPosition, drawScale), drawColor);

            // Calculate the "actual" position of the symbol (add 1/4 of the block height)
            drawPosition.X += (int)Math.Ceiling((float)drawScale.X / 4);
            drawPosition.Y += (int)Math.Ceiling((float)drawScale.Y / 4);

            // Calculate the scale of the symbol (half of that of the block)
            drawScale.X /= 2;
            drawScale.Y /= 2;

            // Draw the symbol
            spriteBatch.Draw(SymbolTexture, new Rectangle(drawPosition, drawScale), drawColor);
         }
      }
   }
}
