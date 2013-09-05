/// Merada Richter, 2013.07.27
/// Based on GSMSample for Windows

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gameception
{
    class MenuEntry
    {
        #region Attributes

        /// <summary>
        /// Text to display
        /// </summary>
        string text;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }


        /// <summary>
        /// For fading selection effect
        /// </summary>
        float selectionFade;
        public float SelectionFade
        {
            get { return selectionFade; }
            set { selectionFade = value; }
        }
        
        /// <summary>
        /// Position of entry, set by MenuScreen each frame in Update
        /// </summary>
        Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }


        /// <summary>
        /// height measurement of text
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Menufont.LineSpacing;
        }


        /// <summary>
        /// width mesaurement of text
        /// </summary>
        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.ScreenManager.Menufont.MeasureString(Text).X;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text"></param>
        public MenuEntry(string text)
        {
            this.text = text;
        }

        #endregion

        #region Events

        /// <summary>
        /// event called when entry is selected
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerIndex"></param>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }

        #endregion

        #region Update

        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // add fade in effect for menu selection
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds *4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }

        #endregion

        #region Draw

        /// <summary>
        /// use to customize
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="isSelected"></param>
        /// <param name="gameTime"></param>
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // draw selected entry in _, otherwise _
            Color colour = isSelected ? Color.LightGreen : Color.Gray;
            colour *= screen.TransitionAlpha;

            // pulsate effect
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            float scale = 1 + pulsate * 0.01f * selectionFade;

            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Menufont;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, text, position, colour, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }

        #endregion
    }
}
 