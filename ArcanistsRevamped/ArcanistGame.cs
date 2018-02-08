using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using FarseerPhysics;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.Collision;
using FarseerPhysics.Factories;

namespace ArcanistsRevamped
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ArcanistGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DebugViewXNA debugView;
        public Matrix Projection;
        public Matrix View;

        // Declare texture variables.
        private Texture2D skyTexture;
        private Texture2D groundTexture;
        private Texture2D groundTextureMask;
        private Texture2D playerTexture;

        // Declare terrain variables
        private GroundTerrain groundTerrain;

        // Declare keyboard and mouse state variable.
        private KeyboardState oldKeyState;
        private MouseState mouseState;
        private MouseState oldMouseState;

        // Declare VelcroPhysics.Dynamics variables.
        private Body playerBody;
        private Body groundBody;
        private World gameWorld;

        // Declare position variables.
        private Vector2 playerPosition;
        private Vector2 lower;
        private Vector2 upper;

        // Simple camera controls
        private Matrix view;
        private Vector2 cameraPosition;
        private Vector2 screenCenter;
        private Vector2 groundOrigin;
        private Vector2 playerOrigin;

        private AABB groundTextureArea;

        #region Constructor
        public ArcanistGame()
        {
            graphics = new GraphicsDeviceManager(this);
            
            //graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            // Content.RootDirectory = "Content";
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            IsFixedTimeStep = true;
            graphics.SynchronizeWithVerticalRetrace = false;

            // Create a new world with gravity.
            gameWorld = new World(new Vector2(0, 10f));
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
            debugView = new DebugViewXNA(gameWorld);
            
            view = Matrix.Identity;

            base.Initialize();
            
            CreateProjection();

        }
        #endregion

        #region Extra Methods
        private void CreateProjection()
        {
            lower = -new Vector2(25.0f * GraphicsDevice.Viewport.AspectRatio, 25.0f);
            upper = new Vector2(25.0f * GraphicsDevice.Viewport.AspectRatio, 25.0f);

            // L/R/B/T
            Projection = Matrix.CreateOrthographicOffCenter(lower.X, upper.X, lower.Y, upper.Y, -1, 1);
        }

        public Vector2 ConvertWorldToScreen(Vector2 position)
        {
            Vector3 temp = GraphicsDevice.Viewport.Project(new Vector3(position, 0), Projection, View, Matrix.Identity);
            return new Vector2(temp.X, temp.Y);
        }

        public Vector2 ConvertScreenToWorld(int x, int y)
        {
            Vector3 temp = GraphicsDevice.Viewport.Unproject(new Vector3(x, y, 0), Projection, View, Matrix.Identity);
            return new Vector2(temp.X, temp.Y);
        }

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
                playerPosition.X -= 0.25f;
            if (state.IsKeyDown(Keys.D))
                playerPosition.X += 0.25f;
            if (state.IsKeyDown(Keys.W))
                playerPosition.Y -= 0.25f;
            if (state.IsKeyDown(Keys.S))
                playerPosition.Y += 0.25f;

            if (state.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space))
                playerBody.ApplyLinearImpulse(new Vector2(0, -10));

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            oldKeyState = state;
        }

        public void TerrainOnClick(MouseState state, MouseState oldState)
        {
            Vector2 position = ConvertScreenToWorld(state.X, state.Y);

            if (state.RightButton == ButtonState.Pressed)
            {
                groundTerrain.DrawCircleOnMap(position, -1);
                groundTerrain.RegenerateTerrain();

                debugView.BeginCustomDraw(ref Projection, ref View);
                debugView.DrawSolidCircle(position, groundTerrain.circleRadius, Vector2.UnitY, Color.Blue * 0.5f);
                debugView.EndCustomDraw();
            }
            else if (state.LeftButton == ButtonState.Pressed)
            {
                groundTerrain.DrawCircleOnMap(position, 1);
                groundTerrain.RegenerateTerrain();

                debugView.BeginCustomDraw(ref Projection, ref View);
                debugView.DrawSolidCircle(position, groundTerrain.circleRadius, Vector2.UnitY, Color.Red * 0.5f);
                debugView.EndCustomDraw();
            }
            else if (state.MiddleButton == ButtonState.Pressed)
            {
                Body circle = BodyFactory.CreateCircle(gameWorld, 1, 1);
                circle.BodyType = BodyType.Dynamic;
                circle.Position = position;
            }
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
            debugView.LoadContent(GraphicsDevice, Content);
            oldKeyState = Keyboard.GetState();
            oldMouseState = Mouse.GetState();

            cameraPosition = Vector2.Zero;
            screenCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);

            #region Ground
            /* Ground */
            Vector2 levelPosition = ConvertUnits.ToSimUnits(screenCenter) + new Vector2(0, 1.25f);

            // Load the ground Texture2D object
            groundTexture = Content.Load<Texture2D>("grassTexture");

            // Load the ground Texture2D mask
            groundTextureMask = Content.Load<Texture2D>("grassTextureMask");

            // Calculate the AABB based off of the texture given.
            groundTextureArea = new AABB(new Vector2(0, 0), 100, 100);

            // Pass the World object to GroundTerrain() to create a terrain object that will keep track of data
            groundTerrain = new GroundTerrain(gameWorld, groundTextureArea);

            // Pass the ground Texture2D object to Initialize()
            //groundTerrain.Initialize(groundTextureMask);
           

            // Give it some bounce and friction
            #endregion

            #region Player
            /* Player */
            // Convert screen center from pixels to meters
            //Vector2 playerPosition = ConvertUnits.ToSimUnits(screenCenter) + new Vector2(0, -1.5f);

            // Create the player fixture

            // Give it some bounce and friction
            #endregion

            /* We need XNA to draw the ground and player at the center of the shapes */
            //groundOrigin = new Vector2(groundTexture.Width / 2f, groundTexture.Height / 2f);
            //playerOrigin = new Vector2(playerTexture.Width / 2f, playerTexture.Height / 2f);

            // Farseer expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 64 pixels here
            //ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            

            

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
            debugView.BeginCustomDraw(ref Projection, ref View);
            debugView.DrawAABB(ref groundTextureArea, Color.Red * 0.5f);
            debugView.EndCustomDraw();

            
            TerrainOnClick(mouseState, oldMouseState);
            HandleKeyboard();

            //We update the world
            gameWorld.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

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

            //Draw player and ground
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, view);
            //spriteBatch.Draw(playerTexture, ConvertUnits.ToDisplayUnits(playerBody.Position), null, Color.White, playerBody.Rotation, playerOrigin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(groundTextureMask, groundOrigin, Color.White);
            
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
        #endregion
    }
}
