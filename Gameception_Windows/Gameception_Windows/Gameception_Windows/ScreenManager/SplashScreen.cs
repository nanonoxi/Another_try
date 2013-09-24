using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Gameception
{
    class SplashScreen : Screen
    {
        #region Attributes
 
        ScreenManager screenManager;
        ContentManager content;
        Texture2D splashTexture;
        Texture2D startTexture;
        int textAlpha = 55;
        int fadeIncrement = 5;
        double fadeDelay = 0.35;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public SplashScreen(ScreenManager screenManager)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.screenManager = screenManager;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            splashTexture = content.Load<Texture2D>("Backgrounds/matrix_splash_0");


#if WINDOWS
            startTexture = content.Load<Texture2D>("MenuTitles/pressEnter");
#else
            startTexture = content.Load<Texture2D>("MenuTitles/pressStart");
#endif
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            screenManager.SoundManager.play("title");

            fadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;

            if (fadeDelay <= 0)
            {
                fadeDelay = .035; // reset
                textAlpha += fadeIncrement;

                if (textAlpha >= 180 || textAlpha <= 50) // switch between increment/decrement
                {
                    fadeIncrement *= -1;
                }
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.IsStart(ControllingPlayer))
            {
                LoadingScreen.Load(ScreenManager, true, null, new BackgroundScreen(), new MainMenuScreen());
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            // text "pressy any key to continue / press start
            Color colour = new Color (textAlpha, textAlpha, textAlpha);
            colour *= TransitionAlpha;
            Vector2 textOrigin = new Vector2((viewport.Width - startTexture.Width) / 2, viewport.Height - ( startTexture.Height * 2));

            spriteBatch.Begin();

            spriteBatch.Draw(splashTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.Draw(startTexture, textOrigin,colour);

            spriteBatch.End();
        }

        #endregion
    }
}
