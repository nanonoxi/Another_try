using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gameception
{
    class Weapon
    {
        #region Attributes

        // The gameObject to which this weapon belongs
        GameObject owner;

        // Position of the weapon holder
        private Vector3 ownerPosition;

        // The rate of fire of the weapon
        private float cooldownTime;

        // The model to represent the projectile
        private Model projectileModel;

        // Used to determine whether or not a weapon can be fired
        private int elapsedFrames;

        // The speed of a single projectile
        private float projectileSpeed;

        // All the projectiles created by this weapon
        Collection<Projectile> allProjectiles;

        #endregion

        #region Properties

        public GameObject Owner
        {
            get { return owner; }
            set { owner = value; }
        }

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

        public float ProjectileSpeed
        {
            get { return projectileSpeed; }
            set { projectileSpeed = value; }
        }

        public Collection<Projectile> AllProjectiles
        {
            get { return allProjectiles; }
            set { allProjectiles = value; }
        }

        #endregion

        public Weapon(float cooldown, Model modelOfProjectile, GameObject obj)
        {
            CooldownTime = cooldown;
            elapsedFrames = 0;

            Owner = obj;

            ProjectileSpeed = 1.5f;

            ProjectileModel = modelOfProjectile;
            allProjectiles = new Collection<Projectile>();
        }

        // Controls the fire of the weapon
        public void fire(Camera gameCamera, Vector3 startPosition, Vector3 shotDirection)
        {
            if (elapsedFrames >= CooldownTime)
            {
                // Create projectile
                startPosition.Y += 1.5f;
                allProjectiles.Add(new Projectile(ProjectileModel, ProjectileSpeed, 0, 10f, startPosition, 0.1f, gameCamera, shotDirection));

                // Play sound here

                ((Player)Owner).Ammo--;
                elapsedFrames = 0;
            }
        }

        // Update the state of the weapon
        public void Update()
        {
            elapsedFrames++;
            Collection<Projectile> inActive = new Collection<Projectile>();

            for (int i = 0; i < allProjectiles.Count; i++)
            {
                if (allProjectiles[i].Active)
                {
                    allProjectiles[i].Update();
                }
                else
                {
                    allProjectiles.RemoveAt(i);
                }
            }

            /*foreach (Projectile proj in allProjectiles)
            {
                if (proj.Active)
                {
                    proj.Update();
                }
                else
                {
                    inActive.Add(proj);
                }
            }

            // Remove in active projectiles from the game
            foreach (Projectile p in inActive)
            {
                allProjectiles.Remove(p);
            }*/
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
