using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Gameception
{
    class minion : Creep
    {
        float attackRange;
        float patrolDistance;
        float distanceToP1, distanceToP2;

        public Vector3 spawnPoint, patrolPoint;

        bool onPatrol, movingToPointB,isDead;
      
        /*no idea how to seed a random number generator*/
        private static int seed = unchecked(DateTime.Now.Ticks.GetHashCode());
        Random random = new Random(seed);
       
        //int for preference of which player to attack should both be viable
        int chasePlayerKey;
        
        //constructor
        public minion(Model model, float moveSpeed, int initialHealth, 
                      Vector3 startPosition,float scale, Camera camera)
            :base(model,  moveSpeed, initialHealth, startPosition, scale, camera)
        { 
           
            chasePlayerKey = random.Next(1,3);
            attackRange = 40f;
            spawnPoint = startPosition;
            patrolDistance = 50f;
           
            float patrolPointX = (float)( patrolDistance*Math.Cos(random.Next(0,361)) );
            patrolPointX = spawnPoint.X - patrolPointX;
           
            float patrolPointZ = (float)(patrolDistance * Math.Sin(random.Next(0, 361)));
            patrolPointZ = spawnPoint.Z - patrolPointZ;

            patrolPoint = new Vector3(patrolPointX, startPosition.Y, patrolPointZ);
           
            onPatrol = true;
            movingToPointB = true;
            isDead = false;
        }


        //update method, patrol if players are out of range,
        //else attack 
        public override void beEvil(Player p1, Player p2)
        {

                distanceToP1 = distanceCheck(p1);
                distanceToP2 = distanceCheck(p2);

                if(distanceToP1 <= attackRange || distanceToP2 <= attackRange)
                {
                    onPatrol = false;

                    //if both players within range
                    if (distanceToP1 <= attackRange && distanceToP2 <= attackRange)
                    {
                        if (chasePlayerKey == 1)
                        {
                            moveTowards(p1);
                        }
                        else
                        {
                            moveTowards(p2);
                        }
                    }

                    else//only one within range
                    {

                        if (chasePlayerKey == 1)
                        {
                            //if p1 in range 
                            //  --> attack p1
                            if (distanceToP1 <= attackRange)
                            {
                                moveTowards(p1);
                            }

                            //if p2 in range 
                            // --> attack p2
                            if (distanceToP2 <= attackRange)
                            {
                                moveTowards(p2);
                            }
                        }
                            //reverse of above 
                            //accounting for player prefference
                            //(chaseplayerkey)
                        else 
                        {
                            if (distanceToP2 <= attackRange)
                            {
                                moveTowards(p2);
                            }
                    
                            if (distanceToP1 <= attackRange)
                            {
                                moveTowards(p1);
                            }
                        }
                    }
                }

            if( (attackRange < distanceToP1) && (attackRange < distanceToP2) )
                patrol();

        }

        public float distanceCheck(Player p)
        {
            return distanceCheck(p.Position.X, p.Position.Z) ; 
        }

        private float distanceCheck(float x, float z)
        {
            float xDistance, zDistance, distanceToPoint;

            //pythagoras in action
            xDistance = ((this.Position.X) - x);
            zDistance = ((this.Position.Z) - z);
            distanceToPoint = (float)Math.Sqrt(((xDistance) * (xDistance)) + ((zDistance) * (zDistance)));

            return distanceToPoint;
        }
 
        public void patrol()
        {
          //  log( "onpatrol: "+onPatrol );
           // log( "moving to pointB: "+movingToPointB);
           // log( "startPoint: "+this.spawnPoint+" patrol point: "+this.patrolPoint );
           // log( "this position: "+this.Position);

            //coming back from a chase
            if (!onPatrol)
            {
                //return to starting position
                moveTowards(spawnPoint.X, spawnPoint.Z);
                
                //continue patrol

                /*  * --A---x---B---
                 * step size (A-B), end point x
                 * result, minion bounces between A and B
                 * difference less than 0.7 accounts for over stepping end point, 
                 * attempt at stopping minions from vibrating between 2 points  
                 */
                if (this.Position.X - spawnPoint.X < 0.7)
                {
                    if (this.Position.Z - spawnPoint.Z < 0.7)
                    {
                        onPatrol = true;
                        movingToPointB = true;
                    }
                }
            }
            //if patrolling
            else
            {
                //move towards patrol point
                if (movingToPointB)
                {
                    moveTowards(patrolPoint.X, patrolPoint.Z);

                    if( Math.Abs(this.Position.X - patrolPoint.X) < 0.7)
                        if( Math.Abs(this.Position.Z - patrolPoint.Z)< 0.7)
                            movingToPointB = false;
                }
                //back to starting position
                else
                {
                    moveTowards(spawnPoint.X, spawnPoint.Z);

                    if (this.Position.X - spawnPoint.X < 0.7)
                        if (this.Position.Z - spawnPoint.Z < 0.7)
                        movingToPointB = true;
                }
            }
       
        }

        public void moveTowards(Player p)
        {
            moveTowards(p.Position.X, p.Position.Z);
        }

        //minion ai 
        public override void moveTowards(float x, float z)
        {
         /*
            Vector3 oldPosition = this.Position;
   
            // beeline algorithm
            if (x > this.Position.X)
                Position +=  Vector3.UnitX*MovementSpeed;
            if (x < this.Position.X)
                Position -=   Vector3.UnitX * MovementSpeed;

            if (z > this.Position.Z)
                Position += Vector3.UnitZ * MovementSpeed;
            if (z < this.Position.Z)
                Position -= Vector3.UnitZ * MovementSpeed;
            */
        //    getFacingDirection(oldPosition, Position);
          
            
            //Astar
            /*
             *  need help on implementing Astar
             *  found an online source for it and
             *  converted everyrthing as best i could into c#
             *  only after i did that, did i realise it doesnt
             *  show how you use it
             *  
             * classes: Node, Cell and Grid
             */
        }

        private void getFacingDirection(Vector3 oldPosition, Vector3 Position)
        {
            //use two points (direction of motion) to get direction model should be facing

            //ja, this would be great but no idea where to start
        }

        
        public override void takeDamage(Projectile p)
        {
            this.Health -= (int)p.getDamage();

            if(this.Health <=0)
            {
                isDead = true;
            }
        }

        public bool isDeadCheck()
        {
            return isDead;
        }

        public override void getPoints()
        {
            log ("spawn point : "+ spawnPoint.ToString() +"patrol point : "+patrolPoint.ToString());
        }

        //too lazy to type it out everytime so i made a method called log
        public void log(string s)
        { Console.WriteLine(s); }
    }
}
