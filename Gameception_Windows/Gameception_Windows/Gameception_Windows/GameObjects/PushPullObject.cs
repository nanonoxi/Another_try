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

        // Determines whether this object has been pulled by NPCs
        bool beingPulled;

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

        #endregion

        // Constructs a new object
        public PushPullObject(Model model, float moveSpeed, int initialHealth, Vector3 startPosition, float scale, Camera camera, int damage) :
            base(model, moveSpeed, initialHealth, startPosition, scale, camera)
        {
            damageDone = damage;
            BeingPulled = false;
        }

        // Sets all appropriate variables when the object is being pulled by NPC
        public void pull(Vector3 npcPosition)
        {
            BeingPulled = true;
            Target = npcPosition;

            direction = Vector3.Normalize(Target - Position);
        }

        // Updates this gameObject
        public override void Update()
        {
            if (BeingPulled)
            {
                if (Position != Target)
                {
                    Position += (direction * MovementSpeed);
                }
            }
            base.Update();
        }
    }
}
