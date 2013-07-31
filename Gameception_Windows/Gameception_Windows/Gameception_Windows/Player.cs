using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gameception
{
    class Player : GameObject
    {
        // Used to determine which player this object represents
        private PlayerIndex playerNumber;

        // Indicates whether the players are allowed to move, this is based on the distance between them
        private bool canMove;

        // The weapon used by this player
        Weapon playerWeapon;

        #region Properties

        public bool CanMove
        {
            get { return canMove; }
            set { canMove = value; }
        }

        public Weapon PlayerWeapon
        {
            get { return playerWeapon; }
            set { playerWeapon = value; }
        }

        #endregion

        public Player(Model model, float moveSpeed, int initialHealth, Vector3 startPosition, float scale, Camera camera, PlayerIndex player, int weaponCooldown)
            : base(model, moveSpeed, initialHealth, startPosition, scale, camera)
        {
            playerNumber = player;

            //PlayerWeapon = new Weapon(weaponCooldown); // Send model of projectile as well, to fix the error
        }

        // Handles input from the user
        public void handleInput(KeyboardState keyboard, GamePadState gamepad)
        {
            PreviousPosition = Position;

            if (playerNumber == PlayerIndex.One)
            {
                if (keyboard.IsKeyDown(Keys.W))
                {
                    Position = Position + Vector3.UnitZ * MovementSpeed;
                }
                if (keyboard.IsKeyDown(Keys.A))
                {
                    Position = Position + Vector3.UnitX * MovementSpeed;
                }
                if (keyboard.IsKeyDown(Keys.S))
                {
                    Position = Position + Vector3.UnitZ * (-MovementSpeed);
                }
                if (keyboard.IsKeyDown(Keys.D))
                {
                    Position = Position + Vector3.UnitX * (-MovementSpeed);
                }
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    //PlayerWeapon.fire();
                }
            }
            else if (playerNumber == PlayerIndex.Two)
            {
                if (keyboard.IsKeyDown(Keys.Up))
                {
                    Position = Position + Vector3.UnitZ * MovementSpeed;
                }
                if (keyboard.IsKeyDown(Keys.Left))
                {
                    Position = Position + Vector3.UnitX * MovementSpeed;
                }
                if (keyboard.IsKeyDown(Keys.Down))
                {
                    Position = Position + Vector3.UnitZ * (-MovementSpeed);
                }
                if (keyboard.IsKeyDown(Keys.Right))
                {
                    Position = Position + Vector3.UnitX * (-MovementSpeed);
                }
            }
        }
    }
}
