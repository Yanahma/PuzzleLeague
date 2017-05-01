using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PuzzleLeague.Utilities;

namespace PuzzleLeague
{
   class Score
   {
      //
      // Private fields
      //

      private int score;
      private string scoreString; // The score in string form, with commas
      private static SpriteFont scoreFont;

      private Vector2 position;
      private Rectangle drawRectangle; // Used for the grey box around the score

      // Score would overflow box if more than this
      private const int MaxScore = 999999999;
      // Default score box width
      private const int ScoreBoxWidth = 350;
      // Default score box height
      private const int ScoreBoxHeight = 50;

      //
      // Constructor
      //
      public Score()
      {
         score = 0;
         scoreFont = ContentHelper.GetFont("KenVectorFutureThin");

         // Efficiency - set the draw variables here and recalculate them
         // if score changed (or screen size changes)
         position = new Vector2(
            ScaleHelper.BackBufferWidth - (ScaleHelper.BackBufferWidth/3),
            ScaleHelper.BackBufferHeight / 15);

         drawRectangle = new Rectangle(
            (int)position.X, 
            (int)position.Y,
            ScaleHelper.ScaleWidth(ScoreBoxWidth),
            ScaleHelper.ScaleHeight(ScoreBoxHeight));

         scoreString = GetScoreString();
         // Make sure the score is right-aligned
         position.X = drawRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(scoreString).X);
      }

      // Add to the score
      public void AddScore(int value)
      {
         score = Math.Min(MaxScore, score + value);
         scoreString = GetScoreString();
         // Make sure the score is right-aligned
         position.X = drawRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(scoreString).X);
      }

      // Reset the score
      public void ResetScore()
      {
         score = 0;
         scoreString = GetScoreString();
         // Make sure the score is right-aligned
         position.X = drawRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(scoreString).X);
      }

      // Main draw method
      public void Draw(SpriteBatch spriteBatch)
      {
         // Draw the score container box
         spriteBatch.Draw(ContentHelper.GetTexture("grey_panel"), drawRectangle, Color.White);

         // Draw the score text
         spriteBatch.DrawString(
            scoreFont,
            scoreString,
            position,
            Color.Black,
            0f,
            Vector2.Zero,
            ScaleHelper.ScreenScaleVector,
            SpriteEffects.None,
            0f);
      }

      // Adds commas every three numbers (i.e. 1234567 -> 1,234,567)
      public string GetScoreString()
      {
         string scoreString = score.ToString();
         if (scoreString.Length > 3)
         {
            StringBuilder returnString = new StringBuilder();
            for (var i = 0; i < scoreString.Length; i++)
            {
               returnString.Append(scoreString[i]);
               // This is horrible
               if ((i + 1) == scoreString.Length - 3)
                  returnString.Append(",");
               else if ((i + 1) == scoreString.Length - 6)
                  returnString.Append(",");
            }
            return returnString.ToString();
         }
         return scoreString.ToString();
      }
   }
}
