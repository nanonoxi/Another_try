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

        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        Effect effect;

        Texture2D groundTexture;
        VertexBuffer groundVertexBuffer;
        int groundWidth = 10;
        int groundLength = 10;

        ContentManager content;
        SpriteFont gameFont;

        Camera camera;
        Player player1;
        Player player2;

        float pauseAlpha;

        #endregion

        #region Initialization

        public TemplateLevel()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            camera = new Camera(this.ScreenManager.Game.Graphics);
            graphics = ScreenManager.Game.Graphics;
            device = graphics.GraphicsDevice;

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            effect = content.Load<Effect>("Ground/effects");
            groundTexture = content.Load<Texture2D>("Ground/groundtexture");
            SetUpVertices();

            gameFont = content.Load<SpriteFont>("Fonts/gamefont");

            player1 = new Player(content.Load<Model>("Models/player1"), 1f, 100, new Vector3(5, 3.5f, 0), 3.5f, camera, PlayerIndex.One);
            player1.setKeys(Keys.W, Keys.D, Keys.S, Keys.A, Keys.Space, PlayerIndex.One);
            Weapon player1Weapon = new Weapon(20f, content.Load<Model>("Models/sphereHighPoly"), player1);
            player1.PlayerWeapon = player1Weapon;

            player2 = new Player(content.Load<Model>("Models/npcModel"), 1f, 100, new Vector3(-5, 4f, 0), 0.6f, camera, PlayerIndex.Two);
            player2.setKeys(Keys.Up, Keys.Right, Keys.Down, Keys.Left, Keys.NumPad0, PlayerIndex.Two);
            Weapon player2Weapon = new Weapon(20f, content.Load<Model>("Models/sphereHighPoly"), player2);
            player2.PlayerWeapon = player2Weapon;

            player1.setSoundManager(ScreenManager.SoundManager);
            player2.setSoundManager(ScreenManager.SoundManager);

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

        private void SetUpVertices()
        {
            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture>();
            int i = 25; // set size of vertex increments
            for (int x = 0; x < groundWidth; x++)
            {
                for (int z = 0; z < groundLength; z++)
                {
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x * i, 0, (-z) * i), new Vector3(0, 1, 0), new Vector2(0, 1)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x * i, 0, (-z - 1) * i), new Vector3(0, 1, 0), new Vector2(0, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3((x + 1) * i, 0, (-z) * i), new Vector3(0, 1, 0), new Vector2(1.0f, 1)));

                    verticesList.Add(new VertexPositionNormalTexture(new Vector3(x * i, 0, (-z - 1) * i), new Vector3(0, 1, 0), new Vector2(0, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3((x + 1) * i, 0, (-z - 1) * i), new Vector3(0, 1, 0), new Vector2(1.0f, 0)));
                    verticesList.Add(new VertexPositionNormalTexture(new Vector3((x + 1) * i, 0, (-z) * i), new Vector3(0, 1, 0), new Vector2(1.0f, 1)));
                }
            }

            groundVertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, verticesList.Count, BufferUsage.WriteOnly);

            groundVertexBuffer.SetData<VertexPositionNormalTexture>(verticesList.ToArray());
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
                player1.Update();
                player2.Update();

                camera.Update(player1, player2);

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

            DrawGround();
            player1.Draw();
            player2.Draw();
    
            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        private void DrawGround()
        {
            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xView"].SetValue(camera.View);//(viewMatrix);
            effect.Parameters["xProjection"].SetValue(camera.Projection);//(projectionMatrix);
            effect.Parameters["xTexture"].SetValue(groundTexture);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(groundVertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, groundVertexBuffer.VertexCount / 3);
            }
        }

        #endregion
    }
}
