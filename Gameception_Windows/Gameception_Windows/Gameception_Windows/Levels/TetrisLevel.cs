using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gameception
{
    class TetrisLevel : Screen
    {
        #region Attributes

        ContentManager content;
        SpriteFont gameFont;

        Camera camera;

        Player player1;
        Player player2;

        // The floor of the level
        private GameObject ground;

        // The texture for the ground
        private Texture2D groundTexture;

        // The basic building block of the level
        private GameObject cube;

        // The model to be used for the wall
        private Model wallModel;

        // The model for the apples
        private Model appleModel;

        // All wall objects in the game
        private Collection<GameObject> walls;

        // The collectible apples in the game
        private Collection<GameObject> apples;

        // The layout for this level
        private Texture2D levelLayout;

        // Draws the HUD for this level
        HUD hud;

        // Used to read the map layout
        Stream fileStream;

        float pauseAlpha;

        #endregion

        #region Initialisation

        public TetrisLevel()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            walls = new Collection<GameObject>();
        }

        public override void LoadContent()
        {
            BoundingSphereRenderer.InitializeGraphics(ScreenManager.GraphicsDevice, 30);

            camera = new Camera(this.ScreenManager.Game.Graphics);

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            groundTexture = content.Load<Texture2D>("AlternateTextures/AngelNebula");

            gameFont = content.Load<SpriteFont>("Fonts/gamefont");
            ground = new GameObject(content.Load<Model>("Models/GroundTemp"), 0, 0, Vector3.Zero, 0.1f, camera);
            ground.UseAlternateTexture = true;
            ground.AlternateTexture = groundTexture;

            cube = new GameObject(content.Load<Model>("Models/Cube"), 0, 0, new Vector3(15,1.5f,0),1f,camera);

            wallModel = content.Load<Model>("Models/Cube");
            appleModel = content.Load<Model>("Models/wumpa_fruit_model");

            // player set up should move
            // also, perhaps two separate player objects for Player1 and NPC, inheriting from class Player,
            // to allow for weapon specialization
            player1 = new Player(content.Load<Model>("Models/player1_model"), 0.3f, 100, new Vector3(5, 3.5f, 0), 3.5f, camera, PlayerIndex.One);
            player1.setKeys(Keys.W, Keys.D, Keys.S, Keys.A, Keys.Space, PlayerIndex.One);
            Weapon player1Weapon = new Weapon(20f, content.Load<Model>("Models/sphereHighPoly"), player1);
            player1.PlayerWeapon = player1Weapon;

            player2 = new Player(content.Load<Model>("Models/npc_model"), 0.3f, 100, new Vector3(-5, 4f, 0), 0.6f, camera, PlayerIndex.Two);
            player2.setKeys(Keys.Up, Keys.Right, Keys.Down, Keys.Left, Keys.NumPad0, PlayerIndex.Two);
            Weapon player2Weapon = new Weapon(30f, content.Load<Model>("Models/sphereHighPoly"), player2);
            player2Weapon.ProjectileSpeed = 3f;
            player2.PlayerWeapon = player2Weapon;

            player1.setSoundManager(ScreenManager.SoundManager);
            player2.setSoundManager(ScreenManager.SoundManager);

            hud = new HUD(ScreenManager, player1, player2);

            loadLevel();

            // reset game time after loading all assets
            ScreenManager.Game.ResetElapsedTime();
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public void loadLevel()
        {
            walls = new Collection<GameObject>();
            apples = new Collection<GameObject>();

            Color [] map = new Color[64*64];
            Texture2D mapTexture = content.Load<Texture2D>("Levels/maze_map_apples");
            mapTexture.GetData<Color>(map);

            float z = -320;
            float x = -320;

            for (int i = 0; i < map.Length; i++)
            {
                x += 6.602f;

                if (i % 64 == 0)
                {
                    z += 6.602f;
                    x = -320;
                }
                if (map[i] == Color.Black)
                {
                    Vector3 objectPosition = new Vector3(x, 4.5f, z);
                    GameObject temp = new GameObject(wallModel, 0, 0, objectPosition, 1.8f, camera);
                    temp.UseAlternateTexture = true;
                    temp.AlternateTexture = groundTexture;
                    walls.Add(temp);
                }
                else if (map[i].R == 100 && map[i].G == 100 && map[i].B == 100)
                {
                    player1.Position = new Vector3(x, player1.Position.Y, z);
                }
                else if (map[i].R == 200 && map[i].G == 200 && map[i].B == 200)
                {
                    player2.Position = new Vector3(x, player2.Position.Y, z);
                }
                else if (map[i] == Color.Red)
                {
                    Vector3 objectPosition = new Vector3(x, 1.5f, z);
                    GameObject temp = new GameObject(appleModel, 0, 0, objectPosition, 1.5f, camera);
                    apples.Add(temp);
                }
            }

            /*fileStream = TitleContainer.OpenStream("Content/Levels/level1.txt");
            StreamReader s = new StreamReader(fileStream);

            Collection<Vector3> positions = new Collection<Vector3>();
            Vector3 objectPosition = new Vector3();

            string currentLine = s.ReadLine();
            float z = -25;

            while (currentLine != null)
            {
                z += 5;
                float x = -25;

                char[] lineContent = currentLine.ToCharArray();

                foreach (char character in lineContent)
                {
                    x += 5;
                    switch (character)
                    {
                        case '*': objectPosition = new Vector3(x, 1.5f, z); 
                                  walls.Add(new GameObject(wallModel,0,0,objectPosition,1f,camera)); 
                                  break;
                        case ' ': ; break;
                    }
                }

                currentLine = s.ReadLine();
            }*/
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

                camera.Update(player1, player2);
            }

            foreach (GameObject gameobj in walls)
            {
                gameobj.Update();
            }

            foreach (GameObject a in apples)
            {
                a.Update();
            }
            ground.Update();

            //checkCollisions();
        }

        // Performs collision detection
        public void checkCollisions()
        {
            /*if (cube.getBoundingSphere().Intersects(player1.getBoundingSphere()))
            {
                player1.revertPosition();
            }*/

            foreach (GameObject gameObj in walls)
            {
                if (gameObj.InFrustrum)
                {
                    if (gameObj.getBoundingSphere().Intersects(player2.getBoundingSphere()))
                    {
                        player2.revertPosition();
                    }
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
            //cube.Draw();

            foreach (GameObject gameObj in walls)
            {
                gameObj.Draw();
                //BoundingSphereRenderer.Render(gameObj.getBoundingSphere(), ScreenManager.GraphicsDevice, camera.View, camera.Projection, Color.Red);
            }

            foreach (GameObject a in apples)
            {
                a.Draw();
            }

            player1.Draw();
            player2.Draw();

            hud.Draw(gameTime);

            BoundingSphereRenderer.Render(player2.getBoundingSphere(), ScreenManager.GraphicsDevice, camera.View, camera.Projection, Color.Green);
            BoundingSphereRenderer.Render(player1.getBoundingSphere(), ScreenManager.GraphicsDevice, camera.View, camera.Projection, Color.Red);

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
