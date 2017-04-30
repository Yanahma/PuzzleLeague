using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleLeague.Utilities;

namespace PuzzleLeague
{
   class GameBoard
   {
      //
      // Private fields
      //

      // Hold an array of six lists, which serve as containing the data of the "columns" 
      private List<Block>[] columns;

      // Hold an array of dirty flags for the six columns as above
      private bool[] columnIsDirty;

      // The list of rows currently on the GameBoard
      private List<Row> rows;

      // Private timer to add rows to the GameBoard
      private Timer rowMovingTimer;

      // Private float for the value of the row moving timer
      private float rowMovingTimerSetValue = 0.05f;

      // Reference to the player object
      private Player player;

      //
      // Public constants
      //

      // Where a row's X should start (used in Row constructor)
      public const int GameBoardXAnchor = 306;

      // The current Y offset (bad naming - affects row and player position, when to add rows etc.)
      public int YOffset = 0;

      //
      // Constructor
      //
      public GameBoard()
      {
         // Init player
         player = new Player(this);

         // Init the list of rows 
         rows = new List<Row>();

         // Init the array of columns + related dirty flags
         columns = new List<Block>[6];
         columnIsDirty = new bool[6];
         for (var i = 0; i < columns.Length; i++)
         {
            columns[i] = new List<Block>();
            columnIsDirty[i] = true;
         }

         // Add the first row
         AddRow();

         // Init the row adding timer
         rowMovingTimer = new Timer(true); // "true" here means it is a looping timer
         rowMovingTimer.OnComplete += MoveRowsUp;
         rowMovingTimer.SetTimer(rowMovingTimerSetValue);
         rowMovingTimer.StartTimer();
      }

      // Mark ALL columns as dirty
      public void FlagAllColumns()
      {
         for (var columnIndex = 0; columnIndex < columns.Length; columnIndex++)
         {
            columnIsDirty[columnIndex] = true;
         }
      }

      // Mark a single column as dirty
      public void FlagColumn(int columnIndex)
      {
         columnIsDirty[columnIndex] = true;
      }

      // Main update method
      public void Update()
      {
         // Once a full row has been exposed on the bottom line, add another
         if(YOffset >= ScaleHelper.ScaleHeight(Block.BlockHeight))
         {
            rows[0].isActive = true; // Flag bottom row as active
            AddRow();
            FlagAllColumns();
            YOffset = 0; // Reset the Y offset
         }

         // Update the player
         player.Update();

         // Update the rows
         for(var i = rows.Count - 1; i >= 0; i--)
         {
            rows[i].Update();
         }

         // Update the columns (if needed)
         for (var i = 0; i < columnIsDirty.Length; i++)
         {
            if (columnIsDirty[i])
               UpdateColumn(i);
         }

         // Check for any matches this frame
         DoMatchChecking();
      }

      // Main draw method
      public void Draw(SpriteBatch spriteBatch)
      {
         // Draw the background (should always fill entire screen)
         Rectangle drawTo = new Rectangle(Point.Zero, new Point(ScaleHelper.BackBufferWidth,ScaleHelper.BackBufferHeight));
         spriteBatch.Draw(ContentHelper.GetTexture("gameBoardBackground"), drawTo, Color.White);

         // Draw each of the rows that belong to this GameBoard
         foreach (Row row in rows)
            row.Draw(spriteBatch);

         // Draw the player
         player.Draw(spriteBatch);

         // Finally, draw the background overlay
         spriteBatch.Draw(ContentHelper.GetTexture("gameBoardOverlayCheat"), drawTo, Color.White);
      }

      // Accessability for Rows to know which position they are in
      public int IndexOf(Row item) => rows.IndexOf(item);

      // Add a random row to this GameBoard
      public void AddRow()
      {
         // Get a new random row
         Row randomRow = Row.RandomRow(this);

         // Insert into position 0 on both the rows and columns (its just easier this way)
         rows.Insert(0, randomRow);
         for (var i = 0; i < columns.Length; i++)
         {
            columns[i].Insert(0, randomRow[i]);
            columnIsDirty[i] = true;
         }

         // Player needs updating when a row is added too
         player.OnRowAdded();
      }

      // Remove a row from the list of rows
      public void RemoveRow(Row row)
      {
         rows.Remove(row);
      }

      // Move all of the rows up a position
      public void MoveRowsUp(object sender, EventArgs e)
      {
         YOffset += 1;
      }

      // Swap values on a specified row at specified position + position+1 (player drawn from top left corner)
      public void SwapAt(int x, int y)
      {
         // Make sure we don't crash from attempting to swap a row that doesn't exist
         if (!(y >= rows.Count))
         {
            // Call the row swap method
            rows[y].SwapAt(x, x + 1);

            // Flag both columns affected as dirty
            FlagColumn(x);
            FlagColumn(x + 1);
         }
      }

