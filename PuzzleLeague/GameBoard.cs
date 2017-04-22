using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleLeague.Utilities;

namespace PuzzleLeague
{
   class GameBoard
   {
      // The list of rows currently on the GameBoard
      private List<Row> rows;

      // This is where the start of the rows is
      public const int GAMEBOARD_X_ANCHOR = 306;

      public GameBoard()
      {
         rows = new List<Row>();
      }

      int cnt = 0; // Temp code for adding rows!!
      #region Game Loop lifecycle
      public void Update()
      {
         if (cnt++ == 53)
         {
            AddRow();
            cnt = 0;
         }

         foreach(Row row in rows)
            row.Update();
      }
      
      public void Draw(SpriteBatch spriteBatch)
      {
         // Draw the background
         spriteBatch.Draw(ContentHelper.GetTexture("gameBoardBackground"), Vector2.Zero, Color.White);
         
         // Draw each of the rows that belong to this GameBoard
         foreach (Row row in rows)
            row.Draw(spriteBatch);

         // Draw the background overlay (super cheaty)
         spriteBatch.Draw(ContentHelper.GetTexture("gameBoardOverlayCheat"), Vector2.Zero, Color.White);
      }
      #endregion

      public void AddRow()
      {
         rows.Add(Row.RandomRow());
      }
   }
}
