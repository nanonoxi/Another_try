using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Gameception
{
     class Creep : GameObject
    {

       bool isDead;
       Random deployAmmo;

       public Creep(Model model, float moveSpeed, int initialHealth,
                    Vector3 startPosition, float scale, Camera camera)
           : base(model, moveSpeed, initialHealth, startPosition, scale, camera)
         { 
            isDead = false;
            deployAmmo =new Random();
        }

        public virtual void beEvil(Player p1, Player p2)
        {}

        public virtual void moveTowards(float x, float z)
        {
            Vector3 oldPosition = this.Position;

            // beeline algorithm
            if (x > this.Position.X)
                Position += Vector3.UnitX * MovementSpeed;
            if (x < this.Position.X)
                Position -= Vector3.UnitX * MovementSpeed;

            if (z > this.Position.Z)
                Position += Vector3.UnitZ * MovementSpeed;
            if (z < this.Position.Z)
                Position -= Vector3.UnitZ * MovementSpeed;
        
        }

    
        public virtual void takeDamage(Projectile p)
          {
              this.Health -= (int)p.getDamage();

              if (this.Health <= 0)
              {
                  isDead = true;
              }
          }
         
     
        public virtual bool isDeadCheck()
        {
            return isDead;
        }

        public virtual void getPoints()
        { }

        public virtual void getFacingDirection(Vector3 a, Vector3 b)
        { }
    }
}
