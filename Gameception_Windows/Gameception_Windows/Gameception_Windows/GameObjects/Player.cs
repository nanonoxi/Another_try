using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using CustomAvatarAnimation;

namespace Gameception
{
    class Player : GameObject
    {
        #region Attributes

        // Used to determine which player this object represents
        private PlayerIndex playerIndex;

        // Handles the sound for this player
        SoundManager soundManager;

        // player control keys
        public Keys Up, Right, Down, Left, Fire;

        // The direction the player is facing, used for firing weapons
        Vector3 playerFacing;

        // The rotation of the model
        float rotationAngle;
        
        // The angle at which to shoot
        float shootAngle;

        // The weapon used by this player
        Weapon playerWeapon;

        // Object held, only used by player 2
        bool objectHeld;

        // Indicates whether the players are allowed to move, this is based on the distance between them
        private bool canMove;

        // The ammount of ammunition the player has
        private int ammo;

        // The score of this player
        private int score;

        // Used to check whether or not the player is idle;
        private bool isIdle;

        // Avatar associated with this player
        AvatarDescription avatarDescription;
        AvatarRenderer avatarRenderer;
        AvatarAnimation avatarAnimation;
        CustomAvatarAnimationData [] allAnimationData;

        IAvatarAnimation[] animations;

        CustomAvatarAnimationPlayer runPlayer;
        CustomAvatarAnimationPlayer faintPlayer;

        // All the possible animations of the avatar
        private enum Animation {NONE, RUN, FAINT, THROW };
        
        // The current animation of the player
        private Animation currentAnimation;
        
        #endregion

        #region Properties

        public bool ObjectHeld
        {
            get { return objectHeld; }
            set { objectHeld = value; }
        }

        public bool CanMove
        {
            get { return canMove; }
            set { canMove = value; }
        }

        public Vector3 PlayerFacing
        {
            get { return playerFacing; }
            set { playerFacing = value; }
        }

        public Weapon PlayerWeapon
        {
            get { return playerWeapon; }
            set { playerWeapon = value; }
        }

