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
      private int combo;
      private string scoreString; // The score in string form, with commas
      private string comboString; // The combo in string form, with "x" at the end
      private static SpriteFont scoreFont;

      // Score display variables
      private Vector2 scorePosition;
      private Rectangle scoreRectangle;

      // Combo display variables
      private Vector2 comboPosition;
      private Rectangle comboRectangle;

      // Score would overflow box if more than this
      private const int MaxScore = 999999999;
      
      private const int ScoreBoxWidth = 350;
      private const int ScoreBoxHeight = 50;

      private const int ComboBoxWidth = 150;
      private const int ComboBoxHeight = 50;

      //
      // Constructor
      //
      public Score()
      {
         score = 0;
         combo = 1;// Combo is always 1x to start with
         scoreFont = ContentHelper.GetFont("KenVectorFutureThin");

         // Efficiency - set the draw variables here and recalculate them
         // if score changed (or screen size changes)
         scorePosition = new Vector2(
            ScaleHelper.BackBufferWidth - (ScaleHelper.BackBufferWidth/3),
            ScaleHelper.BackBufferHeight / 15);

         scoreRectangle = new Rectangle(
            (int)scorePosition.X, 
            (int)scorePosition.Y,
            ScaleHelper.ScaleWidth(ScoreBoxWidth),
            ScaleHelper.ScaleHeight(ScoreBoxHeight));

         scoreString = GetScoreString();
         // Make sure the score is right-aligned
         scorePosition.X = scoreRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(scoreString).X);

         comboPosition = new Vector2(
            ScaleHelper.BackBufferWidth - (int)(ScaleHelper.BackBufferWidth / 5.6),
            ScaleHelper.BackBufferHeight / 5);

         comboRectangle = new Rectangle(
            (int)comboPosition.X,
            (int)comboPosition.Y,
            ScaleHelper.ScaleWidth(ComboBoxWidth),
            ScaleHelper.ScaleHeight(ComboBoxHeight));

         comboString = GetComboString();
         // Make sure the combo is right-aligned
         comboPosition.X = comboRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(comboString).X);
      }

      // Add to the score
      public void AddScore()
      {
         score += RandomHelper.Next(100, 999) * combo;
         score = Math.Min(MaxScore, score); // Make sure score doesn't overflow
         scoreString = GetScoreString();
         // Make sure the score is right-aligned
         scorePosition.X = scoreRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(scoreString).X);
      }

      // Add to the combo
      public void AddCombo()
      {
         combo++;
         comboString = GetComboString();
         // Make sure the combo is right-aligned
         comboPosition.X = comboRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(comboString).X);
      }

      // Reset the score
      public void ResetScore()
      {
         score = 0;
         scoreString = GetScoreString();
         // Make sure the score is right-aligned
         scorePosition.X = scoreRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(scoreString).X);
      }

      // Reset the combo
      public void ResetCombo()
      {
         combo = 1;
         comboString = GetComboString();
         // Make sure the combo is right-aligned
         comboPosition.X = comboRectangle.Right - ScaleHelper.ScaleWidth((int)scoreFont.MeasureString(comboString).X);
      }

      // Main draw method
      public void Draw(SpriteBatch spriteBatch)
      {
         // Draw the score container box
         spriteBatch.Draw(ContentHelper.GetTexture("score_panel"), scoreRectangle, Color.White);

         // Draw the score text
         spriteBatch.DrawString(
            scoreFont,
            scoreString,
            scorePosition,
            Color.Black,
            0f,
            Vector2.Zero,
            ScaleHelper.ScreenScaleVector,
            SpriteEffects.None,
            0f);

         // Draw the combo container box
         spriteBatch.Draw(ContentHelper.GetTexture("score_panel"), comboRectangle, Color.White);

         // Draw the combo text
         spriteBatch.DrawString(
            scoreFont,
            comboString,
            comboPosition,
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

      // Add "x" to the end to the combo
      public string GetComboString()
      {
         StringBuilder returnString = new StringBuilder();
         returnString.Append(combo.ToString());
         returnString.Append("x");

         return returnString.ToString();
      }
   }
}
