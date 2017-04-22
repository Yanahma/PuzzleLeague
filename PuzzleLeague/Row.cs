using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PuzzleLeague
{
   /// <summary>
   /// Class that describes a row of blocks on the GameBoard
   /// </summary>
   class Row
   {
      // Defines the X,Y position of this Row
      private Vector2 position;

      // Array of blocks that belong to this row (0 = leftmost, 5 = rightmost)
      private Block[] blocks;

      // Public accessor for 'position' field 
      public Vector2 Position { get { return position; } }

      //
      // Constructor
      //
      public Row()
      {
         blocks = new Block[6];
         // Set the X position to match the gameboard anchor, the Y position to the height of the screen
         position = new Vector2(GameBoard.GAMEBOARD_X_ANCHOR, Game1.BackBufferHeight);
      }

      /// <summary>
      /// Add a block to the blocks array
      /// </summary>
      /// <param name="block">Block to add to array</param>
      /// <param name="index">Index position to add to</param>
      public void AddBlock(Block block, int index) => blocks[index] = block;

      /// <summary>
      /// Accessability for blocks as to the index of the slot they are in
      /// </summary>
      /// <param name="item">Block to retrieve the index of in the row</param>
      /// <returns>The index in the row</returns>
      public int IndexOfBlock(Block item) => Array.IndexOf(blocks, item);

      #region Game Loop Lifecycle
      public void Update()
      {
         foreach (Block b in blocks)
            b.Update();
         position.Y -= 1;
      }

      public void Draw(SpriteBatch spriteBatch)
      {
         foreach (Block b in blocks)
            b.Draw(spriteBatch);
      }
      #endregion

      /// <summary>
      /// Static generator of a random row
      /// </summary>
      /// <returns>A new row populated with 6 random blocks (not including empties)</returns>
      static public Row RandomRow()
      {
         Row rndRow = new Row();
         for (var i = 0; i <= 5; i++)
         {
            rndRow.AddBlock(Block.RndBlockExcEmpty(rndRow), i);
         }
         return rndRow;
      }
   }
}
