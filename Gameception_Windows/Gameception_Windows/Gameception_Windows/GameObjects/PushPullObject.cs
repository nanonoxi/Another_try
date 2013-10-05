using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gameception
{
    class PushPullObject : GameObject
    {
        #region Attribute

        // The amount of damage that this object can do
        int damageDone;

        // Where this object has to be pulled tos
        Vector3 target;

        // The direction in which the object should move
        Vector3 direction;

        Player player2;

        // Determines whether this object has been pulled by NPC
        bool beingPulled;

        // Determines whether this object is being held by NPC
        bool beingHeld;

        // Determines whether this object is being throws
        bool beingThrown;

        // The direction in which this object is moving
        Vector3 moveDirection;

        // Allows for a thrown object to gradually slow dows
        float speedDecrement;

        public int DamageDone
        {
            get { return damageDone; }
            set { damageDone = value; }
        }

        public Vector3 Target
        {
            get { return target; }
            set { target = value; }
        }

        public bool BeingPulled
        {
            get { return beingPulled; }
            set { beingPulled = value;}
        }

        public bool BeingHeld
        {
            get { return beingHeld; }
            set { beingHeld = value; }
        }

        public bool BeingThrown
        {
            get { return beingThrown; }
            set { beingThrown = value; }
        }

        #endregion

        // Constructs a new object
        public PushPullObject(Model model, float moveSpeed, int initialHealth, Vector3 startPosition, float scale, Camera camera, int damage) :
            base(model, moveSpeed, initialHealth, startPosition, scale, camera)
        {
            damageDone = damage;
            BeingPulled = false;
        }

        #region Response to Abilities

        // Sets all appropriate variables when the object is being pulled by NPC
        public void pull(Vector3 npcPosition, Player p2)
        {
            /*BeingPulled = true;
            Target = npcPosition;
            player2 = p2;

            //direction = Vector3.Normalize(Target - Position);

            Vector3 temp = player2.Position;
            temp.Y += player2.getBoundingSphere().Radius;
            direction = Vector3.Normalize(temp - Position);*/

            // Player 2 can only hold one object at a time and the ability must still be activated (indicated by an inability to move)
            if (p2.ObjectHeld == false && p2.CanMove == false)
            {
                BeingThrown = false;
                BeingHeld = true;
                player2 = p2;
                player2.ObjectHeld = true;
            }
        }

        // Throws this object in a particular direction
        public void push()
        {
            bool throwObject = handleControls();
            
            if (throwObject == true)
            {
                BeingThrown = true;
                speedDecrement = 0;
            }
        }

        #endregion

        // Updates this gameObject
        public override void Update()
        {
            // Second parameter indicates that player2 is trying to pull something
            if (BeingPulled)
            {
                /*if (!(this.getBoundingSphere().Intersects(player2.getBoundingSphere())))
                {
                    Position += (direction * MovementSpeed);
                }
                else*/
                {
                    player2.CanMove = false;
                    player2.ObjectHeld = true;
                    BeingPulled = false;
                    BeingHeld = true;
                }
            }
            else if (BeingHeld && player2.ObjectHeld)
            {
                /*if (!(this.getBoundingSphere().Intersects(player2.getBoundingSphere())))
                {
                    Vector3 temp = player2.Position;
                    temp.Y += player2.getBoundingSphere().Radius;

                    //direction = Vector3.Normalize(player2.Position - Position);
                    direction = Vector3.Normalize(temp - Position);
                    Position += (direction * MovementSpeed);
                }*/
                handleControls();
            }
            else if (BeingThrown)
            {
                if (speedDecrement < (MovementSpeed * 20))
                {
                    Position = Position + moveDirection * ((MovementSpeed * 20) - speedDecrement);
                    speedDecrement += 0.01f;
                }
                else
                {
                    BeingThrown = false;
                }
            }
            else
            {
                // First check if the object was being held before releasing it
                if (BeingHeld == true)
                {
                    push();
                }

                BeingHeld = false;
                BeingPulled = false;
            }
            
            base.Update();
        }

        // Responds to player control
        public bool handleControls()
        {
            bool directionHeld = false;
            moveDirection = Vector3.Zero;

            if (BeingHeld)
            {
                KeyboardState keyboard = Keyboard.GetState();
                GamePadState gamepad = GamePad.GetState(PlayerIndex.Two);

                Keys Up = player2.Up;
                Keys Down = player2.Down;
                Keys Left = player2.Left;
                Keys Right = player2.Right;

                if (Position != PreviousPosition)
                {
                    moveDirection = Position - PreviousPosition;
                }

                PreviousPosition = Position;

                if (keyboard.IsKeyDown(Up) || (gamepad.ThumbSticks.Left.Y > 0))
                {
                    Position = Position + Vector3.UnitZ * MovementSpeed;
                    directionHeld = true;
                }
                else if (keyboard.IsKeyDown(Down) || (gamepad.ThumbSticks.Left.Y < 0))
                {
                    Position = Position + Vector3.UnitZ * (-MovementSpeed);
                    directionHeld = true;
                }

                if (keyboard.IsKeyDown(Right) || (gamepad.ThumbSticks.Left.X > 0))
                {
                    Position = Position + Vector3.UnitX * (-MovementSpeed);
                    directionHeld = true;
                }
                else if (keyboard.IsKeyDown(Left) || (gamepad.ThumbSticks.Left.X < 0))
                {
                    Position = Position + Vector3.UnitX * MovementSpeed;
                    directionHeld = true;
                }
            }

            return directionHeld;
        }
    }
}
