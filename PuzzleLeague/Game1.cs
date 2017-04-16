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
         graphics.PreferredBackBufferHeight = 1080;
         graphics.PreferredBackBufferWidth = 540;
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
      int y = 0;
      protected override void Draw(GameTime gameTime)
      {
         GraphicsDevice.Clear(Color.CornflowerBlue);

         spriteBatch.Begin();

         var x = 0;
         y += 1;
         Texture2D textureBox, textureIcon;
         for (var i = 0; i < 5; i++)
         {
            textureBox = null; textureIcon = null;
            switch (i)
            {
               case 0:
                  textureBox = ContentHelper.GetTexture("tileBlue_27");
                  textureIcon = ContentHelper.GetTexture("tileBlue_31");
                  break;
               case 1:
                  textureBox = ContentHelper.GetTexture("tileGreen_27");
                  textureIcon = ContentHelper.GetTexture("tileGreen_35");
                  break;
               case 2:
                  textureBox = ContentHelper.GetTexture("tilePink_27");
                  textureIcon = ContentHelper.GetTexture("tilePink_30");
                  break;
               case 3:
                  textureBox = ContentHelper.GetTexture("tileRed_27");
                  textureIcon = ContentHelper.GetTexture("tileRed_36");
                  break;
               case 4:
                  textureBox = ContentHelper.GetTexture("tileYellow_27");
                  textureIcon = ContentHelper.GetTexture("tileYellow_33");
                  break;
            }
            spriteBatch.Draw(textureBox, new Rectangle(x, y, 54, 54), Color.White);
            spriteBatch.Draw(textureIcon, new Rectangle(x + 14, y + 14, 27, 27), Color.White);
            x += 108;
         }
         spriteBatch.End();

         base.Draw(gameTime);
      }
   }
}
