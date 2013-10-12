using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using CustomAvatarAnimation;

namespace Gameception
{
    class TetrisLevel : Screen
    {
        #region Attributes

        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        ContentManager content;
        SpriteFont gameFont;

        Camera camera;

        Player player1;
        Player player2;

        // The floor of the level
        private GroundObject ground;

        // The basic building block of the level
        private GameObject cube;

        // The texture for the cubes
        private Texture2D [] cubeTextures;

        // the texure for pullable cubes
        private Texture2D pullCubeTexture;

        // The model to be used for the wall
        private Model [] wallModel;

        // The model for the apples
        private Model appleModel;

        private Model pullModel;

        // All wall objects in the game
        private Collection<GameObject> walls;

        // The collectible apples in the game
        private Collection<GameObject> apples;

        // A collection of pullable objects in the game
        private Collection<PushPullObject> pullableWalls;

        // Used to generate ransdom numbers
        Random random = new Random();

        // Draws the HUD for this level
        HUD hud;

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
            wallModel = new Model[6];
            cubeTextures = new Texture2D[6];

            camera = new Camera(this.ScreenManager.Game.Graphics);
            graphics = ScreenManager.Game.Graphics;
            device = graphics.GraphicsDevice;

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("Fonts/gamefont");

            Effect effect = content.Load<Effect>("Ground/effects");
            Texture2D texture = content.Load<Texture2D>("Ground/groundtexture");
            ground = new GroundObject(device, effect, texture);

            //cube = new GameObject(content.Load<Model>("Models/Cube"), 0, 0, new Vector3(15,1.5f,0),1f,camera);
            pullCubeTexture = content.Load<Texture2D>("Cubes/cube_6");

            cubeTextures[0] = content.Load<Texture2D>("Cubes/cube_0");
            cubeTextures[1] = content.Load<Texture2D>("Cubes/cube_1");
            cubeTextures[2] = content.Load<Texture2D>("Cubes/cube_2");
            cubeTextures[3] = content.Load<Texture2D>("Cubes/cube_3");
            cubeTextures[4] = content.Load<Texture2D>("Cubes/cube_4");
            cubeTextures[5] = content.Load<Texture2D>("Cubes/cube_5");

            wallModel[0] = content.Load<Model>("Cubes/Cube0");
            wallModel[1] = content.Load<Model>("Cubes/Cube1");
            wallModel[2] = content.Load<Model>("Cubes/Cube2");
            wallModel[3] = content.Load<Model>("Cubes/Cube3");
            wallModel[4] = content.Load<Model>("Cubes/Cube4");
            wallModel[5] = content.Load<Model>("Cubes/Cube5");

            pullModel = content.Load<Model>("Models/Cube2");
            appleModel = content.Load<Model>("Models/wumpa_fruit_model");

            #if XBOX
                CustomAvatarAnimationData [] allAnimations = new CustomAvatarAnimationData[3];
                allAnimations[0] = content.Load<CustomAvatarAnimationData>("Animations/run");
                allAnimations[1] = content.Load<CustomAvatarAnimationData>("Animations/faint");
            #endif

            // player set up should move
            // also, perhaps two separate player objects for Player1 and NPC, inheriting from class Player,
            // to allow for weapon specialization
            #if WINDOWS
            player1 = new Player(content.Load<Model>("Models/player1_model"), 0.3f, 100, new Vector3(5, 3.5f, 0), 3.5f, camera, PlayerIndex.One);
            player1.setKeys(Keys.W, Keys.D, Keys.S, Keys.A, Keys.Space, PlayerIndex.One);
            Weapon player1Weapon = new Weapon(20f, content.Load<Model>("Models/sphereHighPoly"), player1);
            player1.PlayerWeapon = player1Weapon;

