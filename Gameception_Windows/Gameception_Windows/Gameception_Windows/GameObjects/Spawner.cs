using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gameception
{
    class Spawner
    {
        minion _minion;
        Cortex _boss;
        ammoSupply _ammo;
        List<ammoSupply> ammoDrop;
        Vector3 bossPrevPosition;
        int minionsPerWave, creepCount;
        public bool bossIsOut, idle;
        
        public List<Creep> minions;
        List<Vector3> minionSpawnPoints;
        List<Vector3> cortexSpawnPoints;

        int restPeriod;
        Random random;
        public Spawner(minion minion, Cortex boss, ammoSupply ammo)
        {
            this._minion = minion;
            this._boss = boss;
            this._ammo = ammo;

            random = new Random();

            minionsPerWave = 4;
            creepCount = 4;
            bossIsOut = false;
            idle = false;
            restPeriod = 180;

            minions = new List<Creep>();
            minionSpawnPoints = new List<Vector3>();
            cortexSpawnPoints = new List<Vector3>();
            ammoDrop = new List<ammoSupply>();

            for (int i = 0;  i < minionsPerWave; i++)
            {
                int spawnArea = random.Next(0,5);
                
                /**************************************/
                cortexSpawnPoints.Add(new Vector3(16.59f, _boss.Position.Y , 31.2f));
                cortexSpawnPoints.Add(new Vector3(-51f, _boss.Position.Y , 43.2f ));
                cortexSpawnPoints.Add(new Vector3(17.8f, _boss.Position.Y ,-10f));
                cortexSpawnPoints.Add(new Vector3(-33f, _boss.Position.Y ,-35.2f));
                /**************************************/

                //x coord
                float randomX = generateSpawnPoint(spawnArea).X;
                //actually z -> z coord
                float randomZ = generateSpawnPoint(spawnArea).Y;

                //creating minion identical to @param minion, with differing spawn points
                minion newMinion = new minion(_minion.ObjectModel, _minion.MovementSpeed, _minion.Health, new Vector3(randomX, 0.8f, randomZ), _minion.ScaleFactor, _minion.GameCamera);
                minions.Add(newMinion);
            }
        }


        //minion spawn point creation
        public Vector2 generateSpawnPoint(int location)
        {
            Vector2 point;

            if (location == 1)
            {
                point = new Vector2(random.Next(10, 26), (-1)*random.Next(4, 15));
            }
            else if (location == 2)
            {
                point = new Vector2((-1) * random.Next(8, 22), (-1) * random.Next(37, 49));
            }
            else if (location == 3)
            {
                point = new Vector2((-1) * random.Next(23, 35), (-1) * random.Next(25, 46));
            }
            else if (location == 4)
            {
                point = new Vector2((-1) * random.Next(39,60), random.Next(33,55));
            }
            else
                point = new Vector2(-16, -4);

            return point;
        }


        public void update(Player p1, Player p2, Grid g)
        {
            for (int i = 0; i < ammoDrop.Count; i++)
                if (ammoDrop.ElementAt(i).isPickedUp())
                    ammoDrop.RemoveAt(i);

                if (!idle)
                {
                    if (!bossIsOut)//if creep wave
                    {
                        for (int i = 0; i < minions.Count; i++)
                        {

                            if (minions.ElementAt(i).isDeadCheck())
                            {
                                if (creepCount < 3 || (creepCount > 12 && creepCount < 15))
                                {
                                    ammoSupply a = new ammoSupply(_ammo.ObjectModel, 0, 0, _ammo.ScaleFactor, _ammo.GameCamera, _ammo.AmmoAmount);
                                    a.setPosition(minions.ElementAt(i));
                                    ammoDrop.Add(a);
                                }
                                minions.RemoveAt(i);
                                creepCount--;
                            }
                            else
                                minions.ElementAt(i).beEvil(p1, p2);
                        }
                        if (minions.Count == 0)
                        {
                            bossIsOut = true;
                            increaseCreepsForNextWave();
                            //have a bit of idle time before boss appears, and afterwards too
                            idle = true; ;
                        }
                    }//if boss wave
                    else
                        _boss.beEvil(p1, p2, this);
                }
                else
                    createIdleTime();
        }

        private void createIdleTime()
        {
            if (restPeriod > 0)
                restPeriod--;
            else
            {
                bossPrevPosition = _boss.Position;

                while(_boss.Position == bossPrevPosition)
                    _boss.Position = cortexSpawnPoints.ElementAt(random.Next(0,5));

                idle = false;
                restPeriod = 180;
            }
        }

        private void increaseCreepsForNextWave()
        {
            //increase number of creeps per wave
            if (minionsPerWave < 21)
            {
                minionsPerWave += 3;
                creepCount = minionsPerWave;
            }
            else//if at max creep count, increase creep speed and health
            {
                _minion.MovementSpeed+=0.1f;
                _minion.Health++;
            }

            for (int i = 0; i < minionsPerWave; i++)
            {
                int randomX = random.Next(-80, 20);
                int randomZ = random.Next(-30, 45);

                //creating creep identical to param basic creep, with differing spawn points
                minion newMinion = new minion(_minion.ObjectModel, _minion.MovementSpeed, _minion.Health, new Vector3(randomX, 0.8f, randomZ), _minion.ScaleFactor, _minion.GameCamera);
                minions.Add(newMinion);
            }
        }

        public void Draw()
        {
            foreach (ammoSupply a in ammoDrop)
                a.Draw();

            if(!idle)
            {
                if (bossIsOut)
                {
                    _boss.Draw();
                }
                else
                {
                    foreach (Creep c in minions)
                        c.Draw();
                }
            }
        }

        internal void checkCollisions(Projectile p, Player p1, Player p2)
        {
            foreach (minion m in minions)
            {
                if (p.getBoundingSphere().Intersects(m.getBoundingSphere()))
                {
                    m.takeDamage(p);
                }
            }

            for (int i = 0; i < ammoDrop.Count; i++)
                if (p1.getBoundingSphere().Intersects(ammoDrop.ElementAt(i).getBoundingSphere()))
                {
                    p1.Ammo += ammoDrop.ElementAt(i).AmmoAmount;
                    ammoDrop.ElementAt(i).pickedUp();
                }
            //************************temporary*********************************
            if (p.getBoundingSphere().Intersects(_boss.getBoundingSphere()) && bossIsOut)
            {
                bossIsOut = false;
                idle = true;
                _boss.hitByProjectile();
              //  createIdleTime();
            }
            //******************************************************************
        }

        public List<Creep> getCreeps()
        {
            List<Creep> list = new List<Creep>();

            if (bossIsOut)
                list.Add(_boss);
            else
                list = minions;

            return list;
        }
    }

    

}
