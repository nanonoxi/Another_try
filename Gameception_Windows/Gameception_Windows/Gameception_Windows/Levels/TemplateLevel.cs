/*
 * note: should inherit from a Level class
 * but i'm not sure what should be in the level class yet
 * so we can mess around here until we know more
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gameception
{
    class TemplateLevel : Screen
    {
        #region Attributes

        ContentManager content;
        SpriteFont gameFont;

        Camera camera;
        Player player1;
        Player player2;

        Creep baddie;
        List<Creep> minions;
        ammoSupply ammo;
        List<ammoSupply> ammoDrops;
        GameObject ground;

        float pauseAlpha;

        // Controls drawing of the heads-up display
        HUD hud;

        #endregion

        #region Initialization

        public TemplateLevel()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        // Needs to move
        PushPullObject tempObstacle;

        public override void LoadContent()
        {
            minions = new List<Creep>();
            ammoDrops = new List<ammoSupply>();

            camera = new Camera(this.ScreenManager.Game.Graphics);

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("Fonts/gamefont");
            ground = new GameObject(content.Load<Model>("Models/crash_level"), 0, 0, Vector3.Zero, 6f, camera);
            
            // player set up should move
            // also, perhaps two separate player objects for Player1 and NPC, inheriting from class Player,
            // to allow for weapon specialization
            player1 = new Player(content.Load<Model>("Models/player1"), 0.1f, 100, new Vector3(5, 3.5f, 0), 3.5f, camera, PlayerIndex.One);
            player1.setKeys(Keys.W, Keys.D, Keys.S, Keys.A, Keys.Space, PlayerIndex.One);
            Weapon player1Weapon = new Weapon(20f, content.Load<Model>("Models/sphereHighPoly"), player1);
            player1.PlayerWeapon = player1Weapon;

            player2 = new Player(content.Load<Model>("Models/npcModel"), 0.1f, 100, new Vector3(-5, 4f, 0), 0.6f, camera, PlayerIndex.Two);
            player2.setKeys(Keys.Up, Keys.Right, Keys.Down, Keys.Left, Keys.NumPad0, PlayerIndex.Two);
            Weapon player2Weapon = new Weapon(20f, content.Load<Model>("Models/sphereHighPoly"), player2);
            player2.PlayerWeapon = player2Weapon;

            player1.setSoundManager(ScreenManager.SoundManager);
            player2.setSoundManager(ScreenManager.SoundManager);

            

            Random r = new Random();
            for (int i = 0; i < 2; i++)
            {
                int randomX = r.Next(-60, 60);
                int randomZ = r.Next(-60, 60);

                baddie = new minion(content.Load<Model>("Models/dude"), 0.1f, 100, new Vector3(randomX, 0.8f, randomZ), 0.1f, camera);
                minions.Add(baddie);
            }

            tempObstacle = new PushPullObject(content.Load<Model>("Models/Cylinder"), 0.1f, 100, new Vector3(0, 4f, 15), 0.5f, camera, 10);

            // HUD has to be initialised here so that it can have access to the initialised player objects
            hud = new HUD(this.ScreenManager, player1, player2);

            // reset game time after loading all assets
            ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Unload graphics content used by the level
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            
            base.Update(gameTime, otherScreenHasFocus, false);

            ScreenManager.SoundManager.play("title3");

            // gradually fade in/out if covered by pause screen
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);


            if (IsActive)
            {
                player1.Update();
                player2.Update();

                tempObstacle.Update();
                
                camera.Update(player1, player2);
            }


            for (int i = 0; i < ammoDrops.Count; i++ )
            {
                if (ammoDrops.ElementAt(i).isPickedUp())
                    ammoDrops.RemoveAt(i);
            }

            for (int i = 0; i < minions.Count;i++ )
            {
                Creep c = minions.ElementAt(i); 

                c.beEvil(player1, player2);
              
                //remove dead creep
                if (((minion)c).isDeadCheck())
                {
                    //check if it should drop ammo
                    if (((minion)c)._dropAmmo())
                    {
                        ammo = new ammoSupply(content.Load<Model>("Models/SphereHighPoly"), 0, 0, 1, camera, 20);
                        ammo.setPosition(c);
                        ammoDrops.Add(ammo);
                    }

                    minions.Remove(c);
                }
            }

            checkCollisions();
        }

        // Temp collision detection
        public void checkCollisions()
        {
            // This is not working correctly yet
            foreach (Projectile p in player2.PlayerWeapon.AllProjectiles)
            {
                if (p.getBoundingSphere().Intersects(tempObstacle.getBoundingSphere()))
                {
                    p.Active = false;
                    //tempObstacle.Position = player2.Position;
                    tempObstacle.pull(player2.Position, player2);
                }
            }

            foreach (Projectile p in player1.PlayerWeapon.AllProjectiles)
            {
                foreach(Creep c in minions)
                {
                    if (p.getBoundingSphere().Intersects(c.getBoundingSphere()))
                    {
                        c.takeDamage(p);   
                    }
                }
            }

            foreach (ammoSupply a in ammoDrops)
            {
                if (a.getBoundingSphere().Intersects(player1.getBoundingSphere()))
                {
                    player1.Ammo += a.AmmoAmount;
                    a.pickedUp();
                }
            }
        }

        /// <summary>
        /// Lets game respond to player input. Only called when gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.SoundManager.pause();
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else if (input.IsLoseGame(ControllingPlayer))
            {
                ScreenManager.AddScreen(new GameOverScreen(), ControllingPlayer);
            }
            else
            {
                player1.HandleInput();
                player2.HandleInput();
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draw game screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            // Ensures that models are drawn at correct depth
            DepthStencilState depth = new DepthStencilState();
            depth.DepthBufferEnable = true;

            ScreenManager.GraphicsDevice.DepthStencilState = depth;

            ground.Draw();
            player1.Draw();
            player2.Draw();

            tempObstacle.Draw();

            foreach (Creep c in minions)
                c.Draw();
            foreach (ammoSupply a in ammoDrops)
                a.Draw();

            // Draw the temp HUD
            hud.Draw(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        #endregion
    }
}
