using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleLeague.Utilities;

namespace PuzzleLeague
{
   /// <summary>
   /// Class that describes a row of blocks on the GameBoard
   /// </summary>
   class Row
   {
      //
      // Private fields
      //

      // Array of blocks that belong to this row (0 = leftmost, 5 = rightmost)
      private Block[] blocks;

      // Dirty flag for the horizontal of this row; no need to check for matches if not dirty
      private bool horizontalDirty = true;

      // Defines the X,Y position of this Row
      private Vector2 position;

      //
      // Public accessors
      //

      // Public index access returns the block at index position
      public Block this[int index]
      {
         get { return blocks[index]; }
      }

      // Public accessor for 'blocks' field (convert to list)
      public List<Block> Blocks
      {
         get
         {
            var rtnBlocks = new List<Block>();
            foreach (Block b in blocks)
            {
               rtnBlocks.Add(b);
            }
            return rtnBlocks;
         }
      }

      // Public accessor for "horizontalDirty" flag
      public bool HorizontalDirty
      {
         get { return horizontalDirty; }
         set { horizontalDirty = value; }
      }

      // Public accessor for 'position' field 
      public Vector2 Position { get { return position; } }

      //
      // Constructor
      //
      public Row()
      {
         // A row is always 6 blocks
         blocks = new Block[6];

         // Set the X position to match the gameboard anchor, the Y position to the height of the screen
         position = new Vector2(GameBoard.GameBoardXAnchor, ScaleHelper.BackBufferHeight);
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

      // Main update method
      public void Update()
      {
         foreach (Block b in blocks)
            b.Update();
         position.Y -= 1;
      }

      // Main draw method
      public void Draw(SpriteBatch spriteBatch)
      {
         foreach (Block b in blocks)
            b.Draw(spriteBatch);
      }

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
