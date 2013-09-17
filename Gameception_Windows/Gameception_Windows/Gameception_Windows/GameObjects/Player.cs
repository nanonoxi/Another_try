using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gameception
{
    class Player : GameObject
    {
        #region Attributes

        // Used to determine which player this object represents
        private PlayerIndex playerIndex;

        // player control keys
        Keys Up, Right, Down, Left, Fire;

        // The direction the player is facing, used for firing weapons
        Vector3 playerFacing;

        // The rotation of the model
        float rotationAngle;

        // The weapon used by this player
        Weapon playerWeapon;

        // Object held, only used by player 2
        bool objectHeld;

        // Indicates whether the players are allowed to move, this is based on the distance between them
        private bool canMove;

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

        #endregion

        #region Initialization

        public Player(Model model, float moveSpeed, int initialHealth, Vector3 startPosition, float scale, Camera camera, PlayerIndex player)
            : base(model, moveSpeed, initialHealth, startPosition, scale, camera)
        {
            playerIndex = player;
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

        public override void Update()
        {
            if (PlayerWeapon != null)
            {
                PlayerWeapon.Update();
            }

            base.Update();
        }

        public void HandleInput()
        {
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamepad = GamePad.GetState(playerIndex);

            if (CanMove == true)
            {
                if (Position != PreviousPosition)
                {
                    PlayerFacing = Position - PreviousPosition;
                }

                PreviousPosition = Position;

                if (keyboard.IsKeyDown(Up) || (gamepad.ThumbSticks.Left.Y > 0))
                {
                    Position = Position + Vector3.UnitZ * MovementSpeed;
                    rotationAngle = 0f;
                }
                else if (keyboard.IsKeyDown(Down) || (gamepad.ThumbSticks.Left.Y < 0))
                {
                    Position = Position + Vector3.UnitZ * (-MovementSpeed);
                    rotationAngle = 180f;
                }

                if (keyboard.IsKeyDown(Right) || (gamepad.ThumbSticks.Left.X > 0))
                {
                    Position = Position + Vector3.UnitX * (-MovementSpeed);
                    rotationAngle = 270f;
                }
                else if (keyboard.IsKeyDown(Left) || (gamepad.ThumbSticks.Left.X < 0))
                {
                    Position = Position + Vector3.UnitX * MovementSpeed;
                    rotationAngle = 90f;
                }
            }

            // This needs to be outside the if so that the release of the button can be detected
            if (keyboard.IsKeyDown(Fire) || (gamepad.Triggers.Right > 0))
            {
                if (playerIndex == PlayerIndex.One)
                {
                    PlayerWeapon.fire(GameCamera, Position, PlayerFacing);
                }
                else if (playerIndex == PlayerIndex.Two && ObjectHeld == false) // Player 2 can't move while pulling an object
                {
                    PlayerWeapon.fire(GameCamera, Position, PlayerFacing);
                    CanMove = false;
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

            Matrix rotation = Matrix.CreateRotationY(MathHelper.ToRadians(rotationAngle));

            // Only draw a gameObject if it's active
            if (Active)
            {
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
            }

            //base.Draw();
        }

        #endregion
    }
}
