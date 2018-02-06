using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using VelcroPhysics.Collision;
using VelcroPhysics.Dynamics;
using VelcroPhysics.Factories;
using VelcroPhysics.Utilities;
using VelcroPhysics.Templates;

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
        private Texture2D levelTexture;
        private Texture2D playerTexture;

        // Declare keyboard state variable.
        private KeyboardState oldKeyState;

        // Declare VelcroPhysics.Dynamics variables.
        private Body playerBody;
        private Body levelBody;
        private World gameWorld;
        private float playerBodyX;
        private float playerBodyY;

        // Declare position variables.
        private Vector2 playerPosition;

        // Simple camera controls
        private Matrix view;
        private Vector2 cameraPosition;
        private Vector2 screenCenter;
        private Vector2 levelOrigin;
        private Vector2 playerOrigin;

        #region Constructor
        public ArcanistGame()
        {
            graphics = new GraphicsDeviceManager(this);
            // Content.RootDirectory = "Content";

            // Create a new world with gravity.
            gameWorld = new World(new Vector2(0, 9.8f));
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

            #region FileStream
            // Uses FileStream to open the sky image and store it in a Texture2D
            FileStream skyStream = new FileStream("Content/sky.jpg", FileMode.Open);
            Texture2D textureSky = Texture2D.FromStream(GraphicsDevice, skyStream);
            skyStream.Dispose();
            this.textureSky = textureSky;

            // Uses FileStream to open the level image and store it in a Texture2D
            FileStream levelStream = new FileStream("Content/level.png", FileMode.Open);
            Texture2D textureLevel = Texture2D.FromStream(GraphicsDevice, levelStream);
            skyStream.Dispose();
            this.levelTexture = textureLevel;

            // Uses FileStream to open the player image and store it in a Texture2D
            FileStream playerStream = new FileStream("Content/player.png", FileMode.Open);
            Texture2D playerTexture = Texture2D.FromStream(GraphicsDevice, playerStream);
            skyStream.Dispose();
            this.playerTexture = playerTexture;
            #endregion

            view = Matrix.Identity;
            cameraPosition = Vector2.Zero;
            screenCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);

            /* We need XNA to draw the ground and player at the center of the shapes */
            levelOrigin = new Vector2(textureLevel.Width / 2f, textureLevel.Height / 2f);
            playerOrigin = new Vector2(playerTexture.Width / 2f, playerTexture.Height / 2f);

            // Velcro Physics expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 64 pixels here
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            #region Player
            /* Player */
            // Convert screen center from pixels to meters
            Vector2 playerPosition = ConvertUnits.ToSimUnits(screenCenter) + new Vector2(0, -1.5f);

            // Create the player fixture
            //playerBody = BodyFactory.CreateBody(gameWorld, playerPosition, 0f, playerBodyType, playerTexture);
            var playerBodyTemplate = new BodyTemplate();
            playerBody = BodyFactory.CreateFromTemplate(gameWorld, playerBodyTemplate);
            playerBody.BodyType = BodyType.Dynamic;
            playerBody.IgnoreGravity = true;
            // Give it some bounce and friction
            playerBody.Restitution = 0.3f;
            playerBody.Friction = 0.5f;
            #endregion

            #region Level
            /* Ground */
            Vector2 levelPosition = ConvertUnits.ToSimUnits(screenCenter) + new Vector2(0, 1.25f);

            // Create the ground fixture
            levelBody = BodyFactory.CreateRectangle(gameWorld, ConvertUnits.ToSimUnits(512f), ConvertUnits.ToSimUnits(64f), 1f, levelPosition);
            levelBody.BodyType = BodyType.Static;
            levelBody.IgnoreGravity = true;
            // Give it some bounce and friction
            levelBody.Restitution = 0.3f;
            levelBody.Friction = 0.5f;
            #endregion

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
            System.Diagnostics.Debug.WriteLine(playerBodyX);
            System.Diagnostics.Debug.WriteLine(playerPosition);
            
            HandleKeyboard();
            //We update the world
            gameWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime);
        }
        #endregion

        #region HandleKeyboard
        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            // Move camera
            if (state.IsKeyDown(Keys.Left))
                cameraPosition.X += 2.5f;
            if (state.IsKeyDown(Keys.Right))
                cameraPosition.X -= 2.5f;
            if (state.IsKeyDown(Keys.Up))
                cameraPosition.Y += 2.5f;
            if (state.IsKeyDown(Keys.Down))
                cameraPosition.Y -= 2.5f;

            view = Matrix.CreateTranslation(new Vector3(cameraPosition - screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(screenCenter, 0f));

            // Move the Player
            if (state.IsKeyDown(Keys.A))
                playerBodyX -= 0.25f;
            if (state.IsKeyDown(Keys.D))
                playerBodyX += 0.25f;
            if (state.IsKeyDown(Keys.W))
                playerBodyY -= 0.25f;
            if (state.IsKeyDown(Keys.S))
                playerBodyY += 0.25f;

            if (state.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space))
                playerBody.ApplyLinearImpulse(new Vector2(0, -10));

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            oldKeyState = state;
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

            //Draw player and ground
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, view);
            spriteBatch.Draw(playerTexture, ConvertUnits.ToDisplayUnits(playerBody.Position), null, Color.White, playerBody.Rotation, playerOrigin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(levelTexture, ConvertUnits.ToDisplayUnits(levelBody.Position), null, Color.White, 0f, levelOrigin, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
        #endregion
    }
}