      // Update a column in the columns array (check for empties and "drop" blocks into place)
      private void UpdateColumn(int columnIndex)
      {
         // Rebuild the new row
         columns[columnIndex].Clear();
         foreach (Row r in rows)
         {
            columns[columnIndex].Add(r[columnIndex]);
         }

         // Queue up the empty positions in the column, to later add blocks to
         Queue<int> dropPositions = new Queue<int>();

         // Loop through all the positions of the columns, from bottom to top
         for (var i = 0; i < columns[columnIndex].Count; i++)
         {
            Block thisBlock = columns[columnIndex][i];
            if (thisBlock.Type == BlockType.Empty)
            {
               dropPositions.Enqueue(i); // Queue up an empty space
            }
            else
            {
               if (dropPositions.Count > 0) // If there are any empty spaces below us
               {
                  // Get the "nextPosition" (lowest empty space)
                  int nextPosition = dropPositions.Dequeue();

                  // Add an empty block where this one was
                  rows[i].AddBlock(new Block(rows[i], BlockType.Empty), columnIndex);
                  dropPositions.Enqueue(i); // (Also queue this new empty space)

                  // Add the current block to the empty space
                  rows[nextPosition].AddBlock(thisBlock, columnIndex);
                  rows[nextPosition].isDirty = true;

                  // Update the column as well
                  columns[columnIndex][i] = rows[i][columnIndex];
                  columns[columnIndex][nextPosition] = rows[nextPosition][columnIndex];
               }
            }
         }
      }

      // #UNUSED 
      // Build all of the column arrays
      private void BuildAllColumns()
      {
         // Init the array of lists
         for (var i = 0; i < columns.Length; i++)
         {
            columns[i].Clear();
            columnIsDirty[i] = true;
         }

         // Loop through rows & add to each column
         foreach (Row r in rows)
         {
            for (var i = 0; i < columns.Length; i++)
            {
               columns[i].Add(r[i]);
            }
         }
      }

      // Method to check for matches in the rows and columns (not to be confused w/ "CheckMatches")
      private void DoMatchChecking()
      {
         var allMatchedBlocks = new List<Block>();

         // Check for horizontal matches
         foreach (Row r in rows)
         {
            if (r.isDirty && r.isActive)
            {
               // Compile all matched blocks into a single, unique list
               List<Block> matchedBlocks = CheckMatches(r.Blocks);
               if (matchedBlocks.Count > 0)
               {
                  // Console.WriteLine("Matches found in row {0}", rows.IndexOf(r));
                  foreach (Block block in matchedBlocks)
                  {
                     if (!(allMatchedBlocks.Contains(block)))
                        allMatchedBlocks.Add(block);
                  }
               }
               r.isDirty = false;
            }
         }

         // Check for vertical matches
         for (var i = 0; i < columns.Length; i++)
         {
            if (columnIsDirty[i])
            {
               // Compile all matched blocks into a single, unique list
               List<Block> matchedBlocks = CheckMatches(columns[i]);
               if (matchedBlocks.Count > 0)
               {
                  // Console.WriteLine("Matches found in column {0}", i);
                  foreach (Block block in matchedBlocks)
                  {
                     if (!(allMatchedBlocks.Contains(block)))
                        allMatchedBlocks.Add(block);
                  }
               }
               columnIsDirty[i] = false;
            }
         }

         // Finally, tag all the blocks that have been matched
         foreach (Block block in allMatchedBlocks)
            block.OnMatched();
      }

      /// <summary>
      /// Takes a list of ordered blocks and returns a list of any matches of length 3 or greater
      /// </summary>
      /// <param name="blocks">List of blocks (in order they appear)</param>
      /// <returns>Unique list of any blocks that match</returns>
      private List<Block> CheckMatches(List<Block> blocks)
      {
         int pos = 1;
         bool prevSame = false; // Used to maximize "chains" of matching blocks
         List<Block> matchedBlocks = new List<Block>();

         // Check from the second position in the array, up to the second to last position
         while (pos <= blocks.Count - 2)
         {
            // If the block in this pos and this pos + 1 aren't already matched,
            // and they are of the same type
            if (blocks[pos].CanBeMatched
               && blocks[pos + 1].CanBeMatched
               && blocks[pos].Type == blocks[pos + 1].Type)
            {
               // If we know the previous block (pos - 1) is of the same type, 
               // add the next block (pos + 1) & progress one block (following a chain)
               if (prevSame)
               {
                  matchedBlocks.Add(blocks[pos + 1]);
                  pos += 1;
               }
               // Otherwise, check the previous block is of the same type, and add all three
               // blocks if they match (and set "prevSame" flag to begin chain checking)
               else if (blocks[pos - 1].CanBeMatched
                  && blocks[pos - 1].Type == blocks[pos].Type)
               {
                  matchedBlocks.Add(blocks[pos - 1]);
                  matchedBlocks.Add(blocks[pos]);
                  matchedBlocks.Add(blocks[pos + 1]);
                  prevSame = true;
                  pos += 1;
               }
               // Otherwise, check the next block to confirm this is a chain of three
               else
               {
                  pos += 1;
               }
            }
            else
            {
               // If we know that these two blocks don't match, we know that a chain can only be made
               // two blocks further down the list instead of one (skip a check)
               prevSame = false;
               pos += 2;
            }
         }
         // Return the list of matched blocks we have compiled
         return matchedBlocks;
      }

      // CheckMatches which takes a block array (from Row.Blocks) - use same logic as above, but 
      // first we should convert it to a List
      private List<Block> CheckMatches(Block[] blocks)
      {
         var newBlocks = new List<Block>();
         foreach (Block b in blocks)
         {
            newBlocks.Add(b);
         }
         return CheckMatches(newBlocks);
      }
   }
}
