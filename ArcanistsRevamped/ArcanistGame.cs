using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace ArcanistsRevamped
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ArcanistGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D textureSky;
        private Texture2D textureLevel;
        private Texture2D textureDeform;
        private Vector2 mousePosition;
        private MouseState currentMouseState;
        private uint[] pixelDeformData;

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

            FileStream deformStream = new FileStream("Content/deform.png", FileMode.Open);
            Texture2D textureDeform = Texture2D.FromStream(GraphicsDevice, deformStream);
            skyStream.Dispose();
            this.textureDeform = textureDeform;

            // Declare an array to hold the pixel data
            pixelDeformData = new uint[textureDeform.Width * textureDeform.Height];
            // Populate the array
            textureDeform.GetData(pixelDeformData, 0, textureDeform.Width * textureDeform.Height);

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
            // UpdateMouse();

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
            #region Load Textures via FileStream
            FileStream grasslandBackgroundStream = new FileStream("Content/Textures/Background/grasslandBackground.png", FileMode.Open);
            Texture2D grasslandBackground = Texture2D.FromStream(GraphicsDevice, grasslandBackgroundStream);
            grasslandBackgroundStream.Dispose();

            FileStream grasslandTerrainStream = new FileStream("Content/Textures/Terrain/grasslandTerrain.png", FileMode.Open);
            Texture2D grasslandTerrain = Texture2D.FromStream(GraphicsDevice, grasslandTerrainStream);
            grasslandTerrainStream.Dispose();
            #endregion

            spriteBatch.Begin();

            //spriteBatch.Draw(grasslandBackground, new Rectangle(0, 0, 800, 480), Color.White);
            //spriteBatch.Draw(grasslandTerrain, new Rectangle(0, 0, 800, 480), Color.White);
            spriteBatch.Draw(textureSky, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(textureLevel, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(textureDeform, mousePosition, Color.White);

            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion

        /**protected void UpdateMouse()
        {
            MouseState previousMouseState = currentMouseState;

            currentMouseState = Mouse.GetState();

            // This gets the mouse co-ordinates
            // relative to the upper left of the game window
            mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

            // Here we make sure that we only call the deform level function
            // when the left mouse button is released
            if (previousMouseState.LeftButton == ButtonState.Pressed &&
              currentMouseState.LeftButton == ButtonState.Released)
            {
                DeformLevel();
            }
        }**/

        /// <summary>
        /// 16777215 = Alpha
        /// 4294967295 = White
        /// </summary>
        /**protected void DeformLevel()
        {
            // Declare an array to hold the pixel data
            uint[] pixelLevelData = new uint[textureLevel.Width * textureLevel.Height];
            // Populate the array
            textureLevel.GetData(pixelLevelData, 0, textureLevel.Width * textureLevel.Height);

            for (int x = 0; x < textureDeform.Width; x++)
            {
                for (int y = 0; y < textureDeform.Height; y++)
                {
                    // Do some error checking so we dont draw out of bounds of the array etc..
                    if (((mousePosition.X + x) < (textureLevel.Width)) &&
                      ((mousePosition.Y + y) < (textureLevel.Height)))
                    {
                        if ((mousePosition.X + x) >= 0 && (mousePosition.Y + y) >= 0)
                        {
                            // Here we check that the current co-ordinate of the deform texture is not an alpha value
                            // And that the current level texture co-ordinate is not an alpha value
                            if (pixelDeformData[x + y * textureDeform.Width] != 16777215
                              && pixelLevelData[((int)mousePosition.X + x) +
                              ((int)mousePosition.Y + y) * textureLevel.Width] != 16777215)
                            {
                                // We then check to see if the deform texture's current pixel is white (4294967295)                
                                if (pixelDeformData[x + y * textureDeform.Width] == 4294967295)
                                {
                                    // It's white so we replace it with an Alpha pixel
                                    pixelLevelData[((int)mousePosition.X + x) + ((int)mousePosition.Y + y)
                                      * textureLevel.Width] = 16777215;
                                }
                                else
                                {
                                    // Its not white so just set the level texture pixel to the deform texture pixel
                                    pixelLevelData[((int)mousePosition.X + x) + ((int)mousePosition.Y + y)
                                      * textureLevel.Width] = pixelDeformData[x + y * textureDeform.Width];
                                }
                            }
                        }
                    }
                }
            }

            // Update the texture with the changes made above
            textureLevel.SetData(pixelLevelData);
        }**/
    }
}
