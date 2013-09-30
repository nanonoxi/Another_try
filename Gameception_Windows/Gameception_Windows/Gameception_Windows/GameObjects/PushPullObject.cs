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

        #endregion

        // Constructs a new object
        public PushPullObject(Model model, float moveSpeed, int initialHealth, Vector3 startPosition, float scale, Camera camera, int damage) :
            base(model, moveSpeed, initialHealth, startPosition, scale, camera)
        {
            damageDone = damage;
            BeingPulled = false;
        }

        // Sets all appropriate variables when the object is being pulled by NPC
        public void pull(Vector3 npcPosition, Player p2)
        {
            BeingPulled = true;
            Target = npcPosition;

            direction = Vector3.Normalize(Target - Position);
            player2 = p2;
        }

        // Updates this gameObject
        public override void Update()
        {
            // Second parameter indicates that player2 is trying to pull something
            if (BeingPulled)
            {
                if (!(this.getBoundingShpere().Intersects(player2.getBoundingShpere())))
                {
                    Position += (direction * MovementSpeed);
                }
                else
                {
                    player2.CanMove = true;
                    player2.ObjectHeld = true;
                    BeingPulled = false;
                    BeingHeld = true;
                }
            }
            else if (BeingHeld && player2.ObjectHeld)
            {
                if (!(this.getBoundingShpere().Intersects(player2.getBoundingShpere())))
                {
                    Vector3 temp = player2.Position;
                    temp.Y += player2.getBoundingShpere().Radius;

                    //direction = Vector3.Normalize(player2.Position - Position);
                    direction = Vector3.Normalize(temp - Position);
                    Position += (direction * MovementSpeed);
                }
            }
            else
            {
                BeingHeld = false;
                BeingPulled = false;
            }
            base.Update();
        }
    }
}
