using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gameception
{
    public class Gameception : Microsoft.Xna.Framework.Game
    {
        #region Attributes

        GraphicsDeviceManager graphics;
        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }
        ScreenManager screenManager;

        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        static readonly string[] preloadAssets =
        {
            "Backgrounds/gradient",
        };


        #endregion

        #region Initialization

        /// <summary>
        /// The main game constructor.
        /// </summary>
        public Gameception()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferMultiSampling = true;
            //graphics.IsFullScreen = true;

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // Activate the first screens
            screenManager.AddScreen(new SplashScreen(screenManager), null);
        }


        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            foreach (string asset in preloadAssets)
            {
                Content.Load<object>(asset);
            }
        }

        #endregion

        #region Draw

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            screenManager.Draw(gameTime);
            base.Draw(gameTime);
        }


        #endregion
    }
}