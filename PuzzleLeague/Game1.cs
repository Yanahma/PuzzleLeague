using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PuzzleLeague.Utilities;
using PuzzleLeague.Interfaces;

namespace PuzzleLeague
{
   /// <summary>
   /// This is the main type for your game.
   /// </summary>
   public class Game1 : Game
   {
      //
      // Private fields
      //

      // The Graphics Device Manager for this game
      private GraphicsDeviceManager graphics;
      
      // The Sprite Batch for this game
      private SpriteBatch spriteBatch;

      // The current GameBoard (temp - replace with scenes)
      private Stack<IScene> scenes;
      //private GameBoard gameBoard;

      //
      // Constructor
      //
      public Game1()
      {
         graphics = new GraphicsDeviceManager(this);
         Content.RootDirectory = "Content";
      }

      /// <summary>
      /// Allows the game to perform any initialization it needs to before starting to run.
      /// This is where it can query for any required services and load any non-graphic
      /// related content.  Calling base.Initialize will enumerate through any components
      /// and initialize them as well.
      /// </summary>
      protected override void Initialize()
      {
         // We want to default to 1280x720
         //graphics.PreferredBackBufferWidth = 640;
         //graphics.PreferredBackBufferHeight = 360;
         graphics.PreferredBackBufferWidth = 1280;
         graphics.PreferredBackBufferHeight = 720;
         //graphics.PreferredBackBufferWidth = 1920;
         //graphics.PreferredBackBufferHeight = 1080;
         graphics.ApplyChanges();
         ScaleHelper.UpdateBufferValues(graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferWidth);

         IsMouseVisible = true;

         base.Initialize();
      }

      /// <summary>
      /// LoadContent will be called once per game and is the place to load
      /// all of your content.
      /// </summary>
      protected override void LoadContent()
      {
         // Create a new SpriteBatch, which can be used to draw textures
         spriteBatch = new SpriteBatch(GraphicsDevice);

         // Add tile textures
         ContentHelper.AddTexture("tileBlue_27", Content.Load<Texture2D>("Graphics\\tileBlue_27"));
         ContentHelper.AddTexture("tileGreen_27", Content.Load<Texture2D>("Graphics\\tileGreen_27"));
         ContentHelper.AddTexture("tilePink_27", Content.Load<Texture2D>("Graphics\\tilePink_27"));
         ContentHelper.AddTexture("tileRed_27", Content.Load<Texture2D>("Graphics\\tileRed_27"));
         ContentHelper.AddTexture("tileYellow_27", Content.Load<Texture2D>("Graphics\\tileYellow_27"));

         // Add symbol textures
         ContentHelper.AddTexture("tileBlue_31", Content.Load<Texture2D>("Graphics\\tileBlue_31"));
         ContentHelper.AddTexture("tileGreen_35", Content.Load<Texture2D>("Graphics\\tileGreen_35"));
         ContentHelper.AddTexture("tilePink_30", Content.Load<Texture2D>("Graphics\\tilePink_30"));
         ContentHelper.AddTexture("tileRed_36", Content.Load<Texture2D>("Graphics\\tileRed_36"));
         ContentHelper.AddTexture("tileYellow_33", Content.Load<Texture2D>("Graphics\\tileYellow_33"));

         // Add player texture
         ContentHelper.AddTexture("player", Content.Load<Texture2D>("Graphics\\player"));

         // Add background textures for the GameBoard
         ContentHelper.AddTexture("gameBoardBackground", Content.Load<Texture2D>("Background\\gameBoardBackground"));
         ContentHelper.AddTexture("gameBoardOverlayCheat", Content.Load<Texture2D>("Background\\gameBoardOverlayCheat"));

         // Add UI textures
         ContentHelper.AddTexture("grey_panel", Content.Load<Texture2D>("UI\\grey_panel"));
         ContentHelper.AddTexture("score_panel", Content.Load<Texture2D>("UI\\score_panel"));
         ContentHelper.AddTexture("yellow_button04", Content.Load<Texture2D>("UI\\yellow_button04"));
         ContentHelper.AddTexture("yellow_button05", Content.Load<Texture2D>("UI\\yellow_button05"));

         // Add font for score
         ContentHelper.AddFont("KenVectorFutureThin", Content.Load<SpriteFont>("Fonts\\KenVectorFutureThin"));
         ContentHelper.AddFont("KenneySpace", Content.Load<SpriteFont>("Fonts\\KenneySpace"));

         // Once all the assets are loaded into the ContentHelper, create the gameboard
         scenes = new Stack<IScene>();
         AddScene(new GameBoard(this));
         // scenes.Push(new GameBoard(this));
         // gameBoard = new GameBoard();
      }

      /// <summary>
      /// UnloadContent will be called once per game and is the place to unload
      /// game-specific content
      /// </summary>
      protected override void UnloadContent()
      { }

      /// <summary>
      /// Allows the game to run logic such as updating the world,
      /// checking for collisions, gathering input, and playing audio.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Update(GameTime gameTime)
      {
         // Update the static "Time" class
         Time.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

         // Update any timers that are running
         Timer.Update();

         // Update the input helper class to detect input on this frame
         InputHelper.Update();

         // Update the active scene
         if (scenes.Count > 0)
            scenes.Peek().Update();
         else
            Exit();

         // Update particle effects
         ParticleEmitter.Update();

         //gameBoard.Update();

         base.Update(gameTime);
      }

      /// <summary>
      /// This is called when the game should draw itself.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Draw(GameTime gameTime)
      {
         GraphicsDevice.Clear(Color.CornflowerBlue);
         spriteBatch.Begin();

         // Draw the active scene
         scenes.Peek().Draw(spriteBatch);
         //gameBoard.Draw(spriteBatch);

         // Draw particle effects
         ParticleEmitter.Draw(spriteBatch);

         spriteBatch.End();
         base.Draw(gameTime);
      }

      // Add a scene to the scene stack
      public void AddScene(IScene scene)
      {
         if(scenes.Count > 0)
            scenes.Peek().OnSceneDisabled();

         scenes.Push(scene);
         scenes.Peek().OnSceneEnabled();
      }

      // Remove a scene from the scene stack
      public void RemoveScene()
      {
         scenes.Peek().OnSceneDisabled();
         scenes.Pop();
         
         // If there is a scene below this one, enabled it
         if (scenes.Count > 0)
            scenes.Peek().OnSceneEnabled();
      }

      // Clear the scenes stack and add a scene to it
      public void GotoScene(IScene scene)
      {
         // If we have existing scene/s in our stack, clear and disable them
         if (scenes.Count > 0) {
            scenes.Peek().OnSceneDisabled();
            scenes.Clear();
         }
         // Add scene & enable it
         scenes.Push(scene);
         scenes.Peek().OnSceneEnabled();
      }
   }
}