        public int Ammo
        {
            get { return ammo; }
            set { ammo = value; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        #endregion

        #region Initialization

        public Player(Model model, float moveSpeed, int initialHealth, Vector3 startPosition, float scale, Camera camera, PlayerIndex player)
            : base(model, moveSpeed, initialHealth, startPosition, scale, camera)
        {
            playerIndex = player;
            ammo = 10;

            avatarDescription = AvatarDescription.CreateRandom(AvatarBodyType.Male);
            avatarAnimation = new AvatarAnimation(AvatarAnimationPreset.MaleIdleLookAround);
            avatarRenderer = new AvatarRenderer(avatarDescription);
            avatarRenderer.Projection = camera.Projection;
            avatarRenderer.View = camera.View;
            avatarRenderer.World = Matrix.CreateScale(4f);

            isIdle = true;
        }

        // constructor on xbox
        public Player(Model model, float moveSpeed, int initialHealth, Vector3 startPosition, float scale, Camera camera, PlayerIndex player, CustomAvatarAnimationData[] animationData)
            : base(model, moveSpeed, initialHealth, startPosition, scale, camera)
        {
            playerIndex = player;
            ammo = 10;

            isIdle = true;

            if (playerIndex == PlayerIndex.One)
            {
                avatarDescription = AvatarDescription.CreateRandom(AvatarBodyType.Male);
                avatarAnimation = new AvatarAnimation(AvatarAnimationPreset.Stand1);
            }
            else
            {
                avatarDescription = AvatarDescription.CreateRandom(AvatarBodyType.Female);
                avatarAnimation = new AvatarAnimation(AvatarAnimationPreset.Stand1);
            }

            avatarRenderer = new AvatarRenderer(avatarDescription);
            avatarRenderer.Projection = camera.Projection;
            avatarRenderer.View = camera.View;
            avatarRenderer.World = Matrix.CreateScale(4f);

            allAnimationData = animationData;

            runPlayer = new CustomAvatarAnimationPlayer(allAnimationData[0].Name, allAnimationData[0].Length, allAnimationData[0].Keyframes, allAnimationData[0].ExpressionKeyframes);
            faintPlayer = new CustomAvatarAnimationPlayer(allAnimationData[1].Name, allAnimationData[1].Length, allAnimationData[1].Keyframes, allAnimationData[1].ExpressionKeyframes);

            this.Position = new Vector3(Position.X, 0 , Position.Z);
            currentAnimation = Animation.NONE;

            avatarBoundingSphere();
        }

        // calculate the bounding sphere for this players avatar (XBOX only)
        private void avatarBoundingSphere()
        {
            BoundingSphere avatarSphere = new BoundingSphere();
            avatarSphere.Center = Position;
            avatarSphere.Radius = (avatarDescription.Height / 2) * ScaleFactor;
            this.ObjectBoundingSphere = avatarSphere;
        }

        /// <summary>
        /// Set the player's keyboard control keys
        /// </summary>
        /// <param name="u"></param>Up
        /// <param name="r"></param>Right
        /// <param name="d"></param>Down
        /// <param name="l"></param>Left
        /// <param name="f"></param>Fire
        /// <param name="i"></param>Player Index
        public void setKeys(Keys u, Keys r, Keys d, Keys l, Keys f, PlayerIndex i)
        {
            Up = u;
            Right = r;
            Down = d;
            Left = l;
            Fire = f;
            playerIndex = i;

            rotationAngle = 0f;

            PlayerWeapon = null;
            CanMove = true;
            ObjectHeld = false;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            if (Position.X < 3 || Position.X > 220)
            {
                revertPosition();
            }
            if (Position.Z > -3 || Position.Z < -198.06)
            {
                revertPosition();
            }

            if (PlayerWeapon != null)
            {
                PlayerWeapon.Update();
            }

            #if XBOX
                switch (currentAnimation)
                {
                    case Animation.NONE: break;
                    case Animation.RUN: runPlayer.Update(gameTime.ElapsedGameTime, true); break;
                    case Animation.FAINT: faintPlayer.Update(gameTime.ElapsedGameTime, false); break;
                }

                if (currentAnimation != Animation.FAINT)
                {
                    if (faintPlayer.CurrentPosition >= System.TimeSpan.Zero)
                    {
                        faintPlayer.CurrentPosition = System.TimeSpan.Zero;
                    }
                }
            #endif

            //avatarAnimation.Update(gameTime.ElapsedGameTime, true);
            avatarRenderer.Projection = GameCamera.Projection;
            avatarRenderer.View = GameCamera.View;

            this.Update();
        }

        public void setSoundManager(SoundManager s)
        {
            this.soundManager = s;
        }

        public void HandleInput()
        {
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamepad = GamePad.GetState(playerIndex, GamePadDeadZone.Circular);

            if (CanMove == true)
            {
                if (Position != PreviousPosition)
                {
                    isIdle = false;
                    PlayerFacing = Position - PreviousPosition;
                }
                else
                {
                    isIdle = true;
                    currentAnimation = Animation.NONE;
                }

                PreviousPosition = Position;

                float gamePadX = gamepad.ThumbSticks.Left.X;
                float gamePadY = gamepad.ThumbSticks.Left.Y;

                if ((gamePadX != 0) || (gamePadY != 0))
                {
                    rotationAngle = (float)(Math.Atan2(-gamePadX, gamePadY));
                    shootAngle = rotationAngle;

                    #if XBOX
                        rotationAngle = (float)(Math.Atan2(gamePadX, -gamePadY));
                    #endif
                }

                #if WINDOWS
                if (gamepad.IsConnected == false)
                {
                    if (keyboard.IsKeyDown(Up) || (gamepad.ThumbSticks.Left.Y > 0))
                    {
                        Matrix moveForward = Matrix.CreateRotationY(rotationAngle);
                        Vector3 velocityVector = new Vector3(0, 0, MovementSpeed);
                        velocityVector = Vector3.Transform(velocityVector, moveForward);
                        Vector3 tempPosition = new Vector3(Position.X + velocityVector.X, Position.Y, Position.Z + velocityVector.Z);
                        Position = tempPosition;
                    }
                    if (keyboard.IsKeyDown(Right) || (gamepad.ThumbSticks.Left.X > 0))
                    {
                        rotationAngle -= MathHelper.ToRadians(2f);
                    }
                    else if (keyboard.IsKeyDown(Left) || (gamepad.ThumbSticks.Left.X < 0))
                    {
                        rotationAngle += MathHelper.ToRadians(2f);
                    }
                }
                else
                {
                    Position = new Vector3(Position.X - (gamepad.ThumbSticks.Left.X * MovementSpeed), Position.Y, Position.Z + (gamepad.ThumbSticks.Left.Y * MovementSpeed));
                }
                #else
                    Position = new Vector3(Position.X - (gamepad.ThumbSticks.Left.X * MovementSpeed), Position.Y, Position.Z + (gamepad.ThumbSticks.Left.Y * MovementSpeed));
                    
                    if ((gamepad.ThumbSticks.Left.X != 0) && (gamepad.ThumbSticks.Left.Y != 0) && (CanMove == true))
                    {
                        isIdle = false;
                        currentAnimation = Animation.RUN;
                    }
                #endif
            }

            // This needs to be outside the if so that the release of the button can be detected
            if (keyboard.IsKeyDown(Fire) || (gamepad.Triggers.Right > 0))
            {
                if (playerIndex == PlayerIndex.One)
                {
                    if (ammo > 0)
                    {
                        Matrix forward = Matrix.CreateRotationY(shootAngle);
                        Vector3 shootingDirection = new Vector3(0, 0, MovementSpeed);
                        shootingDirection = Vector3.Transform(shootingDirection, forward);

                        PlayerWeapon.fire(GameCamera, Position, shootingDirection);
                    }
                    else
                    {
                        // Play sound here that indicates ammo is finished
                    }
                }
                else if (playerIndex == PlayerIndex.Two && ObjectHeld == false) // Player 2 can't move while pulling an object
                {
                    Matrix forward = Matrix.CreateRotationY(shootAngle);
                    Vector3 shootingDirection = new Vector3(0, 0, MovementSpeed);
                    shootingDirection = Vector3.Transform(shootingDirection, forward);

                    PlayerWeapon.fire(GameCamera, Position, shootingDirection);
                    CanMove = false;

                    isIdle = false;
                    currentAnimation = Animation.FAINT;
                }
            }
            else
            {
                ObjectHeld = false;
                CanMove = true;
            }
        }

        #endregion

        #region Draw

        public override void Draw()
        {
            if (PlayerWeapon != null)
            {
                PlayerWeapon.Draw();
            }

            Matrix[] transforms = new Matrix[ObjectModel.Bones.Count];
            ObjectModel.CopyAbsoluteBoneTransformsTo(transforms);

            #if WINDOWS
                Matrix rotation = Matrix.CreateRotationY(rotationAngle);
            #else
                Matrix rotation = Matrix.CreateRotationY(rotationAngle);
            #endif

            // Only draw a gameObject if it's active
            if (Active)
            {
                if (InFrustum)
                {
                    #if WINDOWS
                    foreach (ModelMesh mesh in ObjectModel.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();

                            effect.View = GameCamera.View;
                            effect.Projection = GameCamera.Projection;
                            effect.World = rotation * transforms[mesh.ParentBone.Index] * Matrix.CreateScale(ScaleFactor) * Matrix.CreateTranslation(Position);
                        }

                        mesh.Draw();
                    }
                    #else
                        avatarRenderer.Projection = GameCamera.Projection;
                        avatarRenderer.View = GameCamera.View;
                        avatarRenderer.World = rotation * Matrix.CreateScale(4f) * Matrix.CreateTranslation(Position);
                        if(isIdle)
                        {
                            avatarRenderer.Draw(avatarAnimation);
                        }
                        else
                        {
                            switch(currentAnimation)
                            {
                                case Animation.RUN: avatarRenderer.Draw(runPlayer); break;
                                case Animation.FAINT: avatarRenderer.Draw(faintPlayer); break;
                                default:avatarRenderer.Draw(avatarAnimation); break;
                            }
                        }
                    #endif
                }
            }

            //base.Draw();
        }

        #endregion
    }
}
