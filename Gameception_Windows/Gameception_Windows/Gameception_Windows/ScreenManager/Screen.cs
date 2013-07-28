/// Merada Richter, 2013.07.27
/// Based on GSMSample for Windows

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace Gameception.ScreenManager
{
    public enum ScreenState
    {
        TransitionOn, TransitionOff, Active, Hidden,
    }

    public abstract class Screen
    {
        #region Attributes

        /// <summary>
        /// Sub-screens don't transition off for popups
        /// </summary>
        bool isPopup = false;
        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }

        /// <summary>
        /// How long screen takes to transition on when activated
        /// </summary>
        TimeSpan transitionOnTime = TimeSpan.Zero;
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        /// <summary>
        /// How long screen takes to transition off when activated
        /// </summary>
        TimeSpan transitionOffTime = TimeSpan.Zero;
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        /// <summary>
        /// Current position of the screen transition
        /// 0 = fully active, no transition
        /// 1 = fully off
        /// </summary>
        float transitionPosition = 1;
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        /// <summary>
        /// Current alpha of the screen transition
        /// 0 = fullly active, no transition
        /// 1 = fully off
        /// </summary>
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        /// <summary>
        /// Current screen transition state
        /// </summary>
        ScreenState screenState = ScreenState.TransitionOn;
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        /// <summary>
        /// Screen transitions off if it's:
        /// temporarily making way for another screen on top of it, OR
        /// exiting for good.
        /// </summary>
        bool isExiting = false;
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        /// <summary>
        /// Check if screen is active/responds to user input
        /// </summary>
        bool otherScreenHasFocus;
        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus &&
                    (screenState == ScreenState.TransitionOn || screenState == ScreenState.Active);
            }
        }

        /// <summary>
        /// Manager that screen belongs to
        /// </summary>
        ScreenManager screenManager;
        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }

        /// <summary>
        /// Index of player currently controlling the screen
        /// </summary>
        PlayerIndex? controllingPlayer;
        public PlayerIndex? ControllingPlayer
        {
            get { return controllingPlayer; }
            internal set { controllingPlayer = value; }
        }

        /*
        /// <summary>
        /// Gets the gestures the screen is interested in. Screens should be as specific
        /// as possible with gestures to increase the accuracy of the gesture engine.
        /// For example, most menus only need Tap or perhaps Tap and VerticalDrag to operate.
        /// These gestures are handled by the ScreenManager when screens change and
        /// all gestures are placed in the InputState passed to the HandleInput method.
        /// </summary>
        public GestureType EnabledGestures
        {
            get { return enabledGestures; }
            protected set
            {
                enabledGestures = value;

                // the screen manager handles this during screen changes, but
                // if this screen is active and the gesture types are changing,
                // we have to update the TouchPanel ourself.
                if (ScreenState == ScreenState.Active)
                {
                    TouchPanel.EnabledGestures = value;
                }
            }
        }

        GestureType enabledGestures = GestureType.None; */

        #endregion

        #region Initialization

        /// <summary>
        /// Load graphics content for screen
        /// </summary>
        public virtual void LoadContent() { }

        /// <summary>
        /// Unload content for screen
        /// </summary>
        public virtual void UnloadContent() { }

        #endregion

        #region Methods

        /// <summary>
        /// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        /// instantly kills the screen, this method respects the transition timings
        /// and will give the screen a chance to gradually transition off.
        /// </summary>
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                // If the screen has a zero transition time, remove it immediately.
                ScreenManager.RemoveScreen(this);
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                isExiting = true;
            }
        }


        #endregion

        #region Update

        /// <summary>
        /// Method is called regardless of whether screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (isExiting)
            {
                // If the screen is going away to die, it should transition off.
                screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    ScreenManager.RemoveScreen(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // Still busy transitioning.
                    screenState = ScreenState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenState.Hidden;
                }
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                {
                    // Still busy transitioning.
                    screenState = ScreenState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenState.Active;
                }
            }
        }


        /// <summary>
        /// Helper for updating the screen transition position.
        /// </summary>
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            transitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }


        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        public virtual void HandleInput(InputState input) { }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        public virtual void Draw(GameTime gameTime) { }

        #endregion


    }
}
