using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleLeague.Utilities;
using PuzzleLeague.Interfaces;

namespace PuzzleLeague
{
   class GameBoard : IScene
   {
      //
      // Private fields
      //

      // Parent Game1 of this GameBoard
      private Game1 parent;

      // Hold an array of six lists, which serve as containing the data of the "columns" 
      private List<Block>[] columns;

      // Hold an array of dirty flags for the six columns as above
      private bool[] columnIsDirty;

      // The list of rows currently on the GameBoard
      private List<Row> rows;

      // Private timer to automatically add rows to the GameBoard
      private Timer rowMovingTimer;

      // Private timer to pause the gameboard whilst matches are being made
      private Timer rowPauseTimer;

      // Private timer to reset the combo if the gameboard is stale
      private Timer comboResetTimer;

      // Bool controlled partially by the above timer
      private bool rowPaused = false;

      // Private float for the value of the row moving timer
      //private float rowMovingTimerSetValue = 0.8f;
      private float rowMovingTimerSetValue = 0.05f;

      // Reference to the player object
      private Player player;

      // Reference to the score object
      private Score score;
      
      //
      // Public constants
      //

      // Where a row's X should start (used in Row constructor)
      public const int GameBoardXAnchor = 306;

      // The maximum "level" that can be reached
      public const int maxLevel = 80;

      // The current Y offset (bad naming - affects row and player position, when to add rows etc.)
      public int YOffset = 0;

      //
      // Public fields
      //

      public bool GravityDirty = false;

      //
      // Constructor
      //
      public GameBoard(Game1 parent)
      {
         // Parent game1 of this gameboard
         this.parent = parent;
         
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

         // Init the row adding timer
         rowMovingTimer = new Timer(true); // "true" here means it is a looping timer
         rowMovingTimer.OnComplete += MoveRowsUp;
         rowMovingTimer.SetTimer(rowMovingTimerSetValue);
         rowMovingTimer.StartTimer();

         // Init the row pausing timer
         rowPauseTimer = new Timer(true); // "true" here means it is a looping timer
         rowPauseTimer.OnComplete += UnpauseGameBoard;
         rowPauseTimer.SetTimer(2f);
         rowPauseTimer.StartTimer();

         // Init the combo resetting timer
         comboResetTimer = new Timer(true); // "true" here means it is a looping timer
         comboResetTimer.OnComplete += ResetCombo;
         comboResetTimer.SetTimer(0.5f);
         comboResetTimer.StartTimer();

         // Add the first row
         AddRow();

         // Init the scoreboard
         score = new Score();
      }

