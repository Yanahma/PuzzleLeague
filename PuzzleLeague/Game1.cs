using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PuzzleLeague.Utilities;

namespace PuzzleLeague
{
   /// <summary>
   /// This is the main type for your game.
   /// </summary>
   public class Game1 : Game
   {
      GraphicsDeviceManager graphics;
      SpriteBatch spriteBatch;

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
         graphics.PreferredBackBufferHeight = 768;
         graphics.PreferredBackBufferWidth = 1024;
         graphics.ApplyChanges();

         base.Initialize();
      }

      /// <summary>
      /// LoadContent will be called once per game and is the place to load
      /// all of your content.
      /// </summary>
      protected override void LoadContent()
      {
         // Create a new SpriteBatch, which can be used to draw textures.
         spriteBatch = new SpriteBatch(GraphicsDevice);

         ContentHelper.AddTexture("tileBlue_27", Content.Load<Texture2D>("Graphics\\tileBlue_27"));
         ContentHelper.AddTexture("tileBlue_31", Content.Load<Texture2D>("Graphics\\tileBlue_31"));

         ContentHelper.AddTexture("tileGreen_27", Content.Load<Texture2D>("Graphics\\tileGreen_27"));
         ContentHelper.AddTexture("tileGreen_35", Content.Load<Texture2D>("Graphics\\tileGreen_35"));

         ContentHelper.AddTexture("tilePink_27", Content.Load<Texture2D>("Graphics\\tilePink_27"));
         ContentHelper.AddTexture("tilePink_30", Content.Load<Texture2D>("Graphics\\tilePink_30"));

         ContentHelper.AddTexture("tileRed_27", Content.Load<Texture2D>("Graphics\\tileRed_27"));
         ContentHelper.AddTexture("tileRed_36", Content.Load<Texture2D>("Graphics\\tileRed_36"));

         ContentHelper.AddTexture("tileYellow_27", Content.Load<Texture2D>("Graphics\\tileYellow_27"));
         ContentHelper.AddTexture("tileYellow_33", Content.Load<Texture2D>("Graphics\\tileYellow_33"));
      }

      /// <summary>
      /// UnloadContent will be called once per game and is the place to unload
      /// game-specific content.
      /// </summary>
      protected override void UnloadContent()
      {
         // TODO: Unload any non ContentManager content here
      }

      /// <summary>
      /// Allows the game to run logic such as updating the world,
      /// checking for collisions, gathering input, and playing audio.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Update(GameTime gameTime)
      {
         if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

         // TODO: Add your update logic here

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

         spriteBatch.Draw(ContentHelper.GetTexture("tileBlue_27"), new Rectangle(0, 0, 100, 100), Color.White);
         spriteBatch.Draw(ContentHelper.GetTexture("tileBlue_31"), new Rectangle(25, 25, 50, 50), Color.White);

         spriteBatch.Draw(ContentHelper.GetTexture("tileGreen_27"), new Rectangle(0, 100, 100, 100), Color.White);
         spriteBatch.Draw(ContentHelper.GetTexture("tileGreen_35"), new Rectangle(25, 125, 50, 50), Color.White);

         spriteBatch.Draw(ContentHelper.GetTexture("tilePink_27"), new Rectangle(0, 200, 100, 100), Color.White);
         spriteBatch.Draw(ContentHelper.GetTexture("tilePink_30"), new Rectangle(25, 225, 50, 50), Color.White);

         spriteBatch.Draw(ContentHelper.GetTexture("tileRed_27"), new Rectangle(0, 300, 100, 100), Color.White);
         spriteBatch.Draw(ContentHelper.GetTexture("tileRed_36"), new Rectangle(25, 325, 50, 50), Color.White);

         spriteBatch.Draw(ContentHelper.GetTexture("tileYellow_27"), new Rectangle(0, 400, 100, 100), Color.White);
         spriteBatch.Draw(ContentHelper.GetTexture("tileYellow_33"), new Rectangle(25, 425, 50, 50), Color.White);

         spriteBatch.End();

         base.Draw(gameTime);
      }
   }
}
