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

        // Stores the direction that the player is facing
        Vector3 playerFacing;

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

        public Vector3 PlayerFacing
        {
            get { return playerFacing; }
            set { playerFacing = value; }
        }

        #endregion

        public Player(Model model, float moveSpeed, int initialHealth, Vector3 startPosition, float scale, Camera camera, PlayerIndex player, Model projectileModel, int weaponCooldown)
            : base(model, moveSpeed, initialHealth, startPosition, scale, camera)
        {
            playerNumber = player;

            PlayerWeapon = new Weapon(weaponCooldown, projectileModel); // Send model of projectile as well, to fix the error
        }

        // Handles input from the user
        public void handleInput(KeyboardState keyboard, GamePadState gamepad)
        {
            if (Position != PreviousPosition)
            {
                PlayerFacing = Position - PreviousPosition;
            }

            PreviousPosition = Position;

#if WINDOWS
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
                    PlayerWeapon.fire(this.GameCamera, this.Position, PlayerFacing);
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

#endif

            ///// IF THE CODE BELOW DOES NOT WORK THEN I WILL TRY THIS /////
            /*if (gamepad.ThumbSticks.Left.X > 0)
            {
                Position = Position + Vector3.UnitX * (-MovementSpeed);
            }
            if (gamepad.ThumbSticks.Left.X < 0)
            {
                Position = Position + Vector3.UnitX * MovementSpeed;
            }
            if (gamepad.ThumbSticks.Left.Y > 0)
            {
                Position = Position + Vector3.UnitZ * MovementSpeed;
            }
            if (gamepad.ThumbSticks.Left.Y < 0)
            {
                Position = Position + Vector3.UnitZ * (-MovementSpeed);
            }*/
            //////////////////////////////////////////////////////////////////

#if XBOX
            // GamePads are identified for each player before being sent to this class so if statements are not necessarry

            if ((gamepad.ThumbSticks.Left.X != 0) && (gamepad.ThumbSticks.Left.Y != 0))
            {
                Position = new Vector3(gamepad.ThumbSticks.Left.X * MovementSpeed, Position.Y, gamepad.ThumbSticks.Left.Y);
            }

            // Fire a projectile
            if (gamepad.Triggers.Right > 0)
            {
                PlayerWeapon.fire(this.GameCamera, this.Position, PlayerFacing);
            }
#endif
        }

        // Update this player
        public void Update()
        {
            PlayerWeapon.Update();
        }

        // Draw the player
        public override void Draw()
        {
            foreach (ModelMesh mesh in ObjectModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = GameCamera.View;
                    effect.Projection = GameCamera.Projection;
                    effect.World = Matrix.CreateScale(ScaleFactor) * Matrix.CreateTranslation(Position);
                }

                mesh.Draw();
            }

            PlayerWeapon.Draw();
        }
    }
}