      // Method for when button is pressed
      private void OnButtonPressed(Buttons button)
      {
         if(button == Buttons.Pause)
         {
            parent.AddScene(new PauseMenu(parent));
         }

         if(button == Buttons.Escape)
         {
            parent.RemoveScene();
         }
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
         if (YOffset >= ScaleHelper.ScaleHeight(Block.BlockHeight))
         {
            AddRow(); YOffset = 0; // Reset the Y offset
         }

         // First, check gravity to see if there are any gravity matches (increase combo)
         if (GravityDirty)
         {
            for(var i = 0; i < columns.Length; i++)
               DoGravity(i);

            if (AnyMatch())
               score.AddCombo();

            GravityDirty = false;
         }

         // Update the player
         player.Update();

         // Update the rows (update backwards in case of deletions)
         for (var i = rows.Count - 1; i >= 0; i--)
         {
            rows[i].Update();
         }

         // FlagAllColumns();

         // Rebuild the columns from the updated rows
         for (var i = 0; i < columns.Length; i++)
         {
            if (columnIsDirty[i])
               BuildColumn(i);
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

         // Draw the score
         score.Draw(spriteBatch);

         // Finally, draw the background overlay
         spriteBatch.Draw(ContentHelper.GetTexture("gameBoardOverlayCheat"), drawTo, Color.White);
      }

      // Do something when the scene is enabled
      public void OnSceneEnabled()
      {
         // Subscribe to button input when we are the active scene
         InputHelper.ButtonPressed += OnButtonPressed;
      }

      // Do something when the scene is disabled
      public void OnSceneDisabled()
      {
         // Unsubscribe to button input when we are not the active scene
         InputHelper.ButtonPressed -= OnButtonPressed;
      }

      // Accessability for Rows to know which position they are in
      public int IndexOf(Row item) => rows.IndexOf(item);

      // Add a random row to this GameBoard
      public void AddRow()
      {
         // Flag the bottom row as active, if present
         if (rows.Count > 0)
            rows[0].isActive = true;

         // Get a new random row
         Row randomRow = Row.RandomRow(this);

         // Insert into position 0 on both the rows and columns (its just easier this way)
         rows.Insert(0, randomRow);
         for (var i = 0; i < columns.Length; i++)
         {
            columns[i].Insert(0, randomRow[i]);
            columnIsDirty[i] = true;
         }

         YOffset = 0;
         // Increase speed/game level
         // #Unfinished

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
         if(!(rowPaused))
            YOffset += 1;
      }

      // Unpause the gameboard
      public void UnpauseGameBoard(object sender, EventArgs e)
      {
         if (BoardIsStale())
         {
            rowPaused = false;
         }
         else
         {
            rowPauseTimer.ResetTimer();
         }
      }

      // Reset the combo
      public void ResetCombo(object sender, EventArgs e)
      {
         if (BoardIsStale())
            score.ResetCombo();
      }

      // Swap values on a specified row at specified position + position+1 (player drawn from top left corner)
      public void SwapAt(int x, int y)
      {
         // Make sure we don't crash from attempting to swap a row that doesn't exist
         if (!(y >= rows.Count))
         {
            // Call the row swap method
            rows[y].SwapAt(x, x + 1);

            // If either of these blocks were empty, gravity might be dirty
            if(rows[y][x].Type == BlockType.Empty
               || rows[y][x+1].Type == BlockType.Empty)
            {
               GravityDirty = true;
            }

            // Flag both columns affected as dirty
            FlagColumn(x);
            FlagColumn(x + 1);
            
         }
      }

      // Rebuild a column from the row data
      private void BuildColumn(int columnIndex)
      {
         columns[columnIndex].Clear();
         for (var i = 0; i < rows.Count; i++)
            columns[columnIndex].Add(rows[i][columnIndex]);
      }

      // Replicate a "gravity" effect (check for empties and "drop" blocks into place)
      // Boolean return if any blocks were moved due to gravity (used for combos)
      private void DoGravity(int columnIndex)
      {
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
                  if (thisBlock.CanBeSwapped)
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
                  else
                  {
                     // If we reach here, we have found a block that cannot be moved; therefore, all
                     // blocks above it can only drop above it (avoid "clipping" through inactive blocks)
                     dropPositions.Clear();
                  }
               }
            }
         }
      }

      // Checks to see if there are any "active" blocks (matched/moving)
      // As we want to pause the game board whilst there are, and start moving it again if not
      private bool BoardIsStale()
      {
         bool isStale = true;
         for (var i = 0; i < rows.Count && isStale; i++)
         {
            for(var j = 0; j < rows[i].Blocks.Length && isStale; j++)
            {
               if (!(rows[i][j].CanBeSwapped))
                  isStale = false;
            }
         }
         return isStale;
      }

      // Used to check if there are any matches currently on the game board
      // (Returns true as soon as a single one is found
      private bool AnyMatch()
      {
         bool anyMatches = false;
         // Check rows
         for (var i = 0; i < rows.Count && !(anyMatches); i++)
         {
            var matchedBlocks = CheckMatches(rows[i].Blocks);
            if (matchedBlocks.Count > 0)
               anyMatches = true;
         }
         // Check columns
         for(var i = 0; i < columns.Length && !(anyMatches); i++)
         {
            var matchedBlocks = CheckMatches(columns[i]);
            if (matchedBlocks.Count > 0)
               anyMatches = true;
         }
         return anyMatches;
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
         if (allMatchedBlocks.Count > 0)
         {
            rowPaused = true;
            rowPauseTimer.ResetTimer();
            foreach (Block block in allMatchedBlocks)
            {
               block.OnMatched();
               score.AddScore();
            }
         }
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
                  pos += 1;
                  matchedBlocks.Add(blocks[pos]);
               }
               // Otherwise, check the previous block is of the same type, and add all three
               // blocks if they match (and set "prevSame" flag to begin chain checking)
               else if (blocks[pos - 1].CanBeMatched
                  && blocks[pos - 1].Type == blocks[pos].Type)
               {
                  for(var i = pos - 1; i <= pos + 1; i++)
                     matchedBlocks.Add(blocks[i]);
                  prevSame = true;
                  pos += 1;
               }
               // Otherwise, check the next block to confirm this is a chain of three
               // (Also make sure we don't get an out of index error)
               else if ((pos + 2) <= blocks.Count - 1
                  && blocks[pos + 2].CanBeMatched
                  && blocks[pos + 2].Type == blocks[pos].Type)
               {
                  for(var i = pos; i<= pos + 2; i++)
                     matchedBlocks.Add(blocks[i]);
                  prevSame = true;
                  pos += 2;
               }
               // Otherwise, skip onto the next potential chain
               else
               {
                  pos += 3;
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
