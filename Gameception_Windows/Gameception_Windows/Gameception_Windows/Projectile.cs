using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gameception
{
    class Projectile : GameObject
    {
        // The amount of damage that this weapon does
        private float damageAmount;

        // The time the projectile remains active
        private int timeToLive;

        // The direction in which the projectile should move
        private Vector3 direction;

        // The number of frames that have elapsed since the projectile was created
        private int elapsedFrames;

        #region Properties

        public float DamageAmount
        {
            get { return damageAmount; }
            set { damageAmount = value; }
        }

        public int TimeToLive
        {
            get { return timeToLive; }
            set { timeToLive = value; }
        }

        #endregion

        public Projectile(Model model, float moveSpeed, int initialHealth, float damageDone, Vector3 startPosition, float scale, Camera camera, Vector3 movementDirection) 
            :base(model, moveSpeed, initialHealth, startPosition, scale, camera)
        {
            DamageAmount = damageDone;
            direction = movementDirection;

            TimeToLive = 120;
        }

        // Update the state of this projectile
        public void Update()
        {
            elapsedFrames++;

            if (elapsedFrames >= TimeToLive)
            {
                this.Active = false;
            }
            else
            {
                this.Position = this.Position + MovementSpeed * direction;
            }
        }
    }
}
