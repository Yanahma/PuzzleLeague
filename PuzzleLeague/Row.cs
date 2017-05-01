using System;
using System.Collections.Generic;
using System.Text;
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

      // The parent GameBoard of this Row
      private GameBoard parent;
      
      // Array of blocks that belong to this row (0 = leftmost, 5 = rightmost)
      private Block[] blocks;

      // Defines the X,Y position of this Row
      private Point position;

      // Private property to return if this row is full of empty blocks
      private bool isEmpty
      {
         get
         {
            return (blocks[0].Type == BlockType.Empty
         && blocks[5].Type == BlockType.Empty
         && blocks[1].Type == BlockType.Empty
         && blocks[4].Type == BlockType.Empty
         && blocks[2].Type == BlockType.Empty
         && blocks[3].Type == BlockType.Empty);
         }
      }

      //
      // Public fields
      //

      // Dirty flag for the horizontal of this row; no need to check for matches if not dirty
      public bool isDirty = true;

      // Flag to determine if this row can be affected
      public bool isActive = false;

      // Public index access returns the block at index position
      public Block this[int index]
      {
         get { return blocks[index]; }
      }

      // Public accessor for 'blocks' field
      public Block[] Blocks
      {
         get { return blocks; }
      }

      // Public accessor for 'position' field 
      public Point Position
      {
         get { return position; }
      }

      //
      // Constructor
      //
      public Row(GameBoard parent)
      {
         this.parent = parent;
         blocks = new Block[6];

         // Set the X position to match the gameboard anchor, the Y position to the height of the screen
         position = new Point(GameBoard.GameBoardXAnchor, ScaleHelper.BackBufferHeight);
      }

      // Add a block to the "blocks" array
      public void AddBlock(Block block, int index)
      {
         blocks[index] = block;
         block.SetParent(this);
         parent.FlagColumn(index); // Flag column as dirty
         isDirty = true; // Flag row as dirty
      }

      // Remove a block from the "blocks" array (replace with BlockType.Empty)
      public void RemoveBlock(int index)
      {
         blocks[index] = new Block(this, BlockType.Empty);
         parent.FlagColumn(index); // Flag column as dirty
         isDirty = true; // Flag row as dirty
      }

      // Accessability for Blocks to know which position they are in
      public int IndexOf(Block item) => Array.IndexOf(blocks, item);

      // Main update method
      public void Update()
      {
         if (isEmpty)
         {
            parent.RemoveRow(this);
            Array.Clear(blocks, 0, 6);
         }
         else
         {
            position.Y = ScaleHelper.BackBufferHeight - parent.YOffset - (ScaleHelper.ScaleHeight(Block.BlockHeight * parent.IndexOf(this)));
            foreach (Block b in blocks)
               b.Update();
         }
      }

      // Main draw method
      public void Draw(SpriteBatch spriteBatch)
      {
         foreach (Block b in blocks)
            b.Draw(spriteBatch);
      }

      // Static generator of a random row (does not include empty blocks)
      static public Row RandomRow(GameBoard parent)
      {
         Row rndRow = new Row(parent);
         for (var i = 0; i <= 5; i++)
         {
            rndRow.AddBlock(Block.RndBlockExcEmpty(rndRow), i);
         }

         // Correctly assign positions
         foreach (Block b in rndRow.blocks)
            b.ResetPosition();

         return rndRow;
      }

      // Display more meaningful data about this Row
      public override string ToString()
      {
         StringBuilder rowString = new StringBuilder();
         for(var i = 0; i < blocks.Length; i++)
         {
            rowString.AppendFormat("Index {0} = {1}", i, blocks[i].Type.ToString());
         }
         return rowString.ToString();
      }

      // Swap the blocks at two positions
      public void SwapAt(int position1, int position2)
      {
         if (blocks[position1].CanBeSwapped
            && blocks[position2].CanBeSwapped)
         {
            Block save = blocks[position1];
            blocks[position1] = blocks[position2];
            blocks[position2] = save;
            isDirty = true;
         }
      }

      // Dirty both this row & the affected block column
      public void DirtyRowAndColumn(int columnIndex)
      {
         isDirty = true;
         parent.FlagColumn(columnIndex);
      }
   }
}
