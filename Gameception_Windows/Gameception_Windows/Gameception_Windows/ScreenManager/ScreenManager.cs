/// Merada Richter, 2013.07.27
/// Based on GSMSample for Windows

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Gameception
{
    /// <summary>
    /// Manages Screen instances by maintaining a stack of Screens.
    /// Calls Update & Draw on stack as required, routes input to
    /// top active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        #region Attributes

        List<Screen> screens = new List<Screen>();
        List<Screen> screensToUpdate = new List<Screen>();

        InputState input = new InputState();

        bool isInitialized;

        /// <summary>
        /// Game the ScreenManager belongs to
        /// </summary>
        Gameception game;
        public Gameception Game
        {
            get { return game; }
            set { game = value; }
        }

        /// <summary>
        /// SoundManager belonging to the screenManager
        /// </summary>
        SoundManager soundManager;
        public SoundManager SoundManager
        {
            get { return soundManager; }
            set { soundManager = value; }
        }

        /// <summary>
        /// Default SpriteBatch shared by all Screens (Screens don't have
        /// to bother creating own local instances)
        /// </summary>
        SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }


        /// <summary>
        /// Default font
        /// </summary>
        SpriteFont gamefont;
        public SpriteFont Gamefont
        {
            get { return gamefont; }
        }

        /// <summary>
        /// Menu font
        /// </summary>
        SpriteFont menufont;
        public SpriteFont Menufont
        {
            get { return menufont; }
        }

        /// <summary>
        /// Title font
        /// </summary>
        SpriteFont titlefont;
        public SpriteFont Titlefont
        {
            get { return titlefont; }
        }

        /// <summary>
        /// Messagebox font
        /// </summary>
        SpriteFont msgboxfont;
        public SpriteFont Msgboxfont
        {
            get { return msgboxfont; }
        }

        Texture2D blankTexture;

        /// <summary>
        /// Used for debugging: prints out list of all screens each time it's updated
        /// </summary>
        bool traceEnabled;
        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor
        /// </summary>
        public ScreenManager(Gameception game) : base(game)
        {
            this.game = game;
        }

        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            // Load content belonging to the screen manager.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gamefont = content.Load<SpriteFont>("Fonts/gamefont");
            menufont = content.Load<SpriteFont>("Fonts/menufont");
            titlefont = content.Load<SpriteFont>("Fonts/titlefont");
            msgboxfont = content.Load<SpriteFont>("Fonts/msgboxfont");
            blankTexture = content.Load<Texture2D>("Backgrounds/blank");

            // Load content belonging to the sound manager.
            // Load extra sounds as needed per level in each level class by using
            // this.ScreenManager.SoundManager.add(content.Load<>(),"");
            soundManager = new SoundManager();

            /******************************Songs---need to be mp3 files*********************************/
            soundManager.add(content.Load<Song>("Sounds/for_the_lost_lenore"), "title");
            soundManager.add(content.Load<Song>("Sounds/09-instrumental"), "title2");
            SoundManager.add(content.Load<Song>("Sounds/The_Crusade"), "title3");
            SoundManager.add(content.Load<Song>("Sounds/16-$o$"), "sos");
            SoundManager.add(content.Load<Song>("Sounds/more excuses"), "excuses");
            SoundManager.add(content.Load<Song>("Sounds/FinalFantasy_Remix"), "final");

            /******************************SoundEffects---need to be wav files*********************************/
            SoundManager.add(content.Load<SoundEffect>("Sounds/marioBrosbump"), "bump");
            SoundManager.add(content.Load<SoundEffect>("Sounds/marioBrosPause"), "pause");
            SoundManager.add(content.Load<SoundEffect>("Sounds/yoshi"), "end");
            SoundManager.add(content.Load<SoundEffect>("Sounds/credits"), "gameOver");
            SoundManager.add(content.Load<SoundEffect>("Sounds/pew"), "pew");

            
            foreach (Screen screen in screens)
            {
                screen.LoadContent();
            }
        }

        protected override void UnloadContent()
        {
            foreach (Screen screen in screens)
            {
                screen.UnloadContent();
            }
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            input.Update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            screensToUpdate.Clear();

            foreach (Screen screen in screens)
                screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                Screen screen = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            soundManager.Update();

            // Print debug trace?
            if (traceEnabled)
                TraceScreens();
        }


        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (Screen screen in screens)
                screenNames.Add(screen.GetType().Name);

            Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
        }


        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (Screen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }


        #endregion

        #region Public Methods


        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(Screen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (isInitialized)
            {
                screen.LoadContent();
            }

            screens.Add(screen);
        }


        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(Screen screen)
        {
            if (isInitialized)
                screen.UnloadContent();

            //calling pause after removing pause screen
            //results in music unpausing
            if (screen is PauseMenuScreen)
                soundManager.pause();
            else
                soundManager.stop();

            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }


        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public Screen[] GetScreens()
        {
            return screens.ToArray();
        }


        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            spriteBatch.Begin();

            spriteBatch.Draw(blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black * alpha);

            spriteBatch.End();
        }

        #endregion
    }
}
