using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleLeague.Utilities;

namespace PuzzleLeague
{
   /// <summary>
   /// Class to define the "playable character", handle input & movement etc.
   /// </summary>
   class Player
   {
      //
      // Private fields
      //
      private static Texture2D playerTexture;

      // The gameboard that this player belongs to
      private GameBoard parent;

      // Defer swapping blocks until the next frame
      private bool swapOnNextFrame = false;

      // Not exact screen positions - "grid" based positions around the gameboard (Y = 1 - 12, X = 0 - 4)
      private Point position;

      //
      // Constructor
      //
      public Player(GameBoard parent)
      {
         this.parent = parent;
         InputHelper.ButtonPressed += OnButtonPressed; // Subscribe to InputHelper
         position = new Point(2, 2); // Start somewhere low in the middle
      }

      // Load the content before it is used
      public static void LoadContent()
      {
         playerTexture = ContentHelper.GetTexture("player");
      }

      // Event handler for when a button is pressed in InputHandler
      private void OnButtonPressed(Buttons buttonPressed)
      {
         switch (buttonPressed)
         {
            case Buttons.Left:
               if (position.X - 1 >= 0)
                  position.X -= 1;
               break;
            case Buttons.Right:
               if (position.X + 1 <= 4)
                  position.X += 1;
               break;
            case Buttons.Down:
               if (position.Y - 1 >= 1)
                  position.Y -= 1;
               break;
            case Buttons.Up:
               if (position.Y + 1 <= 12)
                  position.Y += 1;
               break;
            case Buttons.Confirm:
               swapOnNextFrame = true;
               break;
         }
      }

      // Use this to keep the player in line as more rows get added
      public void OnRowAdded()
      {
         position.Y += 1;
      }

      // Main update method
      public void Update()
      {
         if (swapOnNextFrame) // Used to defer the swapping of blocks until the next update loop
         { 
            parent.SwapAt(position.X, position.Y);
            swapOnNextFrame = false;
         }
      }

      // Main draw method
      public void Draw(SpriteBatch spriteBatch)
      {
         Point drawPos = new Point();
         drawPos.X = (ScaleHelper.ScaleWidth(GameBoard.GameBoardXAnchor) + (ScaleHelper.ScaleWidth(Block.BlockWidth) * position.X));
         drawPos.Y = (ScaleHelper.BackBufferHeight - parent.YOffset - (ScaleHelper.ScaleHeight(Block.BlockHeight) * position.Y));

         Point drawScale = new Point();
         drawScale.X = ScaleHelper.ScaleWidth(playerTexture.Width) / 2;
         drawScale.Y = ScaleHelper.ScaleHeight(playerTexture.Height) / 2;

         spriteBatch.Draw(playerTexture, new Rectangle(drawPos, drawScale), Color.White);
      }
   }
}
