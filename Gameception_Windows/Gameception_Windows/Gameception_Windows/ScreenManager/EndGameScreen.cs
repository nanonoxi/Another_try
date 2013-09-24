using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Gameception
{
    class EndGameScreen : Screen
    {
        #region Attributes

        Texture2D titleTexture;
        Texture2D backgroundTexture;
        String thisScreensMusic;
        SoundManager soundManager;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public EndGameScreen()
        {
            thisScreensMusic = "sos";
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

            titleTexture = content.Load<Texture2D>("MenuTitles/endgamemenu");
            backgroundTexture = content.Load<Texture2D>("Backgrounds/tetris_blocks_1");
        }

        #endregion

        #region Handle Input

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
                const string message = "Do you want to return to the main menu?";
                MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

                confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

                ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
            }
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "Do you want to return to the
        /// main menu" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen());
        }

        #endregion
        
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                              bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            ScreenManager.SoundManager.play(thisScreensMusic);
        }
        

        #region Draw

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 titlePosition = new Vector2(viewport.Width / 2, 140);
            Vector2 titleOrigin = new Vector2(titleTexture.Width / 2, titleTexture.Height / 2);
            Color titleColor = Color.White * TransitionAlpha;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.Draw(titleTexture, titlePosition, null, titleColor, 0.0f, titleOrigin, 1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.End();
        }
        #endregion
    }
}
