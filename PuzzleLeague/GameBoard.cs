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
      private List<Block>[] columns = new List<Block>[6];

      // Hold an array of dirty flags for the six columns as above
      private bool[] columnDirty = new bool[6];

      // The list of rows currently on the GameBoard
      private LinkedList<Row> rows;

      // Whether or not the GameBoard is currently active
      private bool paused = false;

      //
      // Public constants
      //

      // Where a row's X should start (used in Row constructor)
      public const int GameBoardXAnchor = 306;
      public const int GameBoardYHeight = 12 * Block.BlockHeight;

      public int YOffSet
      {
         get { return (int)rows.First.Value.Position.Y % Block.BlockHeight; }
      }

      //
      // Constructor
      //
      public GameBoard()
      {
         rows = new LinkedList<Row>();
         // Init the array of lists
         for (var i = 0; i < columns.Length; i++)
         {
            columns[i] = new List<Block>();
            columnDirty[i] = true;
         }
      }

      int cnt = 0; // Temp code for adding rows!!

      // Main update method
      public void Update()
      {
         if (cnt++ == 54 - 1)
         {
            AddRow();
            cnt = 0;
            BuildAllColumns();
         }

         //if (rows.Count == 13)
         //paused = true;

         if (!(paused))
         {
            foreach (Row row in rows)
               row.Update();
         }

         CheckForMatches();
      }

      // Main draw method
      public void Draw(SpriteBatch spriteBatch)
      {
         // Draw the backgrounds (should always fill entire screen)
         Rectangle drawTo = new Rectangle(Point.Zero, new Point(ScaleHelper.BackBufferWidth,ScaleHelper.BackBufferHeight));

         spriteBatch.Draw(ContentHelper.GetTexture("gameBoardBackground"), drawTo, Color.White);

         // Draw each of the rows that belong to this GameBoard
         foreach (Row row in rows)
            row.Draw(spriteBatch);

         // Draw the background overlay (super cheaty)
         spriteBatch.Draw(ContentHelper.GetTexture("gameBoardOverlayCheat"), drawTo, Color.White);
      }

      // Add a random row to this GameBoard
      public void AddRow()
      {
         rows.AddLast(Row.RandomRow());
      }

      // Build a single column in the columns array
      private void BuildColumn(int i)
      {
         // Init column
         columns[i].Clear();
         columnDirty[i] = true;

         // Loop through rows & add to each column
         foreach (Row r in rows)
         {
            columns[i].Add(r[i]);
         }
      }

      // Build the columns array
      private void BuildAllColumns()
      {
         // Init the array of lists
         for (var i = 0; i < columns.Length; i++)
         {
            columns[i].Clear();
            columnDirty[i] = true;
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
      private void CheckForMatches()
      {
         List<Block> matchedBlocks;

         // Check for horizontal matches
         foreach (Row r in rows)
         {
            if (r.HorizontalDirty)
            {
               matchedBlocks = CheckMatches(r.Blocks);
               if (matchedBlocks.Count > 0)
               {
                  foreach (Block b in matchedBlocks)
                     b.IsMatched = true;
               }
               r.HorizontalDirty = false;
            }
         }

         // Check for vertical matches
         for (var i = 0; i < columns.Length; i++)
         {
            if (columnDirty[i])
            {
               matchedBlocks = CheckMatches(columns[i]);
               if (matchedBlocks.Count > 0)
               {
                  foreach (Block b in matchedBlocks)
                     b.IsMatched = true;
               }
               columnDirty[i] = false;
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
         var matchedBlocks = new List<Block>();
         for (var i = 0; i <= blocks.Count - 3; i++)
         {
            if (blocks[i].Type == blocks[i + 1].Type && blocks[i + 1].Type == blocks[i + 2].Type)
            {
               for (var j = i; j <= i + 2; j++)
               {
                  if (!(matchedBlocks.Contains(blocks[j])))
                     matchedBlocks.Add(blocks[j]);
               }
            }
         }
         return matchedBlocks;
      }
   }
}
