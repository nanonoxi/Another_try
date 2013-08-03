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

        ContentManager content;
        Texture2D splashTexture;
        ScreenManager screenManager;
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
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            int playerIndex;
            if (ControllingPlayer.HasValue)
                playerIndex = (int)ControllingPlayer.Value;
            else
                playerIndex = 1;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            if (keyboardState.GetPressedKeys().Length > 0) // if any key is pressed
            {
                //screenManager.AddScreen(new BackgroundScreen(), null);
                //screenManager.AddScreen(new MainMenuScreen(), null);

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

            spriteBatch.Begin();

            spriteBatch.Draw(splashTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();
        }

        #endregion
    }
}
