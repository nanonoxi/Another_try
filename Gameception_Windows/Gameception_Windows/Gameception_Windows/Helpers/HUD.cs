using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gameception
{
    class HUD
    {
        #region Attributes
        
        // Allows access information general to all screens in the game
        ScreenManager screenManager;
        SpriteFont HUDFont;

        // Allows access to the players so that all the proper information can be draw
        Player player1;
        Player player2;

        #endregion

        public HUD(ScreenManager screenMan, Player p1, Player p2)
        {
            screenManager = screenMan;
            HUDFont = screenManager.Gamefont;

            player1 = p1;
            player2 = p2;
        }

        #region Draw

        public void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            spriteBatch.Begin();

            spriteBatch.DrawString(HUDFont, "Player 1 Health: " + player1.Health, new Vector2(15, 15), Color.White);
            spriteBatch.DrawString(HUDFont, player2.Health + " :Player 2 Health", new Vector2(screenManager.GraphicsDevice.Viewport.Width - 390, 15), Color.White);

            spriteBatch.End();
        }

        #endregion
    }
}
