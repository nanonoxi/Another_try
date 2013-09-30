﻿using System;
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

            ProjectileModel = modelOfProjectile;
            allProjectiles = new Collection<Projectile>();
        }

        // Controls the fire of the weapon
        public void fire(Camera gameCamera, Vector3 startPosition, Vector3 shotDirection)
        {
            if (elapsedFrames >= CooldownTime)
            {
                // Create projectile
                allProjectiles.Add(new Projectile(ProjectileModel, 1.5f, 0, 10f, startPosition, 0.1f, gameCamera, shotDirection));

                // Play sound here

                ((Player)Owner).Ammo--;
                elapsedFrames = 0;
            }
        }

        // Update the state of the weapon
        public void Update()
        {
            elapsedFrames++;

            foreach (Projectile proj in allProjectiles)
            {
                if (proj.Active)
                {
                    proj.Update();
                }
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
