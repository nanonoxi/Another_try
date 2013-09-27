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
        { }

    
          public virtual void takeDamage(Projectile p)
          {
           /* 
            if(isDead)
                deployAmmo.Next(0,100);
           */
          }
         
         
        public virtual void getPoints()
        { }

         public virtual void getFacingDirection(Vector3 a, Vector3 b){}
    }
}