            player2 = new Player(content.Load<Model>("Models/npc_model"), 0.3f, 100, new Vector3(-5, 4f, 0), 0.6f, camera, PlayerIndex.Two);
            player2.setKeys(Keys.Up, Keys.Right, Keys.Down, Keys.Left, Keys.NumPad0, PlayerIndex.Two);
            Weapon player2Weapon = new Weapon(30f, content.Load<Model>("Models/sphereHighPoly"), player2);
            player2Weapon.ProjectileSpeed = 3f;
            player2.PlayerWeapon = player2Weapon;
            #else
            player1 = new Player(content.Load<Model>("Models/player1_model"), 0.3f, 100, new Vector3(5, 3.5f, 0), 3.5f, camera, PlayerIndex.One,allAnimations);
            player1.setKeys(Keys.W, Keys.D, Keys.S, Keys.A, Keys.Space, PlayerIndex.One);
            Weapon player1Weapon = new Weapon(20f, content.Load<Model>("Models/sphereHighPoly"), player1);
            player1.PlayerWeapon = player1Weapon;

            player2 = new Player(content.Load<Model>("Models/npc_model"), 0.3f, 100, new Vector3(-5, 4f, 0), 3.5f, camera, PlayerIndex.Two,allAnimations);
            player2.setKeys(Keys.Up, Keys.Right, Keys.Down, Keys.Left, Keys.NumPad0, PlayerIndex.Two);
            Weapon player2Weapon = new Weapon(30f, content.Load<Model>("Models/sphereHighPoly"), player2);
            player2Weapon.ProjectileSpeed = 3f;
            player2.PlayerWeapon = player2Weapon;
            #endif

            player1.setSoundManager(ScreenManager.SoundManager);
            player2.setSoundManager(ScreenManager.SoundManager);

            // HUD ELEMENTS
            hud = new HUD(this, ScreenManager, player1, player2);
            Texture2D[] temp_p1 = new Texture2D[6];
            Texture2D[] temp_npc = new Texture2D[6];
            for (int i = 0; i < 6; i++)
            {
                temp_p1[i] = content.Load<Texture2D>("HUD/hud_player1_" + i);
                temp_npc[i] = content.Load<Texture2D>("HUD/hud_npc_" + i);
            }
            hud.Hud_player1 = temp_p1;
            hud.Hud_npc = temp_npc;
            hud.Health_green = content.Load<Texture2D>("HUD/health_green");
            hud.Health_yellow = content.Load<Texture2D>("HUD/health_yellow");
            hud.Health_red = content.Load<Texture2D>("HUD/health_red");
            hud.SetElements(ScreenManager.GraphicsDevice.Viewport);

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
            pullableWalls = new Collection<PushPullObject>();

            Texture2D mapTexture = content.Load<Texture2D>("Levels/maze_map_small");
            Color [] map = new Color[mapTexture.Width * mapTexture.Height];
            mapTexture.GetData<Color>(map);

            float z = 6.602f;
            float x = 0;

            for (int i = 0; i < map.Length; i++)
            {
                x += 6.602f;

                if (i % 34 == 0)
                {
                    z -= 6.602f;
                    x = 0;
                }
                
                if (map[i] == Color.Black)
                {
                    Vector3 objectPosition = new Vector3(x, 2.5f, z);
                    int index = random.Next(6);
                    GameObject temp = new GameObject(wallModel[index], 0, 0, objectPosition, 1.8f, camera);
                    temp.UseAlternateTexture = true;
                    temp.AlternateTexture = cubeTextures[index];
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
                    Vector3 objectPosition = new Vector3(x, 2.5f, z);
                    GameObject temp = new GameObject(appleModel, 0, 0, objectPosition, 1.5f, camera);
                    apples.Add(temp);
                }
                else if (map[i] == Color.Blue)
                {
                    Vector3 objectPosition = new Vector3(x, 2.5f, z);
                    PushPullObject temp = new PushPullObject(pullModel, 0.15f, 0, objectPosition, 1.8f, camera, 10);
                    temp.UseAlternateTexture = true;
                    temp.AlternateTexture = pullCubeTexture;
                    pullableWalls.Add(temp);
                }
            }
        }

