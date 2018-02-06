using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using VelcroPhysics.Collision;

namespace ArcanistsRevamped
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ArcanistGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Declare texture variables.
        private Texture2D textureSky;
        private Texture2D textureLevel;
        private Texture2D playerTexture;

        // Declare position variables.
        private Vector2 playerPosition;

        // Declare physics constants.
        private const float gravity = 10;

        #region Constructor
        public ArcanistGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Conent";
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            base.Initialize();
        }
        #endregion

        #region LoadContent
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            FileStream skyStream = new FileStream("Content/sky.jpg", FileMode.Open);
            Texture2D textureSky = Texture2D.FromStream(GraphicsDevice, skyStream);
            skyStream.Dispose();
            this.textureSky = textureSky;

            FileStream levelStream = new FileStream("Content/level.png", FileMode.Open);
            Texture2D textureLevel = Texture2D.FromStream(GraphicsDevice, levelStream);
            skyStream.Dispose();
            this.textureLevel = textureLevel;

            FileStream playerStream = new FileStream("Content/player.png", FileMode.Open);
            Texture2D playerTexture = Texture2D.FromStream(GraphicsDevice, playerStream);
            skyStream.Dispose();
            this.playerTexture = playerTexture;

            

        }
        #endregion

        #region UnloadContent
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        #region Update
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
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.A))
                playerPosition.X -= 10;
            if (state.IsKeyDown(Keys.D))
                playerPosition.X += 10;
            if (state.IsKeyDown(Keys.W))
                playerPosition.Y -= 10;
            if (state.IsKeyDown(Keys.S))
                playerPosition.Y += 10;


            base.Update(gameTime);
        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            spriteBatch.Draw(textureSky, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(textureLevel, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(playerTexture, playerPosition, Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion
    }
}
