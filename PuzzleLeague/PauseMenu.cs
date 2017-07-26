using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using PuzzleLeague.Interfaces;
using PuzzleLeague.Utilities;
using PuzzleLeague.GUI;
using Microsoft.Xna.Framework;

namespace PuzzleLeague
{
   public class PauseMenu : IScene
   {
      //
      // Private fields
      //
      private Game1 parent;
      private Texture2D pausePanel;

      private GuiButton continueButton;
      private GuiButton optionsButton;

      // Buffer values to center the panel
      private int widthBuffer;
      private int heightBuffer;

      //
      // Public fields
      //

      //
      // Constructor
      //

      public PauseMenu(Game1 parent)
      {
         this.parent = parent;
         pausePanel = ContentHelper.GetTexture("grey_panel");

         //
         widthBuffer = (ScaleHelper.BackBufferWidth - ScaleHelper.ScaleWidth(pausePanel.Width)) / 2;
         heightBuffer = (ScaleHelper.BackBufferHeight - ScaleHelper.ScaleHeight(pausePanel.Height)) / 2;

         int buttonWidthBuffer = widthBuffer + (widthBuffer - 200) / 2;
         int buttonHeightBuffer = heightBuffer + (heightBuffer - 50) / 2;
         // Init buttons
         continueButton = new GuiButton(new Point(buttonWidthBuffer, buttonHeightBuffer), new Point(200,50), "Continue");
         continueButton.GuiButtonClicked += Unpause;

         optionsButton = new GuiButton(new Point(buttonWidthBuffer, buttonHeightBuffer * 2), new Point(200, 50), "Options");
      }

      private void Unpause(object sender, EventArgs e)
      {
         parent.RemoveScene();
      }


      // Method for when button is pressed
      private void OnButtonPressed(Buttons button)
      {
         /*if(button == Buttons.Escape)
         {
            parent.RemoveScene();
         }*/
      }

      // Main update method
      public void Update()
      {

      }

      // Main draw method
      public void Draw(SpriteBatch spriteBatch)
      {
         // Draw the background
         spriteBatch.Draw(ContentHelper.GetTexture("gameBoardBackground"), new Rectangle(Point.Zero, new Point(ScaleHelper.BackBufferWidth, ScaleHelper.BackBufferHeight)), Color.Gray);

         // Draw the panel
         Rectangle drawRectangle = new Rectangle(
            new Point(widthBuffer, heightBuffer),
            new Point(ScaleHelper.ScaleWidth(pausePanel.Width), ScaleHelper.ScaleHeight(pausePanel.Height)));

         spriteBatch.Draw(pausePanel,  drawRectangle, Color.PowderBlue);
         
         // Draw the buttons
         continueButton.Draw(spriteBatch);

         optionsButton.Draw(spriteBatch);
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

         continueButton.GuiButtonClicked -= Unpause;
      }
   }
}
