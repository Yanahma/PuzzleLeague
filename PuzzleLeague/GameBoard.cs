using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleLeague.Utilities;
using PuzzleLeague.Utilities.Containers;
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

      // Use custom data structure "Board" for holding data about the GameBoard
      private Board<Block> gameBoard;

      // Private timer to automatically add rows to the GameBoard
      private Timer rowMovingTimer;

      // Private timer to pause the gameboard whilst matches are being made
      private Timer rowPauseTimer;

      // Bool controlled partially by the above timer
      private bool rowPaused = false;

      // Private timer to reset the combo if the gameboard is stale
      private Timer comboResetTimer;

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
      // Constructor
      //
      public GameBoard(Game1 parent)
      {
         // Parent game1 of this gameboard
         this.parent = parent;
         
         // Init player
         player = new Player(this);

         // Init the list of rows 
         gameBoard = new Board<Block>(6);

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

      // Main update method
      public void Update()
      {
         // Once a full row has been exposed on the bottom line, add another
         if (YOffset >= ScaleHelper.ScaleHeight(Block.BlockHeight))
         {
            AddRow(); YOffset = 0; // Reset the Y offset
         }

         // Update the player
         player.Update();

         // Update all blocks on the gameboard
         for (var i = 0; i < gameBoard.Rows; i++)
         {
            for (var j = 0; j < gameBoard.Columns; j++)
            {
               var thisBlock = gameBoard[i,j];
               // Only update this block if it is not empty
               if (!(thisBlock.Type == BlockType.Empty))
               {
                  // Update the position of this block based on the position in the board
                  thisBlock.UpdatePosition(j, i, YOffset);

                  // Now do the regular update
                  thisBlock.Update();

                  // If the block is now empty, flag it as empty on the gameboard
                  if (thisBlock.Type == BlockType.Empty)
                     gameBoard.RemoveItem(i, j);
               }
            }
         }

         // Do the "gravity" of the gameboard
         gameBoard.CompressBoardDownwards();
         gameBoard.RemoveEmptyRows();

         // Check for any matches this frame
         DoMatchChecking();
      }

      // Main draw method
      public void Draw(SpriteBatch spriteBatch)
      {
         // Draw the background (should always fill entire screen)
         Rectangle drawTo = new Rectangle(Point.Zero, new Point(ScaleHelper.BackBufferWidth,ScaleHelper.BackBufferHeight));
         spriteBatch.Draw(ContentHelper.GetTexture("gameBoardBackground"), drawTo, Color.White);

         // Draw each of the blocks that belong to this GameBoard
         for (var i = 0; i < gameBoard.Rows; i++)
         {
            for (var j = 0; j < gameBoard.Columns; j++)
            {
               gameBoard[i, j].Draw(spriteBatch);
            }
         }

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

      // Add a random row to this GameBoard
      public void AddRow()
      {
         // Flag the bottom row as active, if present
         if (gameBoard.Rows > 0)
         {
            for (var i = 0; i < gameBoard.Columns; i++)
               gameBoard[0][i].IsActive = true;
         }

         // Get a new random row
         Block[] randomRow = GetRandomRow();

         // Insert into position 0 on both the rows and columns (its just easier this way)
         gameBoard.AddAbove(randomRow);
         // #TODO: Increase speed/game level

         // Player needs updating when a row is added too
         player.OnRowAdded();
      }

      // Generate a random row
      public Block[] GetRandomRow()
      {
         Block[] returnArr = new Block[6];
         for (var i = 0; i <= 5; i++)
         {
            var nextBlock = Block.RndBlockExcEmpty();
            returnArr[i] = nextBlock;
            nextBlock.ForcePosition(i, 0, YOffset);
         }

         return returnArr;
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
            rowPaused = false;
         else
            rowPauseTimer.ResetTimer();
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
         if (!(y >= gameBoard.Rows))
         {
            // Call the row swap method
            if (gameBoard[y][x].CanBeSwapped && gameBoard[y][x + 1].CanBeSwapped)
               gameBoard.SwapItemOnRow(y, x, x + 1);
         }
      }

      // Checks to see if there are any "active" blocks (matched/moving)
      // As we want to pause the game board whilst there are, and start moving it again if not
      private bool BoardIsStale()
      {
         bool isStale = true;
         for (var i = 0; i < gameBoard.Rows && isStale; i++)
         {
            for(var j = 0; j < gameBoard.Columns && isStale; j++)
            {
               if (!(gameBoard[i][j].CanBeSwapped))
                  isStale = false;
            }
         }
         return isStale;
      }

      // Used to check if there are any matches currently on the game board
      // (Returns true as soon as a single one is found
      private bool AnyMatch()
      {
         // Check rows
         for (var i = 0; i < gameBoard.Rows; i++)
         {
            if (CheckMatches(gameBoard[i]).Length > 0)
               return true;
         }

         // Check columns
         for(var i = 0; i < gameBoard.Columns; i++)
         {
            if (CheckMatches(gameBoard.GetColumn(i)).Length > 0)
               return true;
         }
         return false;
      }

      // Method to check for matches in the rows and columns (not to be confused w/ "CheckMatches")
      private void DoMatchChecking()
      {
         var allMatchedBlocks = new List<Block>();
         // Check for matches in the rows
         for (var i = 1; i < gameBoard.Rows; i++)
         {
            if (gameBoard.IsRowDirty(i))
            {
               // Compile all matched blocks into a single, unique list
               Block[] matchedBlocks = CheckMatches(gameBoard.GetRow(i));
               if (matchedBlocks.Length > 0)
               {
                  foreach (Block block in matchedBlocks)
                  {
                     if (!(allMatchedBlocks.Contains(block)))
                        allMatchedBlocks.Add(block);
                  }
               }
               //gameBoard.FlagRowClean(i);
            }
         }

         // Check for matches in the columns
         for (var i = 0; i < gameBoard.Columns; i++)
         {
            if (gameBoard.IsColumnDirty(i))
            {
               // Compile all matched blocks into a single, unique list
               Block[] matchedBlocks = CheckMatches(gameBoard.GetColumn(i));
               if (matchedBlocks.Length > 0)
               {
                  foreach (Block block in matchedBlocks)
                  {
                     if (!(allMatchedBlocks.Contains(block)))
                        allMatchedBlocks.Add(block);
                  }
               }
               //gameBoard.FlagColumnClean(i);
            }
         }
         gameBoard.FlagAllClean();

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
      private Block[] CheckMatches(Block[] blocks)
      {
         int pos = 1;
         bool prevSame = false; // Used to maximize "chains" of matching blocks
         List<Block> matchedBlocks = new List<Block>();

         // Check from the second position in the array, up to the second to last position
         while (pos <= blocks.Length - 2)
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
               else if ((pos + 2) <= blocks.Length - 1
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
         return matchedBlocks.ToArray();
      }
   }
}
