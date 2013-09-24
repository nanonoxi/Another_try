using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gameception
{
    class GameOverScreen : Screen
    {
        #region Attributes
 
        Texture2D gameOverTitleTexture;
        Texture2D startTexture;
        int textAlpha = 55;
        int fadeIncrement = 5;
        double fadeDelay = 0.35;
        String thisScreensMusic;
        SoundManager soundManager;


        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameOverScreen()
        {
            thisScreensMusic = "excuses";
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public void setSoundManager(SoundManager s)
        {
            this.soundManager = s;
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            gameOverTitleTexture = content.Load<Texture2D>("MenuTitles/gameover");

#if WINDOWS
            startTexture = content.Load<Texture2D>("MenuTitles/pressEnter");
#else
            startTexture = content.Load<Texture2D>("MenuTitles/pressStart");
#endif
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            ScreenManager.SoundManager.play(thisScreensMusic);

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
                LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new EndGameScreen());
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

            spriteBatch.Begin();

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(viewport.Width / 2, viewport.Height / 2);
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            titlePosition.Y -= transitionOffset * 100;
            Vector2 titleOrigin = new Vector2(gameOverTitleTexture.Width / 2, gameOverTitleTexture.Height / 2);
            Color titleColor = Color.White * TransitionAlpha;

            spriteBatch.Draw(gameOverTitleTexture, titlePosition, null, titleColor, 0.0f, titleOrigin, 1.0f, SpriteEffects.None, 0.0f);

            // text "press any key to continue / press start"
            Color colour = new Color(textAlpha, textAlpha, textAlpha);
            colour *= TransitionAlpha;
            Vector2 textOrigin = new Vector2((viewport.Width - startTexture.Width) / 2, viewport.Height - (startTexture.Height * 2));

            spriteBatch.Draw(startTexture, textOrigin, colour);

            spriteBatch.End();
        }

        #endregion
    }
}