        #endregion

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            ScreenManager.SoundManager.play("final");

            // gradually fade in/out if covered by pause screen
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                player1.Update(gameTime);
                player2.Update(gameTime);

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

            foreach (PushPullObject pullWall in pullableWalls)
            {
                pullWall.Update();
            }

            checkCollisions();

            checkScores();

            hud.Update();
        }

        // Performs collision detection
        public void checkCollisions()
        {
            foreach (PushPullObject pullWalls in pullableWalls)
            {
                if (pullWalls.InFrustum)
                {
                    BoundingBox pullBox = BoundingBox.CreateFromSphere(pullWalls.getBoundingSphere());

                    foreach (GameObject gameObj in walls)
                    {
                        BoundingBox boundBox = BoundingBox.CreateFromSphere(gameObj.getBoundingSphere());

                        if (pullBox.Intersects(boundBox))
                        {
                            if (pullWalls.BeingThrown)
                            {
                                pullWalls.BeingThrown = false;
                                pullWalls.revertPosition();
                            }
                            else
                            {
                                pullWalls.revertPosition();
                            }
                        }
                    }
                    foreach (Projectile p2Proj in player2.PlayerWeapon.AllProjectiles)
                    {
                        if (pullBox.Intersects(p2Proj.getBoundingSphere()))
                        {
                            pullWalls.pull(player2.Position, player2);
                            p2Proj.Active = false;
                            break;
                        }
                    }
                }
            }

            foreach (GameObject apple in apples)
            {
                if (apple.InFrustum && apple.Active)
                {
                    BoundingSphere appleSphere = apple.getBoundingSphere();

                    if (appleSphere.Intersects(player1.getBoundingSphere()))
                    {
                        apple.Active = false;
                        player1.Score += 10;
                    }
                    else if (appleSphere.Intersects(player2.getBoundingSphere()))
                    {
                        apple.Active = false;
                        player2.Score += 10;
                    }
                }
            }

            foreach (GameObject gameObj in walls)
            {
                if (gameObj.InFrustum)
                {
                    BoundingBox boundBox = BoundingBox.CreateFromSphere(gameObj.getBoundingSphere());
                    //BoundingBox p1BoundBox = BoundingBox.CreateFromSphere(player1.getBoundingSphere());
                    if (boundBox.Intersects(player2.getBoundingSphere()))
                    {
                        player2.revertPosition();
                    }
                    else if (boundBox.Intersects(player1.getBoundingSphere()))
                    {
                        player1.revertPosition();
                    }
                }
            }
        }

        // Checks the scores of both players
        public void checkScores()
        {
            int totalScore = player1.Score + player2.Score;

            if (totalScore >= 50)
            {
                // Do somthing here
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
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            // Ensures that models are drawn at correct depth
            DepthStencilState depth = new DepthStencilState();
            depth.DepthBufferEnable = true;

            ScreenManager.GraphicsDevice.DepthStencilState = depth;

            ground.Draw(camera);
            //cube.Draw();

            foreach (GameObject gameObj in walls)
            {
                gameObj.Draw();
                //BoundingSphereRenderer.Render(gameObj.getBoundingSphere(), ScreenManager.GraphicsDevice, camera.View, camera.Projection, Color.Red);
            }

            foreach (PushPullObject pullWall in pullableWalls)
            {
                pullWall.Draw();
            }

            foreach (GameObject a in apples)
            {
                a.Draw();
            }

            BoundingSphereRenderer.Render(player2.getBoundingSphere(), ScreenManager.GraphicsDevice, camera.View, camera.Projection, Color.Green);
            BoundingSphereRenderer.Render(player1.getBoundingSphere(), ScreenManager.GraphicsDevice, camera.View, camera.Projection, Color.Red);

            player1.Draw();
            player2.Draw();

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
