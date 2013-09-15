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

        // The weapon used by this player
        Weapon playerWeapon;

        // Indicates whether the players are allowed to move, this is based on the distance between them
        private bool canMove;
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

            PlayerWeapon = null;
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

            if (Position != PreviousPosition)
            {
                PlayerFacing = Position - PreviousPosition;
            }

            PreviousPosition = Position;

            if (keyboard.IsKeyDown(Up) || (gamepad.ThumbSticks.Left.Y > 0))
            {
                Position = Position + Vector3.UnitZ * MovementSpeed;
            }
            if (keyboard.IsKeyDown(Down) || (gamepad.ThumbSticks.Left.Y < 0))
            {
                Position = Position + Vector3.UnitZ * (-MovementSpeed);
            }

            if (keyboard.IsKeyDown(Right) || (gamepad.ThumbSticks.Left.X > 0))
            {
                Position = Position + Vector3.UnitX * (-MovementSpeed);
            }
            if (keyboard.IsKeyDown(Left) || (gamepad.ThumbSticks.Left.X < 0))
            {
                Position = Position + Vector3.UnitX * MovementSpeed;
            }

            if (keyboard.IsKeyDown(Fire) || (gamepad.Triggers.Right > 0))
            {
                PlayerWeapon.fire(GameCamera, Position, PlayerFacing);
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
            base.Draw();
        }

        #endregion
    }
}
