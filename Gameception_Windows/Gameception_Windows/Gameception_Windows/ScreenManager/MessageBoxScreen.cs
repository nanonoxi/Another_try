/// Merada Richter, 2013.07.28
/// Based on GSMSample for Windows

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Gameception
{
    /// <summary>
    /// Popup message box screen to display confirmation messages
    /// </summary>
    class MessageBoxScreen : Screen
    {
        #region Attributes

        string message;

        Texture2D gradientTexture;
        Texture2D AbuttonTexture;
        Texture2D BbuttonTexture;
        Texture2D ENTERbuttonTexture;
        Texture2D ESCAPEbuttonTexture;

        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message) : this(message, true)
        { }

        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message, bool includeUsageText)
        {
            this.message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            gradientTexture = content.Load<Texture2D>("Backgrounds/messagebox");
            AbuttonTexture = content.Load<Texture2D>("Controls/Abutton");
            BbuttonTexture = content.Load<Texture2D>("Controls/Bbutton");
            ENTERbuttonTexture = content.Load<Texture2D>("Controls/ENTERbutton");
            ESCAPEbuttonTexture = content.Load<Texture2D>("Controls/ESCAPEbutton");
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                    Cancelled(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Msgboxfont;
            font.LineSpacing = 50;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y * 2 + vPad * 2);

            // Fade the popup alpha during transitions.
            Color colour = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle
            spriteBatch.Draw(gradientTexture, backgroundRectangle, colour);

            // Draw text
            spriteBatch.DrawString(font, message, textPosition, colour);

            // Draw controls specific to platform
#if WINDOWS
            Vector2 controlsPosition = new Vector2(viewportSize.X / 2f - 180, viewportSize.Y / 2f + 20);

            spriteBatch.Draw(ENTERbuttonTexture, controlsPosition, colour);
            spriteBatch.Draw(ESCAPEbuttonTexture, controlsPosition + new Vector2(200, 0), colour);

            spriteBatch.DrawString(font, "yes", controlsPosition + new Vector2(135, 2), colour);
            spriteBatch.DrawString(font, "no", controlsPosition + new Vector2(335, 2), colour);

#else
            Vector2 controlsPosition = new Vector2 (viewportSize.X / 2f - 140, viewportSize.Y / 2f);

            spriteBatch.Draw(AbuttonTexture, controlsPosition, colour);
            spriteBatch.Draw(BbuttonTexture, controlsPosition + new Vector2(150, 0), colour);

            spriteBatch.DrawString(font, "yes", controlsPosition + new Vector2(80, 20), colour);
            spriteBatch.DrawString(font, "no", controlsPosition + new Vector2(230, 20), colour);
#endif

            spriteBatch.End(); 
        }

        #endregion
    }
}
