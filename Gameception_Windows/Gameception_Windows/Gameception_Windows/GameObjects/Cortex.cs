using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gameception
{
    class Cortex : Creep
    {
        bool isDead, pointsFound;
        Weapon cortexGun;
        List<Vector3> sporadicMovementPoints;

        int shotsFired;
        int coolDown;

        public Weapon CortexGun
        {
            get { return cortexGun; }
            set { cortexGun = value; }
        }

        public Cortex(Model model, float moveSpeed, int initialHealth,
                      Vector3 startPosition, float scale, Camera camera)
                      : base(model, moveSpeed, initialHealth, startPosition, scale, camera)
        {
            isDead = false;
            sporadicMovementPoints = new List<Vector3>();
            shotsFired = 5;
            coolDown = 0;

           
        }

        public void setOwner()
        {
            cortexGun.Owner = this;
        }

        public Player findClosestPlayer(Player p1, Player p2)
        {
            float x1Distance = this.Position.X - p1.Position.X;
            float z1Distance = this.Position.X - p1.Position.Z;

            float x2Distance = this.Position.X - p2.Position.X;
            float z2Distance = this.Position.X - p2.Position.Z;


            float distanceToP1 = (float)Math.Sqrt(((x1Distance) * (x1Distance)) + ((z1Distance) * (z1Distance))); ;
            float distanceToP2 = (float)Math.Sqrt(((x2Distance) * (x2Distance)) + ((z2Distance) * (z2Distance))); ;

            return (distanceToP1 < distanceToP2) ? p1 : p2; 
        }

        public void shootAtPlayer(Player p)
        {
            //testing, shooting at origion
            cortexGun.fire(this.GameCamera, this.Position,(Vector3.Zero)/* (this.Position-p.Position) */);

            Console.WriteLine("firing at player: "+p.ToString());
        }

        public void hitByProjectile()
        {
            pointsFound = false;
            coolDown = 0;
            shotsFired = 5;
        }

        public void beEvil(Player p1, Player p2, Spawner s)
        {
            if(!pointsFound)
                findPoints();

            //visit a few random points
            if (sporadicMovementPoints.Count > 0)
            {
                moveTowards(sporadicMovementPoints.ElementAt(0).X, sporadicMovementPoints.ElementAt(0).Z);

                //since its possible to overshoot the point forever, use a close enough rule
                if (Math.Abs(this.Position.X - sporadicMovementPoints.ElementAt(0).X) < 0.4)
                    if (Math.Abs(this.Position.Z - sporadicMovementPoints.ElementAt(0).Z) < 0.4)
                        sporadicMovementPoints.RemoveAt(0);
            }
            else 
            {
                if (coolDown == 0)
                {
                    shootAtPlayer(findClosestPlayer(p1, p2));
                    shotsFired--;
                    coolDown = 100;
                }
                else
                    coolDown--;

               if (shotsFired < 1)
               {
                   pointsFound = false;
                   s.bossIsOut = false;
                   s.idle = true;
                   shotsFired = 5;
               }
            }

            if (cortexGun!= null)
            {
                cortexGun.Update();
            }


        /********************
         state machine, move between unreachable islands
            
         randomly decide to shoot at players
         
         if shot, chase after player until damage dealt or shot again a few times
        ********************/ 
        }

        private void findPoints()
        {
            Random random = new Random();
            sporadicMovementPoints.Clear();

            Vector3 here = this.Position;

            int moveAround = random.Next(3,6);

            for (int i = 0; i < moveAround; i++)
            {
                int a = random.Next(0,361);

                while (here == this.Position)
                {
                    here.X = (float)(this.Position.X + ((4) * Math.Cos(a)));
                    here.Y = this.Position.Y;
                    here.Z = (float)(this.Position.Z + ((4) * Math.Sin(a)));
                }

                sporadicMovementPoints.Add(here);
            }
            pointsFound = true;
        }

        public override void takeDamage(Projectile p)
        {
            this.Health -= (int)p.getDamage();

            if (this.Health <= 0)
            {
                isDead = true;
            }
        }

        public bool isDeadCheck()
        {
            return isDead;
        }

        public override void Draw()
        {
            CortexGun.Draw();

            base.Draw();
        }


        public override void getPoints()
        {
         //   log("spawn point : " + spawnPoint.ToString() + "patrol point : " + patrolPoint.ToString());
        }

        //too lazy to type it out everytime so i made a method called log
        public void log(string s)
        { Console.WriteLine(s); }

    }
}
