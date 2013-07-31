using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gameception
{
    class Weapon
    {
        // Position of the weapon holder
        private Vector3 ownerPosition;

        // The rate of fire of the weapon
        private float cooldownTime;

        // The model to represent the projectile
        private Model projectileModel;

        // Used to determine whether or not a weapon can be fired
        private int elapsedFrames;

        // All the projectiles created by this weapon
        Collection<Projectile> allProjectiles;

        #region Properties

        public Vector3 OwnerPosition
        {
            get { return ownerPosition; }
            set { ownerPosition = value; }
        }

        public float CooldownTime
        {
            get { return cooldownTime; }
            set { cooldownTime = value; }
        }

        public Model ProjectileModel
        {
            get { return projectileModel; }
            set { projectileModel = value; }
        }

        #endregion

        public Weapon(float cooldown, Model modelOfProjectile)
        {
            CooldownTime = cooldown;
            elapsedFrames = 0;

            ProjectileModel = modelOfProjectile;
            allProjectiles = new Collection<Projectile>();
        }

        // Update the state of the weapon
        public void Update()
        {
            elapsedFrames++;

            foreach (Projectile proj in allProjectiles)
            {
                proj.Update();
            }
        }

        // Controls the fire of the weapon
        public void fire(Camera gameCamera)
        {
            if (elapsedFrames >= CooldownTime)
            {
                // Create projectile
                allProjectiles.Add(new Projectile(ProjectileModel,0.04f,0,10f,Vector3.Zero,0.8f,gameCamera,Vector3.UnitZ));

                elapsedFrames = 0;
            }
        }

        // Draw the projectiles created by this weapon
        public void Draw()
        {
            foreach (Projectile proj in allProjectiles)
            {
                proj.Draw();
            }
        }
    }
}
